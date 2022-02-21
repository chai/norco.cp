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


        //Note: Dependecy on IsValid?
        [PropertyCustomValue("PD.DocumentationStatus", Priority =100)]
        public TypedValue DocumentationStatus(PropertyEnvironment env)
        {
            StringBuilder sb = new StringBuilder();
            
            TypedValue typedValue = new TypedValue();
            typedValue.SetValue(MFDataType.MFDatatypeLookup, 1);
            try
            {
                var contractorType = env.ObjVerEx.GetLookupsFromProperty(Configuration.ContractorType);
                foreach (var contype in contractorType)
                {
                    var conTypeConfig = Configuration.ContractorTypeCertificationsDocumentList.FindAll(x => x.ContractorType.Guid == contype.ItemGUID);
                    foreach (var certDocList in conTypeConfig)
                    {
                        foreach (var certDoc in certDocList.CertificationDocument)
                        {

                            var documentState = FoundDocument(env.ObjVerEx, env.Vault, certDoc);
                            if (documentState == DocumentStatus.Missing)
                            {

                                sb.AppendLine($"{env.Vault.ClassOperations.GetObjectClass(certDoc).Name}");
                                
                                typedValue.SetValue(MFDataType.MFDatatypeLookup, (int)documentState);// expired
                               

                            }
                           
                            //valid  each document
                        }
                    }
                    if(sb.Length>0)
                    {
                        env.ObjVerEx.SaveProperty(Configuration.MissingDocuments, MFDataType.MFDatatypeMultiLineText, sb.ToString());
                    }
                   // SysUtils.ReportToEventLog(missingDocumnt, System.Diagnostics.EventLogEntryType.Information);
                    return typedValue;
                }

                //   return ValidateDate(env.ObjVerEx.GetProperty(Configuration.DateOfIssue), env.PropertyValue, ref message);
            }
            catch(Exception ex)
            {
                SysUtils.ReportToEventLog($"DocumentationStatus validation failed for. {Environment.NewLine} {ObjectDetails(env.ObjVerEx)}", ex);
            }
            return typedValue;
        }

        [PropertyCustomValue("PD.DocumentHubName")]
        public TypedValue setHubnameBasedOnCompany(PropertyEnvironment env)
        {
            TypedValue typedValue = new TypedValue();
            typedValue.SetValue(MFDataType.MFDatatypeText, null);

            try
            {
                var companyName = env.ObjVerEx.GetPropertyText(Configuration.CompanyOfContractor);
                if (!companyName.Equals(String.Empty))
                {
                 
                        typedValue.SetValue(MFDataType.MFDatatypeText, companyName);
                    
                }


            }
            catch (Exception ex) { }
            return typedValue;
        }


        [PropertyCustomValue("PD.Blacklisted")]
        public TypedValue setBlacklisted(PropertyEnvironment env)
        {
            TypedValue typedValue = new TypedValue();
            typedValue.SetValue(MFDataType.MFDatatypeBoolean, false);

            try
            {


                var blackListUntil = env.ObjVerEx.GetProperty(Configuration.BlacklistedUntil).GetValueAsTextEx(true, true, true, true, true, true, true);
                if (!blackListUntil.Equals(String.Empty))
                {

                   var dueDate = Convert.ToDateTime(blackListUntil);
                    var t = dueDate <= DateTime.Now;
                        typedValue.SetValue(MFDataType.MFDatatypeBoolean, dueDate > DateTime.Now);
                    
                }


            }
            catch (Exception ex) { }
            return typedValue;
        }




        [PropertyCustomValue("PD.DocumentUploadedBy")]
        public TypedValue setupUploaderBasedOnEmail(PropertyEnvironment env)
        {
            TypedValue typedValue = new TypedValue();
            typedValue.SetValue(MFDataType.MFDatatypeLookup,null);

            try {
                var uploaderEmailAddress = env.ObjVerEx.GetPropertyText(Configuration.UploaderEmailAddress);
                if (!uploaderEmailAddress.Equals(String.Empty))
                {
                   var uploader= FindContractorByEmail(env.Vault, uploaderEmailAddress);
                    if (uploader != null)
                    {
                        typedValue.SetValue(MFDataType.MFDatatypeLookup, uploader.ID);
                    }
                }


            }
            catch (Exception ex) { }
            return typedValue;
        }


        [PropertyCustomValue("PD.Valid", Priority = 500)]
        public TypedValue isDocumentValid(PropertyEnvironment env)
        {
            TypedValue typedValue = new TypedValue();
            typedValue.SetValue(MFDataType.MFDatatypeBoolean, false);
            try
            {
                string dateOfExpiry = env.ObjVerEx.GetProperty(Configuration.DateOfExpiry).GetValueAsTextEx(true, true, true, true, true, true, true);
                if (string.IsNullOrEmpty(dateOfExpiry))
                {
                    typedValue.SetValue(MFDataType.MFDatatypeBoolean, true);
                    return typedValue;

                    //only validate if there is a value set
                }

                
                var endDate = Convert.ToDateTime(dateOfExpiry).Date;
                if(endDate >= DateTime.Now.Date)
                {
                    typedValue.SetValue(MFDataType.MFDatatypeBoolean, true);
                    return typedValue;
                }


            }
            catch (Exception ex)
            {

            }
            return typedValue;
        }



        //[PropertyCustomValue("PD.TestAutoproperty")]
        //public TypedValue Test(PropertyEnvironment env)
        //{
        //    TypedValue typedValue = new TypedValue();
        //    typedValue.SetValue(MFDataType.MFDatatypeText, false);
        //    try
        //    {
        //        typedValue.SetValue(MFDataType.MFDatatypeText, $"{DateTime.Now.ToLongDateString()} -  {DateTime.Now.ToLongTimeString()}");
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return typedValue;
        //}
 
        [PropertyCustomValue("PD.EmployeeContractorEmail")]
        public TypedValue EmployeeContractorEmail(PropertyEnvironment env)
        {
            TypedValue typedValue = new TypedValue();
            typedValue.SetValue(MFDataType.MFDatatypeText, false);
            try
            {
                var contractor = env.ObjVerEx.GetDirectReference(Configuration.ContractorsForCompany);
                if (contractor != null)
                {
                    typedValue.SetValue(MFDataType.MFDatatypeText, contractor.GetPropertyText(Configuration.EmailAddress));
                }


            }
            catch (Exception ex)
            { }

            return typedValue;
        }
        
    }




}
