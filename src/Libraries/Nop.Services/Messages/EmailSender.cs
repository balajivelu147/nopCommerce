using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MimeKit.Cryptography;
using MimeKit.Text;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Media;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial class EmailSender : IEmailSender
    {
        #region Fields

        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;
        private readonly ISmtpBuilder _smtpBuilder;

        #endregion

        #region Ctor

        public EmailSender(IDownloadService downloadService, INopFileProvider fileProvider, ISmtpBuilder smtpBuilder)
        {
            _downloadService = downloadService;
            _fileProvider = fileProvider;
            _smtpBuilder = smtpBuilder;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create an file attachment for the specific download object from DB
        /// </summary>
        /// <param name="download">Attachment download (another attachment)</param>
        /// <returns>A leaf-node MIME part that contains an attachment.</returns>
        protected MimePart CreateMimeAttachment(Download download)
        {
            if (download is null)
                throw new ArgumentNullException(nameof(download));

            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();

            return CreateMimeAttachment($"{fileName}{download.Extension}", download.DownloadBinary, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
        }

        /// <summary>
        /// Create an file attachment for the specific file path
        /// </summary>
        /// <param name="filePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a leaf-node MIME part that contains an attachment.
        /// </returns>
        protected async Task<MimePart> CreateMimeAttachmentAsync(string filePath, string attachmentFileName = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(attachmentFileName))
                attachmentFileName = Path.GetFileName(filePath);

            return CreateMimeAttachment(
                    attachmentFileName,
                    await _fileProvider.ReadAllBytesAsync(filePath),
                    _fileProvider.GetCreationTime(filePath),
                    _fileProvider.GetLastWriteTime(filePath),
                    _fileProvider.GetLastAccessTime(filePath));
        }

        /// <summary>
        /// Create an file attachment for the binary data
        /// </summary>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="binaryContent">The array of unsigned bytes from which to create the attachment stream.</param>
        /// <param name="cDate">Creation date and time for the specified file or directory</param>
        /// <param name="mDate">Date and time that the specified file or directory was last written to</param>
        /// <param name="rDate">Date and time that the specified file or directory was last access to.</param>
        /// <returns>A leaf-node MIME part that contains an attachment.</returns>
        protected MimePart CreateMimeAttachment(string attachmentFileName, byte[] binaryContent, DateTime cDate, DateTime mDate, DateTime rDate)
        {
            if (!ContentType.TryParse(MimeTypes.GetMimeType(attachmentFileName), out var mimeContentType))
                mimeContentType = new ContentType("application", "octet-stream");

            return new MimePart(mimeContentType)
            {
                FileName = attachmentFileName,
                Content = new MimeContent(new MemoryStream(binaryContent)),
                ContentDisposition = new ContentDisposition
                {
                    CreationDate = cDate,
                    ModificationDate = mDate,
                    ReadDate = rDate
                },
                ContentTransferEncoding = ContentEncoding.Base64
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachedDownloadId">Attachment download ID (another attachment)</param>
        /// <param name="headers">Headers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SendEmailAsync(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            using (MailMessage objMailMsg = new MailMessage())
            {
                //objMailMsg.AlternateViews.Add("<p>sdsds</p>");
                objMailMsg.Body = "<p>sdsds</p>";
                objMailMsg.Subject = "message.Subject";
                objMailMsg.BodyEncoding = Encoding.UTF8;


                //Properties.Settings.Default.Reload();

                //string fromEmail = Properties.Settings.Default.FromMail;
                //string fromName = Properties.Settings.Default.FromName;
                //string smtpPassword = Properties.Settings.Default.PwdSmtp;

                //objMailMsg.From = new MailAddress(fromEmail, fromName);
                //objMailMsg.To.Add(message.Destination);
                objMailMsg.IsBodyHtml = true;

                // Now we have to set the value to Mail message properties
                // Note Please change it to correct mail-id to use this in your application
                objMailMsg.From = new MailAddress(fromAddress, "ABC");
                objMailMsg.To.Add(new MailAddress(toAddress, "BCD"));
                //objMailMsg.CC.Add(new MailAddress("zzzzz@xyz.com", "DEF"));// it is optional, only if required
                objMailMsg.Subject = "Send mail with ICS file as an Attachment";
                objMailMsg.Body = "Please Attend the meeting with this schedule";
                objMailMsg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                str.AppendLine("METHOD:REQUEST");
                str.AppendLine("BEGIN:VEVENT");
                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+330)));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+660)));
                //str.AppendLine("LOCATION: " + this.Location);
                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
                str.AppendLine(string.Format("DESCRIPTION:{0}", objMailMsg.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", objMailMsg.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", objMailMsg.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", objMailMsg.From.Address));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", objMailMsg.To[0].DisplayName, objMailMsg.To[0].Address));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                //Now sending a mail with attachment ICS file.                     
                System.Net.Mail.SmtpClient smtpclient = new System.Net.Mail.SmtpClient();
              //  smtpclient.Host = "localhost"; //-------this has to given the Mailserver IP

                smtpclient.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                contype.Parameters.Add("name", "Meeting.ics");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                objMailMsg.AlternateViews.Add(avCal);

                MimeMessage message = MimeMessage.CreateFromMailMessage(objMailMsg);


                //  var message = new MimeMessage();

                message.From.Add(new MailboxAddress(fromName, fromAddress));
                message.To.Add(new MailboxAddress(toName, toAddress));

                if (!string.IsNullOrEmpty(replyTo))
                {
                    message.ReplyTo.Add(new MailboxAddress(replyToName, replyTo));
                }

                //BCC
                if (bcc != null)
                {
                    foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                    {
                        message.Bcc.Add(new MailboxAddress("", address.Trim()));
                    }
                }

                //CC
                if (cc != null)
                {
                    foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                    {
                        message.Cc.Add(new MailboxAddress("", address.Trim()));
                    }
                }

                //content
                message.Subject = subject;

                //headers
                if (headers != null)
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }

                var multipart = new Multipart("mixed")
            {
                new TextPart(TextFormat.Html) { Text = body }
            };

                //create the file attachment for this e-mail message
                if (!string.IsNullOrEmpty(attachmentFilePath) && _fileProvider.FileExists(attachmentFilePath))
                {
                    multipart.Add(await CreateMimeAttachmentAsync(attachmentFilePath, attachmentFileName));
                }

                //another attachment?
                if (attachedDownloadId > 0)
                {
                    var download = await _downloadService.GetDownloadByIdAsync(attachedDownloadId);
                    //we do not support URLs as attachments
                    if (!download?.UseDownloadUrl ?? false)
                    {
                        multipart.Add(CreateMimeAttachment(download));
                    }
                }

                message.Body = multipart;

                //send email
                using var smtpClient = await _smtpBuilder.BuildAsync(emailAccount);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }


        //public async Task SendAsync(IdentityMessage message)
        //{
        //    if (message == null)
        //        return;

        //    //LinkedResource inline = new LinkedResource(System.Web.Hosting.HostingEnvironment.MapPath($"~/Images/logo.png"))
        //    //{
        //    //    ContentId = Guid.NewGuid().ToString(),
        //    //    ContentType = new ContentType("image/png")
        //    //    {
        //    //        Name = "logo.png"
        //    //    }
        //    //};

        //    //var htmlBody = message.Body.Replace("{CID:LOGO}", String.Format("cid:{0}", inline.ContentId));

        //    //AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
        //    //avHtml.LinkedResources.Add(inline);

        //    using (MailMessage objMailMsg = new MailMessage())
        //    {
        //        //objMailMsg.AlternateViews.Add(avHtml);
        //        //objMailMsg.Body = htmlBody;
        //        //objMailMsg.Subject = message.Subject;
        //        //objMailMsg.BodyEncoding = Encoding.UTF8;


        //        //Properties.Settings.Default.Reload();

        //        //string fromEmail = Properties.Settings.Default.FromMail;
        //        //string fromName = Properties.Settings.Default.FromName;
        //        //string smtpPassword = Properties.Settings.Default.PwdSmtp;

        //        //objMailMsg.From = new MailAddress(fromEmail, fromName);
        //        //objMailMsg.To.Add(message.Destination);
        //        //objMailMsg.IsBodyHtml = true;

        //        //Now we have to set the value to Mail message properties

        //        //Note Please change it to correct mail-id to use this in your application
        //        objMailMsg.From = new MailAddress("xxxxx@xyz.com", "ABC");
        //        objMailMsg.To.Add(new MailAddress("yyyyy@xyz.com", "BCD"));
        //        objMailMsg.CC.Add(new MailAddress("zzzzz@xyz.com", "DEF"));// it is optional, only if required
        //        objMailMsg.Subject = "Send mail with ICS file as an Attachment";
        //        objMailMsg.Body = "Please Attend the meeting with this schedule";
        //        objMailMsg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

        //        // Now Contruct the ICS file using string builder
        //        StringBuilder str = new StringBuilder();
        //        str.AppendLine("BEGIN:VCALENDAR");
        //        str.AppendLine("PRODID:-//Schedule a Meeting");
        //        str.AppendLine("VERSION:2.0");
        //        str.AppendLine("METHOD:REQUEST");
        //        str.AppendLine("BEGIN:VEVENT");
        //        str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+330)));
        //        str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
        //        str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+660)));
        //        //  str.AppendLine("LOCATION: " + this.Location);
        //        str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
        //        str.AppendLine(string.Format("DESCRIPTION:{0}", objMailMsg.Body));
        //        str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", objMailMsg.Body));
        //        str.AppendLine(string.Format("SUMMARY:{0}", objMailMsg.Subject));
        //        str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", objMailMsg.From.Address));

        //        str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", objMailMsg.To[0].DisplayName, objMailMsg.To[0].Address));

        //        str.AppendLine("BEGIN:VALARM");
        //        str.AppendLine("TRIGGER:-PT15M");
        //        str.AppendLine("ACTION:DISPLAY");
        //        str.AppendLine("DESCRIPTION:Reminder");
        //        str.AppendLine("END:VALARM");
        //        str.AppendLine("END:VEVENT");
        //        str.AppendLine("END:VCALENDAR");

        //        //Now sending a mail with attachment ICS file.                     
        //        System.Net.Mail.SmtpClient smtpclient = new System.Net.Mail.SmtpClient();
        //        smtpclient.Host = "localhost"; //-------this has to given the Mailserver IP

        //        smtpclient.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

        //        System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
        //        contype.Parameters.Add("method", "REQUEST");
        //        contype.Parameters.Add("name", "Meeting.ics");
        //        AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
        //        objMailMsg.AlternateViews.Add(avCal);

        //        MimeMessage mime = MimeMessage.CreateFromMailMessage(objMailMsg);
        //        HeaderId[] headersToSign = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.Date };
        //        string domain = "example.cl";
        //        string selector = "default";
        //        DkimSigner signer = new DkimSigner(@"C:\inetpub\dkim.pem", domain, selector)
        //        {
        //            HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
        //            BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
        //            AgentOrUserIdentifier = "@contact.example.cl",
        //            QueryMethod = "dns/txt",
        //        };
        //        mime.Prepare(EncodingConstraint.EightBit);
        //        signer.Sign(mime, headersToSign);

        //        using (SmtpClient smtpClient = new SmtpClient())
        //        {
        //            await Task.Run(() =>
        //            {
        //                try
        //                {
        //                    smtpClient.Connect(Properties.Settings.Default.ServidorSmtp, Properties.Settings.Default.PuertoSmtp, Properties.Settings.Default.SSLSmtp);
        //                    smtpClient.Authenticate(fromEmail, smtpPassword);
        //                    smtpClient.Send(mime);
        //                    smtpClient.Disconnect(true);
        //                }
        //                catch (Exception ex)
        //                {
        //                    ErrorLog.Save(ex);
        //                    //InfoLog.Save(htmlBody);
        //                }
        //            });
        //        }
        //    }
        //}

        #endregion
    }
}