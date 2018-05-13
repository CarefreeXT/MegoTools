using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace Caredev.MegoTools.Core.Databases
{
    internal class ExcelDatabase : DatabaseBase
    {
        public override string Title => "Microsoft Excel (OLE DB)";

        public override EDatabaseKind Kind => EDatabaseKind.Excel;

        public override DbProviderFactory Factory => OleDbFactory.Instance;

        public override bool SupportRelation => false;

        public override string ProviderName => "System.Data.OleDb.Excel";

        public override string DefalutConnectionString => "Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties='Excel 12.0 Xml;HDR=YES'";

        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            collection.Names.DataTypeKey = "NativeDataType";
            var con = collection.Initialization();
            var table = con.GetSchema("Tables");
            var creator = new DbObjectCollection.ColumnCreator();
            var rows = table.AsEnumerable().Where(a => !a.Field<string>("TABLE_NAME").EndsWith("$")).ToArray();
            collection.RegisterObjects(rows, true);
            collection.RegisterMembers(con.GetSchema("Columns"), creator);
            return collection;
        }
    }
}
