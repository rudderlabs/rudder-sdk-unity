using System.Collections.Generic;

namespace Rudderlabs
{
    class RudderAdjustIntegrationFactory : RudderIntegrationFactory
    {
        public override RudderIntegration Create(Dictionary<string, object> config, RudderClient client, RudderConfig rudderConfig)
        {
            RudderLogger.LogDebug("Creating RudderAdjustIntegrationFactory");
            return new RudderAdjustIntegration(config, client, rudderConfig);
        }

        public override string Key()
        {
            return "Adjust";
        }

        private static RudderAdjustIntegrationFactory instance;
        public static RudderAdjustIntegrationFactory GetFactory()
        {
            if (instance == null)
            {
                RudderLogger.LogDebug("Instantiating RudderAdjustIntegrationFactory");
                instance = new RudderAdjustIntegrationFactory();
            }
            return instance;
        }
    }
}