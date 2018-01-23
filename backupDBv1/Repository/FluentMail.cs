using MailKit.Net.Smtp;
using MimeKit;
using System.Configuration;
using System.Threading.Tasks;

namespace backupDBv1.Repository
{
    public interface IMailFluent
    {
        IMailFluent From(string fromAddress);
        IMailFluent To(params string[] toAddresses);
        IMailFluent BCC(params string[] bccAddresses);
        IMailFluent CC(params string[] ccAddresses);
        IMailFluent Subject(string subject);
        IMailFluent Body(string body);
        Task SendAsync();
    }

    public class FluentMail : IMailFluent
    {
        private MimeMessage _mimeMessage = new MimeMessage();

        private FluentMail(string fromAddress)
        {
            _mimeMessage.From.Add(new MailboxAddress(fromAddress));
        }

        public static IMailFluent CreateEmailFrom(string fromAddress)
        {
            return new FluentMail(fromAddress);
        }

        public IMailFluent From(string fromAddress)
        {
            return this;
        }

        public IMailFluent To(params string[] toAddresses)
        {
            foreach (string toAddress in toAddresses)
            {
                _mimeMessage.To.Add(new MailboxAddress(toAddress));
            }
            return this;
        }

        public IMailFluent CC(params string[] ccAddresses)
        {
            foreach (string ccAddress in ccAddresses)
            {
                _mimeMessage.Cc.Add(new MailboxAddress(ccAddress));
            }
            return this;
        }

        public IMailFluent BCC(params string[] bccAddresses)
        {
            foreach (string bccAddress in bccAddresses)
            {
                _mimeMessage.Bcc.Add(new MailboxAddress(bccAddress));
            }
            return this;
        }

        public IMailFluent Subject(string subject)
        {
            _mimeMessage.Subject = subject;
            return this;
        }

        public IMailFluent Body(string body)
        {
            var bodys = new BodyBuilder();
            bodys.HtmlBody = body;
            _mimeMessage.Body = bodys.ToMessageBody();
            return this;
        }

        public async Task SendAsync()
        {

            //Smtp Server 
            string SmtpServer = "smtp.gmail.com";
            //Smtp Port Number 
            int SmtpPortNumber = 587;

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(SmtpServer, SmtpPortNumber, false);
                await client.AuthenticateAsync(ConfigurationSettings.AppSettings["email"], ConfigurationSettings.AppSettings["password"]);
                await client.SendAsync(_mimeMessage);
                await client.DisconnectAsync(true);
            }

        }

    }
}
