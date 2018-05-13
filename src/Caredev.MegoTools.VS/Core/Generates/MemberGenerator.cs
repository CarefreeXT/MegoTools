using Caredev.MegoTools.Core.DbObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core.Generates
{
    internal class MemberGenerator
    {
        public MemberGenerator(ColumnElement element, string propertyName)
        {
            Element = element;
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public ColumnElement Element { get; }
    }
}
