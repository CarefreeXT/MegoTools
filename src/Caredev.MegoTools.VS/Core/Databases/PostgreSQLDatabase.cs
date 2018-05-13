using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Caredev.MegoTools.Core.Databases
{
    internal class PostgreSQLDatabase : DatabaseBase
    {
        public override string Title => "PostgreSQL";

        public override EDatabaseKind Kind => EDatabaseKind.PostgreSQL;

        public override DbProviderFactory Factory => NpgsqlFactory.Instance;

        public override string DefalutConnectionString => "Host=localhost;Username=postgres";

        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            var con = collection.Context.Database.Connection;
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }
            collection.DataTypes = new Dictionary<string, DbObjectCollection.DataType>();
            foreach (DataRow row in GetDataTypes().Rows)
            {
                var dt = new DbObjectCollection.DataType(row);
                collection.DataTypes.Add(dt.Name, dt);
            }

            collection.RegisterObjects(con.GetSchema("Tables"), true, "TABLE_SCHEMA NOT IN('information_schema','pg_catalog')");
            collection.RegisterObjects(con.GetSchema("Views"), false, "TABLE_SCHEMA NOT IN('information_schema','pg_catalog')");
            var creator = new DbObjectCollection.ColumnCreator();

            collection.RegisterMembers(con.GetSchema("Columns"), creator);

            collection.RegisterPrimaryKey();

            return collection;
        }

        public DataTable GetDataTypes()
        {
            var table = new DataTable();

            table.Columns.Add("TypeName", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("IsFixedLength", typeof(bool));
            table.Columns.Add("ProviderDbType", typeof(int));

            int index = 1;

            table.Rows.Add("bool", typeof(bool).FullName, true, index++);
            table.Rows.Add("uuid", typeof(Guid).FullName, true, index++);

            table.Rows.Add("xid", typeof(int).FullName, true, index++);
            table.Rows.Add("oid", typeof(int).FullName, true, index++);
            table.Rows.Add("int2", typeof(short).FullName, true, index++);
            table.Rows.Add("int4", typeof(int).FullName, true, index++);
            table.Rows.Add("int8", typeof(long).FullName, true, index++);

            table.Rows.Add("char", typeof(string).FullName, true, index++);
            table.Rows.Add("varchar", typeof(string).FullName, false, index++);
            table.Rows.Add("bytea", typeof(byte[]).FullName, false, index++);


            table.Rows.Add("float4", typeof(float).FullName, true, index++);
            table.Rows.Add("float8", typeof(double).FullName, true, index++);
            table.Rows.Add("interval", typeof(TimeSpan).FullName, true, index++);

            table.Rows.Add("inet", typeof(string).FullName, false, index++);
            table.Rows.Add("name", typeof(string).FullName, true, index++);

            table.Rows.Add("timestamptz", typeof(DateTimeOffset).FullName, true, index++);
            table.Rows.Add("timestamp", typeof(DateTime).FullName, true, index++);
            table.Rows.Add("date", typeof(DateTime).FullName, true, index++);
            table.Rows.Add("time", typeof(DateTime).FullName, true, index++);

            table.Rows.Add("money", typeof(decimal).FullName, true, index++);
            table.Rows.Add("numeric", typeof(decimal).FullName, true, index++);

            table.Rows.Add("text", typeof(string).FullName, false, index++);
            table.Rows.Add("json", typeof(string).FullName, false, index++);
            table.Rows.Add("cidr", typeof(string).FullName, true, index++);
            table.Rows.Add("macaddr", typeof(string).FullName, true, index++);

            return table;
        }

    }
}
