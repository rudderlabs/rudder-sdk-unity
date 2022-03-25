using System.Collections.Generic;

namespace RudderStack
{
    class RudderConfigBuilder
    {
        private string dataPlaneUrl = Constants.DATA_PLANE_URL;
        public RudderConfigBuilder WithDataPlaneUrl(string dataPlaneUrl)
        {
            this.dataPlaneUrl = dataPlaneUrl;
            return this;
        }

        private List<RudderIntegrationFactory> factories = new List<RudderIntegrationFactory>();
        public RudderConfigBuilder WithFactory(RudderIntegrationFactory factory)
        {
            this.factories.Add(factory);
            return this;
        }

        public RudderConfigBuilder WithFactories(List<RudderIntegrationFactory> factories)
        {
            this.factories.AddRange(factories);
            return this;
        }

        private int flushQueueSize = Constants.FLUSH_QUEUE_SIZE;
        public RudderConfigBuilder WithFlushQueueSize(int flushQueueSize)
        {
            this.flushQueueSize = flushQueueSize;
            return this;
        }

        private int dbCountThreshold = Constants.DB_COUNT_THRESHOLD;
        public RudderConfigBuilder WithDBCountThreshold(int dbCountThreshold)
        {
            this.dbCountThreshold = dbCountThreshold;
            return this;
        }

        private int sleepTimeOut = Constants.SLEEP_TIME_OUT;
        public RudderConfigBuilder WithSleepTimeout(int sleepTimeOut)
        {
            this.sleepTimeOut = sleepTimeOut;
            return this;
        }

        private int logLevel = RudderLogLevel.NONE;
        public RudderConfigBuilder WithLogLevel(int logLevel)
        {
            this.logLevel = logLevel;
            return this;
        }

        private int configRefreshInterval = Constants.CONFIG_REFRESH_INTERVAL;
        public RudderConfigBuilder WithConfigRefreshInterval(int configRefreshInterval)
        {
            this.configRefreshInterval = configRefreshInterval;
            return this;
        }

        private bool autoCollectAdvertId = Constants.AUTO_COLLECT_ADVERTID;
        public RudderConfigBuilder withAutoCollectAdvertId(bool autoCollectAdvertId)
        {
            this.autoCollectAdvertId = autoCollectAdvertId;
            return this;
        }
        private bool trackLifecycleEvents = Constants.TRACK_LIFECYCLE_EVENTS;
        public RudderConfigBuilder WithTrackLifecycleEvents(bool trackLifecycleEvents)
        {
            this.trackLifecycleEvents = trackLifecycleEvents;
            return this;
        }

        private bool recordScreenViews = Constants.RECORD_SCREEN_VIEWS;
        public RudderConfigBuilder WithRecordScreenViews(bool recordScreenViews)
        {
            this.recordScreenViews = recordScreenViews;
            return this;
        }

        private string controlPlaneUrl = Constants.CONTROL_PLANE_URL;
        public RudderConfigBuilder WithControlPlaneUrl(string controlPlaneUrl) {
            this.controlPlaneUrl = controlPlaneUrl;
            return this;
        }

        public RudderConfig Build()
        {
            return new RudderConfig(
                this.dataPlaneUrl,
                this.controlPlaneUrl,
                this.flushQueueSize,
                this.dbCountThreshold,
                this.sleepTimeOut,
                this.logLevel,
                this.configRefreshInterval,
                this.autoCollectAdvertId,
                this.trackLifecycleEvents,
                this.recordScreenViews,
                this.factories
            );
        }
    }
}
