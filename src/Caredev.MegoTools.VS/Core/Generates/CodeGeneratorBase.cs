using Caredev.MegoTools.Core.DbObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core.Generates
{
    internal abstract class CodeGeneratorBase
    {

        public CodeGeneratorBase(bool isPluralize)
        {
            IsPluralize = isPluralize;
            Objects = new Dictionary<string, ClassGenerator>();
        }

        public bool IsPluralize { get; }

        protected Dictionary<string, ClassGenerator> Objects { get; }

        private readonly static Regex SafeNameRegex = new Regex("[^A-Z,a-z,0-9,_]");
        private IPluralizationService Pluralization = new EnglishPluralizationService();
        private int _CharIndex = 0;
        private string CharIndex
        {
            get
            {
                var result = (_CharIndex++).ToString();
                if (_CharIndex > 9) _CharIndex = 0;
                return result;
            }
        }
        private string SafeName(string input)
        {
            var name = SafeNameRegex.Replace(input, "");
            if (LanguageKeys.Contains(name))
            {
                name += CharIndex;
            }
            return name;
        }
        public void Add(ObjectElement obj, IEnumerable<ColumnElement> columns)
        {
            var className = SafeName(string.IsNullOrEmpty(obj.ClassName) ? obj.Name : obj.ClassName);
            var propertyName = SafeName(string.IsNullOrEmpty(obj.PropertyName) ? obj.Name : obj.PropertyName);
            if (IsPluralize)
            {
                className = Pluralization.Singularize(className);
                propertyName = Pluralization.Pluralize(propertyName);
            }
            var objectGenerator = new ClassGenerator(obj)
            {
                ClassName = className,
                PropertyName = propertyName
            };
            foreach (var column in columns)
            {
                var name = SafeName(string.IsNullOrEmpty(column.PropertyName) ? column.Name : column.PropertyName);
                if (objectGenerator.ClassName == name)
                {
                    name += CharIndex;
                }
                var memberGenerator = new MemberGenerator(column, name);
                objectGenerator.Members.Add(column.Name, memberGenerator);
            }
            Objects.Add(obj.Key, objectGenerator);
        }

        protected abstract Dictionary<Type, string> TypeAliasNames { get; }

        protected abstract HashSet<string> LanguageKeys { get; }

        protected void WriteType(StringBuilder builder, Type type)
        {
            bool isNullable = false;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullable = true;
                type = type.GetGenericArguments()[0];
            }
            var typename = type.FullName;
            if (!TypeAliasNames.TryGetValue(type, out typename) && type.Namespace == "System")
            {
                typename = type.Name;
            }
            builder.Append(typename);
            if (isNullable)
            {
                builder.Append('?');
            }
        }

        protected void WriteProperty(StringBuilder builder, Type type, string propertyName)
        {
            WriteProperty(builder, b => WriteType(b, type), propertyName);
        }

        protected void WriteProperty(StringBuilder builder, string typename, string propertyName)
        {
            WriteProperty(builder, b => b.Append(typename), propertyName);
        }

        protected abstract void WriteProperty(StringBuilder builder, Action<StringBuilder> typename, string propertyName);

        protected abstract void WriteStartClass(StringBuilder builder, string className);

        protected abstract void WriteEndClass(StringBuilder builder);

        protected abstract void WriteCommit(StringBuilder builder, string content, int tabCount);

        protected abstract void WriteStartAttribute(StringBuilder builder, int tabCount);

        protected abstract void WriteAttributeParameter(StringBuilder builder, Action action);

        protected abstract void WriteAttributeParameter(StringBuilder builder, string content);

        protected abstract void WriteAttributeNameParameter(StringBuilder builder, string name);

        protected abstract void WriteEndAttribute(StringBuilder builder);

        protected abstract void WriteGenerateType(StringBuilder builder, string name, params string[] subnames);

        protected abstract void WritePropertyArray(StringBuilder builder, Type type, string propertyName);

        public string GenerateDbSet()
        {
            var builder = new StringBuilder();
            foreach (var obj in Objects.Values)
            {
                WriteProperty(builder, b => WriteGenerateType(b, "DbSet", obj.ClassName), obj.PropertyName);
            }
            return builder.ToString();
        }

        public string GenerateSetClass()
        {
            var builder = new StringBuilder();
            foreach (var obj in Objects.Values)
            {
                if (!string.IsNullOrEmpty(obj.Element.Comments))
                {
                    WriteCommit(builder, obj.Element.Comments, 1);
                }
                var element = obj.Element;
                if (obj.ClassName != element.Name || !string.IsNullOrEmpty(element.Schema))
                {
                    WriteStartAttribute(builder, 1);
                    WriteAttributeParameter(builder, delegate ()
                    {
                        builder.Append("Table(\"");
                        builder.Append(element.Name);
                        builder.Append('\"');
                        if (!string.IsNullOrEmpty(element.Schema))
                        {
                            builder.Append(", ");
                            WriteAttributeNameParameter(builder, "Schema");
                            builder.Append('\"');
                            builder.Append(element.Schema);
                            builder.Append('\"');
                        }
                        builder.Append(')');
                    });
                    WriteEndAttribute(builder);
                }
                WriteStartClass(builder, obj.ClassName);
                foreach (var member in obj.Members.Values)
                {
                    var col = member.Element;
                    if (!string.IsNullOrEmpty(col.Comments))
                    {
                        WriteCommit(builder, col.Comments, 2);
                    }
                    WriteStartAttribute(builder, 2);
                    if (col.IsKey)
                    {
                        WriteAttributeParameter(builder, "Key");
                    }
                    if (col.Name != member.PropertyName || col.ColumnIndex.HasValue)
                    {
                        WriteAttributeParameter(builder, "Column(\"");
                        builder.Append(col.Name);
                        builder.Append("\"");
                        if (col.ColumnIndex.HasValue)
                        {
                            builder.Append(", ");
                            WriteAttributeNameParameter(builder, "Order");
                            builder.Append(col.ColumnIndex.Value.ToString());
                        }
                        builder.Append(")");
                    }
                    if (!col.ClrType.IsValueType && col.IsNullable)
                    {
                        WriteAttributeParameter(builder, "Nullable(true)");
                    }
                    if (col.ConcurrencyCheck)
                    {
                        WriteAttributeParameter(builder, "ConcurrencyCheck");
                    }
                    if (col.Identity != null)
                    {
                        var identity = col.Identity;
                        if (identity.Seed == 1 && identity.Increment == 1)
                        {
                            WriteAttributeParameter(builder, "Identity");
                        }
                        else
                        {
                            WriteAttributeParameter(builder, "Identity(");
                            builder.Append(identity.Seed.ToString());
                            if (identity.Increment != 1)
                            {
                                builder.Append(", ");
                                builder.Append(identity.Increment.ToString());
                                builder.Append(')');
                            }
                        }
                    }
                    switch (col)
                    {
                        case ColumnStringElement stringColumn:
                            if (stringColumn.MaxLength.HasValue)
                            {
                                WriteAttributeParameter(builder, "mego.String(");
                                builder.Append(stringColumn.MaxLength.ToString());
                                if (stringColumn.IsFixed)
                                {
                                    builder.Append(", true");
                                }
                                if (!stringColumn.IsUnicode)
                                {
                                    builder.Append(", ");
                                    WriteAttributeNameParameter(builder, "IsUnicode");
                                    builder.Append("false");
                                }
                                builder.Append(')');
                            }
                            break;
                        case ColumnLengthElement length:
                            if (length.MaxLength.HasValue)
                            {
                                WriteAttributeParameter(builder, "Length(");
                                builder.Append(length.MaxLength.ToString());
                                if (length.IsFixed)
                                {
                                    builder.Append(", true");
                                }
                                builder.Append(')');
                            }
                            break;
                        case ColumnPrecisionElement preciColumn:
                            if (preciColumn.Precision.HasValue)
                            {
                                WriteAttributeParameter(builder, "Precision(");
                                builder.Append(preciColumn.Precision.ToString());
                                if (preciColumn.Scale.HasValue)
                                {
                                    builder.Append(", ");
                                    builder.Append(preciColumn.Scale.ToString());
                                }
                                builder.Append(')');
                            }
                            break;
                    }
                    WriteEndAttribute(builder);
                    if (member.Element.ClrType.IsArray)
                    {
                        WritePropertyArray(builder, member.Element.ClrType, member.PropertyName);
                    }
                    else
                    {
                        WriteProperty(builder, member.Element.ClrType, member.PropertyName);
                    }
                }
                WriteEndClass(builder);
            }
            return builder.ToString();
        }
    }
}
