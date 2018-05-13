using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Caredev.MegoTools.ViewModel
{
    public abstract class ConnectionBaseViewModel : ViewModelBase
    {
        public ConnectionBaseViewModel(CreateConnectionViewModel parent)
        {
            Parent = parent;
            Builder.ConnectionString = parent.Database.DefalutConnectionString;
        }

        public CreateConnectionViewModel Parent { get; }

        public abstract DbConnectionStringBuilder Builder { get; }

        protected void Set(string name, object value, [CallerMemberName]string propertyName = null)
        {
            if (Builder[name] != value)
            {
                Builder[name] = value;
                OnPropertyChanged(propertyName);
            }
        }

        public void Reset() => OnPropertyChanged(string.Empty);
    }
}
