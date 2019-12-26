#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using Rudderlabs.MiniJSON;

namespace Rudderlabs
{
    public class RudderClient : MonoBehaviour
    {

#if UNITY_ANDROID
        private static readonly string androidClientName = "com.rudderlabs.android.sdk.core.RudderClientWrapper";
        private static AndroidJavaClass androidClientClass;
#endif

#if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void _initiateInstance(
            string _writeKey,
            string _endPointUrl,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout,
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
#endif

        private static RudderClient instance;
        private static RudderIntegrationManager integrationManager;
        /* 
        private constructor to prevent instantiating
         */
        private RudderClient(
            string _writeKey,
            string _endPointUrl,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout,
            int _logLevel
        )
        {
            // initialize android
#if UNITY_ANDROID
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
                    _writeKey,
                    _endPointUrl,
                    _flushQueueSize,
                    _dbCountThreshold,
                    _sleepTimeout,
                    _logLevel
                );
                RudderLogger.LogDebug("Android Core SDK initiated");
            }
#endif

#if UNITY_IPHONE
            RudderLogger.LogDebug("Initializing iOS Core SDK");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _initiateInstance(
                    _writeKey,
                    _endPointUrl,
                    _flushQueueSize,
                    _dbCountThreshold,
                    _sleepTimeout,
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
            if (instance == null)
            {
                RudderLogger.LogDebug("Instantiating RudderClient SDK");
                // initialize the instance
                instance = new RudderClient(
                    writeKey,
                    config.endPointUrl,
                    config.flushQueueSize,
                    config.dbCountThreshold,
                    config.sleepTimeOut,
                    config.logLevel
                );

                RudderLogger.LogDebug("Instantiating RudderIntegrationManager");
                integrationManager = new RudderIntegrationManager(
                    writeKey,
                    config
                );
            } else {
                RudderLogger.LogDebug("RudderClient SDK is already initiated");
            }

            return instance;
        }

        public static RudderClient GetInstance(string writeKey)
        {
            return GetInstance(writeKey, new RudderConfig());
        }

        public static RudderClient GetInstance(string writeKey, string endPointUri)
        {
            RudderConfig config = new RudderConfigBuilder().WithEndPointUrl(endPointUri).Build();
            return GetInstance(writeKey, config);
        }

        public void Track(RudderMessage message)
        {
            RudderLogger.LogDebug("Track Event: " + message.eventName);
            integrationManager.makeIntegrationDump(message);
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
            integrationManager.makeIntegrationDump(message);
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
            integrationManager.makeIntegrationDump(message);

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

        public static RudderClient GetInstance()
        {
            return instance;
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
            if (integrationManager != null)
            {
                integrationManager.Update();
            }
        }
    }
}