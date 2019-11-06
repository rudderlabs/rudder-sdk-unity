using System.Collections.Generic;
using com.adjust.sdk;

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
                string eventToken = eventTokenDict["to"] as string;
                this.eventTokenMap[eventName] = eventToken;
            }

            AdjustConfig adjustConfig = new AdjustConfig(appToken, AdjustEnvironment.Sandbox, true);
            adjustConfig.setLogLevel(AdjustLogLevel.Verbose);

            Adjust.start(adjustConfig);
        }

        public override void Dump(RudderMessage message)
        {
            if (this.eventTokenMap.ContainsKey(message.eventName))
            {
                string eventToken = this.eventTokenMap[message.eventName];
                if (eventToken != null && eventToken.Equals(""))
                {
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

                    Adjust.trackEvent(adjustEvent);
                }
            }
            // Debug.Log("integration dump: " + message.eventName);
        }
    }
}