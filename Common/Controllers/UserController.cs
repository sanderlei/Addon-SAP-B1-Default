using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Controllers
{
    public class UserController : SAPObjectBaseController
    {
        public UserController()
            : base("OUSR")
        { }

        public static int GetUserId(string userCode)
        {
            UserController userController = new UserController();
            string userId = userController.Exists("USERID", String.Format("USER_CODE = '{0}'", userCode));
            
            return Convert.ToInt32(userId);            
        }
    }
}
