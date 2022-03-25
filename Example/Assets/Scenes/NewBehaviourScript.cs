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

        Dictionary<string, object> externalId1 = new Dictionary<string, object>();
        externalId1.Add("id", "some_external_id_1");
        externalId1.Add("type", "brazeExternalId");

        Dictionary<string, object> externalId2 = new Dictionary<string, object>();
        externalId2.Add("id", "some_external_id_2");
        externalId2.Add("type", "brazeExternalId2");

        List<Dictionary<string, object>> externalIds = new List<Dictionary<string, object>>();
        externalIds.Add(externalId1);
        externalIds.Add(externalId2);

        Dictionary<string, object> integrations = new Dictionary<string, object>();
        integrations.Add("All", true);
        integrations.Add("Amplitude", true);

        Dictionary<string, object> options = new Dictionary<string, object>();
        options.Add("integrations", integrations);
        options.Add("externalIds", externalIds);

        RudderConfig config = new RudderConfigBuilder()
          .WithDataPlaneUrl("https://9953-175-101-36-4.ngrok.io")
          .WithLogLevel(RudderLogLevel.VERBOSE)
          .Build();
        rudderClient = RudderClient.GetInstance("1pAKRv50y15Ti6UWpYroGJaO0Dj", config);

        // create event properties
        Dictionary<string, object> eventProperties = new Dictionary<string, object>();
        eventProperties.Add("test_key_1", "test_value_1");
        eventProperties.Add("test_key_2", "test_value_2");

        // create user properties
        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("test_u_key_1", "test_u_value_1");
        userProperties.Add("test_u_key_2", "test_u_value_2");

        // create message to track
        rudderClient.setAnonymousId("anonID1");
        RudderMessageBuilder builder = new RudderMessageBuilder();
        builder.WithEventName("test_event_name");
        builder.WithEventProperties(eventProperties);
        builder.WithUserProperties(userProperties);
        RudderMessage msg1 = builder.Build();
        msg1.options = options;

        rudderClient.Track(msg1);

        rudderClient.setAnonymousId("anonID2");
        // create message to track

        builder = new RudderMessageBuilder();
        builder.WithEventName("test_event_name_2");
        builder.WithEventProperty("foo", "bar");
        builder.WithUserProperty("foo1", "bar1");

        RudderMessage msg2 = builder.Build();
        msg2.options = options;
        rudderClient.Track(msg2);

        builder = new RudderMessageBuilder();
        builder.WithEventName("Home Screen");
        builder.WithEventProperty("First_visit", true);
        RudderMessage msg3 = builder.Build();
        rudderClient.Screen(msg3);

        RudderMessage identifyMessage = new RudderMessageBuilder().Build();
        identifyMessage.options = options;
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
            Dictionary<string, object> externalId1 = new Dictionary<string, object>();
            externalId1.Add("id", "some_external_id_1");
            externalId1.Add("type", "brazeExternalId");

            Dictionary<string, object> externalId2 = new Dictionary<string, object>();
            externalId2.Add("id", "some_external_id_2");
            externalId2.Add("type", "brazeExternalId2");

            List<Dictionary<string, object>> externalIds = new List<Dictionary<string, object>>();
            externalIds.Add(externalId1);
            externalIds.Add(externalId2);

            Dictionary<string, object> integrations = new Dictionary<string, object>();
            integrations.Add("All", true);
            integrations.Add("Amplitude", true);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("integrations", integrations);
            options.Add("externalIds", externalIds);

            // create message to track
            RudderMessageBuilder builder = new RudderMessageBuilder();
            builder.WithEventName("update_event");
            builder.WithEventProperty("foo", "bar");
            RudderMessage msg = builder.Build();
            msg.options = options;
            rudderClient.Track(msg);
        }
    }
}
