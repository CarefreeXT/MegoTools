using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;

namespace Caredev.MegoTools.Core.Databases
{
    internal class SqlServerCeDatabase : DatabaseBase
    {
        public override string Title => "Microsoft SQL Server Compact";

        public override EDatabaseKind Kind => EDatabaseKind.SqlServerCe;

        public override DbProviderFactory Factory => SqlCeProviderFactory.Instance;


        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            var con = collection.Initialization();
            var creator = new DbObjectCollection.ColumnCreator();
            collection.RegisterObjects(con.GetSchema("Tables"), true, "TABLE_TYPE = 'TABLE'");
            collection.RegisterMembers(con.GetSchema("Columns"), creator);

            collection.RegisterPrimaryKey();
            return collection;
        }

        /*
 SELECT TABLE_SCHEMA ,
        TABLE_NAME ,
        COLUMN_NAME ,
        AUTOINC_INCREMENT IDENTITY_INCREMENT,
        AUTOINC_SEED IDENTITY_SEED
 FROM   INFORMATION_SCHEMA.COLUMNS
 WHERE  AUTOINC_SEED IS NOT NULL
         */
    }
}
