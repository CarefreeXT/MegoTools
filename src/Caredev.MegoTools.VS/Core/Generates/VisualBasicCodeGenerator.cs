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

        private static readonly Dictionary<Type, string> _TypeAliasNames = new Dictionary<Type, string>()
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

        private static readonly HashSet<string> _LanguageKeys = new HashSet<string>()
        {
            "AddHandler","AddressOf","Aggregate","Alias","And","AndAlso","Ansi","As"
            ,"Assembly","Async","Auto","Await","Binary","Boolean","ByRef","Byte"
            ,"ByVal","Call","Case","Catch","CBool","CByte","CChar","CDate"
            ,"CDbl","CDec","Char","CInt","Class","CLng","CObj","Compare"
            ,"Const","Continue","CSByte","CShort","CSng","CStr","CType","CUInt"
            ,"CULng","CUShort","Custom","Date","Decimal","Declare","Default","Delegate"
            ,"Dim","DirectCast","Distinct","Do","Double","Each","Else","ElseIf"
            ,"End","EndIf","Enum","Equals","Erase","Error","Event","Exit"
            ,"Explicit","False","Finally","For","Friend","From","Function","Get"
            ,"GetType","GetXMLNamespace","Global","GoSub","GoTo","Group","By","Handles"
            ,"If","Implements","Imports","In","Inherits","Integer","Interface","Into"
            ,"Is","IsFalse","IsNot","IsTrue","Iterator","Join","Key","Let"
            ,"Lib","Like","Long","Loop","Me","Mid","Mod","Module"
            ,"MustInherit","MustOverride","MyBase","MyClass","Namespace","Narrowing","New","Next"
            ,"Not","Nothing","NotInheritable","NotOverridable","Object","Of","Off","On"
            ,"Operator","Option","Optional","Or","OrElse","Out","Overloads","Overridable"
            ,"Overrides","ParamArray","Partial","Preserve","Private","Property","Protected","Public"
            ,"RaiseEvent","ReadOnly","ReDim","REM","RemoveHandler","Resume","Return","SByte"
            ,"Select","Set","Shadows","Shared","Short","Single","Skip","Static"
            ,"Step","Stop","Strict","String","Structure","Sub","SyncLock","Take"
            ,"Text","Then","Throw","To","True","Try","TryCast","TypeOf"
            ,"UInteger","ULong","Unicode","Until","UShort","Using","Variant","Wend"
            ,"When","Where","While","Widening","With","WithEvents","WriteOnly","Xor","Yield"
        };
        protected override HashSet<string> LanguageKeys => _LanguageKeys;

        protected override Dictionary<Type, string> TypeAliasNames => _TypeAliasNames;


        protected override void WriteProperty(StringBuilder builder, Action<StringBuilder> typename, string propertyName)
        {
            builder.Append("\t\t");
            builder.Append("Public Property ");
            builder.Append(propertyName);
            builder.Append(" AS ");
            typename(builder);
            builder.AppendLine();
        }

        protected override void WritePropertyArray(StringBuilder builder, Type type, string propertyName)
        {
            builder.Append("\t\t");
            builder.Append("Public Property ");
            builder.Append(propertyName);
            builder.Append("() AS ");
            WriteType(builder, type.GetElementType());
            builder.AppendLine();
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

        protected override void WriteGenerateType(StringBuilder builder, string name, params string[] subnames)
        {
            builder.Append(name);
            builder.Append("(Of ");
            builder.Append(subnames[0]);
            for (int i = 1; i < subnames.Length; i++)
            {
                builder.Append(", Of ");
                builder.Append(subnames[i]);
            }
            builder.Append(')');
        }

        protected override void WriteAttributeNameParameter(StringBuilder builder, string name)
        {
            builder.Append(name);
            builder.Append(":= ");
        }
    }
}
