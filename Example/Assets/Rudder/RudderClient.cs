#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using System;
using System.Collections.Generic;
using RudderStack.MiniJSON;

namespace RudderStack
{
    public class RudderClient : MonoBehaviour
    {
        private static RudderClient _instance;
#if UNITY_ANDROID
        private static bool isSDKInitialized;
        private static bool fromBackGround = false;
        private static bool trackLifeCycleEvents = true;
        
        private static List<Action> actionsList = new List<Action>();

        void OnApplicationFocus(bool focus)
        {
            Action action;

            if (trackLifeCycleEvents)
            {
                if (focus)
                {
                    action = () =>
                    {
                        RudderLogger.LogDebug("Tracking event Application Opened");
                        Dictionary<string, object> eventProperties = new Dictionary<string, object>();
                        eventProperties.Add("from_background", fromBackGround);
                        RudderMessage message = new RudderMessageBuilder().WithEventName("Application Opened").WithEventProperties(eventProperties).Build();
                        _instance.Track(message);

                    };
                }
                else
                {
                    fromBackGround = true;
                    action = () =>
                    {
                        RudderLogger.LogDebug("Tracking event Application Backgrounded");
                        RudderMessage message = new RudderMessageBuilder().WithEventName("Application Backgrounded").Build();
                        _instance.Track(message);

                    };
                }

                if (isSDKInitialized)
                {
                    RudderLogger.LogDebug("SDK Already initialized, executing the actions directly");
                    action();
                }
                else
                {
                    RudderLogger.LogDebug("SDK not initialized yet, adding the actions to the list");
                    actionsList.Add(action);
                }
            }
        }
#endif



#if UNITY_ANDROID
        private static readonly string androidClientName = "com.rudderstack.android.sdk.wrapper.RudderClientWrapper";
        private static AndroidJavaClass androidClientClass;
#endif

#if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void _initiateInstance(
            string _anonymousId,
            string _writeKey,
            string _dataPlaneUrl,
            string _controlPlaneUrl,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout,
            int _configRefreshInterval,
            bool _trackLifecycleEvents,
            bool _recordScreenViews,
            int _logLevel
        );
        [DllImport("__Internal")]
        private static extern void _logEvent(
            string _eventType,
            string _eventName,
            string _eventPropsJson,
            string _userPropsJson,
            string _optionsJson
        );
        [DllImport("__Internal")]
        private static extern void _identify(
            string _userId,
            string _traitsJson,
            string _optionsJson
        );
        [DllImport("__Internal")]
        private static extern void _reset();
        [DllImport("__Internal")]
        private static extern void _serializeSqlite();
        [DllImport("__Internal")]
        private static extern void _setAnonymousId(string _anonymousId);
#endif

        private static RudderIntegrationManager _integrationManager;

        /*
        private constructor to prevent instantiating
         */
        private RudderClient(
            string _writeKey,
            string _dataPlaneUrl,
            string _controlPlaneUrl,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout,
            int _configRefreshInterval,
            bool _autoCollectAdvertId,
            bool _trackLifecycleEvents,
            bool _recordScreenViews,
            int _logLevel
        )
        {
            // initialize android
#if UNITY_ANDROID
            trackLifeCycleEvents = _trackLifecycleEvents;
            RudderLogger.LogDebug("Initializing Android Core SDK");
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                androidClientClass = new AndroidJavaClass(androidClientName);
                androidClientClass.CallStatic(
                    "_initiateInstance",
                    context,
                    RudderCache.GetAnonymousId(),
                    _writeKey,
                    _dataPlaneUrl,
                    _controlPlaneUrl,
                    _flushQueueSize,
                    _dbCountThreshold,
                    _sleepTimeout,
                    _configRefreshInterval,
                    _autoCollectAdvertId,
                    _trackLifecycleEvents,
                    _recordScreenViews,
                    _logLevel
                );
                RudderLogger.LogDebug("Android Core SDK initiated");
            }
#endif

            // initialize iOS
#if UNITY_IPHONE
            RudderLogger.LogDebug("Initializing iOS Core SDK");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _initiateInstance(
                    RudderCache.GetAnonymousId(),
                    _writeKey,
                    _dataPlaneUrl,
                    _controlPlaneUrl,
                    _flushQueueSize,
                    _dbCountThreshold,
                    _sleepTimeout,
                    _configRefreshInterval,
                    _trackLifecycleEvents,
                    _recordScreenViews,
                    _logLevel
                );
                RudderLogger.LogDebug("iOS Core SDK initiated");
            }
