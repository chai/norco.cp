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
        [PropertyValueValidation("PD.ExpiryDate")]
        public bool ExpiryDate (PropertyEnvironment env, out string message)
        {
            // Set the message (displayed if validation fails).
            message = "";

            // Validate.
            try
            {
                return ValidateDate(env.ObjVerEx.GetProperty(Configuration.DateOfIssue), env.PropertyValue, ref message);
            }
            catch
            {
                return false;
            }

        }




        //[PropertyValueValidation("PD.ActivityExpiryDate")]
        //public bool ActivityExpiryDate(PropertyEnvironment env, out string message)
        //{
        //    // Set the message (displayed if validation fails).
        //    message = "";

        //    // Validate.
        //    try
        //    {
        //        return ValidateDate(env.ObjVerEx.GetProperty(Configuration.ActivityStartDate), env.PropertyValue, ref message);
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}


       // [PropertyValueValidation("PD.Valid")]
        public bool DocumentValid(PropertyEnvironment env, out string message)
        {
            // Set the message (displayed if validation fails).
            message = "";
            bool isValidValue = Convert.ToBoolean(env.PropertyValue.Value.DisplayValue);
            // Validate.
            try
            {


                return ValidateDate(env.ObjVerEx.GetProperty(Configuration.DateOfIssue), env.PropertyValue, ref message);
            }
            catch
            {
                return false;
            }

        }
        

        private bool ValidateDate(PropertyValue start, PropertyValue end, ref string message)
        {

            string dateOfExpiry = end.GetValueAsTextEx(true, true, true, true, true, true, true);
            if (string.IsNullOrEmpty(dateOfExpiry))
            {
                return true;

                //only validate if there is a value set
            }


            string dateOfIssue = start.GetValueAsTextEx(true, true, true, true, true, true, true);

            if (string.IsNullOrEmpty(dateOfIssue))
            {

                message = $"Start date must be set.";
                return false;
                //no need to validate as there is no date of issues
            }




            var startDate = Convert.ToDateTime(dateOfIssue);

            var endDate = Convert.ToDateTime(dateOfExpiry);

            message = $"Expiry date of {dateOfExpiry} can not be before {dateOfIssue}.";

            return startDate <= endDate;

        }
    }
}
