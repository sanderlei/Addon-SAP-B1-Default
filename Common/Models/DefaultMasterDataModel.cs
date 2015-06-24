using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controllers;

namespace Common.Models
{
    public class DefaultMasterDataModel
    {
        [ModelController()]
        public string Code { get; set; }

        [ModelController()]
        public string Name { get; set; }
    }
}
