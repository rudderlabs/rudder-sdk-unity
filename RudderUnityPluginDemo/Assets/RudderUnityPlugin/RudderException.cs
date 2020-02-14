namespace RudderStack
{
    [System.Serializable]
    public class RudderException : System.Exception
    {
        public RudderException() { }
        public RudderException(string message) : base(message) { }
        public RudderException(string message, System.Exception inner) : base(message, inner) { }
        protected RudderException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}