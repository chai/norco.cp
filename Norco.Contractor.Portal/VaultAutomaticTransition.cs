using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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


        //[AutomaticStateTransitionTrigger("WFT.InitialToProvided")]
        //[AutomaticStateTransitionTrigger("WFT.30ToProvided")]
        //    [AutomaticStateTransitionTrigger("WFT.7ToProvided")]
        //public bool InitialTo(StateTransitionEnvironment env, out int nextState)
        //{
        //    nextState = Configuration.StateRequestedDocumentProvided;


        //    try
        //    {


        //        /*
                 
        //         Todo:

        //        Add default workflow and state
        //        Change Document Request workflow
        //        Change default Document Request property to Document Completed bool
        //        Filter Hubshare on this Property

        //         */

        //        var searchBuilder = new MFSearchBuilder(env.Vault);
        //        searchBuilder.ObjType((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument);
        //        searchBuilder.Class(Configuration.OtherDocumentClass);
        //        var condition = new SearchCondition();
        //        condition.ConditionType = MFConditionType.MFConditionTypeEqual;
        //        condition.Expression.SetPropertyValueExpression(Configuration.OwnerDocumentRequest, MFParentChildBehavior.MFParentChildBehaviorNone);
        //        condition.TypedValue.SetValue(MFDataType.MFDatatypeLookup, env.ObjVer.ID);
        //        searchBuilder.References( Configuration.OwnerDocumentRequest, env.ObjVer);
        //        searchBuilder.Deleted(false);
        //        searchBuilder.Conditions.Add(-1, condition);

        //        // Execute the search.
        //        var searchResult = searchBuilder.FindEx();
        //        if (searchResult != null)
        //        {
        //            var expiredDoc = env.ObjVerEx.GetDirectReference(Configuration.ExpiredDocument);
        //            var mfPropertyValuesBuilder = new MFPropertyValuesBuilder(env.Vault);

        //            // Set the class property.


        //            //Get Common properties
        //            List<PropertyValue> commonProperties = expiredDoc.Properties.OfType<PropertyValue>().Where(a => a.PropertyDef > 100).ToList();

        //            ObjectClass LetterClass = env.Vault.ClassOperations.GetObjectClass(expiredDoc.Class);
        //            commonProperties.ForEach(a =>
        //            {
        //                if (!LetterClass.AssociatedPropertyDefs.OfType<AssociatedPropertyDef>().Where(b => b.PropertyDef == a.PropertyDef).Any())
        //                {
        //                    commonProperties.Remove(a);
        //                }
        //            });


        //            // Add a property value by alias.
        //            commonProperties.ForEach(a => mfPropertyValuesBuilder.Add(a));
        //            mfPropertyValuesBuilder.SetClass(expiredDoc.Class);
        //            mfPropertyValuesBuilder.SetWorkflowState(Configuration.WorkflowDocumentRequest, Configuration.StateInitialDocumentRequest);


        //            ObjVerEx newLetter = new ObjVerEx(env.Vault, env.Vault.ObjectOperations.CreateNewObject(expiredDoc.Type, mfPropertyValuesBuilder.Values));


        //            int fileCounter = 0;
        //            foreach (var doc in searchResult)
        //            {


        //                ObjectFiles objFiles = env.Vault.ObjectFileOperations.GetFiles(doc.ObjVer);


        //                foreach (ObjectFile objFile in objFiles)
        //                {
                          
        //                    CopyObjectFiles(env.Vault, objFiles, newLetter.ObjVer);
        //                    fileCounter++;
        //                    //ObjectFiles objFiles = env.Vault.ObjectFileOperations.GetFiles(searchResult.ObjVer);
        //                    //ObjectFile objFile = objFiles[1];


        //                }

                  

              
        //            }
        //            if (fileCounter ==1)
        //            {
        //                env.Vault.ObjectOperations.SetSingleFileObject(newLetter.ObjVer, true);
        //            }
        //            env.Vault.ObjectFileOperations.UpdateMetadataInFile(newLetter.ObjVer, -1, false);
        //            newLetter.CheckIn();

        //        }

        //            //}
        //            //else
        //            //{
        //            //    throw new Exception("Could not find Template");
        //            //}

        //        }
        //    catch (Exception ex1)
        //    {

        //    }           
        //    return true;

        //}

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

    }
}