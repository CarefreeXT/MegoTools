using Caredev.Mego.Resolve;
using Caredev.MegoTools.Core.DbObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core
{
    public class DbObjectCollection
    {
        public DbObjectCollection(IConnectionInformation connection)
        {
            Context = new DbObjectContext(connection);
            Objects = new Dictionary<string, ObjectElement>(StringComparer.OrdinalIgnoreCase);
            var feature = Context.Configuration.DatabaseFeature;
            SupportIdentity = feature.HasCapable(EDbCapable.Identity);
            if (!Context.Database.Provider.IsExclusive)
                SupportSchema = feature.HasCapable(EDbCapable.Schema);
        }

        public DbObjectContext Context { get; }

        public Dictionary<string, ObjectElement> Objects { get; }

        public Dictionary<string, DataType> DataTypes { get; set; }

        public bool SupportSchema { get; }

        public bool SupportIdentity { get; }

        public ObjectNames Names = new ObjectNames()
        {
            TableName = "TABLE_NAME",
            TableSchema = "TABLE_SCHEMA",
            ViewName = "TABLE_NAME",
            ViewSchema = "TABLE_SCHEMA",
            DataType = "DATA_TYPE",
            DataTypeKey = "TypeName"
        };

        public DbConnection Initialization()
        {
            var con = Context.Database.Connection;
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            var datatypes = new Dictionary<string, DataType>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in con.GetSchema("DataTypes").Rows)
            {
                if (!row.IsNull("DataType"))
                {
                    var item = new DataType(row);
                    var key = Convert.ToString(row[Names.DataTypeKey]);
                    if (!datatypes.ContainsKey(key))
                    {
                        datatypes.Add(key, item);
                    }
                }
            }
            DataTypes = datatypes;
            return con;
        }


        public void RegisterObjects(DataTable table, bool istable, string filter = "")
        {
            IEnumerable<DataRow> rows = table.AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
            {
                rows = table.Select(filter);
            }
            RegisterObjects(rows, istable);
        }

        public void RegisterObjects(IEnumerable<DataRow> rows, bool istable)
        {
            if (istable)
            {
                foreach (var row in rows)
                {
                    var elemnt = !SupportSchema ? new TableElement(row.Field<string>(Names.TableName)) :
                        new TableElement(row.Field<string>(Names.TableName), row.Field<string>(Names.TableSchema));
                    Objects.Add(elemnt.Key, elemnt);
                }
            }
            else
            {
                foreach (var row in rows)
                {
                    var elemnt = !SupportSchema ? new ViewElement(row.Field<string>(Names.TableName)) :
                        new ViewElement(row.Field<string>(Names.ViewName), row.Field<string>(Names.ViewSchema));
                    Objects.Add(elemnt.Key, elemnt);
                }
            }
        }

        public void RegisterMembers(DataTable table, ColumnCreator creator, string schema = "TABLE_SCHEMA", string name = "TABLE_NAME")
        {
            var query = SupportSchema ?
                from a in table.AsEnumerable()
                group a by a.Field<string>(schema) + "." + a.Field<string>(name) into g
                select new { g.Key, Columns = g } :
                from a in table.AsEnumerable()
                group a by a.Field<string>(name) into g
                select new { g.Key, Columns = g };
            foreach (var item in query)
            {
                if (Objects.TryGetValue(item.Key, out ObjectElement obj))
                {
                    foreach (var data in item.Columns)
                    {
                        var typename = Convert.ToString(data[Names.DataType]);
                        if (DataTypes.ContainsKey(typename))
                        {
                            var clrtype = DataTypes[typename];
                            var column = creator.CreateColumn(data, clrtype);
                            obj.Columns.Add(column.Name, column);
                        }
                    }
                }
            }
        }

        public void RegisterPrimaryKey()
        {
            var sql = SupportSchema ?
@"SELECT  a.TABLE_CATALOG ,
        a.TABLE_SCHEMA ,
        a.TABLE_NAME ,
        a.COLUMN_NAME ,
        a.ORDINAL_POSITION
FROM    INFORMATION_SCHEMA.KEY_COLUMN_USAGE a
        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS b ON b.CONSTRAINT_NAME = a.CONSTRAINT_NAME
                                                             AND b.CONSTRAINT_SCHEMA = a.CONSTRAINT_SCHEMA
WHERE   b.CONSTRAINT_TYPE = 'PRIMARY KEY';" :
@"SELECT  a.TABLE_CATALOG ,
        a.TABLE_SCHEMA ,
        a.TABLE_NAME ,
        a.COLUMN_NAME ,
        a.ORDINAL_POSITION
FROM    INFORMATION_SCHEMA.KEY_COLUMN_USAGE a
        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS b ON b.CONSTRAINT_NAME = a.CONSTRAINT_NAME
WHERE   b.CONSTRAINT_TYPE = 'PRIMARY KEY';";

            var table = GetDataTable(sql);
            RegisterPrimaryKey(table);
        }

        public void RegisterPrimaryKey(DataTable table)
        {
            var query = SupportSchema ?
                            from a in table.AsEnumerable()
                            group a by a.Field<string>("TABLE_SCHEMA") + "." + a.Field<string>("TABLE_NAME") into g
                            select new { g.Key, Columns = g } :
                            from a in table.AsEnumerable()
                            group a by a.Field<string>("TABLE_NAME") into g
                            select new { g.Key, Columns = g };
            foreach (var item in query)
            {
                if (Objects.TryGetValue(item.Key, out ObjectElement obj))
                {
                    if (item.Columns.Count() > 1)
                    {
                        foreach (var column in item.Columns)
                        {
                            var name = column.Field<string>("COLUMN_NAME");
                            var col = obj.Columns[name];
                            var position = Convert.ToInt32(column["ORDINAL_POSITION"]);
                            col.IsKey = true;
                            col.ColumnIndex = position;
                        }
                    }
                    else
                    {
                        foreach (var column in item.Columns)
                        {
                            var name = column.Field<string>("COLUMN_NAME");
                            obj.Columns[name].IsKey = true;
                        }
                    }
                }
            }
        }

        public DataTable GetDataTable(string sql)
        {
            var con = Context.Database.Connection;
            var com = con.CreateCommand();
            com.CommandText = sql;
            var adapter = Context.Database.Provider.Factory.CreateDataAdapter();
            adapter.SelectCommand = com;
            var datatable = new DataTable();
            adapter.Fill(datatable);
            return datatable;
        }

        public class ColumnCreator
        {
            public ColumnCreator()
            {
                Methods = new Dictionary<Type, Func<DataRow, string, DataType, ColumnElement>>()
                {
                    { typeof(string), CreateColumnForString },
                    { typeof(byte[]), CreateColumnForByteArray },
                    { typeof(decimal), CreateColumnForDeciaml },
                };
            }

            public ColumnNames Names = new ColumnNames()
            {
                Nullable = "IS_NULLABLE",
                TrueValue = "YES",
                Scale = "NUMERIC_SCALE",
                Precision = "NUMERIC_PRECISION",
                MaxLength = "CHARACTER_MAXIMUM_LENGTH",
                CharName = "CHARACTER_SET_NAME"
            };

            public virtual void ProcessColumn(ColumnElement column, DataRow row, DataType type)
            {
                if (!row.IsNull(Names.Nullable))
                {
                    var value = row[Names.Nullable];
                    if (value is bool)
                    {
                        column.IsNullable = (bool)value;
                    }
                    else
                    {
                        column.IsNullable = value.ToString() == Names.TrueValue;
                    }
                }
                column.ClrType = type.ClrType;
            }

            public Dictionary<Type, Func<DataRow, string, DataType, ColumnElement>> Methods { get; }

            public ColumnElement CreateColumn(DataRow row, DataType type)
            {
                var name = row.Field<string>("COLUMN_NAME");
                ColumnElement column = null;
                if (Methods.TryGetValue(type.ClrType, out Func<DataRow, string, DataType, ColumnElement> method))
                {
                    column = method(row, name, type);
                }
                else
                {
                    column = CreateColumnForDefault(row, name, type);
                }
                ProcessColumn(column, row, type);
                return column;
            }

            public virtual ColumnElement CreateColumnForString(DataRow row, string name, DataType type)
            {
                var columnstring = new ColumnStringElement(name);
                if (!row.IsNull(Names.MaxLength))
                {
                    var maxlength = Convert.ToInt64(row[Names.MaxLength]);
                    if (maxlength > -1 && maxlength < int.MaxValue)
                    {
                        columnstring.MaxLength = (int)maxlength;
                    }
                }
                columnstring.IsFixed = type.IsFixedLength;
                if (!string.IsNullOrEmpty(Names.CharName))
                    columnstring.IsUnicode = row.Field<string>(Names.CharName) == "UNICODE";
                return columnstring;
            }
            public virtual ColumnElement CreateColumnForByteArray(DataRow row, string name, DataType type)
            {
                var columnlength = new ColumnLengthElement(name);
                if (!row.IsNull(Names.MaxLength))
                {
                    var maxlength = Convert.ToInt64(row[Names.MaxLength]);
                    if (maxlength > -1 && maxlength < int.MaxValue)
                    {
                        columnlength.MaxLength = (int)maxlength;
                    }
                }
                columnlength.IsFixed = type.IsFixedLength;
                return columnlength;
            }
            public virtual ColumnElement CreateColumnForDeciaml(DataRow row, string name, DataType type)
            {
                var result = new ColumnPrecisionElement(name);
                if (!row.IsNull(Names.Precision))
                {
                    result.Precision = Convert.ToByte(row[Names.Precision]);
                }
                if (!row.IsNull(Names.Scale))
                {
                    result.Scale = Convert.ToByte(row[Names.Scale]);
                }
                return result;
            }
            public virtual ColumnElement CreateColumnForDefault(DataRow row, string name, DataType type)
            {
                return new ColumnElement(name);
            }
        }
        public struct ColumnNames
        {
            public string Nullable;
            public string TrueValue;
            public string Scale;
            public string Precision;
            public string MaxLength;
            public string CharName;
        }
        public struct ObjectNames
        {
            public string TableName;
            public string ViewName;
            public string TableSchema;
            public string ViewSchema;
            public string DataType;
            public string DataTypeKey;
        }
        public class DataType
        {
            public DataType(DataRow row)
            {
                Name = row.Field<string>("TypeName");
                ProviderDbType = Convert.ToString(row["ProviderDbType"]);
                ClrType = Type.GetType(row.Field<string>("DataType"));
                if (!row.IsNull("IsFixedLength"))
                    IsFixedLength = row.Field<bool>("IsFixedLength");
            }
            public string Name { get; }
            public Type ClrType { get; }
            public bool IsFixedLength { get; }
            public string ProviderDbType { get; }
        }
    }
}
