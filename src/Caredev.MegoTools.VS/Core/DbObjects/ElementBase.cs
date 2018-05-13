using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Caredev.MegoTools.Core.DbObjects
{
    public abstract class ElementBase
    {
        public ElementBase(string name)
        {
            Name = name;
        }
        [Browsable(false)]
        public string Name { get; }

        [Browsable(false)]
        public abstract EObjectKind Kind { get; }
    }
}
