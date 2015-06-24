using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controllers;

namespace Common.Models
{
    public class ItemModel
    {
        [ModelController(ColumnName = "ItemCode")]
        public string ItemCode { get; set; }

        [ModelController(ColumnName = "ItemName")]
        public string ItemName { get; set; }

        [ModelController(ColumnName = "U_linha")]
        public string LabelLineProd { get; set; }
    }
}
