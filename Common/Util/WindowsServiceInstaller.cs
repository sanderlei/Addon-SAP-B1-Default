using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace Common.Util
{
    public class WindowsServiceInstaller
    {
        #region Properties
        private string ServiceName;
        private string DisplayName;
        private Assembly ServiceAssembly;
        #endregion

        #region Constructor
        /// <summary>
        /// Instalador de serviço
        /// </summary>
        /// <param name="serviceAssembly">Assembly do serviço</param>
        /// <param name="serviceName">Nome do serviço</param>
        /// <param name="displayName">Nome do serviço que irá aparecer no gerenciador de serviços</param>
        public WindowsServiceInstaller(Assembly serviceAssembly, string serviceName, string displayName)
        {
            this.ServiceAssembly = serviceAssembly;
            this.ServiceName = serviceName;
            this.DisplayName = displayName;
        }
        #endregion

        #region StandardInstallation
        public void StandardInstallation()
        {
            ServiceController sc = new ServiceController(ServiceName);
            if (!ServiceExists())
            {
                if (DialogResult.OK == MessageBox.Show("Deseja instalar o serviço " + DisplayName + "?", DisplayName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    try
                    {
                        this.InstallService();
                        this.StartService();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao instalar o serviço: " + ex.Message);
                    }
                }
            }
            else
            {
                if (DialogResult.OK == MessageBox.Show("Deseja desinstalar o serviço " + DisplayName + "?", DisplayName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    try
                    {
                        this.UninstallService();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao desinstalar o serviço: " + ex.Message);
                    }
                }
            }
        }
        #endregion

        #region Install
        public void InstallService()
        {
            if (IsInstalled()) return;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Unistall
        public void UninstallService()
        {
            if (!IsInstalled()) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Start
        public void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller = new ServiceController(ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region Stop
        public void StopService()
        {
            if (!IsInstalled()) return;
            using (ServiceController controller =
                new ServiceController(ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region IsInstalled
        public bool IsInstalled()
        {
            using (ServiceController controller =
                new ServiceController(this.ServiceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region IsRunning
        public bool IsRunning()
        {
            using (ServiceController controller =
                new ServiceController(ServiceName))
            {
                if (!IsInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }
        #endregion

        #region Exists
        private bool ServiceExists()
        {
            foreach (ServiceController sc in ServiceController.GetServices())
                if (sc.ServiceName == ServiceName)
                    return true;
            return false;
        }
        #endregion

        #region GetInstaller
        private AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(this.ServiceAssembly, null);
            installer.UseNewContext = true;
            return installer;
        }
        #endregion
    }
}
