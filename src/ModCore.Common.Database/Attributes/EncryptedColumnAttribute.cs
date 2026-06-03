using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Attributes
{
    public class EncryptedColumnAttribute : Attribute
    {
        public string[] ContextPropertyNames { get; private set; }
        public EncryptedColumnAttribute(params string[] contextPropertyNames)
        {
            this.ContextPropertyNames = contextPropertyNames;
        }
    }
}
