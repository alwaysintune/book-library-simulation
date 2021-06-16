using BookLibrary.ConsoleApp.UI.ConsoleUI;

namespace BookLibrary.ConsoleApp
{
    class Program
    {
        private static readonly ICLInterface _cli = new CLInterface();

        static void Main(string[] args)
        {
            _cli.Start();
        }
    }
}
