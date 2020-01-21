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
            string appToken = null;
            if (config.ContainsKey("appToken"))
            {
                appToken = config["appToken"] as string;
                RudderLogger.LogDebug("Adjust: appToken: " + appToken);
            }

            List<object> eventTokens = new List<object>();
            if (config.ContainsKey("customMappings"))
            {
                eventTokens = config["customMappings"] as List<object>;
                foreach (var eventConfig in eventTokens)
                {
                    Dictionary<string, object> eventTokenDict = eventConfig as Dictionary<string, object>;
                    string eventName = eventTokenDict["from"] as string;
                    string eventToken = eventTokenDict["to"] as string;
                    RudderLogger.LogDebug("Adjust: " + eventName + " : " + eventToken);
                    this.eventTokenMap[eventName] = eventToken;
                }
            }

            string delayTime = null;
            if (config.ContainsKey("delay"))
            {
                delayTime = config["delay"] as string;
                RudderLogger.LogDebug("delayTime:" + delayTime);
            }

            if (appToken != null && !appToken.Equals(""))
            {
                RudderLogger.LogDebug("Initiating Adjust native SDK");
                AdjustConfig adjustConfig = new AdjustConfig(
                                appToken,
                                rudderConfig.logLevel >= RudderLogLevel.DEBUG ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production,
                                true);
                adjustConfig.setLogLevel(rudderConfig.logLevel >= RudderLogLevel.DEBUG ? AdjustLogLevel.Verbose : AdjustLogLevel.Error);
                double delay = 0;
                try
                {
                    if (delayTime != null)
                    {
                        delay = double.Parse(delayTime);
                    }
                }
                catch (System.Exception ex)
                {
                    RudderLogger.LogError("Invalid delay time" + ex.Message);
                }
                if (delay < 0)
                {
                    delay = 0;
                }
                else if (delay > 10)
                {
                    delay = 10;
                }
                if (delay > 0)
                {
                    adjustConfig.setDelayStart(delay);
                }

                RudderLogger.LogDebug("Starting Adjust native SDK");
                Adjust.start(adjustConfig);
            }
            else
            {
                RudderLogger.LogError("appToken was not set in Dashboard");
            }
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
                    this.AddSessionParameter(RudderCache.GetAnonymousId());

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
                            adjustEvent.addCallbackParameter(key, userProperties[key].ToString());
                        }
                    }

                    RudderLogger.LogDebug("Tracking revenue through Adjust SDK");
                    if (message.eventProperties.ContainsKey("revenue"))
                    {
                        double amount = (double)message.eventProperties["revenue"];
                        string currency = "";
                        if (message.eventProperties.ContainsKey("currency"))
                        {
                            currency = message.eventProperties["currency"] as string;
                        }
                        adjustEvent.setRevenue(amount, currency);
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

        public override void Identify(string userId, RudderTraits traits)
        {
            this.AddSessionParameter(RudderCache.GetUserId());
        }

        private void AddSessionParameter(string userId)
        {
            Adjust.addSessionPartnerParameter("anonymousId", RudderCache.GetAnonymousId());
            if (userId != null)
            {
                Adjust.addSessionPartnerParameter("userId", userId);
            }
        }

        public override void Reset()
        {
            Adjust.resetSessionPartnerParameters();
        }
    }
}