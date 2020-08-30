# What is Rudder?

**Short answer:**
Rudder is an open-source Segment alternative written in Go, built for the enterprise. .

**Long answer:**
Rudder is a platform for collecting, storing and routing customer event data to dozens of tools. Rudder is open-source, can run in your cloud environment (AWS, GCP, Azure or even your data-centre) and provides a powerful transformation framework to process your event data on the fly.

Released under [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)

## Getting Started with Unity SDK

Download the SDK plugin file: ```rudder-sdk-unity.unitypackage``` form the ```SDK``` directory and import in your project.

## Initialize ```RudderClient```
Put this code under the ```Awake``` method of your main scene.
```
RudderConfig config = new RudderConfigBuilder()
.WithDataPlaneUrl(<DATAPLANE_URI>)
.Build();
RudderClient rudderClient = RudderClient.GetInstance(<WRITE_KEY>, config);
```
## Send Events
A simple track event. Other options are ```Screen``` and ```Identify```
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

For more detailed documentation check [here](https://docs.rudderstack.com/sdk-integration-guide/getting-started-with-unity-sdk).

## Contact Us
If you come across any issues while configuring or using RudderStack, please feel free to [contact us](https://rudderstack.com/contact/) or start a conversation on our [Discord](https://discordapp.com/invite/xNEdEGw) channel. We will be happy to help you.
