using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Res = Caredev.MegoTools.Properties.Resources;

namespace Caredev.MegoTools.Core.DbObjects
{
    public class ColumnElement : ElementBase
    {
        public ColumnElement(string name)
            : base(name)
        {
        }
        /// <summary>
        /// CLR类型。
        /// </summary>
        [Browsable(false)]
        public Type ClrType { get; set; }
        /// <summary>
        /// 是否为主键。
        /// </summary>
        [Browsable(false)]
        public bool IsKey { get; set; }
        /// <summary>
        /// 列索引。
        /// </summary>
        [Browsable(false)]
        public int? ColumnIndex { get; set; }
        /// <summary>
        /// 属性名。
        /// </summary>
        [Display(Order = 1, Name = "ColumnElement_PropertyName_Name", Description = "ColumnElement_PropertyName_Description", GroupName = "PropertyGrid_Category_Common", ResourceType = typeof(Res))]
        public string PropertyName { get; set; }
        /// <summary>
        /// 自定义类型名。
        /// </summary>
        [Display(Order = 4, Name = "ColumnElement_TypeName_Name", Description = "ColumnElement_TypeName_Description", GroupName = "PropertyGrid_Category_Common", ResourceType = typeof(Res))]
        public string TypeName { get; set; }
        /// <summary>
        /// 是否为空。
        /// </summary>
        [Display(Order = 2, Name = "ColumnElement_IsNullable_Name", Description = "ColumnElement_IsNullable_Description", GroupName = "PropertyGrid_Category_Common", ResourceType = typeof(Res))]
        public bool IsNullable { get; set; }
        /// <summary>
        /// 并发检查。
        /// </summary>
        [Display(Order = 3, Name = "ColumnElement_ConcurrencyCheck_Name", Description = "ColumnElement_ConcurrencyCheck_Description", GroupName = "PropertyGrid_Category_Common", ResourceType = typeof(Res))]
        public bool ConcurrencyCheck { get; set; } = false;
        /// <summary>
        ///  注释。
        /// </summary>
        [Display(Order = 5, Name = "ColumnElement_Comments_Name", Description = "ColumnElement_Comments_Description", GroupName = "PropertyGrid_Category_Common", ResourceType = typeof(Res))]
        public string Comments { get; set; }
        [Browsable(false)]
        public IdentityInfo Identity { get; set; }

        public override EObjectKind Kind => EObjectKind.Column;
    }

    public class IdentityInfo
    {
        /// <summary>
        /// 标识种子。
        /// </summary>
        public int Seed { get; set; } = 1;
        /// <summary>
        /// 标识增量。
        /// </summary>
        public int Increment { get; set; } = 1;
    }

    public class ColumnPrecisionElement : ColumnElement
    {
        public ColumnPrecisionElement(string name)
            : base(name)
        {
        }
        /// <summary>
        /// 精度。
        /// </summary>
        [Display(Name = "ColumnPrecisionElement_Precision_Name", Description = "ColumnPrecisionElement_Precision_Description", GroupName = "PropertyGrid_Category_Precision", ResourceType = typeof(Res))]
        public byte? Precision { get; set; }
        /// <summary>
        /// 小数位数。
        /// </summary>
        [Display(Name = "ColumnPrecisionElement_Scale_Name", Description = "ColumnPrecisionElement_Scale_Description", GroupName = "PropertyGrid_Category_Precision", ResourceType = typeof(Res))]
        public byte? Scale { get; set; }
    }

    public class ColumnLengthElement : ColumnElement
    {
        public ColumnLengthElement(string name)
            : base(name)
        {
        }
        /// <summary>
        /// 最大长度。
        /// </summary>
        [Display(Name = "ColumnLengthElement_MaxLength_Name", Description = "ColumnLengthElement_MaxLength_Description", GroupName = "PropertyGrid_Category_Length", ResourceType = typeof(Res))]
        public int? MaxLength { get; set; }
        /// <summary>
        /// 是否固定长度。
        /// </summary>
        [Display(Name = "ColumnLengthElement_IsFixed_Name", Description = "ColumnLengthElement_IsFixed_Description", GroupName = "PropertyGrid_Category_Length", ResourceType = typeof(Res))]
        public bool IsFixed { get; set; } = false;
    }

    public class ColumnStringElement : ColumnLengthElement
    {
        public ColumnStringElement(string name)
            : base(name)
        {
        }
        /// <summary>
        /// 是否Unicode编码
        /// </summary>
        [Display(Name = "ColumnStringElement_IsUnicode_Name", Description = "ColumnStringElement_IsUnicode_Description", GroupName = "PropertyGrid_Category_String", ResourceType = typeof(Res))]
        public bool IsUnicode { get; set; } = true;
    }
}
