namespace BookLibrary.ConsoleApp.Entities
{
    public class BookInventory
    {
        public int TakenCount { get; set; }

        public int AvailableCount { get; set; }

        public int TotalCount { get => TakenCount + AvailableCount; }

        public override string ToString()
        {
            return $"{nameof(TakenCount)}: {TakenCount} out of {TotalCount} \n" +
                $"{nameof(AvailableCount)}: {AvailableCount} out of {TotalCount}";
        }
    }
}
