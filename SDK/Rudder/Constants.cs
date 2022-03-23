/*
    Default values for rudder client
*/
namespace RudderStack
{
    public static class Constants
    {
        // how often config should be fetched from the server (in hours) (2 hrs by default)
        public static int CONFIG_REFRESH_INTERVAL = 2;
        // default base url or rudder-backend-server
        public static string DATA_PLANE_URL = "https://hosted.rudderlabs.com";
        // default flush queue size for the events to be flushed to server
        public static int FLUSH_QUEUE_SIZE = 30;
        // default timeout for event flush
        // if events are registered and flushQueueSize is not reached
        // events will be flushed to server after sleepTimeOut seconds
        public static int SLEEP_TIME_OUT = 10;
        // default threshold of number of events to be persisted in sqlite db
        public static int DB_COUNT_THRESHOLD = 10000;
        // config-plane url to get the config for the writeKey
        public static string CONTROL_PLANE_URL = "https://api.rudderlabs.com";
        // whether we should automatically capture the advertisingId or not
        public static bool AUTO_COLLECT_ADVERTID = false;
        // whether we should track application life cycle events automatically or not
        public static bool TRACK_LIFECYCLE_EVENTS = true;
        // whether we should record screen views automatically or not
        public static bool RECORD_SCREEN_VIEWS = false;
    }
}
