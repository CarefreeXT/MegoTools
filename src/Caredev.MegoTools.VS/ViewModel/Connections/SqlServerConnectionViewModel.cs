using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class SqlServerConnectionViewModel : ConnectionBaseViewModel
    {
        public SqlServerConnectionViewModel(CreateConnectionViewModel parent)
            : base(parent)
        {

        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly SqlConnectionStringBuilder _Builder = new SqlConnectionStringBuilder();

        public string Host
        {
            get => _Builder.DataSource;
            set => Set("Data Source", value);
        }

        public bool IntegratedSecurity
        {
            get => _Builder.IntegratedSecurity;
            set => Set("Integrated Security", value);
        }

        public string UserName
        {
            get => _Builder.UserID;
            set => Set("User ID", value);
        }

        public string Password
        {
            get => _Builder.Password;
            set => Set("Password", value);
        }

        public string Database
        {
            get => _Builder.InitialCatalog;
            set => Set("Initial Catalog", value);
        }

        public IEnumerable<string> Databases
        {
            get
            {
                GetDatabases();
                return _Databases;
            }
            set => DialogHelper.Execute(() => Set(ref _Databases, value));
        }
        private IEnumerable<string> _Databases;

        private string LastConnection = string.Empty;
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            GetDatabases();
        }

        private void GetDatabases()
        {
            if (LastConnection != _Builder.ToString())
            {
                LastConnection = _Builder.ToString();
                if (!string.IsNullOrEmpty(LastConnection))
                {
                    Task.Factory.StartNew(delegate ()
                    {
                        try
                        {
                            var builder = new SqlConnectionStringBuilder(LastConnection);
                            if (!string.IsNullOrEmpty(builder.InitialCatalog))
                            {
                                builder.InitialCatalog = "master";
                            }
                            using (var con = new SqlConnection(builder.ToString()))
                            {
                                con.Open();
                                var table = con.GetSchema("Databases");
                                Databases = table.AsEnumerable().Select(a => a.Field<string>("database_name")).ToArray();
                            }
                        }
                        catch
                        {
                        }
                    });
                }
            }
        }
    }
}