global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;

namespace CXCommenter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.CXCommenterString)]
    public sealed class CXCommenterPackage : ToolkitPackage
    {
        /// <summary>
        /// TODO: Add XML comment for InitializeAsync
        /// </summary>
        ///<param name="System.Threading.CancellationToken cancellationToken">TODO: Describe System.Threading.CancellationToken cancellationToken here</param>
        ///<param name="System.IProgress<Microsoft.VisualStudio.Shell.ServiceProgressData> progress">TODO: Describe System.IProgress<Microsoft.VisualStudio.Shell.ServiceProgressData> progress here</param>
        ///<returns>System.Threading.Tasks.Task</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}