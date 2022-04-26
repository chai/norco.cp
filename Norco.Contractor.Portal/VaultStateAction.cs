using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Norco.Contractor.Portal
{
    public partial class VaultApplication
    {
        [StateAction("WFS.DocumentExpiryNotification.30Days")]
        public void DocumentExpiredIn30Days(StateEnvironment env)
        {
            try
            {
                try
                {
                    SendEmail(false, env);

                    try
                    {


                        var propertyValues = env.ObjVerEx.Properties;
                        propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefWorkflow, MFDataType.MFDatatypeLookup, Configuration.DocumentRequestWorkflow);
                        propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefState, MFDataType.MFDatatypeLookup, Configuration.InitialDocumentRequestState);
                        propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass, MFDataType.MFDatatypeLookup, Configuration.DocumentRequestClass);
                        propertyValues.SetProperty(Configuration.ExpiredDocument, MFDataType.MFDatatypeLookup, env.ObjVer.ID);

                        propertyValues.RemoveProperty(Configuration.IsDocumentValid);
                        propertyValues.RemoveProperty(Configuration.SingleFile);
                        propertyValues.RemoveProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefObjectID);
                        propertyValues.RemoveProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefNameOrTitle);

                        var docRequest = env.Vault.ObjectOperations.CreateNewObjectExQuick(
                              Configuration.DocumentRequestObject, propertyValues, null, false, true, null);

                        
                        env.ObjVerEx.SaveProperty(Configuration.DocumentRequest, MFDataType.MFDatatypeLookup, docRequest);
                    }
                    catch(Exception createDocumentRequest)
                    {
                        SysUtils.ReportErrorToEventLog("DocumentExpiredIn30Days.", $"Creating document request failed.{ObjectDetails(env.ObjVerEx)}", createDocumentRequest);
                    }
                }
                catch (Exception ex)
                {
                    SysUtils.ReportErrorToEventLog("DocumentExpiredIn30Days.", $"Error in State Action for 30 days to expiry. {ObjectDetails(env.ObjVerEx)}", ex);

                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"DocumentExpiredIn30Days.", $"Error in State Action for 30 days to expiry. {ObjectDetails(env.ObjVerEx)}", ex);

            }
        }


        [StateAction("WFS.DocumentExpiryNotification.7Days")]
        public void DocumentExpiredIn7DaysEmailNotification(StateEnvironment env)
        {
            try
            {
                SendEmail(false, env);
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"DocumentExpiredIn7Days.", $"Error in State Action for 7 days to expiry.{ObjectDetails(env.ObjVerEx)}", ex);

            }
        }
        [StateAction("WFS.DocumentExpiryNotification.Expired")]
        public void DocumentExpiredEmailNotification(StateEnvironment env)
        {
            try
            {
                SendEmail(true, env);
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"DocumentExpired.", $"Error in State Action for Expired.{ObjectDetails(env.ObjVerEx)}", ex);

            }
        }
      
        
        [StateAction ("WFS.TemporaryDocumentUploaded.DocumentUploaded")]
        public void ConvertDocumentClass(StateEnvironment env)
        {

            try
            {
                var parentDocumentRequest = env.ObjVerEx.GetDirectReference(Configuration.DocumentRequestsOwner);
                if (parentDocumentRequest != null)
                {
                    var expiredDoc = parentDocumentRequest.GetDirectReference(Configuration.ExpiredDocument);
                    if (expiredDoc != null)
                    {
                        var renewalDocument = Configuration.RenewalDocuments.Find(r => r.DocumentType.ID == expiredDoc.Class);
                        if (renewalDocument != null)
                        {
                            foreach (var propertyToTrim in renewalDocument.PropertyToRemove)
                            {
                                env.ObjVerEx.RemoveProperty(propertyToTrim.Prop);
                            }
                            foreach (var propertyToCopy in renewalDocument.PropertyToCopyOver)
                            {
                                env.ObjVerEx.SetProperty(expiredDoc.Properties.GetProperty(propertyToCopy.Prop));
                            }

                        }



                        env.ObjVerEx.SaveProperties();
                        parentDocumentRequest.SetProperty(Configuration.DocumentUploaded, MFDataType.MFDatatypeBoolean, true);
                        parentDocumentRequest.SetProperty(Configuration.ReplacementDocument, MFDataType.MFDatatypeLookup, env.ObjVer.ID);
                        parentDocumentRequest.SaveProperties();
                    }
                }
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"DocumentUploaded.", $"Error in State Action for 30 days to expiry. {ObjectDetails(env.ObjVerEx)}", ex);
            }
        }





        [StateAction("WFS.DocumentRequest.RequestedDocuentValidated")]
        public void SetValidatedBy(StateEnvironment env)
        {

            try
            {
                var replacementDocument = env.ObjVerEx.GetDirectReference(Configuration.ReplacementDocument);
                if (replacementDocument != null)
                {
                    replacementDocument.SetProperty(Configuration.UploaderEmailAddress, MFDataType.MFDatatypeText, env.ObjVerEx.GetPropertyText(Configuration.UploaderEmailAddress));
                    
                    replacementDocument.SetProperty(Configuration.ValidatedBy, MFDataType.MFDatatypeLookup, env.CurrentUserID);
                    replacementDocument.SetWorkflowState(Configuration.DocumentExpiryNotificationWorkflow);//, Configuration.InitialDocumentExpiryNotificationState);
                    replacementDocument.SaveProperties();
                }

                var expiredDocument = env.ObjVerEx.GetDirectReference(Configuration.ExpiredDocument);
                if(expiredDocument != null)
                {
                    expiredDocument.SetWorkflowState(null,Configuration.ExpiredDocumentReplacedWithValidState);
                    expiredDocument.SaveProperties();
                    //replacementDocument.SaveProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefState, MFDataType.MFDatatypeLookup, Configuration.ExpiredDocumentReplacedWithValidState);
                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"SetValidatedBy.", $"Error in State Action SetValidated By.{ObjectDetails(env.ObjVerEx)}", ex);
            }
        }



        [StateAction("WFS.DocusignWorkflow.Signed")]
        public void SetInductionDate(StateEnvironment env)
        {
            try
            {
                env.ObjVerEx.SetProperty(Configuration.DateOfIssue, MFDataType.MFDatatypeDate, DateTime.Now);

                env.ObjVerEx.SetProperty(Configuration.DateOfExpiry, MFDataType.MFDatatypeDate, DateTime.Now.AddMonths(Configuration.InductionExpiryInMonth));
                env.ObjVerEx.SetWorkflowState(Configuration.DocumentExpiryNotificationWorkflow, Configuration.DocumentExpiryNotificationVerifiedDocumentCheckState);

                env.ObjVerEx.SaveProperties();
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"SetInductionDate.", $"Error in State Action SetInductionDate By. {ObjectDetails(env.ObjVerEx)}", ex);
            }
        }



    }

}
