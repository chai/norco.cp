using DocumentFormat.OpenXml.Packaging;
using MFiles.VAF.Common;
using MFilesAPI;
using MFilesAPI.Extensions.Email;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Xml.Linq;

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

                    // Create a message.
                    using (var emailMessage = new EmailMessage(this.Configuration.SmtpConfiguration))
                    {
                    string ExpiryDate = "";
                    string IssueDate = "";
                   if ( env.ObjVerEx.TryGetProperty(Configuration.DateOfExpiry, out PropertyValue ExpiryDateProperty))
                    {
                        ExpiryDate = ExpiryDateProperty.Value.DisplayValue;
                    }
                  if(  env.ObjVerEx.TryGetProperty(Configuration.DateOfIssue, out PropertyValue IssueDateProperty))
                    {
                        IssueDate = IssueDateProperty.Value.DisplayValue;
                    }

                    emailMessage.SetFrom(this.Configuration.SmtpConfiguration.DefaultSender);

                        var company = env.ObjVerEx.GetDirectReference(Configuration.CompanyOfContractor);
                        if(company!=null)
                        {
                            if(company.TryGetProperty(Configuration.EmailAddress, out PropertyValue email1))
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

                    if(expired)
                    {
                        emailMessage.Subject = $"Document {env.ObjVerEx.Title} isseued on {IssueDate} has expired on {ExpiryDate}";
                        emailMessage.HtmlBody = $"Our records indicate that Contractor {contractor.Title} from {company.Title} certification for {env.ObjVerEx.Title} has expired on {ExpiryDate}." +
                            $"<br>" +
                            $"The certificate {env.ObjVerEx.Title} is set to expire on the <b>{ExpiryDate}</b>." +
                            $"<br>" +
                            $"Details of the certificate are below: <br> " +
                            $"{certificateProperties}" +
                            $"<br>" +
                            $"Regards," +
                            $"<br>" +
                            $"Norco";

                    }
                    else
                    {
                        emailMessage.Subject = $"Document {env.ObjVerEx.Title} isseued on {IssueDate} will expire in {NumberOfDays} days, on {ExpiryDate}";
                        emailMessage.HtmlBody = $"Our records indicate that Contractor {contractor.Title} from {company.Title} certification for {env.ObjVerEx.Title} will expired on {ExpiryDate}." +
                                                    $"<br>" +
                                                    $"The certificate {env.ObjVerEx.Title} is set to expire on the <b>{ExpiryDate}</b>." +
                                                    $"<br>" +
                                                    $"Details of the certificate are below: <br> " +
                                                    $"{certificateProperties}" +
                                                    $"<br>" +
                                                    $"Regards," +
                                                    $"<br>" +
                                                    $"Norco";

                    }



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

        private string getEmailContent(string emailName, List<emailInlineImage> images, EnvironmentBase env)
        {
            string bodyMessage = "";
            string filePathname = SysUtils.CreateTempFolder();
            try
            {
                // Create our search builder.
                var searchBuilder = new MFSearchBuilder(env.Vault);
                searchBuilder.ObjType((int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument);
                searchBuilder.Property(MFBuiltInPropertyDef.MFBuiltInPropertyDefNameOrTitle, new TypedValue { Value = emailName });
                // Execute the search.

                var searchResult = searchBuilder.FindOneEx();
                if (searchResult != null)
                {
                    //https://loicsterckx.wordpress.com/2015/10/30/c-sends-emails-based-on-a-word-template-with-the-net-mail-library/
                    using (MemoryStream memoryStream = new MemoryStream())
                    {


                        
                        string fileName = $@"{filePathname}\emailTemplate.docx";
                        ObjectFiles objFiles = env.Vault.ObjectFileOperations.GetFiles(searchResult.ObjVer);
                        env.Vault.ObjectFileOperations.DownloadFile(objFiles[1].ID, objFiles[1].Version, fileName);

                     var result=env.Vault.ObjectFileOperations.DownloadFileAsDataURIEx(searchResult.ObjVer, objFiles[1].FileVer);

                        byte[] byteArray = File.ReadAllBytes(fileName);
                        File.Delete(fileName);

                        memoryStream.Write(byteArray, 0, byteArray.Length);
                        using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                        {
                            int imageCounter = 0;
                            HtmlConverterSettings settings = new HtmlConverterSettings()
                            {
                                PageTitle = "Letter",
                                //AdditionalCss = "p { margin-bottom: 20px; }",

                                //used for tag <title> of html page
                                ImageHandler = imageInfo =>
                                {
                                    DirectoryInfo localDirInfo = new DirectoryInfo("img");
                                    if (!localDirInfo.Exists)
                                        localDirInfo.Create();
                                    ++imageCounter;
                                    string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                                    ImageFormat imageFormat = null;
                                    if (extension == "png")
                                    {
                                        extension = "gif";
                                        imageFormat = ImageFormat.Gif;
                                    }
                                    else if (extension == "gif")
                                        imageFormat = ImageFormat.Gif;
                                    else if (extension == "bmp")
                                        imageFormat = ImageFormat.Bmp;
                                    else if (extension == "jpeg")
                                        imageFormat = ImageFormat.Jpeg;
                                    else if (extension == "tiff")
                                    {
                                        extension = "gif";
                                        imageFormat = ImageFormat.Gif;
                                    }
                                    else if (extension == "x-wmf")
                                    {
                                        extension = "wmf";
                                        imageFormat = ImageFormat.Wmf;
                                    }
                                    if (imageFormat == null)
                                        return null;

                                    string imageFileName = "img/image" +
                                        imageCounter.ToString() + "." + extension;
                                    try
                                    {
                                        imageInfo.Bitmap.Save(imageFileName, imageFormat);
                                    }
                                    catch (System.Runtime.InteropServices.ExternalException) { }

                                    string contentIDHeader = Guid.NewGuid().ToString();
                                    images.Add(new emailInlineImage { guid = contentIDHeader, filepath = imageFileName, type = @"image/" + extension });

                                    XElement img = new XElement(Xhtml.img,
                                        new XAttribute(NoNamespace.src, "cid:" + contentIDHeader),
                                        imageInfo.ImgStyleAttribute,
                                        imageInfo.AltText != null ?
                                            new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                                    return img;
                                }
                            };
                            ;
                            XElement html = HtmlConverter.ConvertToHtml(doc, settings);
                            XNamespace w = "http://www.w3.org/1999/xhtml";
                            var bodyContainer = html.Element(w + "body");
                            XElement myNewElement = new XElement("p", $"%%{env.ObjVerEx.ID}%%");
                            myNewElement.SetAttributeValue("Style", "font-size: 1px");
                            bodyContainer.AddAfterSelf(myNewElement);
                            bodyMessage = bodyContainer.ToStringNewLineOnAttributes();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //Not loging usual error to Windows Event per M-File cloud requirement
                //Log SysUtils.ReportErrorToEventLog(e);
            }
            finally
            {
                if (Directory.Exists(filePathname))
                {
                    SysUtils.DeleteFromDisk(filePathname);
                }
            }
            return bodyMessage;
        }


        public class emailInlineImage
        {
            public string guid { get; set; }
            public string type { get; set; }
            public string filepath { get; set; }
        }
    }
}
