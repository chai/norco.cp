using MFiles.VAF.Common;
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
        [StateAction("WFS.Template")]

        public void CreateTemplatedInvoice(StateEnvironment env)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            ////Create PropertyValues
            //// Create a property values builder.
            //var mfPropertyValuesBuilder = new MFPropertyValuesBuilder(env.Vault);

            //// Set the class property.
            //mfPropertyValuesBuilder.SetClass(Configuration.SignatureDetailsClass);

            ////Get Common properties
            //List<PropertyValue> commonProperties = env.ObjVerEx.Properties.OfType<PropertyValue>().Where(a => a.PropertyDef > 100).ToList();

            //ObjectClass LetterClass = env.Vault.ClassOperations.GetObjectClass(Configuration.SignatureDetailsClass);
            //commonProperties.ForEach(a =>
            //{
            //    if (!LetterClass.AssociatedPropertyDefs.OfType<AssociatedPropertyDef>().Where(b => b.PropertyDef == a.PropertyDef).Any())
            //    {
            //        commonProperties.Remove(a);
            //    }
            //});

            //// Add a property value by alias.
            //commonProperties.ForEach(a => mfPropertyValuesBuilder.Add(a));

            //// Add reference to this enquiry
            ////  mfPropertyValuesBuilder.AddLookup(Configuration.EnquiriesID.ID, env.ObjVerEx.ObjVer);

            //mfPropertyValuesBuilder.SetTitle(env.ObjVerEx.Title);


            var mfPropertyValuesBuilder = GenerateTemplateProperty(env.Vault, env.ObjVerEx);


            mfPropertyValuesBuilder.SetWorkflowState(Configuration.DocusignWorkflow, Configuration.DocusignUnsignedState);

            try
            {
                // Create our search builder.
                //var searchBuilder = new MFSearchBuilder(env.Vault);
                //searchBuilder.ObjType((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument);
                //searchBuilder.Property(MFBuiltInPropertyDef.MFBuiltInPropertyDefIsTemplate, new TypedValue { Value = true });
                //searchBuilder.Class(Configuration.SignatureDetailsClass);
                //searchBuilder.Property(Configuration.DocuSignTitle, new TypedValue { Value = Configuration.TemplateName });

                // Execute the search.
                // var searchResult2 = searchBuilder.FindOneEx();

                var templateObjVerEx = FindTemplate(env.Vault, Configuration.SignatureDetailsClass, Configuration.DocuSignTemplateName);
                if (templateObjVerEx != null)
                {
                    //ObjectFiles objTemplateFiles = env.Vault.ObjectFileOperations.GetFiles(templateObjVerEx.ObjVer);
                    //ObjVerEx newLetter = new ObjVerEx(env.Vault, env.Vault.ObjectOperations.CreateNewObject((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument, mfPropertyValuesBuilder.Values));
                    //CopyObjectFiles(env.Vault, objTemplateFiles, newLetter.ObjVer);
                    //env.Vault.ObjectOperations.SetSingleFileObject(newLetter.ObjVer, true);
                    //env.Vault.ObjectFileOperations.UpdateMetadataInFile(newLetter.ObjVer, -1, false);
                    //newLetter.CheckIn();

                    CreateDocumentFromTemplate(env.Vault, templateObjVerEx, mfPropertyValuesBuilder);

                }
                else
                {
                    throw new Exception("Could not find Template");
                }

            }
            catch (Exception ex)
            { }
        }




    }


}
