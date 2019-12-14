using System.Collections.Generic;

namespace Rudderlabs
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

        public RudderConfig(
            string endPointUrl,
            int flushQueueSize,
            int dbCountThreshold,
            int sleepTimeOut,
            int logLevel,
            int configRefreshInterval,
            List<RudderIntegrationFactory> factories)
        {
            this.endPointUrl = endPointUrl;
            this.flushQueueSize = flushQueueSize;
            this.dbCountThreshold = dbCountThreshold;
            this.sleepTimeOut = sleepTimeOut;
            this.factories = factories;
            this.logLevel = logLevel;
            this.configRefreshInterval = configRefreshInterval;

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
                null
            );
        }
    }
}