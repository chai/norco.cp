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

        private void SendEmail(bool expired, StateEnvironment env)
        {
            try
            {

                // Create a message.
                using (var emailMessage = new EmailMessage(this.Configuration.DocumentEmailSettings.SmtpConfiguration))
                {


                    emailMessage.SetFrom(this.Configuration.DocumentEmailSettings.SmtpConfiguration.DefaultSender);


                    if (Configuration.DocumentEmailSettings?.CarbonCopyCompanyEmail ==true)
                    {

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
                    }


                    if (Configuration.DocumentEmailSettings?.NorcoNotificationPerson != String.Empty)
                    {
                        emailMessage.AddRecipient(AddressType.CarbonCopy, Configuration.DocumentEmailSettings.NorcoNotificationPerson);
                    }



                    var certificateProperties = GetEmailProperties(env.ObjVerEx, expired);
                    if (certificateProperties.subject != string.Empty && certificateProperties.emailBody != string.Empty)
                    {

                        emailMessage.Subject = certificateProperties.subject;                        
                        emailMessage.HtmlBody = certificateProperties.emailBody;

                        // Add all files from the current object.
                        emailMessage.AddAllFiles(env.ObjVerEx.Info, env.Vault, MFFileFormat.MFFileFormatDisplayOnlyPDF);

                        var contractor = env.ObjVerEx.GetDirectReference(Configuration.ContractorsForCompany);
                        if (contractor != null)
                        {
                            if (contractor.TryGetProperty(Configuration.EmailAddress, out PropertyValue email1))
                            {
                                if (IsValidEmail(email1.Value.DisplayValue))
                                {
                                    emailMessage.AddRecipient(AddressType.To, email1.Value.DisplayValue);
                                    emailMessage.Send();
                                }
                                else
                                {
                                    SysUtils.ReportToEventLog($"Send e-mail notification failed. Contracor email ({email1.Value.DisplayValue}) is invalid .{Environment.NewLine} {ObjectDetails(env.ObjVerEx)}{Environment.NewLine}" +
                                        $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
                                        $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                                    throw new Exception("Email address invalid");
                                }

                            }
                            else
                            {
                                SysUtils.ReportToEventLog($"Send e-mail notification failed. Contractor does not have an email set. {Environment.NewLine} {ObjectDetails(env.ObjVerEx)}{Environment.NewLine}" +
        $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
        $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                                throw new Exception("No email set");
                            }

                        }
                        else
                        {
                            SysUtils.ReportToEventLog($"Send e-mail notification failed. Mo Contractor set. {Environment.NewLine} {ObjectDetails(env.ObjVerEx)}{Environment.NewLine}" +
        $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
        $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                            throw new Exception("No contractor set");
                        }


                    }

                    else
                    {
                        SysUtils.ReportToEventLog($"Send e-mail notification failed. No template set for document.{Environment.NewLine} {ObjectDetails(env.ObjVerEx)}{Environment.NewLine}" +
    $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
    $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                        throw new Exception("Email template not set");
                    }
                }
            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("SendEmail", "Error Sending Email", ex);
                throw ex;
            }

        }

        private (string subject, string emailBody) GetEmailProperties(ObjVerEx objVerEx, bool expired)
        {
            string subject = String.Empty;

            string emailBody = String.Empty;
            try
            {
                //if (Configuration.CertificateEmailProperties != null || Configuration.CertificateEmailProperties.Count > 0)
             if(Configuration.DocumentEmailSettings!=null)
                {
                    if (Configuration.DocumentEmailSettings.CertificateEmailProperties.Count > 0)
                    {
                        var docDefinition = Configuration.DocumentEmailSettings.CertificateEmailProperties.Where(type => type.DocumentType.ID == objVerEx.Class).FirstOrDefault();

                        subject = expired 
                            ? objVerEx.ExpandPlaceholderText(docDefinition.EmailSubjectExpiredTextTemplate)
                            : objVerEx.ExpandPlaceholderText(docDefinition.EmailSubjectTextTemplate);

                        emailBody = objVerEx.ExpandPlaceholderText(docDefinition.EmailBodyTemplate);

                    }
                    //if the value not found or set above use the defaults
                    if(emailBody.Equals(String.Empty))
                    {
                        subject = expired 
                            ? objVerEx.ExpandPlaceholderText(Configuration.DocumentEmailSettings.DefaultEmailSubjectExpiredTextTemplate)
                            : objVerEx.ExpandPlaceholderText(Configuration.DocumentEmailSettings.DefaultEmailSubjectTextTemplate);

                        emailBody = objVerEx.ExpandPlaceholderText(Configuration.DocumentEmailSettings.DefaultEmailBodyTemplate);
                    }

                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("GetCertificateProperty", "Error getting Certificate property for email body.", ex);
            }

            return (subject,emailBody);
        }

    }
}
