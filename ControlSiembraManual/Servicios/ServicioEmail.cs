
using System.Net;
using System.Net.Mail;

namespace GestionSiembra.Servicios
{
    public interface IServicioEmail
    {
        Task EnviarEmailCambioPassword(string receptor, string enlace);
    }

    public class ServicioEmail : IServicioEmail
    {
        private readonly IConfiguration configuration;

        public ServicioEmail(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task EnviarEmailCambioPassword(string receptor, string enlace)
        {
            //var email = configuration.GetValue<string>("CONFIGURACIONES_HOTMAIL:EMAIL");
            //var password = configuration.GetValue<string>("CONFIGURACIONES_HOTMAIL:PASSWORD");
            //var host = configuration.GetValue<string>("CONFIGURACIONES_HOTMAIL:HOST"); // Clave corregida
            //var puerto = configuration.GetValue<int>("CONFIGURACIONES_HOTMAIL:PUERTO");
            var email = configuration.GetValue<string>("CONFIGURACIONES_GMAIL:EMAIL");
            var password = configuration.GetValue<string>("CONFIGURACIONES_GMAIL:PASSWORD");
            var host = configuration.GetValue<string>("CONFIGURACIONES_GMAIL:HOST");
            var puerto = configuration.GetValue<int>("CONFIGURACIONES_GMAIL:PUERTO");


            var cliente = new SmtpClient(host, puerto)
            {
                EnableSsl = true,
                UseDefaultCredentials = false, // Cambiado a false
                Credentials = new NetworkCredential(email, password)
            };

            var emisor = email;
            var tema = "Has olvidado tu contraseña";

            var contenidoHTML = $@"
                <p>Saludos,</p>
                <p>Este mensaje le llega porque ha solicitado un cambio de contraseña. Si usted no solicitó el cambio de contraseña, por favor ignore este mensaje.</p>
                <p>Para cambiar su contraseña, haga click en el siguiente enlace:</p>
                <a href=""{enlace}"">{enlace}</a>
                <p>Atentamente,</p>
                <p>Gremial Azucarero de Guatemala</p>
            ";

            var mensajeEmail = new MailMessage(emisor, receptor, tema, contenidoHTML)
            {
                IsBodyHtml = true // Establecer como HTML
            };

            await cliente.SendMailAsync(mensajeEmail);
        }
    }
}
