using System.Collections.Generic;

class RudderAdjustIntegrationFactory : RudderIntegrationFactory
{
    public override RudderIntegration create(Dictionary<string, object> config, RudderClient client)
    {
        return new RudderAdjustIntegration(config, client);
    }

    public override string key()
    {
        return "ADJ";
    }

    private static RudderAdjustIntegrationFactory instance;
    public static RudderAdjustIntegrationFactory getFactory()
    {
        if (instance == null)
        {
            instance = new RudderAdjustIntegrationFactory();
        }
        return instance;
    }
}