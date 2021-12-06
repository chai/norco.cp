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
       // //[EventHandler(MFEventHandlerType.MFEventHandlerAfterFileUpload,Class = "Cl.OtherDocument")]
       // [EventHandler(MFEventHandlerType.MFEventHandlerAfterCheckInChangesFinalize, Class = "Cl.OtherDocument")]
       //// [EventHandler(MFEventHandlerType.MFEventHandlerAfterCreateNewObjectFinalize, Class = "Cl.OtherDocument")]

       // public void CheckForName(EventHandlerEnvironment env)
       // {
       //     try {
       //         var propertyValues = env.ObjVerEx.Properties;
       //         propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefWorkflow, MFDataType.MFDatatypeLookup, Configuration.DocumentRequestWorkflow);
       //         propertyValues.SetProperty((int)MFBuiltInPropertyDef.MFBuiltInPropertyDefState, MFDataType.MFDatatypeLookup, Configuration.InitialDocumentRequest);


       //         SysUtils.ReportInfoToEventLog($"Event for HubShare", $"{env.ObjVerEx.Title} {env.EventType} {env.CurrentUserID}");
       //         env.ObjVerEx.SaveProperties(propertyValues);
       //         SysUtils.ReportInfoToEventLog($"Event for HubShare SAVED", $"{env.ObjVerEx.Title} {env.EventType} {env.CurrentUserID}");

       //     }
       //     catch (Exception e) {
       //         SysUtils.ReportErrorToEventLog("Hubsher", e);
       //     }


            
       // }
    }
}
