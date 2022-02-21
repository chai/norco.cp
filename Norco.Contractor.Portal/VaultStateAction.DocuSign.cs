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

            var mfPropertyValuesBuilder = GenerateTemplateProperty(env.Vault, env.ObjVerEx);
            mfPropertyValuesBuilder.SetWorkflowState(Configuration.DocusignWorkflow, Configuration.DocusignUnsignedState);

            try
            {


                var templateObjVerEx = FindTemplate(env.Vault, Configuration.SignatureDetailsClass, Configuration.DocuSignTemplateName);
                if (templateObjVerEx != null)
                {
                    CreateDocumentFromTemplate(env.Vault, templateObjVerEx, mfPropertyValuesBuilder);
                }
                else
                {

                    SysUtils.ReportToEventLog($"CreateTemplatedInvoice. Could not find Template: {ObjectDetails(env.ObjVerEx)}",System.Diagnostics.EventLogEntryType.Information);

                 
                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog($"CreateTemplatedInvoice.", $"Error in State Action SetValidated By. {ObjectDetails(env.ObjVerEx)}", ex);
            }
        }




    }


}
