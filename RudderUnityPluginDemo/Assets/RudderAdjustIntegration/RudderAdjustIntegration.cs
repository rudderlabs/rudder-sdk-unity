using System.Collections.Generic;
using com.adjust.sdk;

namespace Rudderlabs
{
    class RudderAdjustIntegration : RudderIntegration
    {
        private Dictionary<string, string> eventTokenMap = new Dictionary<string, string>();
        public RudderAdjustIntegration(Dictionary<string, object> config, RudderClient client, RudderConfig rudderConfig)
        {
            RudderLogger.LogDebug("Instantiating RudderAdjustIntegration");
            string appToken = config["appToken"] as string;
            RudderLogger.LogDebug("Adjust: appToken: " + appToken);
            List<object> eventTokens = config["customMappings"] as List<object>;
            foreach (var eventConfig in eventTokens)
            {
                Dictionary<string, object> eventTokenDict = eventConfig as Dictionary<string, object>;
                string eventName = eventTokenDict["from"] as string;
                string eventToken = eventTokenDict["to"] as string;
                RudderLogger.LogDebug("Adjust: " + eventName + " : " + eventToken);
                this.eventTokenMap[eventName] = eventToken;
            }

            RudderLogger.LogDebug("Initiating Adjust native SDK");
            AdjustConfig adjustConfig = new AdjustConfig(
                appToken,
                rudderConfig.logLevel >= RudderLogLevel.DEBUG ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production,
                true);
            adjustConfig.setLogLevel(rudderConfig.logLevel >= RudderLogLevel.DEBUG ? AdjustLogLevel.Verbose : AdjustLogLevel.Error);

            RudderLogger.LogDebug("Starting Adjust native SDK");
            Adjust.start(adjustConfig);
        }

        public override void Dump(RudderMessage message)
        {
            RudderLogger.LogDebug("Adjust integration dump event: " + message.eventName);
            if (message.eventName != null && this.eventTokenMap.ContainsKey(message.eventName))
            {
                string eventToken = this.eventTokenMap[message.eventName];
                RudderLogger.LogDebug("Adjust integration dump: eventToken: " + eventToken);
                if (eventToken != null && !eventToken.Equals(""))
                {
                    RudderLogger.LogDebug("Creating Adjust event");
                    AdjustEvent adjustEvent = new AdjustEvent(eventToken);

                    RudderLogger.LogDebug("Adding Event Properties");
                    Dictionary<string, object> eventProperties = message.eventProperties;
                    if (eventProperties != null)
                    {
                        foreach (string key in eventProperties.Keys)
                        {
                            adjustEvent.addCallbackParameter(key, eventProperties[key].ToString());
                        }
                    }

                    RudderLogger.LogDebug("Adding User Properties");
                    Dictionary<string, object> userProperties = message.userProperties;
                    if (userProperties != null)
                    {
                        foreach (string key in userProperties.Keys)
                        {
                            adjustEvent.addPartnerParameter(key, userProperties[key].ToString());
                        }
                    }

                    if (message.eventName.Equals("revenue"))
                    {
                        RudderLogger.LogDebug("Tracking revenue through Adjust SDK");
                        if (message.eventProperties.ContainsKey("price"))
                        {
                            double amount = (double)message.eventProperties["price"];
                            string currency = "USD";
                            if (message.eventProperties.ContainsKey("currency"))
                            {
                                currency = message.eventProperties["currency"] as string;
                            }
                            adjustEvent.setRevenue(amount, currency);
                        }
                    }

                    RudderLogger.LogDebug("Tracking event");
                    Adjust.trackEvent(adjustEvent);
                }
                else
                {
                    RudderLogger.LogDebug("Incorrect event token for Adjust");
                }
            }
            else
            {
                RudderLogger.LogDebug("RudderAdjustIntegration: Event is not tracked through Adjust");
            }
        }
    }
}