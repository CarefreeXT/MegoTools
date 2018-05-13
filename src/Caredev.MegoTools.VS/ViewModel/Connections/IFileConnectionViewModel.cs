namespace Caredev.MegoTools.ViewModel.Connections
{
    public interface IFileConnectionViewModel
    {

        string FileName { get; set; }

        string Password { get; set; }

        string DefaultExt { get; }

        string Filter { get; }
    }
}