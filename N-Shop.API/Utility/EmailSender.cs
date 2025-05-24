using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace N_Shop.API.Utility;

public class EmailSender:IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("nailasaleh2004@gmail.com", "dpem futr skwa ptqr")
        };
        return client.SendMailAsync(
            new MailMessage(from:"nailasaleh2004@gmail.com",to: email, subject, message)
            {
                IsBodyHtml = true
            }
            );
    }
}