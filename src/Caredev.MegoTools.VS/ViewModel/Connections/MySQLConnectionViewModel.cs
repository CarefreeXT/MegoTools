using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class MySQLConnectionViewModel : ConnectionBaseViewModel
    {
        public MySQLConnectionViewModel(CreateConnectionViewModel parent)
          : base(parent)
        {

        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly MySqlConnectionStringBuilder _Builder = new MySqlConnectionStringBuilder();

        public string Host
        {
            get => _Builder.Server;
            set => Set("Server", value);
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
            get => _Builder.Database;
            set => Set("Database", value);
        }

        public IEnumerable<string> Databases
        {
            get => _Databases;
            set
            {
                DialogHelper.Execute(() => Set(ref _Databases, value));
            }
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
                        var builder = new MySqlConnectionStringBuilder(LastConnection);
                        builder.Database = "mysql";
                        using (var con = new MySqlConnection(builder.ToString()))
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
