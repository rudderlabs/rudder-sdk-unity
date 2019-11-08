    using System.Collections.Generic;
using com.adjust.sdk;
using UnityEngine;

namespace Rudderlabs
{
    class RudderAdjustIntegration : RudderIntegration
    {
        private Dictionary<string, string> eventTokenMap = new Dictionary<string, string>();
        public RudderAdjustIntegration(Dictionary<string, object> config, RudderClient client)
        {
            string appToken = config["apiToken"] as string;
            List<object> eventTokens = config["eventTokenMap"] as List<object>;
            foreach (var eventConfig in eventTokens)
            {
                Dictionary<string, object> eventTokenDict = eventConfig as Dictionary<string, object>;
                string eventName = eventTokenDict["from"] as string;
                // UnityEngine.Debug.Log("eventName: " + eventName);
                string eventToken = eventTokenDict["to"] as string;
                // UnityEngine.Debug.Log("eventToken: " + eventToken);
                this.eventTokenMap[eventName] = eventToken;
            }

            AdjustConfig adjustConfig = new AdjustConfig(appToken, AdjustEnvironment.Sandbox, true);
            adjustConfig.setLogLevel(AdjustLogLevel.Verbose);

            Adjust.start(adjustConfig);
        }

        public override void Dump(RudderMessage message)
        {
            // UnityEngine.Debug.Log("Dump: event:" + message.eventName);
            if (this.eventTokenMap.ContainsKey(message.eventName))
            {
                string eventToken = this.eventTokenMap[message.eventName];
                // UnityEngine.Debug.Log("Dump: event token:" + eventToken);
                if (eventToken != null && eventToken.Equals(""))
                {
                    // Debug.Log("eventToken dump: " + eventToken);   
                    AdjustEvent adjustEvent = new AdjustEvent(eventToken);

                    Dictionary<string, object> eventProperties = message.eventProperties;
                    foreach (string key in eventProperties.Keys)
                    {
                        adjustEvent.addCallbackParameter(key, eventProperties[key].ToString());
                    }

                    Dictionary<string, object> userProperties = message.userProperties;
                    foreach (string key in userProperties.Keys)
                    {
                        adjustEvent.addPartnerParameter(key, userProperties[key].ToString());
                    }

                    if (message.eventName.Equals("revenue")) {
                        if (message.eventProperties.ContainsKey("price")) {
                            double amount = (double) message.eventProperties["price"];
                            string currency = "USD";
                            if (message.eventProperties.ContainsKey("currency")) {
                                 currency = message.eventProperties["currency"] as string;
                            }
                            adjustEvent.setRevenue(amount, currency);
                        }
                    }
                    
                    Adjust.trackEvent(adjustEvent);
                }
            } else {
                // UnityEngine.Debug.Log("Dump: event: not found");
            }
        }
    }
}