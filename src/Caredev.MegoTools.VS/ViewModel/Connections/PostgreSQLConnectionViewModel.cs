using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npgsql;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class PostgreSQLConnectionViewModel : ConnectionBaseViewModel
    {
        public PostgreSQLConnectionViewModel(CreateConnectionViewModel parent)
           : base(parent)
        {

        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly NpgsqlConnectionStringBuilder _Builder = new NpgsqlConnectionStringBuilder();

        public string Host
        {
            get => _Builder.Host;
            set => Set("Host", value);
        }

        public bool IntegratedSecurity
        {
            get => _Builder.IntegratedSecurity;
            set => Set("Integrated Security", value);
        }

        public string UserName
        {
            get => _Builder.Username;
            set => Set("Username", value);
        }

        public string Password
        {
            get => _Builder.Password;
            set => Set("Password", value);
        }

        public string Database
        {
            get => _Builder.Database;
            set => Set("Database", value);
        }

        public IEnumerable<string> Databases
        {
            get => _Databases;
            set => DialogHelper.Execute(() => Set(ref _Databases, value));
        }
        private IEnumerable<string> _Databases;

        private string LastConnection = string.Empty;
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (LastConnection != _Builder.ToString())
            {
                LastConnection = _Builder.ToString();
                Task.Factory.StartNew(delegate ()
                {
                    try
                    {
                        var builder = new NpgsqlConnectionStringBuilder(LastConnection);
                        builder.Database = "postgres";
                        using (var con = new NpgsqlConnection(builder.ToString()))
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
