namespace Caredev.MegoTools.Core
{
    public interface IConnectionInformation
    {
        string ConnectionString { get; }
        string ProviderName { get; }
    }
}