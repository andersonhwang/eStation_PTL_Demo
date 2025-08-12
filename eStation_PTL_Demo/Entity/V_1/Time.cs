using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStation_PTL_Demo.Entity
{
    public class Time : SequenceEntity
    {
        public Time() => Code = 0x70;

        public DateTime ApTime { get; set; } = DateTime.Now;
    }
}
