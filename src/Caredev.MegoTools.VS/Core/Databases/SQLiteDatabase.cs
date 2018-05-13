using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using Caredev.MegoTools.Core.DbObjects;

namespace Caredev.MegoTools.Core.Databases
{
    internal class SQLiteDatabase : DatabaseBase
    {
        public override string Title => "SQLite";

        public override EDatabaseKind Kind => EDatabaseKind.SQLite;

        public override DbProviderFactory Factory => SQLiteFactory.Instance;

        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            var con = collection.Initialization();
            var creator = new ColumnCreator();
            collection.RegisterObjects(con.GetSchema("Tables"), true);
            collection.RegisterObjects(con.GetSchema("Views"), false);
            collection.RegisterMembers(con.GetSchema("Columns"), creator);
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

                if (!row.IsNull("PRIMARY_KEY"))
                {
                    if (row.Field<bool>("PRIMARY_KEY"))
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
