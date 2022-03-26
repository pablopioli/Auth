namespace MemoryStorage.Contracts
{
    internal class Scope
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Properties { get; set; } = string.Empty;
        public List<string> Resources { get; set; } = new List<string>();

        internal Domain.Scope ToApplication()
        {
            return new Domain.Scope
            {
                Id = Id,
                Description = Description,
                Name = Name,
                DisplayName = DisplayName,
                Properties = Properties,
                Resources = Resources
            };
        }
    }
}
