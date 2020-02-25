using System.Collections.Generic;

namespace RudderStack
{
    public class RudderConfig
    {
        public string endPointUrl;
        public int flushQueueSize;
        public int dbCountThreshold;
        public int sleepTimeOut;
        public List<RudderIntegrationFactory> factories;
        public int logLevel;
        public int configRefreshInterval;
        public bool trackLifecycleEvents;
        public bool recordScreenViews;
        public string configPlaneUrl;

        public RudderConfig(
            string endPointUrl,
            int flushQueueSize,
            int dbCountThreshold,
            int sleepTimeOut,
            int logLevel,
            int configRefreshInterval,
            bool trackLifecycleEvents,
            bool recordScreenViews,
            string configPlaneUrl,
            List<RudderIntegrationFactory> factories)
        {
            this.endPointUrl = endPointUrl;
            this.flushQueueSize = flushQueueSize;
            this.dbCountThreshold = dbCountThreshold;
            this.sleepTimeOut = sleepTimeOut;
            this.factories = factories;
            this.logLevel = logLevel;
            this.configRefreshInterval = configRefreshInterval;
            this.trackLifecycleEvents = trackLifecycleEvents;
            this.recordScreenViews = recordScreenViews;
            this.configPlaneUrl = configPlaneUrl;

            RudderLogger.Init(logLevel);
        }

        public RudderConfig()
        {
            new RudderConfig(
                Constants.BASE_URL,
                Constants.FLUSH_QUEUE_SIZE,
                Constants.DB_COUNT_THRESHOLD,
                Constants.SLEEP_TIME_OUT,
                RudderLogLevel.NONE,
                Constants.CONFIG_REFRESH_INTERVAL,
                Constants.TRACK_LIFECYCLE_EVENTS,
                Constants.RECORD_SCREEN_VIEWS,
                Constants.CONFIG_PLANE_URL,
                null
            );
        }
    }
}