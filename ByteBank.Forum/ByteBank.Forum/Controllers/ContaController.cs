using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        private UserManager<UsuarioAplicacao> _userManager;
        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    return contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        private SignInManager<UsuarioAplicacao, string> _signInManager;
        public SignInManager<UsuarioAplicacao, string> SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    return contextOwin.GetUserManager<SignInManager<UsuarioAplicacao, string>>();
                }
                return _signInManager;
            }
            set
            {
                _signInManager = value;
            }
        }
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var novoUsuario = new UsuarioAplicacao();
                novoUsuario.Email = modelo.Email;
                novoUsuario.UserName = modelo.UserName;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;

                var user = await UserManager.FindByEmailAsync(modelo.Email);

                if (user != null)
                {
                    Console.WriteLine($"E-mail {modelo.Email} já cadastrado. Enviando usuário para tela inicial.");
                    return View("AguardandoConfirmacao");
                }

                var result = await UserManager.CreateAsync(novoUsuario, modelo.Senha);
                if (result.Succeeded)
                {
                    await EnviarEmailDeConfirmacao(novoUsuario);
                    return View("AguardandoConfirmacao");
                }
                else
                {
                    AdicionaErros(result.Errors);
                }


            }
            return View(modelo);
        }

        public async Task<ActionResult> ConfirmacaoEmail(string usuarioId, string token)
        {
            if (usuarioId == null || token == null)
            {
                return View("Error");
            }

            var resultado = await UserManager.ConfirmEmailAsync(usuarioId, token);

            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");

        }

        private async Task EnviarEmailDeConfirmacao(UsuarioAplicacao usuario)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(usuario.Id);

            var linkDeCallback = Url.Action(
                "ConfirmacaoEmail",
                "Conta",
                new { usuarioId = usuario.Id, token = token },
                Request.Url.Scheme
                );

            await UserManager.SendEmailAsync(usuario.Id,
                "Fórum ByteBank - Confirmação de Email",
                $"Bem vindo ao fórum ByteBank, clique aqui {linkDeCallback} para confirmar seu e-mail");
        }

        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(ContaLoginViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var usuario = await UserManager.FindByEmailAsync(modelo.Email);
                if (usuario == null)
                {
                    return SenhaOuUsuarioInvalidos();
                }

                var signInResultado = await SignInManager.PasswordSignInAsync(usuario.UserName, modelo.Senha, modelo.ContinuarLogado, false);

                switch (signInResultado)
                {
                    case SignInStatus.Success:
                        return RedirectToAction("Index", "Home");
                    default:
                        return SenhaOuUsuarioInvalidos();
                }

            }
            return View(modelo);
        }

        private ActionResult SenhaOuUsuarioInvalidos()
        {
            ModelState.AddModelError("", "Credenciais inválidas!");
            return View("Login");
        }

        private void AdicionaErros(IEnumerable<string> errors)
        {
            foreach (var erro in errors)
            {
                ModelState.AddModelError("", erro);
            }
        }
    }
}