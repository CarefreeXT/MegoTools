using Caredev.MegoTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.ViewModel
{
    public class ConnectionInfoViewModel : ViewModelBase, IConnectionInformation
    {
        public ConnectionInfoViewModel(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        public string ConnectionString { get; }

        public string ProviderName { get; }

        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        private string _Title;
    }
}
