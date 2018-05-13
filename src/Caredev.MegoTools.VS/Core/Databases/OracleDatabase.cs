using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caredev.MegoTools.Core.DbObjects;
using Oracle.ManagedDataAccess.Client;

namespace Caredev.MegoTools.Core.Databases
{
    internal class OracleDatabase : DatabaseBase
    {
        public override string Title => "Oracle";

        public override EDatabaseKind Kind => EDatabaseKind.Oracle;

        public override DbProviderFactory Factory => OracleClientFactory.Instance;

        public override string DefalutConnectionString => "USER ID=SYSTEM;DATA SOURCE=localhost";

        public override DbObjectCollection CreateCollection(IConnectionInformation info)
        {
            var collection = new DbObjectCollection(info);
            collection.Names.DataType = "DATATYPE";
            var con = collection.Initialization();
            collection.RegisterObjects(collection.GetDataTable(
@"SELECT OWNER TABLE_SCHEMA, TABLE_NAME
FROM ALL_TABLES 
WHERE OWNER NOT IN 
('SYS', 'SYSTEM', 'SYSMAN','CTXSYS', 'MDSYS', 'OLAPSYS', 'ORDSYS', 'OUTLN','WKSYS', 'WMSYS', 'XDB', 'ORDPLUGINS')
AND OWNER NOT LIKE 'APEX%'
ORDER BY OWNER, TABLE_NAME"), true);
            collection.RegisterObjects(collection.GetDataTable(
@"SELECT OWNER TABLE_SCHEMA, VIEW_NAME TABLE_NAME
FROM ALL_VIEWS 
WHERE OWNER NOT IN 
('SYS', 'SYSTEM', 'SYSMAN','CTXSYS', 'MDSYS', 'OLAPSYS', 'ORDSYS', 'OUTLN','WKSYS', 'WMSYS', 'XDB', 'ORDPLUGINS')
AND OWNER NOT LIKE 'APEX%'
ORDER BY OWNER, VIEW_NAME"), false);
            var creator = new DbObjectCollection.ColumnCreator();
            creator.Names.TrueValue = "Y";
            creator.Names.CharName = "";
            collection.RegisterMembers(collection.GetDataTable(
@"SELECT OWNER TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, COLUMN_ID AS ID, DATA_TYPE AS DataType, 
DATA_LENGTH AS CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION AS NUMERIC_PRECISION, DATA_SCALE AS NUMERIC_SCALE, 
NULLABLE AS IS_NULLABLE, CHAR_USED, CHAR_LENGTH AS LengthInChars 
FROM ALL_TAB_COLUMNS WHERE OWNER NOT IN 
('SYS', 'SYSTEM', 'SYSMAN','CTXSYS', 'MDSYS', 'OLAPSYS', 'ORDSYS', 'OUTLN','WKSYS', 'WMSYS', 'XDB', 'ORDPLUGINS')
AND OWNER NOT LIKE 'APEX%'
ORDER BY OWNER, TABLE_NAME, ID"), creator);

            var table = collection.GetDataTable(
@"select  b.OWNER TABLE_SCHEMA,b.TABLE_NAME,b.COLUMN_NAME,b.POSITION ORDINAL_POSITION from all_constraints a
inner join all_cons_columns b ON a.OWNER=b.OWNER and a.CONSTRAINT_NAME=b.CONSTRAINT_NAME
where a.constraint_type='P'");
            collection.RegisterPrimaryKey(table);
            return collection;
        }
    }
}
