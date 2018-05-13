using Caredev.MegoTools.Core.DbObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.MegoTools.Core.Generates
{
    internal class ClassGenerator
    {

        public ClassGenerator(ObjectElement element)
        {
            Element = element;
            Members = new Dictionary<string, MemberGenerator>();
        }

        public ObjectElement Element { get; }

        public string ClassName { get; set; }

        public string PropertyName { get; set; }

        public Dictionary<string, MemberGenerator> Members { get; }
    }
}
