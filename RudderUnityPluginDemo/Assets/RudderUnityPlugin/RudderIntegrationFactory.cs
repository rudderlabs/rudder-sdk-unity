using System.Collections.Generic;
namespace Rudderlabs
{
    public abstract class RudderIntegrationFactory
    {
        public abstract RudderIntegration create(Dictionary<string, object> config, RudderClient client);
        public abstract string key();
    }
}