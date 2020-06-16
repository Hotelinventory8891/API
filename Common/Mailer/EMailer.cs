using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Common.Mailer
{
    public class EMailer
    {

        #region Enums

        public enum Priority
        {
            Normal = MailPriority.Normal,
            High = MailPriority.High,
            Low = MailPriority.Low
        }

        #endregion Enums

        #region Private Variables

        private string _MailFrom;
        private string _MailToCcList;
        private string _MailToBccList;
        private string _MailSubject;
        private string _MailBody;
        private string _SMTPServer;
        private int _SMTPPort;
        private string _SMTPUsername;
        private string _SMTPPassword;
        private bool _mSMTPSSL;
        private MailMessage _MailObject;
        private bool _SendAsync;
        private bool _TryAgianOnFailure;
        private int _TryAgainDelayTime;
        private List<string> _MailAttachments;
        private List<string> _MailToList;
        private string _ErrorMsg;
        private bool _isHTML = true;
        private Priority _Priority;
        private bool _isLogged;
        private bool _useDefaultCredentials;
        private string _mailTemplate;
        private string _mailFontStyleTemplate;


        #endregion Private Variables

        #region Properties

        public string MailFontStyleTemplate
        {
            get { return _mailFontStyleTemplate; }
            set { _mailFontStyleTemplate = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return _useDefaultCredentials; }
            set { _useDefaultCredentials = value; }
        }

        public string MailTemplate
        {
            get { return _mailTemplate; }
            set { _mailTemplate = value; }
        }

        public string MailFrom
        {
            set { _MailFrom = value; }
            get { return _MailFrom; }
        }

        public List<string> MailToList
        {
            set { _MailToList = value; }
            get { return _MailToList; }
        }

        public string MailSubject
        {
            set { _MailSubject = value; }
            get { return _MailSubject; }
        }

        public string MailBody
        {
            set { _MailBody = value; }
            get { return _MailBody; }
        }

        public string SMTPServer
        {
            set { _SMTPServer = value; }
            get { return _SMTPServer; }
        }

        public int SMTPPort
        {
            set { _SMTPPort = value; }
            get { return _SMTPPort; }
        }

        public string SMTPUsername
        {
            set { _SMTPUsername = value; }
            get { return _SMTPUsername; }
        }

        public string SMTPPassword
        {
            set { _SMTPPassword = value; }
            get { return _SMTPPassword; }
        }

        public bool SMTPSSL
        {
            set { _mSMTPSSL = value; }
            get { return _mSMTPSSL; }
        }

        public bool SendAsync
        {
            set { _SendAsync = value; }
            get { return _SendAsync; }
        }

        public bool TryAgianOnFailure
        {
            set { _TryAgianOnFailure = value; }
            get { return _TryAgianOnFailure; }
        }

        public int TryAgainDelayTime
        {
            set { _TryAgainDelayTime = value; }
            get { return _TryAgainDelayTime; }
        }
        public List<string> MailAttachments
        {
            set { _MailAttachments = value; }
            get { return _MailAttachments; }
        }

        public bool IsHTML
        {
            get { return _isHTML; }
            set { _isHTML = value; }
        }

        public Priority MessagePriority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }

        public string ErrorMsg
        {
            set { _ErrorMsg = value; }
        }

        public string MailToCcListString
        {
            get { return _MailToCcList; }
            set { _MailToCcList = value; }
        }

        public string MailToBccListString
        {
            get { return _MailToBccList; }
            set { _MailToBccList = value; }
        }

        public bool IsLogged
        {
            get { return _isLogged; }
        }


        #endregion Properties

        #region Constructor

        public EMailer()
        {
            _MailObject = new MailMessage();

            _MailFrom = "";
            _MailSubject = "";
            _MailBody = "";
            _SMTPPort = 25;
            _SMTPUsername = "";
            _SMTPPassword = "";
            _mSMTPSSL = false;
            _SendAsync = false;
            _TryAgianOnFailure = false;
            _TryAgainDelayTime = 10000;
            _isHTML = true;

            _MailToList = new List<string>();
            _MailAttachments = new List<string>();
            _Priority = Priority.Normal;

            ReadFromConfig();
            if (string.IsNullOrEmpty(_SMTPServer))
            {
                foreach (IPAddress Ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (Ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        _SMTPServer = Ip.ToString();
                }
            }
        }

        #endregion Constructor

        #region Methods

        #region Private Methods

        private bool ValidateMailSettings()
        {
            if (string.IsNullOrEmpty(_MailFrom))
                throw new ArgumentNullException("Mail From should not be empty");
            else if (_MailToList.Count == 0)
                throw new ArgumentNullException("Mail To should not be empty");
            return true;
        }

        private void ReadFromConfig()
        {
            List<string> configKeys = new List<string>(WebConfigurationManager.AppSettings.AllKeys);
            if (configKeys.Contains("DefaultSMTPServer"))
                _SMTPServer = WebConfigurationManager.AppSettings["DefaultSMTPServer"];
            if (configKeys.Contains("DefaultSMTPPort"))
                Int32.TryParse(WebConfigurationManager.AppSettings["DefaultSMTPPort"], out _SMTPPort);
            if (configKeys.Contains("DefaultSMTPUserName"))
                _SMTPUsername = WebConfigurationManager.AppSettings["DefaultSMTPUserName"];
            if (configKeys.Contains("DefaultSMTPPassword"))
                _SMTPPassword = WebConfigurationManager.AppSettings["DefaultSMTPPassword"];
            if (configKeys.Contains("TryAgianOnFailure"))
                Boolean.TryParse(WebConfigurationManager.AppSettings["TryAgianOnFailure"], out _TryAgianOnFailure);
            if (configKeys.Contains("DefaultMailFrom"))
                _MailFrom = WebConfigurationManager.AppSettings["DefaultMailFrom"];
            if (configKeys.Contains("DefaultFontStyle"))
                _MAILFONTSTYLE = WebConfigurationManager.AppSettings["DefaultFontStyle"];
        }

        #endregion Private Methods

        #region Public Methods
        public bool Send()
        {

            MailMessage Email = new MailMessage();
            MailAddress MailFrom = new MailAddress(_MailFrom, _MailFrom);
            Email.From = MailFrom;
            foreach (string Receiptent in _MailToList)
                Email.To.Add(Receiptent);
            if (!string.IsNullOrEmpty(_MailToCcList))
                Email.CC.Add(_MailToCcList);
            if (!string.IsNullOrEmpty(_MailToBccList))
                Email.Bcc.Add(_MailToBccList);
            Email.Subject = _MailSubject;

            //Email.Body = GetMailBodyWithTemplate(_MailBody);
            Email.Body = _MailBody;
            Email.IsBodyHtml = _isHTML;
            Email.Priority = (MailPriority)_Priority;

            foreach (string attachment in _MailAttachments)
            {
                if (File.Exists(attachment))
                {
                    Attachment TempAttachment = new Attachment(attachment);
                    Email.Attachments.Add(TempAttachment);
                }
            }


            // Smtp Client
            SmtpClient SmtpMail = null;
            if (UseDefaultCredentials)
            {
                SmtpMail = new SmtpClient();
                SmtpMail.UseDefaultCredentials = true;
            }
            else
            {
                SmtpMail = new SmtpClient(_SMTPServer, _SMTPPort);
                //if (!string.IsNullOrEmpty(mSMTPUsername) && !string.IsNullOrEmpty(mSMTPPassword))
                SmtpMail.Credentials = new NetworkCredential(_SMTPUsername, _SMTPPassword);
            }

            SmtpMail.EnableSsl = _mSMTPSSL;

            Boolean bResult = true;
            Exception exSend = null;
            try
            {
                ValidateMailSettings();
                if (_SendAsync)
                    SmtpMail.SendAsync(Email, Email);
                else
                    SmtpMail.Send(Email);
            }

            catch (ArgumentNullException aex)
            {
                bResult = false;
                exSend = aex;
            }
            catch (SmtpFailedRecipientsException e)
            {
                for (int k = 0; k < e.InnerExceptions.Length; k++)
                {
                    bResult = false;

                    SmtpStatusCode StatusCode = e.InnerExceptions[k].StatusCode;
                    if (StatusCode == SmtpStatusCode.MailboxUnavailable ||
                        StatusCode == SmtpStatusCode.MailboxBusy)
                    {
                        try
                        {
                            if (_TryAgianOnFailure)
                            {
                                Thread.Sleep(_TryAgainDelayTime);
                                // send the message
                                string sTemp = "";
                                if (_SendAsync)
                                    SmtpMail.SendAsync(Email, sTemp);
                                else
                                    SmtpMail.Send(Email);
                                // Message was sent.
                                bResult = true;

                            }
                        }
                        catch (Exception ex)
                        {
                            exSend = ex;
                            bResult = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bResult = false;
                exSend = ex;
            }


            return bResult;
        }

        private string _MAILFONTSTYLE = "style=\"font-family:Verdana;font-size:100%\"";

        public string GetMailBodyWithTemplate(string MailBody)
        {
            StringBuilder sbContent = new StringBuilder();
            if (!string.IsNullOrEmpty(MailTemplate))
                sbContent.Append(MailBody);
            else
            {
                sbContent.Append("No Template Available. Contact Administrator.");
                //string strTemplate = WebConfigurationManager.AppSettings[MailTemplate].ToString();
                //if (!string.IsNullOrEmpty(MailFontStyleTemplate))
                //    _MAILFONTSTYLE = WebConfigurationManager.AppSettings[MailFontStyleTemplate].ToString();
                //sbContent.Append(string.Format(strTemplate, _MAILFONTSTYLE, MailBody));                    
            }

            return sbContent.ToString();
        }

        #endregion Public Methods

        #endregion Methods

    }
}
