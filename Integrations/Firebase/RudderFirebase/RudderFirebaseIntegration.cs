using System.Collections.Generic;
using RudderStack;
using RudderStack.MiniJSON;

namespace RudderStack
{
    class RudderFirebaseIntegration : RudderIntegration
    {
        public RudderFirebaseIntegration(Dictionary<string, object> config, RudderClient client, RudderConfig rudderConfig)
        {
            RudderLogger.LogDebug("Instantiating RudderFirebaseIntegration");
            RudderLogger.LogDebug("Starting Firebase native SDK");
        }

        public override void Dump(RudderMessage message)
        {
            string eventName = message.eventName;
            RudderLogger.LogDebug(eventName + " is sent to Firebase");
            List<Firebase.Analytics.Parameter> parameters = new List<Firebase.Analytics.Parameter>();
            if (message.eventProperties != null)
            {
                foreach (string key in message.eventProperties.Keys)
                {
                    parameters.Add(new Firebase.Analytics.Parameter(
                        key,
                        Json.Serialize(message.eventProperties[key])
                    ));
                }
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(
                eventName,
                parameters.ToArray()
            );
        }

        public override void Identify(string userId, RudderTraits traits)
        {
            // set user ID. FIrebase Unity doesn't support setUserId method. Set a custom property
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty("userId", userId);
            // set custom user properties to firebase
            if (traits != null && traits.traitsDict != null)
            {
                foreach (string key in traits.traitsDict.Keys)
                {
                    Firebase.Analytics.FirebaseAnalytics.SetUserProperty(
                        key,
                        Json.Serialize(traits.traitsDict[key])
                    );
                }
            }

        }

        public override void Reset()
        {
            // nothing to do with firebase
        }
    }
}