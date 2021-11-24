using MFiles.VAF.Common;
using MFilesAPI.Extensions.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace Norco.Contractor.Portal
{
    public partial class VaultApplication
    {


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }



        /*

        private void SendEmail(int NumberOfDays, bool expired, StateEnvironment env)
        {
            try
            {
                var mfileVendorId = env.ObjVerEx.Class == Configuration.RawMaterialClass.ID
                    ? Configuration.RawMaterialVendor : Configuration.PackagingVendor;
                var vendorObjVerExList = env.ObjVerEx.GetAllDirectReferences(mfileVendorId);
                //Only want to do this if there is a vendor to contact
                if (vendorObjVerExList != null || vendorObjVerExList.Count > 0)
                {
                    // Create a message.
                    using (var emailMessage = new EmailMessage(this.Configuration.SmtpConfiguration))
                    {


                        var ExpiryDate = env.ObjVerEx.GetPropertyText(Configuration.CerticationExpiryDate);
                        // Configure the message metadata.
                        if (expired)
                        {
                            emailMessage.Subject = $"Certificate {env.ObjVerEx.Title} has expired";
                        }
                        else
                        {
                            emailMessage.Subject = $"Certificate {env.ObjVerEx.Title} will expire in {NumberOfDays} days, on {ExpiryDate}";
                        }
                        emailMessage.SetFrom(this.Configuration.SmtpConfiguration.DefaultSender);


                        // To.


                        foreach (var vendorinList in vendorObjVerExList)
                        {
                            //for each vendor get their contact details
                            var contactDetailsList = vendorinList.GetPropertyText(Configuration.VendorContactDetails).Split(';');
                            foreach (var toEmail in contactDetailsList)
                            {
                                if (IsValidEmail(toEmail))
                                {
                                    emailMessage.AddRecipient(AddressType.To, toEmail);
                                }
                                else
                                {
                                    SysUtils.ReportInfoToEventLog("SendEmail", $"Could not send email { emailMessage.Subject} to {toEmail} as its not a valid email address");
                                }
                            }
                        }




                        var certificateProperties = GetCertificateProperty(env.ObjVerEx);


                        var mfileManufactureList = env.ObjVerEx.Class == Configuration.RawMaterialClass.ID
    ? Configuration.RawManufactureList : Configuration.PackageManufactureList;


                        string ManufactureName = env.ObjVerEx.GetPropertyText(mfileManufactureList);



                        var mfileManufactureName = env.ObjVerEx.Class == Configuration.RawMaterialClass.ID
? Configuration.RawMaterialList : Configuration.PackageMaterialList;


                        string MaterialName = env.ObjVerEx.GetPropertyText(mfileManufactureName);

                        emailMessage.HtmlBody = $"Our records indicate that {ManufactureName} Food Safety & Quality certification for {MaterialName} has expired." +
                            $"<br>" +
                            $"The certificate {env.ObjVerEx.Title} is set to expire on the <b>{ExpiryDate}</b>." +
                            $"<br>" +
                            $"Details of the certificate are below: <br> " +
                            $"{certificateProperties}" +
                            $"<br>" +
                            $"Regards," +
                            $"<br>" +
                            $"Norco";

                        // Add all files from the current object.

                        emailMessage.AddAllFiles(env.ObjVerEx.Info, env.Vault, MFFileFormat.MFFileFormatDisplayOnlyPDF);
                        // Send the message.
                        emailMessage.Send();
                    }
                }
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("SendEmail", "Error Sending Email", ex);
            }

        }
*/
        private string GetCertificateProperty(ObjVerEx objVerEx)
        {
            string properties = "";
            try
            {
                if (Configuration.CertificateEmailProperties != null || Configuration.CertificateEmailProperties.Count > 0)
                {

                    var docDefinition = Configuration.CertificateEmailProperties.Where(type => type.DocumentType.ID == objVerEx.Class).FirstOrDefault();


                    foreach (var propID in docDefinition.EmailProperties)
                    {
                        properties = $"{properties}<b>{propID.PropertyName}:</b> {objVerEx.GetPropertyText(propID.CertificateProperty)}<br>";
                    }



                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("GetCertificateProperty", "Error getting Certificate property for email body.", ex);
            }

            return properties;
        }


    }
}
