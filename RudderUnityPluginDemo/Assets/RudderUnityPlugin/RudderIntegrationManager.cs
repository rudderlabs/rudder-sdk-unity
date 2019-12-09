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

        object _lockingObj = new object();

        public RudderIntegrationManager(string writeKey, RudderConfig config)
        {
            this.writeKey = writeKey;
            this.config = config;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            downloadIntegrations();
        }

        private long lastUpdatedTimestamp = 0;
        private void downloadIntegrations()
        {
            try
            {
                lock (this._lockingObj)
                {
                    this.lastUpdatedTimestamp = long.Parse(PlayerPrefs.GetString("rl_server_update_time", "0"));
                    this.serverConfigJson = PlayerPrefs.GetString("rl_server_config", null);
                }

                if (this.serverConfigJson == null || isServerConfigOutdated())
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
                if (this.rudderServerConfig == null || this.config.factories == null || this.config.factories.Count == 0)
                {
                    // no factory to initialize
                    RudderLogger.LogInfo("No integrations");
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
                            string definitionName = destinationDefinition["name"] as string;
                            RudderLogger.LogDebug("Extracted Native Destination information: " + definitionName);

                            destinationMap[definitionName] = destination;
                        }

                        foreach (RudderIntegrationFactory factory in this.config.factories)
                        {
                            string factoryKey = factory.key();
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
                                        RudderIntegration integrationOp = factory.create(destinationConfig, client);
                                        RudderLogger.LogDebug("Native integration initiated for " + factoryKey);
                                        this.integrationOpsMap[factoryKey] = integrationOp;
                                    }
                                }
                            }
                        }
                    }
                }
                this.isFactoryPrepared = true;
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
                return null;
            }
            try
            {
                return MiniJSON.Json.Deserialize(configJson) as Dictionary<string, object>;
            }
            catch (Exception ex)
            {
                RudderLogger.LogError(ex.StackTrace);
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
                    string configEndpointUrl = Constants.CONFIG_PLANE_URL;
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

        private void prepareIntegrations()
        {
            // prepare integration dict
            if (this.integrations == null)
            {
                if (this.rudderServerConfig == null)
                {
                    return;
                }

                this.integrations = new Dictionary<string, object>();
                Dictionary<string, object> source = this.rudderServerConfig["source"] as Dictionary<string, object>;
                List<object> destinations = source["destinations"] as List<object>;

                foreach (var destinationObj in destinations)
                {
                    Dictionary<string, object> destination = destinationObj as Dictionary<string, object>;
                    Dictionary<string, object> destinationDefinition = destination["destinationDefinition"] as Dictionary<string, object>;
                    string definitionName = destinationDefinition["name"] as string;
                    bool? isDestinationEnabled = destination["enabled"] as bool?;
                    this.integrations[definitionName] = isDestinationEnabled == null ? false : isDestinationEnabled;
                }

            }
        }

        public void makeIntegrationDump(RudderMessage message)
        {
            // make native integration calls
            foreach (string key in integrationOpsMap.Keys)
            {
                integrationOpsMap[key].Dump(message);
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
                    if (this.serverConfigJson != null)
                    {
                        if (isServerConfigOutdated())
                        {
#if !UNITY_EDITOR
          PlayerPrefs.SetString("rl_server_update_time", Stopwatch.GetTimestamp().ToString());
          PlayerPrefs.SetString("rl_server_config", this.serverConfigJson);
          PlayerPrefs.Save();
#endif
                        }

                        this.rudderServerConfig = parseServerConfig(this.serverConfigJson);
                        this.prepareFactories();
                    }
                }
            }
        }
    }
}