using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStation_PTL_Demo.Entity
{
    public class Message
    {
        public required int ConnType { get; set; }

        public required string Data { get; set; }
    }
}
