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

    public partial class VaultApplication
    {
        [EventHandler(MFEventHandlerType.MFEventHandlerAfterCreateNewObjectFinalize, Class = "CL.MemberContract")]        
        public void CheckForName(EventHandlerEnvironment env)
        {
            try
            {
                var currentWorkflow = env.ObjVerEx.Workflow;
                if (currentWorkflow != -1 && currentWorkflow == Configuration.WorkflowDocusignTemplate.ID)
                {


                    bool updated = false;
                    var propertyValues = env.ObjVerEx.Properties;


                    var uploaderEmailAddress = env.ObjVerEx.GetPropertyText(Configuration.UploaderEmailAddress);
                    if (!uploaderEmailAddress.Equals(String.Empty))
                    {
                        var uploader = FindContractorByEmail(env.Vault, uploaderEmailAddress);
                        if (uploader != null)
                        {
                            propertyValues.SetProperty(Configuration.SignerGroup, MFDataType.MFDatatypeMultiSelectLookup, uploader.ID);
                            propertyValues.SetProperty(Configuration.ContractorsForCompany, MFDataType.MFDatatypeLookup, uploader.ID);

                            updated = true;
                        }
                    }


                    var currentCompany = env.ObjVerEx.GetPropertyText(Configuration.CompanyOfContractor);
                    if (currentCompany != String.Empty)
                    {
                        var companyName = env.ObjVerEx.GetPropertyText(Configuration.InductionHubName);
                        if (!companyName.Equals(String.Empty))
                        {
                            var company = FindCompanyBasedOnName(env.Vault, companyName);
                            if (company != null)
                            {
                                propertyValues.SetProperty(Configuration.CompanyOfContractor, MFDataType.MFDatatypeLookup, company.ID);
                                updated = true;
                            }


                        }
                    }

                    //SysUtils.ReportInfoToEventLog($"Event for HubShare", $"{env.ObjVerEx.Title} {env.EventType} {env.CurrentUserID}");
                    if (updated)
                    {
                        env.ObjVerEx.SaveProperties(propertyValues);
                    }
                    //SysUtils.ReportInfoToEventLog($"Event for HubShare SAVED", $"{env.ObjVerEx.Title} {env.EventType} {env.CurrentUserID}");
                }
                else
                {
                    //no need to run anything as this is a manual creation
                    return;
                }
            }
            catch (Exception e)
            {
                SysUtils.ReportErrorToEventLog("Hubsher", e);
            }



        }



    }
}
