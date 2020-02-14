using System.Collections.Generic;
namespace RudderStack
{
    public abstract class RudderIntegrationFactory
    {
        public abstract RudderIntegration Create(Dictionary<string, object> config, RudderClient client, RudderConfig rudderConfig);
        public abstract string Key();
    }
}