#endif
        }

        public static RudderClient GetInstance(
            string writeKey,
            RudderConfig config
        )
        {
            if (_instance == null)
            {
                // initialize the cache
                RudderCache.Init();

                RudderLogger.LogDebug("Instantiating RudderClient SDK");
                // initialize the instance
                _instance = new RudderClient(
                    writeKey,
                    config.dataPlaneUrl,
                    config.controlPlaneUrl,
                    config.flushQueueSize,
                    config.dbCountThreshold,
                    config.sleepTimeOut,
                    config.configRefreshInterval,
                    config.autoCollectAdvertId,
                    config.trackLifecycleEvents,
                    config.recordScreenViews,
                    config.logLevel
                );

#if UNITY_ANDROID
                if (config.trackLifecycleEvents)
                {
                    foreach (Action action in actionsList)
                    {
                        RudderLogger.LogDebug("SDK Initialized, executing all the actions in the list");
                        action();
                    }
                }
                RudderLogger.LogDebug("Clearing all the actions in the action list");
                actionsList.Clear();
                isSDKInitialized = true;
#endif

                RudderLogger.LogDebug("Instantiating RudderIntegrationManager");
                _integrationManager = new RudderIntegrationManager(
                    writeKey,
                    config
                );
            }
            else
            {
                RudderLogger.LogDebug("RudderClient SDK is already initiated");
            }

            return _instance;
        }

        public static RudderClient GetInstance(string writeKey)
        {
            return GetInstance(writeKey, new RudderConfig());
        }

        public static RudderClient GetInstance(string writeKey, string dataPlaneUrl)
        {
            RudderConfig config = new RudderConfigBuilder().WithDataPlaneUrl(dataPlaneUrl).Build();
            return GetInstance(writeKey, config);
        }

        public void Track(RudderMessage message)
        {
            RudderLogger.LogDebug("Track Event: " + message.eventName);
            if (_integrationManager != null)
            {
                _integrationManager.MakeIntegrationDump(message);
            }

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                androidClientClass.CallStatic(
                    "_logEvent",
                    "track",
                    message.eventName,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getOptionsJson()
                );
            }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _logEvent(
                    "track",
                    message.eventName,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getOptionsJson()
                );
            }
#endif
        }

        public void Screen(RudderMessage message)
        {
            RudderLogger.LogDebug("Screen Event: " + message.eventName);
            if (_integrationManager != null)
            {
                _integrationManager.MakeIntegrationDump(message);
            }
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                androidClientClass.CallStatic(
                    "_logEvent",
                    "screen",
                    message.eventName,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getOptionsJson()
                );
            }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _logEvent(
                    "screen",
                    message.eventName,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getOptionsJson()
                );
            }
#endif
        }

        public void Identify(string userId, RudderTraits traits, RudderMessage message)
        {
            RudderLogger.LogDebug("Identify Event: " + message.eventName);
            RudderCache.SetUserId(userId);
            if (_integrationManager != null)
            {
                _integrationManager.MakeIntegrationIdentify(userId, traits);
            }

            // put supplied userId under traits as well if it is not set
            if (traits.getId() == null)
            {
                traits.PutId(userId);
            }
            string traitsJson = Json.Serialize(traits.traitsDict);
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                androidClientClass.CallStatic(
                    "_identify",
                    userId,
                    traitsJson,
                    message.getOptionsJson()
                );
            }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _identify(
                    userId,
                    traitsJson,
                    message.getOptionsJson()
                );
            }
#endif
        }

        public void Reset()
        {
            RudderLogger.LogDebug("SDK reset");
            if (_integrationManager != null)
            {
                _integrationManager.Reset();
            }
            RudderCache.Reset();
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                androidClientClass.CallStatic(
                    "_reset"
                );
            }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _reset();
            }
#endif
        }

        public void setAnonymousId(string _anonymousId)
        {
            RudderLogger.LogDebug("SetAnonymousId: " + _anonymousId);
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                androidClientClass.CallStatic(
                    "_setAnonymousId",
                    _anonymousId
                );
            }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _setAnonymousId(_anonymousId);
            }
#endif
        }

        public static RudderClient GetInstance()
        {
            return _instance;
        }

        public static void SerializeSqlite()
        {
#if UNITY_IPHONE
            RudderLogger.LogDebug("SQLite Serialized");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _serializeSqlite();
            }
#endif
        }
        void Update()
        {
            if (_integrationManager != null)
            {
                _integrationManager.Update();
            }
        }
    }
}
