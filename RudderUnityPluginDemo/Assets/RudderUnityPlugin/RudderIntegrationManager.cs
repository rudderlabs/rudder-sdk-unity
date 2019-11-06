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
        private Dictionary<string, RudderIntegration> integrationOpsMap = new Dictionary<string, RudderIntegration>();
        private Dictionary<string, object> integrations = null;
        private string writeKey = null;
        private RudderConfig config = null;

        public RudderIntegrationManager(string writeKey, RudderConfig config)
        {
            this.writeKey = writeKey;
            this.config = config;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            // UnityEngine.Debug.Log("downloading integrations");
            downloadIntegrations();
        }

        private void prepareFactories()
        {
            // UnityEngine.Debug.Log("preparing factories");
            if (this.rudderServerConfig == null || this.config.factories == null || this.config.factories.Count == 0)
            {
                // no factory to initialize
                // UnityEngine.Debug.Log("no integrations");
                return;
            }

            RudderClient client = RudderClient.GetInstance();
            Dictionary<string, object> source = this.rudderServerConfig["source"] as Dictionary<string, object>;
            List<object> destinations = source["destinations"] as List<object>;

            if (destinations.Count > 0)
            {
                Dictionary<string, object> destinationMap = new Dictionary<string, object>();
                // UnityEngine.Debug.Log("destinations are not empty");
                foreach (var destinationObj in destinations)
                {
                    // UnityEngine.Debug.Log("loop started");
                    Dictionary<string, object> destination = destinationObj as Dictionary<string, object>;
                    // UnityEngine.Debug.Log("object created");
                    Dictionary<string, object> destinationDefinition = destination["destinationDefinition"] as Dictionary<string, object>;
                    // UnityEngine.Debug.Log("destination definition created");
                    string definitionName = destinationDefinition["name"] as string;
                    // UnityEngine.Debug.Log("definitionName: " + definitionName);

                    destinationMap[definitionName] = destination;
                }

                foreach (RudderIntegrationFactory factory in this.config.factories)
                {
                    // UnityEngine.Debug.Log("facotory loop started");
                    string factoryKey = factory.key();
                    // UnityEngine.Debug.Log("factoryKey: " + factoryKey);
                    if (destinationMap.ContainsKey(factoryKey))
                    {
                        // UnityEngine.Debug.Log("factoryKey present in server config map");
                        Dictionary<string, object> serverDestination = destinationMap[factoryKey] as Dictionary<string, object>;
                        if (serverDestination != null)
                        {
                            bool? isDestinationEnabled = serverDestination["enabled"] as bool?;
                            // UnityEngine.Debug.Log("is destination enabled: " + isDestinationEnabled);
                            if (isDestinationEnabled != null && isDestinationEnabled == true)
                            {
                                Dictionary<string, object> destinationConfig = serverDestination["config"] as Dictionary<string, object>;
                                // UnityEngine.Debug.Log("server config for destination" );
                                RudderIntegration integrationOp = factory.create(destinationConfig, client);
                                // UnityEngine.Debug.Log("integration operation fixed");
                                this.integrationOpsMap[factoryKey] = integrationOp;
                            }
                        }
                    }
                }
            }
        }

        private void downloadIntegrations()
        {
            try
            {
                long lastUpdatedTime = long.Parse(PlayerPrefs.GetString("rl_server_update_time", "0"));
                string persistedConfig = PlayerPrefs.GetString("rl_server_config", null);

                // UnityEngine.Debug.Log("lastUpdatedTime: " + lastUpdatedTime);
                // UnityEngine.Debug.Log("persistedConfig: " + persistedConfig);

                if (persistedConfig == null || isServerConfigOutdated(lastUpdatedTime))
                {
                    Thread t = new Thread(downloadConfig);
                    t.Start();
                }
                else
                {
                    this.rudderServerConfig = parseServerConfig(persistedConfig);
                    prepareFactories();
                }
            }
            catch (Exception ex)
            {
                // UnityEngine.Debug.Log("RudderSDK: CreateConnection ERROR: " + ex.Message);
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
                // UnityEngine.Debug.Log(ex.StackTrace);
            }
            return null;
        }

        private bool isServerConfigOutdated(long lastUpdatedTime)
        {
            if (lastUpdatedTime == 0)
            {
                // no config available. cold start
                return true;
            }
            return (Stopwatch.GetTimestamp() - lastUpdatedTime) > (24 * 60 * 60 * 1000);
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
                    // check if the endPointUri is formed correctly
                    string configEndpointUrl = "https://api.rudderlabs.com/source-config?write_key=" + this.writeKey;
                    // create http request object
                    var http = (HttpWebRequest)WebRequest.Create(new Uri(configEndpointUrl));
                    http.Method = "GET";
                    // get the response
                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    // return the response as a string
                    string responseJson = sr.ReadToEnd();
                    // UnityEngine.Debug.Log("responseJson: " + responseJson);
                    if (responseJson != null)
                    {
#if !UNITY_EDITOR
                    // PlayerPrefs.SetString("rl_server_update_time", Stopwatch.GetTimestamp().ToString());
                    // PlayerPrefs.SetString("rl_server_config", responseJson);
                    // PlayerPrefs.Save();
#endif

                        this.rudderServerConfig = parseServerConfig(responseJson);

                        // UnityEngine.Debug.Log("rudderServerConfig: " + rudderServerConfig);

                        prepareFactories();
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
                    // UnityEngine.Debug.Log(ex.Message);
                    retryCount += 1;
                    Thread.Sleep(1000 * retryCount * retryTimeOut);
                }

            }
        }

        public Dictionary<string, object> getIntegrations()
        {
            if (this.integrations == null)
            {
                prepareIntegrations();
            }
            if (this.integrations == null)
            {
                Dictionary<string, object> allIntegrationPlaceHolder = new Dictionary<string, object>();
                allIntegrationPlaceHolder["All"] = true;
                return allIntegrationPlaceHolder;
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

                // UnityEngine.Debug.Log("serverConfig is not null");
                this.integrations = new Dictionary<string, object>();
                Dictionary<string, object> source = this.rudderServerConfig["source"] as Dictionary<string, object>;
                List<object> destinations = source["destinations"] as List<object>;

                foreach (var destinationObj in destinations)
                {
                    Dictionary<string, object> destination = destinationObj as Dictionary<string, object>;
                    Dictionary<string, object> destinationDefinition = destination["destinationDefinition"] as Dictionary<string, object>;
                    string definitionName = destinationDefinition["name"] as string;
                    bool? isDestinationEnabled = destination["enabled"] as bool?;

                    // UnityEngine.Debug.Log("destination definition name" + definitionName);
                    // UnityEngine.Debug.Log("destinationeneable: " + isDestinationEnabled);

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
    }
}