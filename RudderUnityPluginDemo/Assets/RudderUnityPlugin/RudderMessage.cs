using System.Collections.Generic;
using LitJson;
/* 
wrapper class for Unity to form event
*/
public class RudderMessage
{
    public string eventName;
    public string userId;
    public Dictionary<string, object> eventProperties;
    public Dictionary<string, object> userProperties;
    public Dictionary<string, object> integrations;

    public string getEventPropertiesJson()
    {
        return convertToJson(eventProperties);
    }

    public string getUserPropertiesJson()
    {
        return convertToJson(userProperties);
    }

    public string getIntegrationsJson()
    {
        return convertToJson(integrations);
    }

    public string convertToJson(Dictionary<string, object> dict)
    {
        if (dict == null) return "{}";

        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = false;
        JsonMapper.ToJson(dict, writer);
        return writer.ToString();
    }
}