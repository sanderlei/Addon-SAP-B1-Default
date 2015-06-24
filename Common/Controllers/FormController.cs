using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SAPbouiCOM;
using SAPbobsCOM;

namespace Common.Controllers
{
    public class FormController
    {
        /// <summary>
        /// Cria o objeto do tipo IForm
        /// </summary>
        /// <typeparam name="T">Tipo do evento</typeparam>
        /// <param name="form">Tipo do Form</param>
        /// <param name="evento">Objeto do evento</param>
        /// <returns>Objeto IForm</returns>
        public static Interfaces.IForm CreateForm<T>(Type form, ref T evento)
        {
            ConstructorInfo constructor = form.GetConstructor(new Type[] { typeof(T) });
            Interfaces.IForm newForm = null;

            try
            {
                newForm = (Interfaces.IForm)constructor.Invoke(new object[] { evento });
            }
            catch (NullReferenceException) { }
            catch (Exception ex) { throw ex; }

            return newForm;
        }

        /// <summary>
        /// Retorna o Tipo do objeto de acordo com o ID
        /// </summary>
        /// <param name="id">Id do Form</param>
        /// <returns>Tipo do Form</returns>
        public static Type GetFormType(String id)
        {
			//String sFormName = String.Empty;
			//String sFormId = id;

			//// Cria assembly de acordo com a variavel ViewAssemblyName
			////Assembly assemblyObj = Assembly.LoadFile(SBOApp.ViewsAssemblyPath);
			//var assemblyObj = SBOApp.ViewsAssembly;

			//// início-foreach :: Percorre os objetos do assembly para encontrar o tipo a ser criado
			//foreach (Type feType in assemblyObj.GetTypes())
			//{
			//	// início-if :: Se a variável feType contiver o nome completo do
			//	//              processo do add-on que de fato está sendo executado 
			//	//              no momento, armazena esse nome na variável sFormName
			//	if (feType.FullName.EndsWith("f" + sFormId))
			//	{
			//		sFormName = feType.FullName;
			//		break;
			//	}
			//	// fim-if
			//}

			//if (!String.IsNullOrEmpty(sFormName))
			//{
			//	return assemblyObj.GetType(sFormName);
			//}
			//else
			//{
			//	return null;
			//}

	        var className = "f" + id;
	        var assembly = SBOApp.ViewsAssembly;

			var type = assembly.GetTypes().SingleOrDefault(t => t.Name == className);

	        return type;
        }

        /// <summary>
        /// Atualiza os dados do form
        /// Utilizado em forms do B1 em que o método Form.Update() não funciona
        /// </summary>
        /// <param name="formType">Id do form</param>
        /// <returns>Executado com sucesso</returns>
        public static bool UpdateForm(int formType)
        {
            // A atualização do form é feita navegando para o próximo registro e voltando para o original
            // Não foi encontrada uma maneira mais elegante para atualizar um form do B1
            try
            {
                // Busca o form
                Form frmUpdateForm = SBOApp.Application.Forms.GetFormByTypeAndCount(formType, 1);
                // Seta o foco no form
                frmUpdateForm.Select();

                frmUpdateForm.Freeze(true);

                // Navega para o próximo item
                MenuItem menu = SBOApp.Application.Menus.Item("1288");
                menu.Enabled = true;
                menu.Activate();

                // Volta para o item original
                menu = SBOApp.Application.Menus.Item("1289");
                menu.Enabled = true;
                menu.Activate();
                frmUpdateForm.Freeze(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void OpenForm(string srfFileName)
        {
            string appPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            if (!appPath.EndsWith(@"\")) appPath += @"\";
            if (!srfFileName.EndsWith(".srf"))
                srfFileName += ".srf";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(String.Format(@"{0}srfFiles\{0}", appPath, srfFileName));
            SBOApp.Application.LoadBatchActions(xmlDoc.InnerXml);
        }

        public static Form GenerateForm(string srfPath, int formID, int formCount)
        {
            var appPath = Environment.CurrentDirectory;

            if (!appPath.EndsWith(@"\")) appPath += @"\";

	        var fileName = String.Format(@"{0}{1}\f{2}.", appPath, srfPath, formID);
	        var fi = new FileInfo(fileName + "srf");

			if (!fi.Exists)
			{
				fi = new FileInfo(fileName + "xml");

				if (!fi.Exists)
				{
					throw new FileNotFoundException(String.Format("O arquivo {0} não foi encontrado.", fi.FullName));
				}
			}

			var xmlDoc = new XmlDocument();
	        var creationPackage =
		        (FormCreationParams) SBOApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);

            xmlDoc.Load(fi.FullName);

            creationPackage.XmlData = xmlDoc.InnerXml;
            creationPackage.UniqueID = String.Format("f200000{0}_{1}", formID, formCount);

            return SBOApp.Application.Forms.AddEx(creationPackage);
        }

		public static Form GenerateForm(XmlDocument xmlDoc, int formID, int formCount)
		{
			var creationPackage =
				(FormCreationParams) SBOApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);

			creationPackage.XmlData = xmlDoc.InnerXml;
			creationPackage.UniqueID = String.Format("f200000{0}_{1}", formID, formCount);

			return SBOApp.Application.Forms.AddEx(creationPackage);
		}

        public static Form GenerateForm(StringReader srfFile, int formID, int formCount)
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(srfFile);

	        return GenerateForm(xmlDoc, formID, formCount);
        }

        public static void AddDataTable(Form form, string dataTableId)
        {
            for (int i = 0; i < form.DataSources.DataTables.Count; i++)
            {
                if (form.DataSources.DataTables.Item(i).UniqueID == dataTableId)
                {
                    return;
                }
            }
            form.DataSources.DataTables.Add(dataTableId);
        }
    }
}