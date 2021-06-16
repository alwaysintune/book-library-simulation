namespace BookLibrary.ConsoleApp.Services.Exceptions
{
    [System.Serializable]
    public class BookRecordNotFoundException : System.Exception
    {
        public BookRecordNotFoundException() { }
        public BookRecordNotFoundException(string message) : base(message) { }
        public BookRecordNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected BookRecordNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
