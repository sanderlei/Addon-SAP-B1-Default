using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controllers;

namespace Common.Models
{
    public class CityModel
    {
        [ModelController()]
        public int AbsId { get; set; }

        [ModelController()]
        public string Code { get; set; }

        [ModelController()]
        public string State { get; set; }

        [ModelController()]
        public string Name { get; set; }

        [ModelController()]
        public string TaxZone { get; set; }

        [ModelController()]
        public string IbgeCode { get; set; }

        [ModelController()]
        public string GiaCode { get; set; }
    }
}
