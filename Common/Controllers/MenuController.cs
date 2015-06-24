using System;
using System.Linq;
using System.Windows.Forms;
using Common.Enums;
using Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

namespace Common.Controllers
{
    public class MenuController
    {
        public MenuController()
        {

        }

        public static void LoadFromXML(string fileName)
        {
            System.Xml.XmlDocument oXmlDoc = null;

            oXmlDoc = new System.Xml.XmlDocument();
            oXmlDoc.Load(fileName);

			var node = oXmlDoc.SelectSingleNode("/Application/Menus/action/Menu");
	        var imageAttr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Name == "Image");

			if (imageAttr != null && !String.IsNullOrWhiteSpace(imageAttr.Value))
			{
				imageAttr.Value = String.Format(imageAttr.Value, Application.StartupPath + @"\img");
			}

            // load the form to the SBO application in one batch
            string tmpStr;
            tmpStr = oXmlDoc.InnerXml;
            SBOApp.Application.LoadBatchActions(ref tmpStr);
            string xml = SBOApp.Application.GetLastBatchResults();
            
        
        } 

        /// <summary>
        /// Adiciona novo item no menu
        /// </summary>
        /// <param name="menu">Item a ser adicionado</param>
        public void Add(MenuModel menu)
        {
            SAPbouiCOM.MenuItem oMenuItem = null;
            SAPbouiCOM.Menus oMenus = SBOApp.Application.Menus;
            SAPbouiCOM.MenuCreationParams oCreationPackage = null;

            if (!String.IsNullOrEmpty(menu.Parent))
            {
                oMenuItem = SBOApp.Application.Menus.Item(menu.Parent);
            }

            if (oMenuItem.SubMenus != null)
            {
                oMenus = oMenuItem.SubMenus;
            }

            if (!oMenus.Exists(menu.UniqueID))
            {
                oCreationPackage = (SAPbouiCOM.MenuCreationParams)SBOApp.Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
                oCreationPackage.Type = (SAPbouiCOM.BoMenuType)Convert.ToInt32(menu.Type);
                oCreationPackage.UniqueID = menu.UniqueID;
                oCreationPackage.String = menu.Description;
                oCreationPackage.Enabled = menu.Enabled;

                if (menu.Type == EnumMenuType.mt_POPUP)
                {
                    oCreationPackage.Image = menu.ImageName;
                }

                oCreationPackage.Position = menu.Position;

                oMenus.AddEx(oCreationPackage);

         
            }
        }

        /// <summary>
        /// Remove item do menu
        /// </summary>
        /// <param name="menu">Item a ser removido</param>
        public void Remove(MenuModel menu)
        {
            SAPbouiCOM.MenuItem oMenuItem = null;
            SAPbouiCOM.Menus oMenus;

            oMenus = SBOApp.Application.Menus;
            if (!String.IsNullOrEmpty(menu.Parent))
            {
                oMenuItem = SBOApp.Application.Menus.Item(menu.Parent);
            }

            if (oMenuItem.SubMenus != null)
            {
                oMenus = oMenuItem.SubMenus;
            }

            if (oMenus.Exists(menu.UniqueID))
            {
                oMenus.Remove(oMenuItem);
            }
        }


    }
}