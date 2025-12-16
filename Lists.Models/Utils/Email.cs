using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.IO;


namespace Lists.Models.Utils
{
    public class Email
    {
        private SmtpClient client;
        public MailMessage Message;
        public int Id { get; set; }
        /// <summary>
        /// This is will contain an exception if one occured during 
        /// the attempt to transmit a message
        /// </summary>
        public Exception SendException { get; set; }

        public Email(List<string> to, string from, string body, bool isHtml, string subject)
        {
            try
            {
                this.BuildMailMessage(to, from, body, isHtml, subject);
            }
            catch (Exception e) 
            {
                SendException = e;
            }
        }

        public Email(string to, string from, string body, bool isHtml, string subject)
        {
            try
            {
                this.BuildMailMessage(
                    new List<string>() 
                    {
                        to
                    }, from, body, isHtml, subject);
            }
            catch (Exception e)
            {
                SendException = e;
            }
        }

        /// <summary>
        /// uses the default from email in appsettings and a default isHtml = true
        /// </summary>
        /// <param name="to"></param>
        /// <param name="body"></param>
        /// <param name="subject"></param>
        public Email(string to, string body, string subject)
        {
            try
            {
                this.BuildMailMessage(
                new List<string>() 
                {
                    to
                }, null, body, true, subject);
            }
            catch (Exception e)
            {
                SendException = e;
            }
        }

        /// <summary>
        /// Attempts to send email message
        /// </summary>
        /// <returns>true for success false for fail NOTE: if false check SendException property to get details</returns>
        public bool Send()
        {
            bool success = true;

            try
            {
                client.Send(Message);
            }
            catch(Exception e)
            {
                this.SendException = e;
                success = false;
            }

            return success;
        }

        private void BuildMailMessage(List<string> to, string from, string body, bool isHtml, string subject)
        {
            client = this.DefaultClient;
            Message = new MailMessage();

            if (from == null)
            {
                Message.From = new MailAddress(ConfigurationManager.AppSettings["DefaultSmtpFromEmail"]);
            }
            else
            {
                Message.From = new MailAddress(from);
            }

            foreach (string address in to)
            {
                Message.To.Add(new MailAddress(address));
            }

            Message.IsBodyHtml = isHtml;
            Message.Subject = subject;
            Message.Body = body;
        }

        /// <summary>
        /// Returns default SMTP client based on 
        /// AppSettings information
        /// </summary>
        private SmtpClient DefaultClient
        {
            get
            {
                return new SmtpClient()
                {
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUsername"], ConfigurationManager.AppSettings["SmtpPassword"]),
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = ConfigurationManager.AppSettings["SmtpServer"]
                };
            }
        }
    }
}
