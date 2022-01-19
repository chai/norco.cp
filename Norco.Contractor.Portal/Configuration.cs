using MFiles.VAF.Configuration;
using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MFiles.VAF.Extensions;
using MFiles.VAF.Extensions.ScheduledExecution;
using DailyTrigger = MFiles.VAF.Extensions.ScheduledExecution.DailyTrigger;

namespace Norco.Contractor.Portal
{
    [DataContract]
    public class DocumentEmail
    {
        [DataMember]
        [MFClass(Required = true)]
        public MFIdentifier DocumentType { get; set; }
        [DataMember]
        public List<EmailBodyProperty> EmailProperties { get; set; }



    }

    [DataContract]
    public class EmailBodyProperty
    {



        [DataMember]

        public string PropertyName { get; set; }

        [DataMember]
        [MFPropertyDef(Required = true)]
        public MFIdentifier CertificateProperty { get; set; }

    }


    [DataContract]
    public class RenewalDocument
    {
        [DataMember]
        [MFClass(Required = true)]
        public MFIdentifier DocumentType { get; set; }

        [DataMember]
        public List<Property> PropertyToRemove { get; set; }
        [DataMember]
        public List<Property> PropertyToCopyOver { get; set; }


    }

    [DataContract]
    public class Property
    {




        [DataMember]
        [MFPropertyDef(Required = true)]
        public MFIdentifier Prop { get; set; }

    }



    [DataContract]
    public class ContractorTypeCertification
    {



        /// <summary> Refers to an item in a value list in the vault. </summary>
        /// <remarks> Use its GUID instead. </remarks>
        [DataMember]
        [MFValueListItem(Required = true, ValueList = "VL.ContractorType")]
        public MFIdentifier ContractorType { get; set; }

        [DataMember]
        [MFClass(Required = true)]
        public List<MFIdentifier> CertificationDocument { get; set; }


    }


    [DataContract]
    public class Configuration
    {
        public const int OwnerDocumentRequest= 1189;

        // NOTE: The default value needs to be placed in both the JsonConfEditor
        // (or derived) attribute, and as a default value on the member.
        #region Object ID

        [MFObjType(Required = true)]
        public MFIdentifier DocumentRequestObject { get; set; }
            = "OT.DocumentRequest";

        [MFObjType(Required = true)]
        public MFIdentifier EmployeeContractorObject { get; set; }
            = "OT.Contractor";



        #endregion
        #region Class ID
        [MFClass(Required = true)]
        public MFIdentifier DocumentRequestClass { get; set; }
            = "CL.DocumentRequest";

        [MFClass(Required = true)]
        public MFIdentifier OtherDocumentClass { get; set; }
    = "Cl.OtherDocument";


        [MFClass(Required = true)]
        public MFIdentifier EmployeeContractorClass { get; set; }
            = "CL.Employee_Contractor";


        #endregion

        #region Workflow
        [MFWorkflow]
        public MFIdentifier WorkflowTemporaryDocumentUploaded { get; set; }
        = "WF.TemporaryDocumentUploaded";


        [MFWorkflow]
        public MFIdentifier WorkflowDocumentExpiry { get; set; }
       = "WF.ContractorDocStatus";
        #endregion

        #region Workflow State

        [MFState(Required = true)]
        public MFIdentifier StateDocumentUploaded { get; set; }
    = "WFS.TemporaryDocumentUploaded.DocumentUploaded";

        [MFState(Required = true)]
        public MFIdentifier StateInitialExpiryCheck { get; set; }
= "WFS.DocumentExpiryNotification.InitialExpiryCheck";

        [MFState(Required = true)]
        public MFIdentifier StateRequestedDocumentProvided { get; set; }
= "WFS.DocumentRequest.RequestedDocumentProvided";

        #endregion

        [MFPropertyDef(Required = true)]
        public MFIdentifier DocumentRequest { get; set; }
            = "PD.DocumentRequest";

        [MFPropertyDef(Required = true)]
        public MFIdentifier DateOfIssue { get; set; }
            = "PD.DateOfIssue";

