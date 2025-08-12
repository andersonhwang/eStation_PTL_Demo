using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Entity
{
    public class CertificateOrder : SequenceEntity
    {
        public byte[] Certificate { get; set; } = [];
        public string CertificateMD5 { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public byte[] CAChain { get; set; } = [];
        public string CAChainMD5 { get; set; } = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CertificateOrder() => Code = 0x88;
    }
}
