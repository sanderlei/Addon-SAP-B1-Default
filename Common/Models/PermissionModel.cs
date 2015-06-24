using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class PermissionModel
    {
        public string PermissionID { get; set; }
        public string Name { get; set; }
        public SAPbobsCOM.BoUPTOptions Options { get; set; }
        public string ParentId { get; set; }
        public SAPbobsCOM.BoYesNoEnum IsItem { get; set; }
        public string FormType { get; set; }
    }
}
