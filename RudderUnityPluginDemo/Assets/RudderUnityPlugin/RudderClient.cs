#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Rudderlabs
{


    public class RudderClient
    {

#if UNITY_ANDROID
    private static readonly string androidClientName = "com.rudderlabs.android.sdk.core.RudderClient";
    private static AndroidJavaClass androidClientClass;
#endif

#if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void _initiateInstance(
            string _writeKey,
            string _endpointUri,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout
        );
        [DllImport("__Internal")]
        private static extern void _logEvent(
            string eventType,
            string eventName,
            string userId,
            string eventPropertiesJson,
            string userPropertiesJson,
            string integrationsJson
        );
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
            string _endpointUri,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout
        )
        {
            // initialize android
#if UNITY_ANDROID
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
                _endpointUri, 
                _flushQueueSize, 
                _dbCountThreshold, 
                _sleepTimeout
            );
        }
#endif

#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _initiateInstance(
                    _writeKey,
                    _endpointUri,
                    _flushQueueSize,
                    _dbCountThreshold,
                    _sleepTimeout
                );
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
                // initialize the instance
                instance = new RudderClient(
                    writeKey,
                    config.endPointUrl,
                    config.flushQueueSize,
                    config.dbCountThreshold,
                    config.sleepTimeOut
                );

                integrationManager = new RudderIntegrationManager(
                    writeKey,
                    config
                );
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
            message.integrations = integrationManager.getIntegrations();
            integrationManager.makeIntegrationDump(message);
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "_logEvent",
                "track",
                message.eventName,
                message.userId,
                message.getEventPropertiesJson(),
                message.getUserPropertiesJson(),
                message.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _logEvent(
                    "track",
                    message.eventName,
                    message.userId,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getIntegrationsJson()
                );
            }
#endif
        }

        public void Page(RudderMessage message)
        {
            message.integrations = integrationManager.getIntegrations();
            integrationManager.makeIntegrationDump(message);
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "_logEvent",
                "page",
                message.eventName,
                message.userId,
                message.getEventPropertiesJson(),
                message.getUserPropertiesJson(),
                message.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _logEvent(
                    "page",
                    message.eventName,
                    message.userId,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getIntegrationsJson()
                );
            }
#endif
        }

        public void Screen(RudderMessage message)
        {
            message.integrations = integrationManager.getIntegrations();
            integrationManager.makeIntegrationDump(message);
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "_logEvent",
                "screen",
                message.eventName,
                message.userId,
                message.getEventPropertiesJson(),
                message.getUserPropertiesJson(),
                message.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _logEvent(
                    "screen",
                    message.eventName,
                    message.userId,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getIntegrationsJson()
                );
            }
#endif
        }

        public void Identify(RudderMessage message)
        {
            message.integrations = integrationManager.getIntegrations();
            integrationManager.makeIntegrationDump(message);
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "_logEvent",
                "identify",
                message.eventName,
                message.userId,
                message.getEventPropertiesJson(),
                message.getUserPropertiesJson(),
                message.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _logEvent(
                    "identify",
                    message.eventName,
                    message.userId,
                    message.getEventPropertiesJson(),
                    message.getUserPropertiesJson(),
                    message.getIntegrationsJson()
                );
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
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _serializeSqlite();
            }
#endif
        }
    }
}