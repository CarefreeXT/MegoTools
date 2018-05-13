using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class OracleConnectionViewModel : ConnectionBaseViewModel
    {
        public OracleConnectionViewModel(CreateConnectionViewModel parent)
           : base(parent)
        {

        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly OracleConnectionStringBuilder _Builder = new OracleConnectionStringBuilder();

        public string Host
        {
            get => _Builder.DataSource;
            set => Set("Data Source", value);
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
    }
}
