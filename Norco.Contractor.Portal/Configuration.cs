using MFiles.VAF.Configuration;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MFiles.VAF.Configuration.JsonAdaptor;
using MFilesAPI;
using MFiles.VAF.Extensions;
using System;

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
    public class ObjectsToUpdate
    {
        [DataMember]
        [JsonConfEditor(DefaultValue = 12)]
        public int AutoCalculatePolling = 12;

        [DataMember]
        [JsonConfEditor(DefaultValue = 2)]
        public int AutoCalculateAtTime = 2;


        [DataMember]
        [JsonConfEditor(DefaultValue = false)]
        public bool StartAutoCalculatePolling = false;


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

        [MFObjType(Required = true)]
        public MFIdentifier ContractorCompanyObject { get; set; }
    = "OT.Company";


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
        [MFClass(Required = true)]
        public MFIdentifier ContractorCompanyClass { get; set; }
    = "CL.Company";

        #endregion

        #region Workflow
        [MFWorkflow]
        public MFIdentifier WorkflowTemporaryDocumentUploaded { get; set; }
        = "WF.TemporaryDocumentUploaded";


        [MFWorkflow]
        public MFIdentifier WorkflowDocumentExpiry { get; set; }
       = "WF.ContractorDocStatus";


        [MFWorkflow]
        public MFIdentifier WorkflowDocusignTemplate { get; set; }
= "WF.DocusignTemplate";

        
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
        public MFIdentifier Blacklisted { get; set; }
= "PD.Blacklisted";

        [MFPropertyDef(Required = true)]
        public MFIdentifier BlacklistedUntil { get; set; }
= "PD.BlacklistedUntil";
        [MFPropertyDef(Required = true)]
        public MFIdentifier SignerGroup { get; set; }
= "PD.SignerGroup";

        [MFPropertyDef(Required = true)]
        public MFIdentifier InductionHubName { get; set; }
= "PD.InductionHubName";


                [MFPropertyDef(Required = true)]
        public MFIdentifier ReplacementDocument { get; set; }
= "PD.ReplacementDocument";

        [MFPropertyDef(Required = true)]
        public MFIdentifier EmployeeContractorEmail { get; set; }
= "PD.EmployeeContractorEmail";

        [MFPropertyDef(Required = true)]
        public MFIdentifier CompanyTitle { get; set; }
= "PD.CompanyTitle";



        [DataMember]
        public ObjectsToUpdate TaskConfig { get; set; }
        = new ObjectsToUpdate();


        [DataMember]
        [Security(ChangeBy = SecurityAttribute.UserLevel.SystemAdmin)]
        [JsonConfEditor(DefaultValue = "{6A203C49-C4F8-4F25-AE2E-8C557FDBD3CF}")]
        public string VaultGUID = @"{6A203C49-C4F8-4F25-AE2E-8C557FDBD3CF}";

        [DataMember]
        [Security(ChangeBy = SecurityAttribute.UserLevel.SystemAdmin)]
        [JsonConfEditor(DefaultValue = "")]
        public string Domain = @"";

        [DataMember]
        [Security(IsPassword = true, ChangeBy = SecurityAttribute.UserLevel.SystemAdmin)]
        [JsonConfEditor(DefaultValue = @"")]
        public string MFilesPassword { get; set; } = @"";

        [DataMember]
        [Security(ChangeBy = SecurityAttribute.UserLevel.SystemAdmin)]
        [JsonConfEditor(DefaultValue = @"")]
        public string MFilesUsername = @"";

        [DataMember]
        [Security(ChangeBy = SecurityAttribute.UserLevel.SystemAdmin)]
        [JsonConfEditor(DefaultValue = MFAuthType.MFAuthTypeSpecificMFilesUser)]
        public MFAuthType AuthType { get; set; }

        [DataMember]
        public SearchConditionsJA IsValudFilter { get; set; }


        [DataMember]
        public SearchConditionsJA IsBlacklistFilter { get; set; }


        [DataMember]
        [JsonConfEditor(DefaultValue = 12)]
        public int InductionExpiryInMonth = 12;


        [MFPropertyDef(Required = true)]
        public MFIdentifier MissingDocuments { get; set; }
= "PD.MissingDocuments";




        // The import will run every hour  but can be configured via the M-Files Admin software.

        [DataMember]

        [RecurringOperationConfiguration

        (

            VaultApplication.QueueId,

            VaultApplication.ImportDataFromRemoteSystemTaskType,

            Label = "Import Frequency",

            DefaultValue = "Once per hour"

        )]

        public Frequency Frequency { get; set; } = TimeSpan.FromHours(1);
    }



}