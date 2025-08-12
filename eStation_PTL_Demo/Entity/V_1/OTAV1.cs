using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Entity.V_1
{
    public class OTAV1
    {
        public int Type { get; init; } = 0;
        public string Version { get; init; } = "";
        public FactoryCode Factory { get; init; } = FactoryCode.PHY6252;
        public string MD5 { get; init; } = string.Empty;
        public byte[] Firmware { get; init; } = [];
    }
}
