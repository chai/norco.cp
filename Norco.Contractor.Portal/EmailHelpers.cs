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
                using (var emailMessage = new EmailMessage(this.Configuration.SmtpConfiguration))
                {


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




                    emailMessage.AddRecipient(AddressType.CarbonCopy, Configuration.NorcoNotificationPerson);




                    var certificateProperties = GetEmailProperties(env.ObjVerEx);
                    if (certificateProperties.subject != string.Empty && certificateProperties.emailBody != string.Empty)
                    {
                        if (expired)
                        {
                            emailMessage.Subject = certificateProperties.subjectExpired;
                        }
                        else
                        {
                            emailMessage.Subject = certificateProperties.subject;
                        }
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
                                    SysUtils.ReportToEventLog($"Send e-mail notification failed. Contracor email ({email1.Value.DisplayValue}) is invalid . Document {env.ObjVerEx.Title}({env.ObjVerEx.ID}).{Environment.NewLine}" +
                                        $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
                                        $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                                    throw new Exception("Email address invalid");
                                }

                            }
                            else
                            {
                                SysUtils.ReportToEventLog($"Send e-mail notification failed. Contractor does not have an email set. Document {env.ObjVerEx.Title}({env.ObjVerEx.ID}).{Environment.NewLine}" +
        $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
        $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                                throw new Exception("No email set");
                            }

                        }
                        else
                        {
                            SysUtils.ReportToEventLog($"Send e-mail notification failed. Mo Contractor set. Document {env.ObjVerEx.Title}({env.ObjVerEx.ID}).{Environment.NewLine}" +
        $"Email subject : {certificateProperties.subject} {Environment.NewLine}" +
        $"Email body : {certificateProperties.emailBody}", System.Diagnostics.EventLogEntryType.Error);
                            throw new Exception("No contractor set");
                        }


                    }

                    else
                    {
                        SysUtils.ReportToEventLog($"Send e-mail notification failed. No template set for document type id {env.ObjVerEx.Class}. Document {env.ObjVerEx.Title}({env.ObjVerEx.ID}).{Environment.NewLine}" +
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

        private (string subject, string subjectExpired, string emailBody) GetEmailProperties(ObjVerEx objVerEx)
        {
            string subject = String.Empty;
            string subjectExpired = String.Empty;
            string emailBody = String.Empty;
            try
            {
                if (Configuration.CertificateEmailProperties != null || Configuration.CertificateEmailProperties.Count > 0)
                {

                    var docDefinition = Configuration.CertificateEmailProperties.Where(type => type.DocumentType.ID == objVerEx.Class).FirstOrDefault();



                    subject = objVerEx.ExpandPlaceholderText(docDefinition.EmailSubjectTextTemplate);
                    subjectExpired = objVerEx.ExpandPlaceholderText(docDefinition.EmailSubjectExpiredTextTemplate);
                    emailBody = objVerEx.ExpandPlaceholderText(docDefinition.EmailBodyTemplate);


                }

            }
            catch (Exception ex)
            {
                SysUtils.ReportErrorToEventLog("GetCertificateProperty", "Error getting Certificate property for email body.", ex);
            }

            return (subject, subjectExpired,emailBody);
        }

    }
}
