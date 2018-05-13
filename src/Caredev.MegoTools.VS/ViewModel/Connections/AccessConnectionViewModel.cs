using System.Data.Common;
using System.Data.OleDb;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class AccessConnectionViewModel : ConnectionBaseViewModel, IFileConnectionViewModel
    {
        public AccessConnectionViewModel(CreateConnectionViewModel parent)
           : base(parent)
        {
        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly OleDbConnectionStringBuilder _Builder = new OleDbConnectionStringBuilder();

        public string DefaultExt => ".accdb";

        public string Filter => "Access Database(*.accdb)|*.accdb|Access 2000-2003 Database(*.mdb)|*.mdb|All files (*.*)|*.*";

        public string FileName
        {
            get => _Builder.DataSource;
            set => Set("Data Source", value);
        }

        public string Password
        {
            get => _Builder["Jet OLEDB:Database Password"] as string;
            set => Set("Jet OLEDB:Database Password", value);
        }
    }
}
