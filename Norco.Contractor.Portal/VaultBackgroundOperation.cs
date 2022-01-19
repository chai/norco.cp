using MFiles.VAF.AppTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFiles.VAF.Configuration;
using MFiles.VAF.Extensions.Dashboards;
using MFilesAPI;
using MFiles.VAF.Common;

namespace Norco.Contractor.Portal
{
    public partial class VaultApplication
    {
        [TaskQueue]
        public const string QueueId = "Norco.Contractor.Portal";
        public const string UpdateDocumentIsValid = "isDocumentValid";

        [TaskProcessor(QueueId, UpdateDocumentIsValid, TransactionMode = TransactionMode.Full)]
        [ShowOnDashboard("Update if a document is valid", ShowRunCommand = true)]
        public void UpdateDocumentIsValidProperty(ITaskProcessingJob<TaskDirective> job)
        {
            try
            {

                System.Diagnostics.Debugger.Launch();
                var searchBuilder = new MFSearchBuilder(job.Vault);
                searchBuilder.ObjType((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument);
                searchBuilder.Property(Configuration.IsDocumentValid, MFDataType.MFDatatypeBoolean, true);
                var objVerExList = searchBuilder.FindEx();
                foreach(var objVerEx in objVerExList)
                {

      
                    job.Vault.ObjectFileOperations.UpdateMetadataInFile(objVerEx.ObjVer, -1, false);
                    

                }

            }
            catch (Exception ex)
            {

            }
        }

    }
}
