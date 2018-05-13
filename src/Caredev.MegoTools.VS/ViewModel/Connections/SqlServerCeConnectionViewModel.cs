using System.Data.Common;
using System.Data.SqlServerCe;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class SqlServerCeConnectionViewModel : ConnectionBaseViewModel, IFileConnectionViewModel
    {
        public SqlServerCeConnectionViewModel(CreateConnectionViewModel parent)
           : base(parent)
        {

        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly SqlCeConnectionStringBuilder _Builder = new SqlCeConnectionStringBuilder();

        public string DefaultExt => ".sdf";

        public string Filter => "SQL Server Compace (*.sdf)|*.sdf|All files (*.*)|*.*";

        public string FileName
        {
            get => _Builder.DataSource;
            set => Set("Data Source", value);
        }

        public string Password
        {
            get => _Builder.Password;
            set => Set("Password", value);
        }

        public void Create()
        {
            var engine = new SqlCeEngine
            {
                LocalConnectionString = Builder.ToString()
            };
            engine.CreateDatabase();
        }
    }
}
