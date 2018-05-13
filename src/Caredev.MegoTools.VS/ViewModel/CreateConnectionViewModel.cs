using Caredev.MegoTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Caredev.MegoTools.View;
using System.Windows;
using Res = Caredev.MegoTools.Properties.Resources;

namespace Caredev.MegoTools.ViewModel
{
    public class CreateConnectionViewModel : ViewModelBase
    {
        static CreateConnectionViewModel()
        {
            var query = typeof(CreateConnectionViewModel).Assembly.GetTypes()
                .Where(a => !a.IsAbstract && typeof(ConnectionBaseViewModel).IsAssignableFrom(a));
            var paraTypes = new Type[] { typeof(CreateConnectionViewModel) };
            _ConnectionTypes = new Dictionary<EDatabaseKind, ConstructorInfo>();
            foreach (var type in query)
            {
                var name = type.Name.Replace("ConnectionViewModel", "");
                if (Enum.TryParse(name, out EDatabaseKind value))
                {
                    _ConnectionTypes.Add(value, type.GetConstructor(paraTypes));
                }
            }
        }
        private readonly static IDictionary<EDatabaseKind, ConstructorInfo> _ConnectionTypes;

        public CreateConnectionViewModel()
        {
            _Connections = new Dictionary<EDatabaseKind, ConnectionBaseViewModel>();
            Database = Databases.First(a => a.Kind == EDatabaseKind.SqlServer);
        }

        public IEnumerable<DatabaseBase> Databases => DatabaseBase.Databases;

        public DatabaseBase Database
        {
            get => _Database;
            set
            {
                if (Set(ref _Database, value))
                {
                    if (!_Connections.TryGetValue(value.Kind, out ConnectionBaseViewModel connection))
                    {
                        connection = (ConnectionBaseViewModel)_ConnectionTypes[value.Kind].Invoke(new object[] { this });
                        _Connections.Add(value.Kind, connection);
                    }
                    Connection = connection;
                }
            }
        }
        public DatabaseBase _Database;

        public ConnectionBaseViewModel Connection
        {
            get => _Connection;
            private set => Set(ref _Connection, value);
        }
        private ConnectionBaseViewModel _Connection;
        private readonly Dictionary<EDatabaseKind, ConnectionBaseViewModel> _Connections;

        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }
        private string _Name;

        public RelayCommand Test
        {
            get
            {
                if (_Test == null)
                {
                    _Test = new RelayCommand(obj =>
                    {
                        if (TestConnection())
                        {
                            DialogHelper.MessageBox(Res.CreateConnection_Sucess);
                        }
                    });
                }
                return _Test;
            }
        }
        private RelayCommand _Test;

        public RelayCommand Close
        {
            get
            {
                if (_Close == null)
                {
                    _Close = new RelayCommand(obj =>
                    {
                        if (obj != null)
                        {
                            if (TestConnection())
                            {
                                DialogHelper.Close(true);
                            }
                        }
                        else
                        {
                            DialogHelper.Close();
                        }
                    });
                }
                return _Close;
            }
        }
        private RelayCommand _Close;

        public RelayCommand Setting
        {
            get
            {
                if (_Setting == null)
                {
                    _Setting = new RelayCommand(obj =>
                    {
                        var connectinString = Connection.Builder.ToString();
                        var builder = Database.Factory.CreateConnectionStringBuilder();
                        builder.ConnectionString = Connection.Builder.ToString();
                        using (var vm = new AdvancedPropertyViewModel(builder))
                        {
                            var viewmodel = vm;
                            if (DialogHelper.ShowDialog(ref viewmodel) == true)
                            {
                                Connection.Builder.ConnectionString = builder.ToString();
                                Connection.Reset();
                            }
                        }
                    });
                }
                return _Setting;
            }
        }
        private RelayCommand _Setting;

        private bool TestConnection()
        {
            try
            {
                using (var con = Database.Factory.CreateConnection())
                {
                    con.ConnectionString = Connection.Builder.ToString();
                    con.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                DialogHelper.MessageBox(ex.Message);
            }
            return false;
        }
    }
}
