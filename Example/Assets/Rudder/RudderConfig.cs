using System.Collections.Generic;

namespace RudderStack
{
    public class RudderConfig
    {
        public string dataPlaneUrl;
        public string controlPlaneUrl;
        public int flushQueueSize;
        public int dbCountThreshold;
        public int sleepTimeOut;
        public List<RudderIntegrationFactory> factories;
        public int logLevel;
        public int configRefreshInterval;
        public bool autoCollectAdvertId;
        public bool trackLifecycleEvents;
        public bool recordScreenViews;
        public string configPlaneUrl;

        public RudderConfig(
            string dataPlaneUrl,
            string controlPlaneUrl,
            int flushQueueSize,
            int dbCountThreshold,
            int sleepTimeOut,
            int logLevel,
            int configRefreshInterval,
            bool autoCollectAdvertId,
            bool trackLifecycleEvents,
            bool recordScreenViews,
            List<RudderIntegrationFactory> factories
        )
        {
            this.dataPlaneUrl = dataPlaneUrl;
            this.controlPlaneUrl = controlPlaneUrl;
            this.flushQueueSize = flushQueueSize;
            this.dbCountThreshold = dbCountThreshold;
            this.sleepTimeOut = sleepTimeOut;
            this.factories = factories;
            this.logLevel = logLevel;
            this.configRefreshInterval = configRefreshInterval;
            this.autoCollectAdvertId = autoCollectAdvertId;
            this.trackLifecycleEvents = trackLifecycleEvents;
            this.recordScreenViews = recordScreenViews;

            RudderLogger.Init(logLevel);
        }

        public RudderConfig()
        {
            new RudderConfig(
                Constants.DATA_PLANE_URL,
                Constants.CONTROL_PLANE_URL,
                Constants.FLUSH_QUEUE_SIZE,
                Constants.DB_COUNT_THRESHOLD,
                Constants.SLEEP_TIME_OUT,
                RudderLogLevel.NONE,
                Constants.CONFIG_REFRESH_INTERVAL,
                Constants.AUTO_COLLECT_ADVERTID,
                Constants.TRACK_LIFECYCLE_EVENTS,
                Constants.RECORD_SCREEN_VIEWS,
                null
            );
        }
    }
}
