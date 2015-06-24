using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SAPbouiCOM;
using System.Xml;
using Common.Controllers;

namespace Common.Forms
{
	public class BaseForm : Common.Interfaces.IForm
	{
		#region Properties

		private static readonly Dictionary<Type, int> _formId; 
		private static readonly Dictionary<Type, int> _formCount;
		private StringReader _srdSrfFile;
		private string _srfFolder;
		private XmlDocument _formXml;

		public ItemEvent ItemEventInfo { get; set; }
		public BusinessObjectInfo BusinessObjectInfo { get; set; }
		public ContextMenuInfo ContextMenuInfo { get; set; }
		public MenuEvent MenuEventInfo { get; set; }
		private Form _form;
        
		protected virtual bool IsSystemForm { get { return false; } }
        
		public XmlDocument FormXml
		{
			get
			{
				if (_formXml == null)
				{
					var xml = SBOApp.ViewsResourceManager.GetString("x" + FormID);

					if (xml != null)
					{
						_formXml = new XmlDocument();

						_formXml.LoadXml(xml);
					}
				}

				return _formXml;
			}
		}

		/// <summary>
		/// Arquivo Srf convertido para o tipo StringReader
		/// </summary>
		public StringReader SrdSrfFile
		{
			get
			{
				if (_srdSrfFile == null)
				{
					var srf = SBOApp.ViewsResourceManager.GetString("f" + FormID);

					if (srf != null)
					{
						_srdSrfFile = new StringReader(srf);
					}
				}

				return _srdSrfFile;
			}
			set { _srdSrfFile = value; }
		}

		/// <summary>
		/// Pasta aonde esta o arquivo Srf (Caso nao utilize o srf no tipo StringReader)
		/// </summary>
		public virtual string SrfFolder { get { return _srfFolder ?? "srfFiles"; } set { _srfFolder = value; } }

		/// <summary>
		/// ID unico do form
		/// </summary>
		public virtual int FormID
		{
			get
			{
				int value;

				if (!_formId.TryGetValue(GetType(), out value))
				{
					var className = GetType().Name;

					if (!int.TryParse(className.Substring(className.Length < 5 ? 1 : (className.Length - 4)), out value))
					{
						throw new NotImplementedException();
					}

					_formId[GetType()] = value;
				}

				return value;
			}

			set { _formId[GetType()] = value; }
		}

		/// <summary>
		/// Count único do form
		/// </summary>
	    public virtual int FormCount
	    {
		    get
		    {
			    int count;

				if (!_formCount.TryGetValue(GetType(), out count))
				{
					count = 0;
				}

			    return count;
		    }

			set { _formCount[GetType()] = value; }
	    }

	    /// <summary>
        /// EditText de controle da navegacao
        /// </summary>
        public string BrowseBy { get; set; }

	    protected Form Form
	    {
		    get
		    {
			    if (_form == null)
			    {
				    string formId = null;

				    if (ItemEventInfo != null) formId = ItemEventInfo.FormUID;
				    if (BusinessObjectInfo != null) formId = BusinessObjectInfo.FormUID;
				    if (ContextMenuInfo != null) formId = ContextMenuInfo.FormUID;

					if (formId == null)
					    throw new Exception("Para instanciar o form é necessário estar em um formulário.");

				    _form = SBOApp.Application.Forms.Item(formId);
			    }

			    return _form;
		    }
	    }

	    #endregion

        #region Methods

		static BaseForm()
		{
			_formId = new Dictionary<Type, int>();
			_formCount = new Dictionary<Type, int>();
		}

        public virtual void Freeze(bool freeze)
        {
			//if (ItemEventInfo.EventType != BoEventTypes.et_FORM_UNLOAD)
			//	form = SBOApp.Application.Forms.GetFormByTypeAndCount(ItemEventInfo.FormType, ItemEventInfo.FormTypeCount);

			//if (form != null)
			//	form.Freeze(freeze);
			Form.Freeze(freeze);
        }

        #endregion

        #region Events

        public virtual object Show()
        {
			if (FormXml != null)
			{
				_form = FormController.GenerateForm(FormXml, FormID, FormCount);
			}
            else if (SrdSrfFile != null)
            {
                _form = FormController.GenerateForm(SrdSrfFile, FormID, FormCount);
            }
            else
            {
                _form = FormController.GenerateForm(SrfFolder, FormID, FormCount);
            }

            if (!String.IsNullOrEmpty(BrowseBy))
            {
                _form.DataBrowser.BrowseBy = BrowseBy;
            }

            return _form;
        }

        public virtual object Show(string[] args)
        {
            throw new NotImplementedException();
        }

        public virtual bool ItemEvent()
        {
			if (IsSystemForm)
			{
				if (ItemEventInfo.EventType == BoEventTypes.et_FORM_LOAD)
				{
					if (!ItemEventInfo.BeforeAction)
					{
						var xmlDocument = FormXml;
						var form = xmlDocument.SelectSingleNode("/Application/forms/action[@type='update']/form");
						var formUID = form.Attributes["uid"];

						form.Attributes.RemoveAll();
						formUID.Value = Form.UniqueID;
						form.Attributes.Append(formUID);

						var innerXml = xmlDocument.InnerXml;
						SBOApp.Application.LoadBatchActions(ref innerXml);
					}
				}
			}

            return true;
        }

        public virtual bool FormDataEvent()
        {
            return true;
        }

        public virtual bool AppEvent()
        {
            return true;
        }

        public virtual bool MenuEvent()
        {
			// Se o Form for referênciado diretamente do Menu
			if (MenuEventInfo.BeforeAction && MenuEventInfo.MenuUID == GetType().Name.Substring(1))
			{
				// Criar o formulário
				Show();

				return true;
			}

	        return false;
        }

        public virtual bool PrintEvent()
        {
            return true;
        }

        public virtual bool ProgressBarEvent()
        {
            return true;
        }

        public virtual bool ReportDataEvent()
        {
            return true;
        }

        public virtual bool RightClickEvent()
        {
            return true;
        }

        public virtual bool StatusBarEvent()
        {
            return true;
        }
        #endregion
    }
}
