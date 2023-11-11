namespace DataBase
{
    public class UniqueAddressGenerator
    {
        private readonly LinkStorage storage;
        public UniqueAddressGenerator(LinkStorage storage)
        {
            this.storage = storage;
        }

        public string GetUniqueAddress(string primaryName)
        {
            var uniqueName = primaryName;
            var number = 1;
            while (storage.ContainsEntryWithAddress(uniqueName))
            {
                uniqueName = $"{primaryName}{number}";
                number++;
            }

            return uniqueName;
        }
    }
}