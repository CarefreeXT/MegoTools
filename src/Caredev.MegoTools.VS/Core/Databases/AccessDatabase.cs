using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Caredev.MegoTools.Core.Databases
{
    internal class AccessDatabase : DatabaseBase
    {
        public override string Title => "Microsoft Access (OLE DB)";

        public override EDatabaseKind Kind => EDatabaseKind.Access;

        public override DbProviderFactory Factory => OleDbFactory.Instance;

        public override string ProviderName => "System.Data.OleDb.Access";

        public override string DefalutConnectionString => "Provider =MicroSoft.ACE.OLEDB.12.0;Persist Security Info=False;";

        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            collection.Names.DataTypeKey = "NativeDataType";
            var con = collection.Initialization();
            var creator = new DbObjectCollection.ColumnCreator();
            collection.RegisterObjects(con.GetSchema("Tables"), true, "TABLE_TYPE = 'TABLE'");
            collection.RegisterObjects(con.GetSchema("Views"), false);

            collection.RegisterMembers(con.GetSchema("Columns"), creator);

            var table = con.GetSchema("Indexes");

            var rows = table.Select("PRIMARY_KEY=0");
            foreach (var row in rows)
            {
                table.Rows.Remove(row);
            }
            collection.RegisterPrimaryKey(table);
            return collection;
        }
    }
}
