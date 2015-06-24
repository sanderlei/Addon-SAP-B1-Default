using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class DocumentItemModel
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public string Warehouse { get; set; }
        public string Account { get; set; }
    }
}
