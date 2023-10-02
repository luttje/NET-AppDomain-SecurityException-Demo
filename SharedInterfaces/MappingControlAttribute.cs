using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MappingControlAttribute : Attribute
    {
        public string Title { get; set; }
        public int Order { get; set; } = 0;
    }
}
