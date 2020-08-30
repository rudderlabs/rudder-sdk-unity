namespace RudderStack
{
    public abstract class RudderIntegration
    {
        public abstract void Dump(RudderMessage message);
        public abstract void Identify(string userId, RudderTraits traits);
        public abstract void Reset();
    }
}