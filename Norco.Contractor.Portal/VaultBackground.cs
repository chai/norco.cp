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
            try
            {
                //job.Commit((transactionalVault) =>
                //{
                //    try
                //    {
                //        job.Vault.PropertyDefOperations.Recalculate(Configuration.TestAutoproperty.ID, false);
                //    }
                //    catch(Exception ex)
                //    {
                //        SysUtils.ReportErrorToEventLog("Background task Runner - Suppressed", ex);
                //        //M-Files error see here:
                //        //https://www.m-files.com/api/documentation/#MFilesAPI~VaultPropertyDefOperations~Recalculate.html
                //    }
                //});


                // Find items.

                List<ObjVerEx> foundDocumentsObjects = FindObjVerExByFilter(job.Vault, this.Configuration.IsDocumentFilter.ToApiObject(job.Vault));

           //     foundDocumentsObjects.AddRange(FindObjVerExByFilter(job.Vault, this.Configuration.IsDocumentFilter.ToApiObject(job.Vault)));







                if (foundDocumentsObjects != null || foundDocumentsObjects.Count > 0)
                {


                        for (var i = 0; i < foundDocumentsObjects.Count; i++)
                        {


                            UpdateFoundObject(foundDocumentsObjects[i], job.Vault);

                            job.Update(
                            percentComplete: (int)Math.Ceiling((i + 1.00) / foundDocumentsObjects.Count * 100),
                            details: $"Processing object {(i + 1)} / {foundDocumentsObjects.Count}"
                            );
                        }
                    
                    job.Commit((transactionalVault) =>
                    {
                    });
                    job.Update(
                        percentComplete: 100,
                        details: $"Completed"
                    );
                }


            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("Background task Runner", ex);
            }

        }

        private void UpdateFoundObject(ObjVerEx objVerEx, Vault vault)
        {
            try
            {
                if (objVerEx != null)
                {
                    //PropertyValue pv = objVerEx.GetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefComment);
                    //    objVerEx.CheckOut();
                    //pv.TypedValue.SetValue(MFDataType.MFDatatypeMultiLineText, $"Check status by background task: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                    //    objVerEx.SaveProperty(pv);
                    //    objVerEx.CheckIn();



                        objVerEx.SaveProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefComment, MFDataType.MFDatatypeMultiLineText, $"Checked by background task at : {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                    
                    }
            }
            catch(Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"Document Background update: {objVerEx.Title} {objVerEx.ID}", ex);
            }
        }

        private List<ObjVerEx> FindObjVerExByFilter(Vault vault, SearchConditions searchConditions)
        {
            try
            {
                var documentSearchBuilder = new MFSearchBuilder(vault, searchConditions);// this.Configuration.IsDocumentFilter.ToApiObject(vault));


                documentSearchBuilder.Deleted(false);


                var dataFunctionCall = new DataFunctionCall();

                dataFunctionCall.SetDataDate(); // Ignore the time portion of the created timestamp.

                // Create the condition.

                var lastModifiedcondition = new SearchCondition();

                // Set the expression.

                lastModifiedcondition.Expression.SetPropertyValueExpression(

                    (int)MFilesAPI.MFBuiltInPropertyDef.MFBuiltInPropertyDefLastModified,

                    MFParentChildBehavior.MFParentChildBehaviorNone,

                    dataFunctionCall

                    );

                // Set the condition type.
                lastModifiedcondition.ConditionType = MFConditionType.MFConditionTypeLessThan;
                // Set the value.
                lastModifiedcondition.TypedValue.SetValue(MFDataType.MFDatatypeDate, DateTime.Today);
                // Add the condition to the collection for searching.
                documentSearchBuilder.Conditions.Add(-1, lastModifiedcondition);
                // Find items.
                return documentSearchBuilder.FindEx();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

    }
}
