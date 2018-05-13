using System.Data.Common;
using System.Data.SQLite;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class SQLiteConnectionViewModel : ConnectionBaseViewModel, IFileConnectionViewModel
    {
        public SQLiteConnectionViewModel(CreateConnectionViewModel parent)
           : base(parent)
        {
        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly SQLiteConnectionStringBuilder _Builder = new SQLiteConnectionStringBuilder();

        public string DefaultExt => ".sqlite3";

        public string Filter => "SQLite Database(*.sqlite,*.sqlite3)|*.sqlite;*.sqlite3;*.db|All files (*.*)|*.*";

        public string FileName
        {
            get => _Builder.DataSource;
            set
            {
                if (_Builder.DataSource != value)
                {
                    _Builder.DataSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _Builder.Password;
            set
            {
                if (_Builder.Password != value)
                {
                    _Builder.Password = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Create()
        {
            SQLiteConnection.CreateFile(FileName);
        }
    }
}
