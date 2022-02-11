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

        private enum DocumentStatus
        {
            FoundValid = 1,
            FoundInvalid = 2,
            Missing = 3
        }
        private ObjVerEx FindContractorByEmail(Vault vault, string email)
        {
            try
            {
                var searchBuilder = new MFSearchBuilder(vault);
                searchBuilder.ObjType(Configuration.EmployeeContractorObject);
                searchBuilder.Class(Configuration.EmployeeContractorClass);
                searchBuilder.Property(Configuration.EmailAddress, MFDataType.MFDatatypeText, email);
                return searchBuilder.FindOneEx();
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        private DocumentStatus FoundDocument(ObjVerEx contractor, Vault vault, MFIdentifier certDoc)
        {
            try
            {
                // Create our search builder.

                var searchBuilder = new MFSearchBuilder(vault);



                // Add an object type filter.

                searchBuilder.ObjType((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument);
                searchBuilder.Class(certDoc);
                //  searchBuilder.Property(Configuration.IsDocumentValid, MFDataType.MFDatatypeBoolean, isValid);

                searchBuilder.Property(Configuration.ContractorsForCompany, MFDataType.MFDatatypeLookup, contractor.ID);


                // Add a "not deleted" filter.

                searchBuilder.Deleted(false);



                // Execute the search.

                var searchResults = searchBuilder.FindEx();
                if (searchResults == null)
                {

                    return DocumentStatus.Missing;
                }
                foreach (var docObjVerEx in searchResults)
                {
                    if (docObjVerEx.GetProperty(Configuration.IsDocumentValid).Value.DisplayValue
                        .ToLower()
                        .Equals("yes"))
                    {
                        return DocumentStatus.FoundValid;
                    }
                    else
                    {
                        return DocumentStatus.FoundInvalid;
                    }
                }




            }
            catch (Exception ex)
            {

            }
            return DocumentStatus.Missing;
        }

        private void CopyObjectFiles(Vault vault, ObjectFiles ObjFilesToCopy, ObjVer ObjVerToCopyTo)
        {
            try
            {
                foreach (ObjectFile ObjFile in ObjFilesToCopy)
                {
                    var TargetFileVer = vault.ObjectFileOperations.AddEmptyFile(ObjVerToCopyTo, ObjFile.Title, ObjFile.Extension);
                    int upload_id = vault.ObjectFileOperations.UploadFileBlockBegin();

                    FileDownloadSession DownloadSession = null;
                    DownloadSession = vault.ObjectFileOperations.DownloadFileInBlocks_Begin(ObjFile.ID, ObjFile.Version);

                    var download_id = DownloadSession.DownloadID;
                    long filesize = DownloadSession.FileSize;

                    int blockSize = 15 * 64 * 1024; // Magic number from samppa example
                    byte[] buffer;
                    long totalCopied = 0;
                    long offset = 0;

                    while (filesize > totalCopied && download_id != 0)
                    {
                        // Get block of data
                        buffer = vault.ObjectFileOperations.DownloadFileInBlocks_ReadBlock(download_id, blockSize, offset);

                        // Upload block of data
                        vault.ObjectFileOperations.UploadFileBlock(upload_id, buffer.LongLength, offset, buffer);

                        // Calculate a new offset and total downloaded size.
                        offset = offset + buffer.LongLength;
                        totalCopied = totalCopied + buffer.LongLength;
                    }
                    vault.ObjectFileOperations.UploadFileCommit(upload_id, TargetFileVer.ID, TargetFileVer.Version, filesize);
                    vault.ObjectFileOperations.CloseUploadSession(upload_id);
                }
            }
            catch (Exception e)
            {
                //Not loging usual error to Windows Event per M-File cloud requirement
                //Log SysUtils.ReportErrorToEventLog(e);
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
