using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

namespace Rudderlabs
{
    class RudderIntegrationManager
    {
        private Dictionary<string, object> rudderServerConfig = null;
        private string serverConfigJson = null;
        private bool isFactoryPrepared = false;

        private Dictionary<string, RudderIntegration> integrationOpsMap = new Dictionary<string, RudderIntegration>();
        private Dictionary<string, object> integrations = null;
        private string writeKey = null;
        private RudderConfig config = null;

        private object _lockingObj = new object();

        private List<RudderMessage> factoryDumpQueue = new List<RudderMessage>();
        private string persistedUserId = null;
        private RudderTraits persistedTraits = null;


        private long lastUpdatedTimestamp = 0;

        public RudderIntegrationManager(string writeKey, RudderConfig config)
        {
            this.writeKey = writeKey;
            this.config = config;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            downloadIntegrations();
        }

        private void downloadIntegrations()
        {
            try
            {
                lock (this._lockingObj)
                {
                    this.lastUpdatedTimestamp = long.Parse(PlayerPrefs.GetString("rl_server_update_time", "0"));
                    RudderLogger.LogDebug("RudderIntegrationManager: downloadIntegrations: lastUpdatedTimeStamp: " + lastUpdatedTimestamp);
                    this.serverConfigJson = PlayerPrefs.GetString("rl_server_config", null);
                    RudderLogger.LogDebug("RudderIntegrationManager: downloadIntegrations: serverConfigJson: " + this.serverConfigJson);
                }

                if (this.serverConfigJson == null || this.serverConfigJson.Equals("") || isServerConfigOutdated())
                {
                    Thread t = new Thread(downloadConfig);
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                RudderLogger.LogError("CreateConnection ERROR: " + ex.Message);
            }
        }

        private void prepareFactories()
        {
            try
            {
                if (this.rudderServerConfig == null)
                {
                    RudderLogger.LogInfo("No integrations: rudderServerConfig is null");
                }
                else if (this.config.factories == null)
                {
                    RudderLogger.LogInfo("No integrations: config.factories is null");
                }
                else if (this.config.factories.Count == 0)
                {
                    // no factory to initialize
                    RudderLogger.LogInfo("No integrations: config.factories.Count is 0");
                }
                else
                {
                    RudderClient client = RudderClient.GetInstance();
                    Dictionary<string, object> source = this.rudderServerConfig["source"] as Dictionary<string, object>;
                    List<object> destinations = source["destinations"] as List<object>;

                    if (destinations.Count > 0)
                    {
                        Dictionary<string, object> destinationMap = new Dictionary<string, object>();
                        RudderLogger.LogDebug("destinations are not empty");
                        foreach (var destinationObj in destinations)
                        {
                            Dictionary<string, object> destination = destinationObj as Dictionary<string, object>;
                            Dictionary<string, object> destinationDefinition = destination["destinationDefinition"] as Dictionary<string, object>;
                            string definitionName = destinationDefinition["displayName"] as string;
                            RudderLogger.LogDebug("Extracted Native Destination information: " + definitionName);

                            destinationMap[definitionName] = destination;
                        }

                        foreach (RudderIntegrationFactory factory in this.config.factories)
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
                                        RudderIntegration integrationOp = factory.Create(destinationConfig, client, this.config);
                                        RudderLogger.LogDebug("Native integration initiated for " + factoryKey);
                                        this.integrationOpsMap[factoryKey] = integrationOp;
                                    }
                                }
                            }
                        }
                    }
                }
                this.isFactoryPrepared = true;

