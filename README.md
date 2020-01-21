# What is Rudder?

**Short answer:** 
Rudder is an open-source Segment alternative written in Go, built for the enterprise. .

**Long answer:** 
Rudder is a platform for collecting, storing and routing customer event data to dozens of tools. Rudder is open-source, can run in your cloud environment (AWS, GCP, Azure or even your data-centre) and provides a powerful transformation framework to process your event data on the fly.

Released under [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)

## Getting Started with Unity SDK

Download the SDK plugin file: **```rudder-unity-plugin.unitypackage```** and import in your project.

## Initialize ```RudderClient```
Put this code under the ```Awake``` method of your main scene.
```
RudderConfig config = new RudderConfigBuilder()
.WithEndPointUrl(YOUR_DATAPLANE_URI)
.Build();
RudderClient rudderClient = RudderClient.GetInstance(YOUR_WRITE_KEY, config);
```
## Send Events
A simple track event. Other options are ```Screen```, ```Page```
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

## Adding Other Native SDKs

### [Adjust](https://adjust.com)
Import ```rudder-unity-extension-adjust.unitypackage``` in your current project. The extension package comes with [Adjust](https://adjust.com) SDK v4.15.0 

Now during intialization of ```RudderClient```:
```
RudderConfig config = new RudderConfigBuilder()
.WithEndPointUrl(YOUR_DATAPLANE_URI)
.WithFactory(RudderAdjustIntegrationFactory.getFactory())
.Build();
RudderClient rudderClient = RudderClient.GetInstance(YOUR_WRITE_KEY, config);
```

# Coming Soon
1. Native platform SDK integration support
2. More documentation
3. More destination support
