using System.Collections.Generic;
using RudderStack.MiniJSON;
/* 
wrapper class for Unity to form event
*/
namespace RudderStack
{
    public class RudderMessage
    {
        public string eventName;
        public Dictionary<string, object> eventProperties;
        public Dictionary<string, object> userProperties;
        public Dictionary<string, object> options;

        public string getEventPropertiesJson()
        {
            return convertToJson(eventProperties);
        }

        public string getUserPropertiesJson()
        {
            return convertToJson(userProperties);
        }

        public string getOptionsJson()
        {
            return convertToJson(options);
        }

        public string convertToJson(Dictionary<string, object> dict)
        {
            if (dict == null) return "{}";

            return Json.Serialize(dict);
        }
    }
}