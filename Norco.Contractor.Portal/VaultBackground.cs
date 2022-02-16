using MFiles.VAF.Common;
using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFiles.VAF.Extensions.Dashboards;
using MFiles.VAF.AppTasks;

namespace Norco.Contractor.Portal
{
    public partial class VaultApplication
    {


        [TaskQueue]

        public const string QueueId = "norco.contractor.portal.VaultApplication";

        public const string UpdateDocumentTaskType = "UpdateDocument";



        [TaskProcessor(QueueId, UpdateDocumentTaskType)]

        [ShowOnDashboard("Check document is valid.", ShowRunCommand = true)]
        public void ImportDataFromRemoteSystem(ITaskProcessingJob<TaskDirective> job)

        {
            var builder = new MFSearchBuilder(job.Vault, this.Configuration.IsValudFilter.ToApiObject(job.Vault));


            builder.Deleted(false);

            // Find items.

            var foundObjects = builder.FindEx();

            if (foundObjects != null || foundObjects.Count > 0)
            {
                job.Commit((transactionalVault) =>
                {
                    foreach (var objverex in foundObjects)
                    {


                        UpdateFoundObject(objverex, job.Vault);

                    }
                });

            }




        }
        // TODO: Connect to the remote system and import data.




        private bool UpdateObject(Vault vault, out bool currentlyExporting)
        {
            try
            {

                //Search Criteria
                /*
                 
                 iManageDocumentURL is empty
                 Class != iManageDocumentClass
                 */

                currentlyExporting = true;


                // Create our search builder.

                var searchBuilder = new MFSearchBuilder(vault);
                // Create our search builder.

                //foreach (var updateConditions in Configuration.FilterForObjects)
                //{
                //    var builder = new MFSearchBuilder(vault, updateConditions.ToApiObject(vault));



                //    // Find items.

                //    var foundObjects= builder.FindEx();
                //    if (foundObjects != null || foundObjects.Count>0)
                //    {
                //        foreach(var objverex in foundObjects)
                //        {
                //            UpdateFoundObject(objverex, vault);
                //        }
                //    }
                //}


            }
            catch(Exception ex)
            {
                currentlyExporting = false;
                return false;
            }

            currentlyExporting = false;
            return true;
        }

        private void UpdateFoundObject(ObjVerEx objVerEx, Vault vault)
        {
            try
            {
                if (objVerEx != null)
                {
                    objVerEx.SaveProperty(Configuration.PropertyToTouch, MFDataType.MFDatatypeText, $"Last Updated: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                }
            }
            catch(Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"Document Background update: {objVerEx.Title} {objVerEx.ID}", ex);
            }
        }

    }
}
