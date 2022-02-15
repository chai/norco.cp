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

                        SysUtils.ReportInfoToEventLog($"Updating {objverex.Title} ({objverex.ID})");
                        UpdateFoundObject(objverex, job.Vault);

                    }
                });

            }




        }
        // TODO: Connect to the remote system and import data.



        /*
        protected override void StartApplication()

        {



            // Start writing content to the event log every ten seconds.

            // The background operation will continue until the vault goes offline.



            // Instantiate an MFilesServerApplication object.

            // https://www.m-files.com/api/documentation/latest/MFilesAPI~MFilesServerApplication.html

            var mfServerApplication = new MFilesServerApplication();

               System.Diagnostics.Debugger.Launch();

            Vault vault = null;


            bool currentlyExporting = false;
            this.BackgroundOperations.StartRecurringBackgroundOperation("Updating AutoCalculated fields",
               CalculateSpanToNextUpdate(), () =>
               {
                   if (Configuration.TaskConfig.StartAutoCalculatePolling)
                   {
                       try
                       {



                           if (!currentlyExporting)
                           {
                               currentlyExporting = true;
                               //Flag to stop multiple update if the update takes longer than period of poll.
                               mfServerApplication.Connect(
                                   AuthType: Configuration.AuthType,
                                   UserName: Configuration.MFilesUsername,
                                   Password: Configuration.MFilesPassword,
                                   Domain: Configuration.Domain
                                   );


                               // Obtain a connection to the vault with GUID {C840BE1A-5B47-4AC0-8EF7-835C166C8E24}.

                               // Note: this will except if the vault is not found.

                               vault = mfServerApplication.LogInToVault(
                                   Configuration.VaultGUID);






                               //string filename = $"{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}." +
                               //$"{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";

                               // SysUtils.ReportInfoToEventLog("Start Auto Calculate force update.");
                               if (UpdateObject(vault, out currentlyExporting))
                               {
                                   // SysUtils.ReportInfoToEventLog($@"MYOB Exported to : {ExportLocation}{filename}");
                               }
                               else
                               {
                                   SysUtils.ReportInfoToEventLog("Auto Calculate Failed.");
                               }
                           }
                       }
                       catch (Exception)
                       {

                       }
                       finally
                       {
                           try
                           {
                               if (vault != null && vault.LoggedIn)
                               {
                                   vault.LogOutSilent();
                                   mfServerApplication.Disconnect();
                               }
                           }
                           catch (Exception)
                           {

                           }

                       }
                   }

               });


            


        }
        */
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
                    objVerEx.SaveProperty(Configuration.TestText, MFDataType.MFDatatypeMultiLineText, $"Updated {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                    //objVerEx.CheckOut();
                    //vault.ObjectFileOperations.UpdateMetadataInFile(objVerEx.ObjVer, -1, false);
                    //objVerEx.CheckIn();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private TimeSpan CalculateSpanToNextUpdate()
        {
            if (Configuration.TaskConfig.AutoCalculatePolling == -1)
            {
                //debug options
                return TimeSpan.FromSeconds(10);
            }
            else if (Configuration.TaskConfig.AutoCalculatePolling > 0)
            {
                // Hourly polling mode
                return TimeSpan.FromHours(Configuration.TaskConfig.AutoCalculatePolling);
            }

            TimeSpan timeSpan = new TimeSpan();

            DateTime dateNow = DateTime.Now;

            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, Configuration.TaskConfig.AutoCalculateAtTime, 0, 0);

            if (date > dateNow)
                timeSpan = date - dateNow;
            else
            {
                date = date.AddDays(1);
                timeSpan = date - dateNow;
            }



            return timeSpan;
        }
    }
}
