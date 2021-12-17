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
                //env.ObjVerEx.Prop
                ////       SendEmail(5, false, env);

                //// Where "env" is the current environment passed
                //// to the method.
                ////env.ObjVerEx.SetCreatedBy(env.CurrentUserID);
                ////env.ObjVerEx.SetModifiedBy(env.CurrentUserID);
                //var propertyValues = new PropertyValues();
                //propertyValues.Add(-1, new PropertyValue
                //{
                //    PropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass
                //}.Value.SetValue(MFDataType.MFDatatypeLookup, Configuration.DocumentRequestClass.ID));

                //retPropValTxt(ref propertyValues, Configuration.APVoucherTaxRateArea, MFDataType.MFDatatypeLookup, item, TAXRATE);
                //retPropValTxt(ref propertyValues, Configuration.APVoucherExplanationCode, MFDataType.MFDatatypeLookup, item, TAXEXPLCODE);
                //retPropValTxt(ref propertyValues, Configuration.APVoucherOverride, MFDataType.MFDatatypeBoolean, item, OVERRIDE);

                var propertyValues = env.ObjVerEx.Properties;

                propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefWorkflow,MFDataType.MFDatatypeLookup,Configuration.DocumentRequestWorkflow);
                propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefState, MFDataType.MFDatatypeLookup, Configuration.InitialDocumentRequest);
                propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass, MFDataType.MFDatatypeLookup, Configuration.DocumentRequestClass);

                propertyValues.SetProperty(Configuration.ExpiredDocument, MFDataType.MFDatatypeLookup, env.ObjVer.ID);


                propertyValues.RemoveProperty(Configuration.DateOfExpiry);
                propertyValues.RemoveProperty(Configuration.DateOfIssue);
                propertyValues.RemoveProperty(Configuration.IsDocumentValid);
                propertyValues.RemoveProperty(Configuration.SingleFile);
                propertyValues.RemoveProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefObjectID);
                propertyValues.RemoveProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefNameOrTitle);
                //env.Vault.ObjectOperations.CreateNewObjectEx(
                //    env.ObjVer.Type,
                //    propertyValues,
                //    CheckIn: true);
                //SourceObjectFiles sourceObjectFiles = new SourceObjectFiles();

                //sourceObjectFiles.Add(-1, new SourceObjectFile()

                //{

                //    Extension = "txt",

                //    SourceFilePath = Configuration.PlaceHolderFile,

                //    Title = $"Document Request for {env.ObjVerEx.Title}"

                //});

                
              var docRequest=  env.Vault.ObjectOperations.CreateNewObjectExQuick(                 
                    Configuration.DocumentRequestObject, propertyValues, null, false, true, null);


                env.ObjVerEx.SaveProperty(Configuration.DocumentRequest, MFDataType.MFDatatypeLookup, docRequest);


                try
                {
                    SendEmail(30, false, env);
                }
                catch (Exception ex)
                {
                    SysUtils.ReportErrorToEventLog("DocumentExpiredIn30Days", "Error in State Action for 30 days to expiry.", ex);

                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("DocumentExpiredIn30Days", "Error in State Action for 30 days to expiry.", ex);

            }
        }


        [StateAction("WFS.DocumentExpiryNotification.30Days")]
        public void DocumentExpiredIn30DaysEmailNotification(StateEnvironment env)
        {

        }

        [StateAction("WFS.DocumentExpiryNotification.7Days")]
        public void DocumentExpiredIn7DaysEmailNotification(StateEnvironment env)
        {
            try
            {
                SendEmail(7, false, env);
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("DocumentExpiredIn7Days", "Error in State Action for 30 days to expiry.", ex);

            }
        }
        [StateAction("WFS.DocumentExpiryNotification.Expired")]
        public void DocumentExpiredEmailNotification(StateEnvironment env)
        {
            try
            {
                SendEmail(0, true, env);
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("DocumentExpiredIn7Days", "Error in State Action for 30 days to expiry.", ex);

            }
        }

        
                    [StateAction("WFS.DocumentRequest.RequestedDocuentValidated")]
        public void CreateReplacementDocument(StateEnvironment env)
        {
            try
            {


                /*
                 
                 Todo:

                Add default workflow and state
                Change Document Request workflow
                Change default Document Request property to Document Completed bool
                Filter Hubshare on this Property

                 */

                var searchBuilder = new MFSearchBuilder(env.Vault);
                searchBuilder.ObjType((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument);
                searchBuilder.Class(Configuration.OtherDocumentClass);
                var condition = new SearchCondition();
                condition.ConditionType = MFConditionType.MFConditionTypeEqual;
                condition.Expression.SetPropertyValueExpression(Configuration.OwnerDocumentRequest, MFParentChildBehavior.MFParentChildBehaviorNone);
                condition.TypedValue.SetValue(MFDataType.MFDatatypeLookup, env.ObjVer.ID);
                searchBuilder.References(Configuration.OwnerDocumentRequest, env.ObjVer);
                searchBuilder.Deleted(false);
                searchBuilder.Conditions.Add(-1, condition);

                // Execute the search.
                var searchResult = searchBuilder.FindEx();
                if (searchResult != null)
                {
                    var expiredDoc = env.ObjVerEx.GetDirectReference(Configuration.ExpiredDocument);
                    var mfPropertyValuesBuilder = new MFPropertyValuesBuilder(env.Vault);

                    // Set the class property.


                    //Get Common properties
                    List<PropertyValue> commonProperties = expiredDoc.Properties.OfType<PropertyValue>().Where(a => a.PropertyDef > 100).ToList();

                    ObjectClass LetterClass = env.Vault.ClassOperations.GetObjectClass(expiredDoc.Class);
                    commonProperties.ForEach(a =>
                    {
                        if (!LetterClass.AssociatedPropertyDefs.OfType<AssociatedPropertyDef>().Where(b => b.PropertyDef == a.PropertyDef).Any())
                        {
                            commonProperties.Remove(a);
                        }
                    });


                    // Add a property value by alias.
                    commonProperties.ForEach(a => mfPropertyValuesBuilder.Add(a));
                    mfPropertyValuesBuilder.SetClass(expiredDoc.Class);
                    mfPropertyValuesBuilder.SetWorkflowState(Configuration.WorkflowDocumentExpiry, Configuration.StateInitialExpiryCheck);


                    ObjVerEx newLetter = new ObjVerEx(env.Vault, env.Vault.ObjectOperations.CreateNewObject(expiredDoc.Type, mfPropertyValuesBuilder.Values));


                    int fileCounter = 0;
                    foreach (var doc in searchResult)
                    {


                        ObjectFiles objFiles = env.Vault.ObjectFileOperations.GetFiles(doc.ObjVer);


                        foreach (ObjectFile objFile in objFiles)
                        {

                            CopyObjectFiles(env.Vault, objFiles, newLetter.ObjVer);
                            fileCounter++;
                            //ObjectFiles objFiles = env.Vault.ObjectFileOperations.GetFiles(searchResult.ObjVer);
                            //ObjectFile objFile = objFiles[1];


                        }




                    }
                    if (fileCounter == 1)
                    {
                        env.Vault.ObjectOperations.SetSingleFileObject(newLetter.ObjVer, true);
                    }
                    env.Vault.ObjectFileOperations.UpdateMetadataInFile(newLetter.ObjVer, -1, false);
                    newLetter.CheckIn();

                }

                //}
                //else
                //{
                //    throw new Exception("Could not find Template");
                //}

            }
            catch (Exception ex1)
            {

            }


        }
        //private PropertyValue SetPropertyID(MFIdentifier id, MFDataType type)
        //{

        //    var value = new MFilesAPI.PropertyValue()
        //    {
        //        PropertyDef = id
        //    };

        //    value.Value.SetValue()
        //}
    }

}
