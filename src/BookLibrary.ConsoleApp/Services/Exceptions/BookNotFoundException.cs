namespace BookLibrary.ConsoleApp.Services.Exceptions
{
    [System.Serializable]
    public class BookNotFoundException : System.Exception
    {
        public BookNotFoundException() { }
        public BookNotFoundException(string message) : base(message) { }
        public BookNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected BookNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
