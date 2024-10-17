using GestionSiembra.Models;
using GestionSiembra.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text;

namespace GestionSiembra.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IServicioEmail servicioEmail;
        
        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, ApplicationDbContext context, IServicioEmail servicioEmail)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.servicioEmail = servicioEmail;
            
        }


        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };
            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);
            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)//quitar el diferente para ve si funciona
            {
                return View(modelo);
            }
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else {
                ModelState.AddModelError(string.Empty, "nombre de usuario o password incorrecto");
                return View(modelo);

            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Usuarios");//

        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuarios = await context.Users.Select(u => new UsuarioViewModel
            {
                Email = u.Email
            }).ToListAsync();
            var modelo = new UsuariosListadoViewModel();
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;
            return View(modelo);
        }
        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();

            }
            else {
                await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);
                return RedirectToAction("Listado",
                   routeValues: new { mensaje = "Rol Asignado Correctamente a" + email });
            }
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();

            }
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);
            return RedirectToAction("Listado",
               routeValues: new { mensaje = "Rol Quitado Correctamente a" + email });
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvideMiPassword(string mensaje = "") {
            ViewBag.Mensaje = mensaje;
            return View();

        }
        //recuperacion por correo la contraseña
     [HttpPost]
      [AllowAnonymous]
        public async Task<IActionResult> OlvideMiPassword(OlvideMiPasswordViewModel modelo)
               {
            var mensaje = "El proceso a sido concluido. Si el Email enviado corresponde con uno de nuestros" +
                "usuarios en su bandeja de entrada encontra los pasos para recueprar su contraseña";
            ViewBag.Mensaje = mensaje;
            ModelState.Clear();
            var usuario = await userManager.FindByEmailAsync(modelo.Email);

            if (usuario is null)
            {
                return View();
            }//lo converti a url
            var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
            var codigoBase64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            var enlace = Url.Action("RecuperarPassword", "Usuarios", new { codigo = codigoBase64, },
            protocol: Request.Scheme);

            await servicioEmail.EnviarEmailCambioPassword(modelo.Email, enlace);
            return View();
        }
       

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecuperarPassword(string codigo = null)
        {
            if (codigo is null)
            {
                var mensaje = "Codigo no encontrado";
                return RedirectToAction("OlvideMiPassword", new { mensaje });

            }
            var modelo = new RecuperarPasswordViewModel();
            modelo.CodigoReseteo = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codigo));
            return View(modelo);
        }
        //aca no lo hace
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarPassword(RecuperarPasswordViewModel modelo)
        {
            var usuario = await userManager.FindByEmailAsync(modelo.Email);

            if (usuario is null)
            {
                return RedirectToAction("PasswordCambiado");
            }

            var resultados = await userManager.ResetPasswordAsync(usuario, modelo.CodigoReseteo, modelo.Password);
                return RedirectToAction("PasswordCambiado");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordCambiado()//le agregue la d
        {
            return View();
        }
        ////nuevo metodo
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ActualizarPassword(RecuperarPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo); // Retorna la vista con los mensajes de error.
            }

            // Busca el usuario por email
            var usuario = await userManager.FindByEmailAsync(modelo.Email);

            if (usuario == null)
            {
                // Si no se encuentra el usuario, muestra un mensaje de error o redirige a otra acción.
                ModelState.AddModelError(string.Empty, "El usuario no existe.");
                return View(modelo);
            }

            // Cambia la contraseña del usuario
            var resultado = await userManager.ChangePasswordAsync(usuario, modelo.CodigoReseteo, modelo.Password);

            if (resultado.Succeeded)
            {
                return RedirectToAction("PasswordCambiado");

            }

            // Si ocurre algún error, se agregan los errores al ModelState y se vuelve a la vista.
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(modelo);
        }


    }
} 

