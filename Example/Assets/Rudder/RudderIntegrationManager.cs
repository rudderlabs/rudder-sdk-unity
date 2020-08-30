using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

namespace RudderStack
{
    class RudderIntegrationManager
    {
        private Dictionary<string, object> _rudderServerConfig = null;
        private string _serverConfigJson = null;
        private bool _isFactoryPrepared = false;
        private Dictionary<string, RudderIntegration> _integrationOpsMap = new Dictionary<string, RudderIntegration>();
        private string _writeKey = null;
        private RudderConfig _config = null;
        private object _lockingObj = new object();
        private List<RudderMessage> _factoryDumpQueue = new List<RudderMessage>();
        private string _persistedUserId = null;
        private RudderTraits _persistedTraits = null;
        private long _lastUpdatedTimestamp = 0;

        public RudderIntegrationManager(string writeKey, RudderConfig config)
        {
            this._writeKey = writeKey;
            this._config = config;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            DownloadIntegrations();
        }

        private void DownloadIntegrations()
        {
            try
            {
                lock (this._lockingObj)
                {
                    this._lastUpdatedTimestamp = long.Parse(PlayerPrefs.GetString("rl_server_update_time", "0"));
                    RudderLogger.LogDebug("RudderIntegrationManager: downloadIntegrations: lastUpdatedTimeStamp: " + _lastUpdatedTimestamp);
                    this._serverConfigJson = PlayerPrefs.GetString("rl_server_config", null);
                    RudderLogger.LogDebug("RudderIntegrationManager: downloadIntegrations: serverConfigJson: " + this._serverConfigJson);
                }

                if (this._serverConfigJson == null || this._serverConfigJson.Equals("") || IsServerConfigOutdated())
                {
                    Thread t = new Thread(DownloadConfig);
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                RudderLogger.LogError("CreateConnection ERROR: downloadConfig error");
            }
        }

        private void PrepareFactories()
        {
            try
            {
                if (this._rudderServerConfig == null)
                {
                    RudderLogger.LogInfo("No integrations: rudderServerConfig is null");
                }
                else if (this._config.factories == null)
                {
                    RudderLogger.LogInfo("No integrations: config.factories is null");
                }
                else if (this._config.factories.Count == 0)
                {
                    // no factory to initialize
                    RudderLogger.LogInfo("No integrations: config.factories.Count is 0");
                }
                else
                {
                    RudderClient client = RudderClient.GetInstance();
                    Dictionary<string, object> source = this._rudderServerConfig["source"] as Dictionary<string, object>;
                    List<object> destinations = source["destinations"] as List<object>;

                    if (destinations.Count > 0)
                    {
                        Dictionary<string, object> destinationMap = new Dictionary<string, object>();
                        RudderLogger.LogDebug("Native SDKs enabled in Dashboard");
                        foreach (var destinationObj in destinations)
                        {
                            Dictionary<string, object> destination = destinationObj as Dictionary<string, object>;
                            Dictionary<string, object> destinationDefinition = destination["destinationDefinition"] as Dictionary<string, object>;
                            string destinationName = destinationDefinition["displayName"] as string;
                            RudderLogger.LogDebug("Extracted Native Destination information: " + destinationName);

                            destinationMap[destinationName] = destination;
                        }

                        foreach (RudderIntegrationFactory factory in this._config.factories)
                        {
                            string factoryKey = factory.Key();
                            RudderLogger.LogDebug("Initiating native destination factory: " + factoryKey);
                            if (destinationMap.ContainsKey(factoryKey))
                            {
                                Dictionary<string, object> serverDestination = destinationMap[factoryKey] as Dictionary<string, object>;
                                if (serverDestination != null)
                                {
                                    bool? isDestinationEnabled = serverDestination["enabled"] as bool?;
                                    if (isDestinationEnabled != null && isDestinationEnabled == true)
                                    {
                                        Dictionary<string, object> destinationConfig = serverDestination["config"] as Dictionary<string, object>;
                                        RudderIntegration integrationOp = factory.Create(destinationConfig, client, this._config);
                                        RudderLogger.LogDebug("Native integration initiated for " + factoryKey);
                                        this._integrationOpsMap[factoryKey] = integrationOp;
                                    }
                                }
                            }
                        }
                    }
                }
                this._isFactoryPrepared = true;

                lock (this._lockingObj)
                {
                    if (this._factoryDumpQueue.Count > 0)
                    {
                        for (int index = 0; index < this._factoryDumpQueue.Count; index++)
                        {
                            this.MakeIntegrationDump(this._factoryDumpQueue[index]);
                        }
                        this._factoryDumpQueue.Clear();
                    }
                    if (_persistedTraits != null && _persistedUserId != null)
                    {
                        this.MakeIntegrationIdentify(_persistedUserId, _persistedTraits);
                        this._persistedTraits = null;
                        this._persistedUserId = null;
                    }
                }
            }
            catch (Exception ex)
            {
                RudderLogger.LogError(ex.Message);
            }
        }

        private Dictionary<string, object> ParseServerConfig(string configJson)
        {
            if (configJson == null || configJson.Equals(""))
            {
                RudderLogger.LogDebug("parseServerConfig: configJson is null");
                return null;
            }
            try
            {
                return MiniJSON.Json.Deserialize(configJson) as Dictionary<string, object>;
            }
            catch (Exception ex)
            {
                RudderLogger.LogError(ex.Message);
            }
            return null;
        }

        private bool IsServerConfigOutdated()
        {
            if (this._lastUpdatedTimestamp == 0)
            {
                // no config available. cold start
                return true;
            }
            return (Stopwatch.GetTimestamp() - this._lastUpdatedTimestamp) > (_config.configRefreshInterval * 60 * 60 * 1000);
        }

        void DownloadConfig(object obj)
        {
            bool isDone = false;
            int retryCount = 0;
            int retryTimeOut = 10;

            while (!isDone && retryCount <= 3)
            {
                try
                {
                    string configEndpointUrl = _config.controlPlaneUrl + "/sourceConfig";
                    RudderLogger.LogDebug("configEndpontUrl: " + configEndpointUrl);
                    // create http request object
                    var http = (HttpWebRequest)WebRequest.Create(new Uri(configEndpointUrl));
                    http.Method = "GET";
                    var authKeyBytes = System.Text.Encoding.UTF8.GetBytes(_writeKey + ":");
                    string authHeader = System.Convert.ToBase64String(authKeyBytes);
                    http.Headers.Add("Authorization", "Basic " + authHeader);
                    // get the response
                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    // return the response as a string
                    string responseJson = sr.ReadToEnd();
                    RudderLogger.LogDebug("Config Server Response: " + responseJson);
                    if (responseJson != null)
                    {
                        lock (this._lockingObj)
                        {
                            this._serverConfigJson = responseJson;
                        }
                        isDone = true;
                    }
                    else
                    {
                        retryCount += 1;
                        Thread.Sleep(1000 * retryCount * retryTimeOut);
                    }
                }
                catch (Exception ex)
                {
                    // RudderLogger.LogError(ex.Message);
                    retryCount += 1;
                    Thread.Sleep(1000 * retryCount * retryTimeOut);
                }
            }
        }

        public void MakeIntegrationDump(RudderMessage message)
        {
            // if factories are not prepared dump those in the queue
            if (!this._isFactoryPrepared || this._rudderServerConfig == null)
            {
                lock (this._lockingObj)
                {
                    _factoryDumpQueue.Add(message);
                }
            }
            // make native integration calls
            else
            {
                foreach (string key in _integrationOpsMap.Keys)
                {
                    RudderLogger.LogDebug("Dumping " + message.eventName + " to " + key + " native SDK");
                    _integrationOpsMap[key].Dump(message);
                }
            }
        }

        public void MakeIntegrationIdentify(string userId, RudderTraits traits)
        {
            if (!this._isFactoryPrepared || this._rudderServerConfig == null)
            {
                lock (this._lockingObj)
                {
                    this._persistedUserId = userId;
                    this._persistedTraits = traits;
                }
                RudderLogger.LogDebug("Factories are not prepared yet");
            }
            // make native integration calls
            else
            {
                foreach (string key in _integrationOpsMap.Keys)
                {
                    RudderLogger.LogDebug("Identify to " + key + " native SDK");
                    _integrationOpsMap[key].Identify(userId, traits);
                }
            }


        }

        // ssl check validator
        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Update()
        {
            if (!this._isFactoryPrepared)
            {
                lock (this._lockingObj)
                {
                    if (this._serverConfigJson != null && !this._serverConfigJson.Equals(""))
                    {
                        RudderLogger.LogDebug("RudderIntegrationManager Update: serverConfigJson: " + this._serverConfigJson);
                        if (IsServerConfigOutdated())
                        {
#if !UNITY_EDITOR
                            PlayerPrefs.SetString("rl_server_update_time", Stopwatch.GetTimestamp().ToString());
                            PlayerPrefs.SetString("rl_server_config", this._serverConfigJson);
                            PlayerPrefs.Save();
#endif
                        }

                        this._rudderServerConfig = ParseServerConfig(this._serverConfigJson);
                        this.PrepareFactories();
                    }
                }
            }
        }

        public void Reset()
        {
            if (!this._isFactoryPrepared)
            {
                RudderLogger.LogDebug("Factories are not prepared yet");
            }
            // make native integration calls
            else
            {
                foreach (string key in _integrationOpsMap.Keys)
                {
                    RudderLogger.LogDebug("Resetting native SDK " + key);
                    _integrationOpsMap[key].Reset();
                }
            }
        }
    }
}
