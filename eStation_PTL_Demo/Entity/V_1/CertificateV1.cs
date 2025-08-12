using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStation_PTL_Demo.Entity.V_1
{
    public class CertificateV1
    {
        public byte[] Certificate { get; set; } = [];
        public string CertificateMD5 { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public byte[] CAChain { get; set; } = [];
        public string CAChainMD5 { get; set; } = string.Empty;
    }
}
