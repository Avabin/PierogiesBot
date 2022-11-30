namespace GrainInterfaces.Wow;

public interface IWowCharacterGrain : IGrainWithStringKey
{
    Task RefreshAsync();

    Task<string>           GetServerAsync();
    Task<string>           GetRealmAsync();
    Task<string>           GetNameAsync();
    Task<WowCharacterView> GetViewAsync();
}