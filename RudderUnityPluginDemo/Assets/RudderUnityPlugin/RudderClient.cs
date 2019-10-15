#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif
using UnityEngine;

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
#endif

    private static RudderClient instance;
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
                "initiateInstance", 
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
        string endPointUri,
        int flushQueueSize,
        int dbCountThreshold,
        int sleepTimeOut
    )
    {
        if (instance == null)
        {
            // initialize the instance
            instance = new RudderClient(
                writeKey, 
                endPointUri, 
                flushQueueSize, 
                dbCountThreshold, 
                sleepTimeOut
            );
        }

        return instance;
    }

    public static RudderClient GetInstance(string writeKey)
    {
        return GetInstance(
            writeKey,
            Constants.BASE_URL,
            Constants.FLUSH_QUEUE_SIZE,
            Constants.DB_COUNT_THRESHOLD,
            Constants.SLEEP_TIME_OUT
        );
    }

    public static RudderClient GetInstance(string writeKey, string endPointUri)
    {
        return GetInstance(
            writeKey,
            endPointUri,
            Constants.FLUSH_QUEUE_SIZE,
            Constants.DB_COUNT_THRESHOLD,
            Constants.SLEEP_TIME_OUT
        );
    }

    public static RudderClient GetInstance(string writeKey, string endPointUri, int flushQueueSize)
    {
        return GetInstance(
            writeKey,
            endPointUri,
            flushQueueSize,
            Constants.DB_COUNT_THRESHOLD,
            Constants.SLEEP_TIME_OUT
        );
    }

    public void Track(RudderElement element)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "logEvent",
                "track",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _logEvent(
                "track",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
    }

    public void Page(RudderElement element)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "logEvent",
                "page",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _logEvent(
                "page",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
    }

    public void Screen(RudderElement element)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "logEvent",
                "screen",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _logEvent(
                "screen",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
    }

    public void Identify(RudderElement element)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            androidClientClass.CallStatic(
                "logEvent",
                "identify",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
#if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _logEvent(
                "identify",
                element.eventName,
                element.userId,
                element.getEventPropertiesJson(),
                element.getUserPropertiesJson(),
                element.getIntegrationsJson()
            );
        }
#endif
    }
}