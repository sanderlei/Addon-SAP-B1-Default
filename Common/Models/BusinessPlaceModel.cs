using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controllers;

namespace Common.Models
{
    public class BusinessPlaceModel
    {
        [ModelController]
        public int BPlId { get; set; }
        
        [ModelController]
        public string BPlName { get; set; }

        [ModelController(ColumnName = "TaxIdNum")]
        public string Cnpj { get; set; }
    }
}
