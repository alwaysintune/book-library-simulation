namespace BookLibrary.ConsoleApp.Services.Exceptions
{
    [System.Serializable]
    public class BusinessRuleException : System.Exception
    {
        public BusinessRuleException() { }
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string message, System.Exception inner) : base(message, inner) { }
        protected BusinessRuleException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
