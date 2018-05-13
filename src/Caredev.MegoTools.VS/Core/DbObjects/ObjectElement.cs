using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res = Caredev.MegoTools.Properties.Resources;

namespace Caredev.MegoTools.Core.DbObjects
{

    public abstract class ObjectElement : ElementBase
    {
        public ObjectElement(string name, string schema)
          : base(name)
        {
            Schema = schema;
            if (string.IsNullOrEmpty(schema))
            {
                Key = name;
            }
            else
            {
                Key = schema + "." + name;
            }
            Columns = new Dictionary<string, ColumnElement>();
        }
        [Browsable(false)]
        public string Schema { get; }

        [Display(Name = "ObjectElement_ClassName_Name", GroupName = "PropertyGrid_Category_Common", Description = "ObjectElement_ClassName_Description", ResourceType = typeof(Res))]
        public string ClassName { get; set; }
        [Display(Name = "ObjectElement_PropertyName_Name", GroupName = "PropertyGrid_Category_Common", Description = "ObjectElement_PropertyName_Description", ResourceType = typeof(Res))]
        public string PropertyName { get; set; }
        /// <summary>
        ///  注释。
        /// </summary>
        [Display(Name = "ObjectElement_Comments_Name", Description = "ObjectElement_Comments_Description", GroupName = "PropertyGrid_Category_Common", ResourceType = typeof(Res))]
        public string Comments { get; set; }
        [Browsable(false)]
        public string Key { get; }
        [Browsable(false)]
        public Dictionary<string, ColumnElement> Columns { get; }
    }

    public class TableElement : ObjectElement
    {
        public TableElement(string name, string schema = "")
            : base(name, schema)
        {
        }

        public override EObjectKind Kind => EObjectKind.Table;
    }
    public class ViewElement : ObjectElement
    {
        public ViewElement(string name, string schema = "")
            : base(name, schema)
        {
        }

        public override EObjectKind Kind => EObjectKind.View;
    }
}
