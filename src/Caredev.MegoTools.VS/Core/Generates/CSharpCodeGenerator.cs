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

        private static readonly Dictionary<Type, string> _TypeAliasNames = new Dictionary<Type, string>()
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

        private static readonly HashSet<string> _LanguageKeys = new HashSet<string>()
        {
            "abstract","break","char","continue","do","event","finally","foreach"
            ,"in","is","new","out","protected","return","sizeof","struct"
            ,"true","ulong","using","volatile","add","async","dynamic","global"
            ,"join","orderby","remove","value","as","byte","checked","decimal"
            ,"double","explicit","fixed","goto","int","lock","null","override"
            ,"public","sbyte","stackalloc","switch","try","unchecked","using static","while"
            ,"alias","await","from","group","let","partial(type)","select","var"
            ,"where","base","case","class","default","else","extern","float"
            ,"if","interface","long","object","params","readonly","sealed","static"
            ,"this","typeof","unsafe","virtual","ascending","descending","get","into"
            ,"nameof","partial","set","when","yield","bool","catch","const"
            ,"delegate","enum","false","for","implicit","internal","namespace","operator"
            ,"private","ref","short","string","throw","uint","ushort","void"
        };
        protected override HashSet<string> LanguageKeys => _LanguageKeys;

        protected override Dictionary<Type, string> TypeAliasNames => _TypeAliasNames;

        protected override void WritePropertyArray(StringBuilder builder, Type type, string propertyName)
            => WriteProperty(builder, type, propertyName);

        protected override void WriteProperty(StringBuilder builder, Action<StringBuilder> typename, string propertyName)
        {
            builder.Append("\t\t");
            builder.Append("public ");
            typename(builder);
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

        protected override void WriteGenerateType(StringBuilder builder, string name, params string[] subnames)
        {
            builder.Append(name);
            builder.Append('<');
            builder.Append(subnames[0]);
            for (int i = 1; i < subnames.Length; i++)
            {
                builder.Append(", ");
                builder.Append(subnames[i]);
            }
            builder.Append('>');
        }

        protected override void WriteAttributeNameParameter(StringBuilder builder, string name)
        {
            builder.Append(name);
            builder.Append(" = ");
        }
    }
}
