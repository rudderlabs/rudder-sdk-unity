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

        public RudderConfig(string endPointUrl, int flushQueueSize, int dbCountThreshold, int sleepTimeOut, List<RudderIntegrationFactory> factories)
        {
            this.endPointUrl = endPointUrl;
            this.flushQueueSize = flushQueueSize;
            this.dbCountThreshold = dbCountThreshold;
            this.sleepTimeOut = sleepTimeOut;
            this.factories = factories;
        }

        public RudderConfig()
        {
            new RudderConfig(Constants.BASE_URL, Constants.FLUSH_QUEUE_SIZE, Constants.DB_COUNT_THRESHOLD, Constants.SLEEP_TIME_OUT, null);
        }
    }
}