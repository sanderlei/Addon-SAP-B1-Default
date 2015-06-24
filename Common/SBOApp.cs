using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using SAPbobsCOM;
using SAPbouiCOM;
using Common.Models;
using Common.Enums;
using Common.Controllers;
using System.Globalization;
using Company = SAPbobsCOM.Company;
using Translator;


namespace Common
{
    public class SBOApp
    {
        #region Variables

	    public static NumberFormatInfo NumberFormatInfo { get; private set; }

        public static TranslatorTool Translator { get; private set; }
		/// <summary>
		/// Identifica o formato da data e hora da aplicação.
		/// </summary>
		public static DateTimeFormatInfo DateTimeFormatInfo { get; private set; }

	    /// <summary>
	    /// Aplicação corrente do Business One
	    /// </summary>
	    public static Application Application { get; private set; }

	    /// <summary>
        /// Companhia da aplicação
        /// </summary>
        public static SAPbobsCOM.Company Company { get; set; }

	    /// <summary>
	    /// String que faz a conexão com o Business One
	    /// </summary>
	    public static string Connection { get; private set; }

	    /// <summary>
	    /// Caminho completo do assembly que irá conter as Views
	    /// </summary>
	    public static string ViewsAssemblyPath { get; private set; }

	    /// <summary>
		/// Assembly que irá conter as Views.
		/// </summary>
		public static Assembly ViewsAssembly { get; private set; }

		/// <summary>
		/// Namespace das Views.
		/// </summary>
		public string ViewsNamespace { get; private set; }

	    /// <summary>
	    /// ResourceManager das Views.
	    /// </summary>
		public static ResourceManager ViewsResourceManager { get; private set; }

	    #endregion

        #region Constructor
        public SBOApp()
        {
	        NumberFormatInfo = new NumberFormatInfo
		        {
			        NumberDecimalSeparator = ".",
			        NumberGroupSeparator = ","
		        };

	        DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
		}

        /// <summary>
        /// Objeto do tipo SAP Business One Application
        /// </summary>
        /// <param name="connection">String passada nos parametros para conectar o add-on ao SAP</param>
        /// <param name="viewsAssemblyPath">Assembly que possui as telas a serem criadas</param>
        public SBOApp(String connection, String viewsAssemblyPath)
			: this()
        {
            Connection = connection;
            ViewsAssemblyPath = viewsAssemblyPath;

			ViewsAssembly = Assembly.LoadFile(ViewsAssemblyPath);
			ViewsNamespace = ViewsAssembly.FullName.Substring(0, ViewsAssembly.FullName.IndexOf(','));
			ViewsResourceManager = new ResourceManager(ViewsNamespace + ".Properties.Resources", ViewsAssembly);
		}

	    #endregion

        #region InitializeApplication
        /// <summary>
        /// Inicia a aplicação
        /// </summary>
        public void InitializeApplication()
        {
            var oSboGuiApi = new SAPbouiCOM.SboGuiApi();

            try
            {
                // Conecta a uma aplicação do SBO que esteja rodando                                                    
                oSboGuiApi.Connect(Connection);
                
                // Obtém o objeto da aplicação inicializada pelo método Connect acima executado                
                Application = oSboGuiApi.GetApplication(-1);

                Application.StatusBar.SetText("Conexão do add-on: " + System.Windows.Forms.Application.ProductName, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                // Obtém a companhia da aplicação 
                Company = (SAPbobsCOM.Company)Application.Company.GetDICompany();

                Translator = new TranslatorTool();

				// Inicializa informações de formato conforme informações da aplicação
				FormatInitializer();

                this.SetEvents();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Não foi possível conectar ao SAP Business One: " + ex.Message, "Erro na conexão", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

                // Finalização do add-on                                                                 
                System.Environment.Exit(0);
            }
        }

		private void FormatInitializer()
		{
			var adminInfo = Company.GetCompanyService().GetAdminInfo();

			#region NumberFormatInfo

			NumberFormatInfo = new NumberFormatInfo
				{
					NumberDecimalSeparator = adminInfo.DecimalSeparator,
					NumberGroupSeparator = adminInfo.ThousandsSeparator,
				};

			#endregion

			#region DateTimeFormatInfo

			string dateFormatPattern;

			switch (adminInfo.DateTemplate)
			{
				case BoDateTemplate.dt_DDMMYY:
					dateFormatPattern = "dd{0}MM{0}yy";
					break;
				case BoDateTemplate.dt_DDMMCCYY:
					dateFormatPattern = "dd{0}MM{0}yyyy";
					break;
				case BoDateTemplate.dt_MMDDYY:
					dateFormatPattern = "MM{0}dd{0}yy";
					break;
				case BoDateTemplate.dt_MMDDCCYY:
					dateFormatPattern = "MM{0}dd{0}yyyy";
					break;
				case BoDateTemplate.dt_CCYYMMDD:
					dateFormatPattern = "yyyy{0}MM{0}dd";
					break;
				case BoDateTemplate.dt_DDMonthYYYY:
					dateFormatPattern = "dd{0}MMMM{0}yyyy";
					break;
				default: // Desconhecido
					throw new InvalidOperationException("Formato de data identificado não é conhecido (AdminInfo.DateTemplate).");
			}

			var datePattern = String.Format(dateFormatPattern, adminInfo.DateSeparator);
			var timePattern = adminInfo.TimeTemplate == BoTimeTemplate.tt_24H ? "HH:mm" : "hh:mm";

			var newDateTimeFormat = (DateTimeFormatInfo) DateTimeFormatInfo.Clone();

			newDateTimeFormat.DateSeparator = adminInfo.DateSeparator;
			newDateTimeFormat.LongDatePattern = datePattern;
			newDateTimeFormat.ShortDatePattern = datePattern;
			newDateTimeFormat.LongTimePattern = timePattern;
			newDateTimeFormat.ShortTimePattern = timePattern;

			DateTimeFormatInfo = newDateTimeFormat;

			#endregion
		}

        public static int ConnectToCompany(Company customCompany)
        {
            Translator = new TranslatorTool();
            Company = customCompany;
            
            return Company.Connect();
        }

        public static string TranslateToHana(string sql)
        {
            int count;
            int errCount;

            if (Company.DbServerType == (BoDataServerTypes)9) // 9 = Hana
            {
                if (Translator == null)
                {
                    Translator = new TranslatorTool();
                }
                sql = Translator.TranslateQuery(sql, out count, out errCount);
                sql = sql.Substring(0, sql.Length - 3);
            }
            return sql;

        }
        #endregion

        #region SetEvents
        /// <summary>
        /// Seta os eventos da aplicação
        /// </summary>
        public virtual void SetEvents()
        {
            Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(EventController.AppEvent);
            Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(EventController.FormDataEvent);
            Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(EventController.ItemEvent);
            Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(EventController.MenuEvent);
            Application.PrintEvent += new SAPbouiCOM._IApplicationEvents_PrintEventEventHandler(EventController.PrintEvent);
            Application.ProgressBarEvent += new SAPbouiCOM._IApplicationEvents_ProgressBarEventEventHandler(EventController.ProgressBarEvent);
            Application.ReportDataEvent += new SAPbouiCOM._IApplicationEvents_ReportDataEventEventHandler(EventController.ReportDataEvent);
            Application.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(EventController.RightClickEvent);
            Application.StatusBarEvent += new SAPbouiCOM._IApplicationEvents_StatusBarEventEventHandler(EventController.StatusBarEvent);
        }
        #endregion
        
    }
}

