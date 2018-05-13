using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core.Generates
{
    internal sealed class CSharpCodeGenerator : CodeGeneratorBase
    {
        public CSharpCodeGenerator(bool isPluralize)
            : base(isPluralize)
        {
        }

        private static readonly Dictionary<Type, string> TypeAliasNames = new Dictionary<Type, string>()
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
        };

        protected override void WriteProperty(StringBuilder builder, Type type, string propertyName)
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
            if (isNullable)
            {
                WriteProperty(builder, typename + "?", propertyName);
            }
            else
            {
                WriteProperty(builder, typename, propertyName);
            }
        }

        protected override void WriteProperty(StringBuilder builder, string typename, string propertyName)
        {
            builder.Append("\t\t");
            builder.Append("public ");
            builder.Append(typename);
            builder.Append(' ');
            builder.Append(propertyName);
            builder.AppendLine(" { get; set; }");
        }

        protected override void WriteEndClass(StringBuilder builder)
        {
            builder.AppendLine("\t}");
        }

        protected override void WriteStartClass(StringBuilder builder, string className)
        {
            builder.Append("\tpublic class ");
            builder.AppendLine(className);
            builder.AppendLine("\t{");
        }

        private int AttributeStartIndex = -1;
        private bool IsFirstAttributeParameter = true;
        protected override void WriteStartAttribute(StringBuilder builder, int tabCount)
        {
            IsFirstAttributeParameter = true;
            AttributeStartIndex = builder.Length;
            builder.Append(new string('\t', tabCount));
            builder.Append('[');
        }

        protected override void WriteAttributeParameter(StringBuilder builder, Action action)
        {
            if (IsFirstAttributeParameter)
            {
                IsFirstAttributeParameter = false;
                AttributeStartIndex = -1;
            }
            else
            {
                builder.Append(", ");
            }
            action();
        }

        protected override void WriteAttributeParameter(StringBuilder builder, string content)
        {
            WriteAttributeParameter(builder, () => builder.Append(content));
        }

        protected override void WriteEndAttribute(StringBuilder builder)
        {
            if (AttributeStartIndex > 0)
            {
                builder.Remove(AttributeStartIndex, builder.Length - AttributeStartIndex);
            }
            else
            {
                builder.AppendLine("]");
            }
        }
        
        protected override void WriteCommit(StringBuilder builder, string content, int tabCount)
        {
            builder.Append(new string('\t', tabCount));
            builder.AppendLine(@"/// <summary>");
            builder.Append(new string('\t', tabCount));
            builder.Append(@"/// ");
            builder.AppendLine(content);
            builder.Append(new string('\t', tabCount));
            builder.AppendLine(@"/// </summary>");
        }
    }
}