        [MFPropertyDef(Required = true)]
        public MFIdentifier DateOfExpiry { get; set; }
        = "PD.ExpiryDate";


        [MFPropertyDef(Required = true)]
        public MFIdentifier ActivityStartDate { get; set; }
        = "PD.ActivityStartDate";


        [MFPropertyDef(Required = true)]
        public MFIdentifier ContractorType { get; set; }
        = "PD.ContractorType";


        [MFPropertyDef(Required = true)]
        public MFIdentifier ContractorsForCompany { get; set; }
        = "PD.CompanyContractors";

                [MFPropertyDef(Required = true)]
        public MFIdentifier CompanyOfContractor { get; set; }
        = "PD.ContractorCompany";
        

        [MFPropertyDef(Required = true)]
        public MFIdentifier IsDocumentValid { get; set; }
= "PD.Valid";

        [MFPropertyDef(Required = true)]
        public MFIdentifier EmailAddress { get; set; }
= "PD.EmailAddress";

        [MFPropertyDef(Required = true)]
        public MFIdentifier EmailAddress2 { get; set; }
= "PD.EmailAddress2";

        [MFPropertyDef(Required = true)]
        public MFIdentifier ExpiredDocument { get; set; }
= "PD.ExpiredDocument";
        [MFPropertyDef(Required = true)]
        public MFIdentifier UploaderEmailAddress { get; set; }
= "PD.UploaderEmail";


        public const int SingleFile = 22;

        [DataMember]
        public List<ContractorTypeCertification> contractorTypeCertifications { get; set; }


        [DataMember]
        public List<DocumentEmail> CertificateEmailProperties { get; set; }

        [DataMember]
        public MFilesAPI.Extensions.Email.SmtpConfiguration SmtpConfiguration
        {
            get;
            set;
        } = new MFilesAPI.Extensions.Email.SmtpConfiguration();


        [DataMember]
        public string PlaceHolderFile { get; set; }
        = $@"C:\Temp\Placeholder.txt";
        [DataMember]
        public string NorcoNotificationPerson { get; set; }
= $@"contractoradmin@norco.com.au";


        [DataMember]
        public List<RenewalDocument> RenewalDocuments { get; set; }


        [MFWorkflow]
        public MFIdentifier DocumentRequestWorkflow { get; set; }
        = "WF.DocumentRequest";

        [MFWorkflow]
        public MFIdentifier DocumentExpiryNotificationWorkflow { get; set; }
= "WF.ContractorDocStatus";



        [MFState]
        public MFIdentifier InitialDocumentExpiryNotificationState { get; set; }
= "WFS.DocumentExpiryNotification.InitialExpiryCheck";



        [MFState]
        public MFIdentifier InitialDocumentRequestState { get; set; }
= "WFS.DocumentRequest.InitialDocumentRequest";

        [MFState]
        public MFIdentifier ExpiredDocumentReplacedWithValidState { get; set; }
= "WFS.DocumentExpiryNotification.Valid";

        

        [MFPropertyDef(Required = true)]
        public MFIdentifier DocumentUploaded { get; set; }
= "PD.AllowUpload";

        [MFPropertyDef(Required = true)]
        public MFIdentifier ValidatedBy { get; set; }
= "PD.ValidatedBy";



        [MFPropertyDef(Required = true)]
        public MFIdentifier ReplacementDocument { get; set; }
= "PD.ReplacementDocument";

        [MFPropertyDef(Required = true)]
        public MFIdentifier EmployeeContractorEmail { get; set; }
= "PD.EmployeeContractorEmail";







        [DataMember]
        [RecurringOperationConfiguration(VaultApplication.QueueId, VaultApplication.UpdateDocumentIsValid)]
        public Schedule ImportDataSchedule { get; set; } = new Schedule()
        {
            Enabled = true,
            Triggers = new List<Trigger>()
            {
                new DailyTrigger()
                {
                    TriggerTimes = new List<TimeSpan>()
                    {
                        new TimeSpan(4, 0, 0) // 4am
					}
                }
            }
        };

    }
}