using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStation_PTL_Demo.Entity
{
    public class OrderV1
    {
        public int Time { get; set; }

        public TaskItemData[] Items { get; set; } = [];
    }
}
