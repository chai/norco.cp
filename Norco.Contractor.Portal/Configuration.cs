using MFiles.VAF.Configuration;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        // NOTE: The default value needs to be placed in both the JsonConfEditor
        // (or derived) attribute, and as a default value on the member.
        #region Object ID
        
        [MFObjType(Required = true)]
        [DataMember]
        public MFIdentifier DocumentRequestObject { get; set; }
            = "OT.DocumentRequest";


        #endregion
        #region Class ID
        [MFClass(Required = true)]
        [DataMember]
        public MFIdentifier DocumentRequestClass { get; set; }
            = "CL.DocumentRequest";

        #endregion


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
        public MFIdentifier CompanyContractors { get; set; }
        = "PD.CompanyContractors";

        [MFPropertyDef(Required = true)]
        public MFIdentifier IsDocumentValid { get; set; }
= "PD.Valid";


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

    }
}