using MFiles.VAF.Common;
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
         //       SendEmail(5, false, env);


            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("DocumentExpiredIn30Days", "Error in State Action for 30 days to expiry.", ex);

            }
        }


    }

}
