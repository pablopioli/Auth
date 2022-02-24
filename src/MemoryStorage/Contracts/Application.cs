namespace MemoryStorage.Contracts;

internal class Application
{
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AppType { get; set; } = string.Empty;
    public string ConsentType { get; set; } = string.Empty;
    public List<string> RedirectUris { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public List<string> Requirements { get; set; } = new();

    internal Domain.Application ToApplication()
    {
        return new Domain.Application
        {
            Id = Id,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            Type = AppType,
            ConsentType = ConsentType,
            RedirectUris = RedirectUris,
            Permissions = Permissions,
            Requirements = Requirements
        };
    }
}
