using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTaxi_Entities
{
    public class Location
    {
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Long { get; set; }
        public string Name { get; set; }
        public Nullable<bool> SupportsMap { get; set; }
    }
}
