using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using ByteBank.Forum.App_Start.Identity;
using Microsoft.Owin.Security.Cookies;
using System;

[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]

namespace ByteBank.Forum
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<DbContext>(() =>
                new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<UsuarioAplicacao>>((options, contextoOwin) =>
            {
                var dbContext = contextoOwin.Get<DbContext>();
                return new UserStore<UsuarioAplicacao>(dbContext);
            });

            builder.CreatePerOwinContext<UserManager<UsuarioAplicacao>>((options, contextoOwin) =>
            {
                var userStore = contextoOwin.Get<IUserStore<UsuarioAplicacao>>();
                var userManager = new UserManager<UsuarioAplicacao>(userStore);
                var userValidator = new UserValidator<UsuarioAplicacao>(userManager)
                {
                    RequireUniqueEmail = true
                };

                userManager.UserValidator = userValidator;
                userManager.PasswordValidator = new SenhaValidador
                {
                    TamanhoRequerido = 6,
                    ObrigatorioCaracteresEspeciais = true,
                    ObrigatorioDigitos = true,
                    ObrigatorioLowerCase = true,
                    ObrigatorioUpperCase = true
                };

                userManager.EmailService = new EmailServico();

                userManager.UserTokenProvider = new DataProtectorTokenProvider<UsuarioAplicacao>(options.DataProtectionProvider.Create("ByteBank.Forum"));

                userManager.MaxFailedAccessAttemptsBeforeLockout = 3;

                userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);

                userManager.UserLockoutEnabledByDefault = true;

                return userManager;
            });

            builder.CreatePerOwinContext<SignInManager<UsuarioAplicacao, string>>((options, contextoOwin) =>
            {
                var userManager = contextoOwin.Get<UserManager<UsuarioAplicacao>>();
                var signInManager = new SignInManager<UsuarioAplicacao, string>(userManager, contextoOwin.Authentication);

                return signInManager;
            });

            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}