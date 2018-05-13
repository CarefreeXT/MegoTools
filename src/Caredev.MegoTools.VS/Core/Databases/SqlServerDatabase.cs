using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Caredev.MegoTools.Core.DbObjects;

namespace Caredev.MegoTools.Core.Databases
{
    internal class SqlServerDatabase : DatabaseBase
    {
        public override string Title => "Microsoft SQL Server";

        public override EDatabaseKind Kind => EDatabaseKind.SqlServer;

        public override DbProviderFactory Factory => SqlClientFactory.Instance;

        public override string DefalutConnectionString => "Data Source=localhost;Integrated Security=True";

        /*
 SELECT c.name TABLE_SCHEMA,
        b.name TABLE_NAME,
        a.name COLUMN_NAME,
        IDENT_SEED(c.name + '.' + b.name) IDENTITY_SEED,
        IDENT_INCR(c.name + '.' + b.name) IDENTITY_INCREMENT
 FROM   sys.columns a
        INNER JOIN sys.tables b ON b.object_id = a.object_id
        INNER JOIN sys.schemas c ON c.schema_id = b.schema_id
 WHERE  a.is_identity = 1;
             */
        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            var con = collection.Initialization();
            var table = con.GetSchema("Tables");
            collection.RegisterObjects(table, true, "TABLE_TYPE = 'BASE TABLE'");
            collection.RegisterObjects(table, false, "TABLE_TYPE = 'VIEW'");
            collection.RegisterMembers(con.GetSchema("Columns"), new DbObjectCollection.ColumnCreator());

            collection.RegisterPrimaryKey();
            return collection;
        }
    }
}
