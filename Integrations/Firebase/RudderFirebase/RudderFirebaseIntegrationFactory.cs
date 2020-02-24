using System.Collections.Generic;

namespace RudderStack
{
    class RudderFirebaseIntegrationFactory : RudderIntegrationFactory
    {
        public override RudderIntegration Create(Dictionary<string, object> config, RudderClient client, RudderConfig rudderConfig)
        {
            RudderLogger.LogDebug("Creating RudderFirebaseIntegrationFactory");
            return new RudderFirebaseIntegration(config, client, rudderConfig);
        }

        public override string Key()
        {
            return "Firebase";
        }

        private static RudderFirebaseIntegrationFactory instance;
        public static RudderFirebaseIntegrationFactory GetFactory()
        {
            if (instance == null)
            {
                RudderLogger.LogDebug("Instantiating RudderFirebaseIntegrationFactory");
                instance = new RudderFirebaseIntegrationFactory();
            }
            return instance;
        }
    }
}