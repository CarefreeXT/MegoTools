using System.Data.Common;
using System.Data.OleDb;

namespace Caredev.MegoTools.ViewModel.Connections
{
    public class ExcelConnectionViewModel : ConnectionBaseViewModel, IFileConnectionViewModel
    {
        public ExcelConnectionViewModel(CreateConnectionViewModel parent)
           : base(parent)
        {
        }

        public override DbConnectionStringBuilder Builder => _Builder;
        private readonly OleDbConnectionStringBuilder _Builder = new OleDbConnectionStringBuilder();

        public string DefaultExt => ".xlsx";

        public string Filter => "Excel WorkSheet(*.xlsx)|*.xlsx|Excel (97-2003) WorkSheet(*.xls)|*.xls|All files (*.*)|*.*";

        public string FileName
        {
            get => _Builder.DataSource;
            set => Set("Data Source", value);
        }

        public string Password { get; set; }
    }
}
