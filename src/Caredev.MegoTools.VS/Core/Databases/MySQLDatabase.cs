using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caredev.MegoTools.Core.DbObjects;
using MySql.Data.MySqlClient;

namespace Caredev.MegoTools.Core.Databases
{
    internal class MySQLDatabase : DatabaseBase
    {
        public override string Title => "MySQL";

        public override EDatabaseKind Kind => EDatabaseKind.MySQL;

        public override DbProviderFactory Factory => MySqlClientFactory.Instance;

        public override string DefalutConnectionString => "server=localhost;user id=root;characterset=utf8;Allow User Variables=True";

        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            var con = collection.Initialization();
            collection.RegisterObjects(con.GetSchema("Tables"), true);
            collection.RegisterObjects(con.GetSchema("Views"), false);

            var creator = new ColumnCreator();

            collection.RegisterMembers(con.GetSchema("Columns"), creator);
            collection.RegisterMembers(con.GetSchema("ViewColumns"), creator, "VIEW_SCHEMA", "VIEW_NAME");
            foreach (var obj in collection.Objects.Values
                .Where(a => a.Kind == EObjectKind.Table && a.Columns.Values.Count(b => b.IsKey) == 1))
            {
                obj.Columns.Values.Where(a => a.IsKey).Single().ColumnIndex = null;
            }
            return collection;
        }

        private class ColumnCreator : DbObjectCollection.ColumnCreator
        {
            public override ColumnElement CreateColumnForDefault(DataRow row, string name, DbObjectCollection.DataType type)
            {
                ColumnElement column = column = base.CreateColumnForDefault(row, name, type);
                if (!row.IsNull("EXTRA") && row.Field<string>("EXTRA") == "auto_increment")
                {
                    column.Identity = new IdentityInfo();
                }
                if (!row.IsNull("COLUMN_KEY"))
                {
                    if (row.Field<string>("COLUMN_KEY") == "PRI")
                    {
                        column.IsKey = true;
                        column.ColumnIndex = Convert.ToInt32(row["ORDINAL_POSITION"]);
                    }
                }
                return column;
            }
        }
    }
}
