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
        //[EventHandler(MFEventHandlerType.MFEventHandlerAfterFileUpload,Class = "Cl.OtherDocument")]
        [EventHandler(MFEventHandlerType.MFEventHandlerAfterCreateNewObjectFinalize, Class = "CL.MemberContract")]
        // [EventHandler(MFEventHandlerType.MFEventHandlerAfterCreateNewObjectFinalize, Class = "Cl.OtherDocument")]

        public void CheckForName(EventHandlerEnvironment env)
        {
            try
            {
                var propertyValues = env.ObjVerEx.Properties;


                var uploaderEmailAddress = env.ObjVerEx.GetPropertyText(Configuration.UploaderEmailAddress);
                if (!uploaderEmailAddress.Equals(String.Empty))
                {
                    var uploader = FindContractorByEmail(env.Vault, uploaderEmailAddress);
                    if (uploader != null)
                    {
                        propertyValues.SetProperty(Configuration.SignerGroup, MFDataType.MFDatatypeMultiSelectLookup, uploader.ID);
                        propertyValues.SetProperty(Configuration.ContractorsForCompany, MFDataType.MFDatatypeLookup, uploader.ID);

                    }
                }


                var companyName = env.ObjVerEx.GetPropertyText(Configuration.InductionHubName);
                if (!companyName.Equals(String.Empty))
                {
                    var company = FindCompanyBasedOnName(env.Vault, companyName);
                    if (company != null)
                    {
                        propertyValues.SetProperty(Configuration.CompanyOfContractor, MFDataType.MFDatatypeLookup, company.ID);
                    }


                }
                
                //SysUtils.ReportInfoToEventLog($"Event for HubShare", $"{env.ObjVerEx.Title} {env.EventType} {env.CurrentUserID}");
                env.ObjVerEx.SaveProperties(propertyValues);
                //SysUtils.ReportInfoToEventLog($"Event for HubShare SAVED", $"{env.ObjVerEx.Title} {env.EventType} {env.CurrentUserID}");

            }
            catch (Exception e)
            {
                SysUtils.ReportErrorToEventLog("Hubsher", e);
            }



        }


        private ObjVerEx FindCompanyBasedOnName(Vault vault, string companyName)
        {
            try
            {
                var searchBuilder = new MFSearchBuilder(vault);
                searchBuilder.ObjType(Configuration.ContractorCompanyObject);
                searchBuilder.Class(Configuration.ContractorCompanyClass);
                searchBuilder.Property(Configuration.CompanyTitle, MFDataType.MFDatatypeText, companyName);
                return searchBuilder.FindOneEx();
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
