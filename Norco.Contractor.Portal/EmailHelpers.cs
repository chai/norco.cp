using MFiles.VAF.Common;
using MFilesAPI;
using MFilesAPI.Extensions.Email;
using System;
using System.Linq;
using System.Net.Mail;

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

        private void SendEmail(int NumberOfDays, bool expired, StateEnvironment env)
        {
            try
            {


                //string emailBody = "";

                //var mfPropertyValuesBuilder = GenerateTemplateProperty(env.Vault, env.ObjVerEx);                
                //try
                //{


                //    var templateObjVerEx = FindTemplate(env.Vault, Configuration.SignatureDetailsClass, Configuration.TemplateName);
                //    if (templateObjVerEx != null)
                //    {

                //        var newEmailNotification = CreateDocumentFromTemplate(env.Vault, templateObjVerEx, mfPropertyValuesBuilder);
                //        List<emailInlineImage> CIDImages = new List<emailInlineImage>();
                //        CIDImages.Add(new emailInlineImage()
                //            { filepath = @"C:\norcoemmail\footer.jpeg", guid = env.ObjVerEx.Info.VersionGUID, type = "jpeg" });

                //        emailBody = getEmailContent(env.Vault, newEmailNotification, "email notification", CIDImages);

                //    }
                //    else
                //    {
                //        throw new Exception("Could not find Template");
                //    }

                //}
                //catch (Exception ex)
                //{ }


                // Create a message.
                using (var emailMessage = new EmailMessage(this.Configuration.SmtpConfiguration))
                {
                    string ExpiryDate = "";
                    string IssueDate = "";
                    if (env.ObjVerEx.TryGetProperty(Configuration.DateOfExpiry, out PropertyValue ExpiryDateProperty))
                    {
                        ExpiryDate = ExpiryDateProperty.Value.DisplayValue;
                    }
                    if (env.ObjVerEx.TryGetProperty(Configuration.DateOfIssue, out PropertyValue IssueDateProperty))
                    {
                        IssueDate = IssueDateProperty.Value.DisplayValue;
                    }

                    emailMessage.SetFrom(this.Configuration.SmtpConfiguration.DefaultSender);

                    var company = env.ObjVerEx.GetDirectReference(Configuration.CompanyOfContractor);
                    if (company != null)
                    {
                        if (company.TryGetProperty(Configuration.EmailAddress, out PropertyValue email1))
                        {
                            if (IsValidEmail(email1.Value.DisplayValue))
                            {
                                emailMessage.AddRecipient(AddressType.CarbonCopy, email1.Value.DisplayValue);
                            }
                        }
                        if (company.TryGetProperty(Configuration.EmailAddress, out PropertyValue email2))
                        {
                            if (IsValidEmail(email2.Value.DisplayValue))
                            {
                                emailMessage.AddRecipient(AddressType.CarbonCopy, email2.Value.DisplayValue);
                            }
                        }

                    }


                    var contractor = env.ObjVerEx.GetDirectReference(Configuration.ContractorsForCompany);
                    if (contractor != null)
                    {
                        if (contractor.TryGetProperty(Configuration.EmailAddress, out PropertyValue email1))
                        {
                            if (IsValidEmail(email1.Value.DisplayValue))
                            {
                                emailMessage.AddRecipient(AddressType.To, email1.Value.DisplayValue);
                            }
                        }
                    }


                    emailMessage.AddRecipient(AddressType.CarbonCopy, Configuration.NorcoNotificationPerson);




                    var certificateProperties = GetCertificateProperty(env.ObjVerEx);

                    //if (expired)
                    //{
                    //emailMessage.Subject = $"Document {env.ObjVerEx.Title} isseued on {IssueDate} has expired on {ExpiryDate}";
                    //emailMessage.HtmlBody = $"Our records indicate that Contractor {contractor.Title} from {company.Title} certification for {env.ObjVerEx.Title} has expired on {ExpiryDate}." +
                    //    $"<br>" +
                    //    $"The certificate {env.ObjVerEx.Title} is set to expire on the <b>{ExpiryDate}</b>." +
                    //    $"<br>" +
                    //    $"Details of the certificate are below: <br> " +
                    //    $"{certificateProperties}" +
                    //    $"<br>" +
                    //    $"Regards," +
                    //    $"<br>" +
                    //    $"Norco";
                    //}
                    //else
                    //{
                    //    emailMessage.Subject = $"Document {env.ObjVerEx.Title} isseued on {IssueDate} will expire in {NumberOfDays} days, on {ExpiryDate}";
                    //    emailMessage.HtmlBody = $"Our records indicate that Contractor {contractor.Title} from {company.Title} certification for {env.ObjVerEx.Title} will expired on {ExpiryDate}." +
                    //                                $"<br>" +
                    //                                $"The certificate {env.ObjVerEx.Title} is set to expire on the <b>{ExpiryDate}</b>." +
                    //                                $"<br>" +
                    //                                $"Details of the certificate are below: <br> " +
                    //                                $"{certificateProperties}" +
                    //                                $"<br>" +
                    //                                $"Regards," +
                    //                                $"<br>" +
                    //                                $"Norco";
                    //}
                    //ResourceManager rm = new ResourceManager("AppResources", typeof(Example).Assembly);
                    //TextFile1Any

                       emailMessage.Subject = "test";
                    emailMessage.HtmlBody = env.ObjVerEx.ExpandPlaceholderText( Configuration.EmailTemplate) ;

                    // Add all files from the current object.
                    emailMessage.AddAllFiles(env.ObjVerEx.Info, env.Vault, MFFileFormat.MFFileFormatDisplayOnlyPDF);

                    // Send the message.
                    emailMessage.Send();
                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("SendEmail", "Error Sending Email", ex);
            }

        }

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
