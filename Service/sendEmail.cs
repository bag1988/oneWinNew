using MailKit.Net.Smtp;
using MimeKit;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class sendEmail
    {
        public async Task<bool> SendEmailAsync(string subjectMail, string textMessage, string emailFrom, string passFrom, string nameFrom, string emailTo, string nameTo, string fileMail = null/*, string smtpMail= "smtp.yandex.ru", int portMail= 587, bool tlsMail=true*/)
        {
            string smtpMail = "smtp.yandex.ru";
            int portMail = 587;
            var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress(nameFrom, emailFrom));
            emailMessage.To.Add(new MailboxAddress(nameTo, emailTo));
            emailMessage.Subject = subjectMail;
            var builder = new BodyBuilder();
            builder.TextBody = textMessage;

            if (fileMail != null)
            {
                builder.Attachments.Add(fileMail);
            }
            
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(smtpMail, portMail, MailKit.Security.SecureSocketOptions.StartTls);//SecureSocketOptions.StartTls
                await client.AuthenticateAsync(emailFrom, passFrom);
                await client.SendAsync(emailMessage);                
                await client.DisconnectAsync(true);
            }           
            return true;
        }
    }
}
