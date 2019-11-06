using System.Collections.Generic;

namespace Rudderlabs
{


    class RudderConfigBuilder
    {
        private string endPointUrl = Constants.BASE_URL;
        public RudderConfigBuilder WithEndPointUrl(string endPointUrl)
        {
            this.endPointUrl = endPointUrl;
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

        int dbCountThreshold = Constants.DB_COUNT_THRESHOLD;
        public RudderConfigBuilder WithDBCountThreshold(int dbCountThreshold)
        {
            this.dbCountThreshold = dbCountThreshold;
            return this;
        }

        int sleepTimeOut = Constants.SLEEP_TIME_OUT;
        public RudderConfigBuilder WithSleepTimeout(int sleepTimeOut)
        {
            this.sleepTimeOut = sleepTimeOut;
            return this;
        }

        public RudderConfig Build()
        {
            return new RudderConfig(
                this.endPointUrl,
                this.flushQueueSize,
                this.dbCountThreshold,
                this.sleepTimeOut,
                this.factories
            );
        }
    }
}