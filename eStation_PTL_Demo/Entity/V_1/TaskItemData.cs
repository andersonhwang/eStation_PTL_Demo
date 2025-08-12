using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStation_PTL_Demo.Entity
{
    public class TaskItemData()
    {
        public string TagID { get; set; } = string.Empty;
        public bool Beep { get; set; }
        public bool? Flashing { get; set; }

        public Color[] Colors { get; set; } = [];

    }
}
