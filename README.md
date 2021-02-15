# What is RudderStack?

[RudderStack](https://rudderstack.com/) is a **customer data pipeline** tool for collecting, routing and processing data from your websites, apps, cloud tools, and data warehouse.

More information on RudderStack can be found [here](https://github.com/rudderlabs/rudder-server).

## Getting Started with Unity SDK

Download the SDK plugin file: ```rudder-sdk-unity.unitypackage``` from the ```SDK``` directory and import it in your project.

## Initialize ```RudderClient```

Put this code under the ```Awake``` method of your main scene.

```
RudderConfig config = new RudderConfigBuilder()
.WithDataPlaneUrl(<DATAPLANE_URI>)
.Build();
RudderClient rudderClient = RudderClient.GetInstance(<WRITE_KEY>, config);
```

## Send Events

A simple track event is as shown: 
```
RudderMessage message = new RudderMessageBuilder()
  .WithEventName("test_event_name")
  .WithEventProperty("test_key_1", "test_value_1")
  .WithEventProperty("test_key_2", "test_value_2")
  .WithUserProperty("test_user_key_1", "test_user_value_1")
  .WithUserProperty("test_user_key_2", "test_user_value_2")
  .Build();
rudderClient.Track(message);
```

Other options are ```Screen``` and ```Identify```

For more details, check out our [documentation](https://docs.rudderstack.com/rudderstack-sdk-integration-guides/getting-started-with-unity-sdk).

## Contact Us

If you come across any issues while using the RudderStack Unity SDK, please feel free to [contact us](https://rudderstack.com/contact/) or start a conversation on our [Slack](https://resources.rudderstack.com/join-rudderstack-slack) channel. We will be happy to help you.
