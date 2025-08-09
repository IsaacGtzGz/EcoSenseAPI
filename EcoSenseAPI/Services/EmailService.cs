using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EcoSenseAPI.Services
{
    public class EmailService
    {
        private readonly string smtpHost = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "isaacgtzgz@gmail.com"; // Cambia por tu correo
        private readonly string smtpPass = "AQUÍ_TU_CONTRASEÑA_O_APP_PASSWORD"; // Usa App Password

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            var mail = new MailMessage(smtpUser, to, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}
