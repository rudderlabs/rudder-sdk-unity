using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RudderStack;

public class NewBehaviourScript : MonoBehaviour
{
    RudderClient rudderClient;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Info Here");

        RudderClient.SerializeSqlite();

        RudderConfig config = new RudderConfigBuilder()
          .WithDataPlaneUrl("https://8452ddb9ed62.ngrok.io")
          .WithLogLevel(RudderLogLevel.VERBOSE)
          .Build();
        rudderClient = RudderClient.GetInstance("1glg5JWDIVF1c90oLs6CDePrFy9", config);

        // create event properties
        Dictionary<string, object> eventProperties = new Dictionary<string, object>();
        eventProperties.Add("test_key_1", "test_value_1");
        eventProperties.Add("test_key_2", "test_value_2");

        // create user properties
        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("test_u_key_1", "test_u_value_1");
        userProperties.Add("test_u_key_2", "test_u_value_2");

        // create message to track
        RudderMessageBuilder builder = new RudderMessageBuilder();
        builder.WithEventName("test_event_name");
        builder.WithEventProperties(eventProperties);
        builder.WithUserProperties(userProperties);

        rudderClient.Track(builder.Build());

        // create message to track
        builder = new RudderMessageBuilder();
        builder.WithEventName("test_event_name");
        builder.WithEventProperty("foo", "bar");
        builder.WithUserProperty("foo1", "bar1");

        rudderClient.Track(builder.Build());

        RudderMessage identifyMessage = new RudderMessageBuilder().Build();
        RudderTraits traits = new RudderTraits().PutEmail("some@example.com");
        rudderClient.Identify("some_user_id", traits, identifyMessage);
    }

    int count = 0;

    // Update is called once per frame
    void Update()
    {
        count += 1;

        if (count % 100 == 0)
        {
            // create message to track
            RudderMessageBuilder builder = new RudderMessageBuilder();
            builder.WithEventName("update_event");
            builder.WithEventProperty("foo", "bar");

            rudderClient.Track(builder.Build());
        }
    }
}
