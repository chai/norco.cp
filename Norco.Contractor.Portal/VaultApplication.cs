using System;
using System.Diagnostics;
using MFiles.VAF;
using MFiles.VAF.AppTasks;
using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFiles.VAF.Core;
using MFilesAPI;

namespace Norco.Contractor.Portal
{
    /// <summary>
    /// The entry point for this Vault Application Framework application.
    /// </summary>
    /// <remarks>Examples and further information available on the developer portal: http://developer.m-files.com/. </remarks>
    public partial class VaultApplication
        : ConfigurableVaultApplicationBase<Configuration>
    {
        /// <inheritdoc />
        public override void StartOperations(Vault vaultPersistent)
        {
            base.StartOperations(vaultPersistent);

            // An instance of the current configuration can be found in this.Configuration.
            //SysUtils.ReportInfoToEventLog(this.Configuration.ConfigValue1);
        }


    }
}