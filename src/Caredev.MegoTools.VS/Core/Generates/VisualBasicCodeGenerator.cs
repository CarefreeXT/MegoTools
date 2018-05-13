using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core.Generates
{
    internal class VisualBasicCodeGenerator : CodeGeneratorBase
    {
        public VisualBasicCodeGenerator(bool isPluralize)
           : base(isPluralize)
        {
        }

        private static readonly Dictionary<Type, string> TypeAliasNames = new Dictionary<Type, string>()
        {
            { typeof(bool), "Boolean" },
            { typeof(byte), "Byte" },
            { typeof(char), "Char" },
            { typeof(decimal), "Decimal" },
            { typeof(double), "Double" },
            { typeof(float), "Float" },
            { typeof(int), "Integer" },
            { typeof(long), "Long" },
            { typeof(sbyte), "SByte" },
            { typeof(short), "Short" },
            { typeof(string), "String" },
            { typeof(uint), "UInteger" },
            { typeof(ulong), "ULong" },
            { typeof(ushort), "UShort" },
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
            builder.Append("Public Property ");
            builder.Append(propertyName);
            builder.Append(" AS ");
            builder.AppendLine(typename);
        }

        protected override void WriteEndClass(StringBuilder builder)
        {
            builder.AppendLine("\tEnd Class");
        }

        protected override void WriteStartClass(StringBuilder builder, string className)
        {
            builder.Append("\tPublic Class ");
            builder.AppendLine(className);
        }

        private int AttributeStartIndex = -1;
        private bool IsFirstAttributeParameter = true;
        protected override void WriteStartAttribute(StringBuilder builder, int tabCount)
        {
            IsFirstAttributeParameter = true;
            AttributeStartIndex = builder.Length;
            builder.Append(new string('\t', tabCount));
            builder.Append('<');
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
                builder.AppendLine(">");
            }
        }

        protected override void WriteCommit(StringBuilder builder, string content, int tabCount)
        {
            builder.Append(new string('\t', tabCount));
            builder.AppendLine(@"''' <summary>");
            builder.Append(new string('\t', tabCount));
            builder.Append(@"''' ");
            builder.AppendLine(content);
            builder.Append(new string('\t', tabCount));
            builder.AppendLine(@"''' </summary>");
        }
    }
}