                lock (this._lockingObj)
                {
                    if (this.factoryDumpQueue.Count > 0)
                    {
                        for (int index = 0; index < this.factoryDumpQueue.Count; index++)
                        {
                            this.makeIntegrationDump(this.factoryDumpQueue[index]);
                        }
                        this.factoryDumpQueue.Clear();
                    }
                    if (persistedTraits != null && persistedUserId != null)
                    {
                        this.makeIntegrationIdentify(persistedUserId, persistedTraits);
                        this.persistedTraits = null;
                        this.persistedUserId = null;
                    }
                }
            }
            catch (Exception ex)
            {
                RudderLogger.LogError(ex.Message);
            }
        }

        private Dictionary<string, object> parseServerConfig(string configJson)
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

        private bool isServerConfigOutdated()
        {
            if (this.lastUpdatedTimestamp == 0)
            {
                // no config available. cold start
                return true;
            }
            return (Stopwatch.GetTimestamp() - this.lastUpdatedTimestamp) > (config.configRefreshInterval * 60 * 60 * 1000);
        }

        void downloadConfig(object obj)
        {
            bool isDone = false;
            int retryCount = 0;
            int retryTimeOut = 10;

            while (!isDone && retryCount <= 3)
            {
                try
                {
                    string configEndpointUrl = Constants.CONFIG_PLANE_URL + "/sourceConfig";
                    // create http request object
                    var http = (HttpWebRequest)WebRequest.Create(new Uri(configEndpointUrl));
                    http.Method = "GET";
                    var authKeyBytes = System.Text.Encoding.UTF8.GetBytes(writeKey + ":");
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
                            this.serverConfigJson = responseJson;
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
                    RudderLogger.LogError(ex.Message);
                    retryCount += 1;
                    Thread.Sleep(1000 * retryCount * retryTimeOut);
                }
            }
        }

        public Dictionary<string, object> getIntegrations()
        {
            if (this.integrations == null)
            {
                this.integrations = new Dictionary<string, object>();
                this.integrations["All"] = true;
            }
            return this.integrations;
        }

        public void makeIntegrationDump(RudderMessage message)
        {
            // if factories are not prepared dump those in the queue
            if (!this.isFactoryPrepared)
            {
                lock (this._lockingObj)
                if (this.rudderServerConfig == null)
                {
                    return;
                }

                this.integrations = new Dictionary<string, object>();
                Dictionary<string, object> source = this.rudderServerConfig["source"] as Dictionary<string, object>;
                List<object> destinations = source["destinations"] as List<object>;

                foreach (var destinationObj in destinations)
                {
                    factoryDumpQueue.Add(message);
                }
            }
            // make native integration calls
            else
            {
                foreach (string key in integrationOpsMap.Keys)
                {
                    RudderLogger.LogDebug("Dumping " + message.eventName + " to " + key + " native SDK");
                    integrationOpsMap[key].Dump(message);
                }
            }
        }

        public void makeIntegrationIdentify(string userId, RudderTraits traits)
        {
            if (!this.isFactoryPrepared)
            {
                lock (this._lockingObj)
                {
                    this.persistedUserId = userId;
                    this.persistedTraits = traits;
                }
                RudderLogger.LogDebug("Factories are not prepared yet");
            }
            // make native integration calls
            else
            {
                foreach (string key in integrationOpsMap.Keys)
                {
                    RudderLogger.LogDebug("Identify to " + key + " native SDK");
                    integrationOpsMap[key].Identify(userId, traits);
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
            if (!this.isFactoryPrepared)
            {
                lock (this._lockingObj)
                {
                    if (this.serverConfigJson != null && !this.serverConfigJson.Equals(""))
                    {
                        RudderLogger.LogDebug("RudderIntegrationManager Update: serverConfigJson: " + this.serverConfigJson);
                        if (isServerConfigOutdated())
                        {
#if !UNITY_EDITOR
          PlayerPrefs.SetString("rl_server_update_time", Stopwatch.GetTimestamp().ToString());
          PlayerPrefs.SetString("rl_server_config", this.serverConfigJson);
          PlayerPrefs.Save();
#endif
                        }

                        RudderLogger.LogDebug("RudderIntegrationManager Update: serverConfigJson: " + this.serverConfigJson);
                        this.rudderServerConfig = parseServerConfig(this.serverConfigJson);
                        this.prepareFactories();
                    }
                }
            }
        }

        public void Reset()
        {
            if (!this.isFactoryPrepared)
            {
                RudderLogger.LogDebug("Factories are not prepared yet");
            }
            // make native integration calls
            else
            {
                foreach (string key in integrationOpsMap.Keys)
                {
                    RudderLogger.LogDebug("Resetting native SDK " + key);
                    integrationOpsMap[key].Reset();
                }
            }
        }
    }
}