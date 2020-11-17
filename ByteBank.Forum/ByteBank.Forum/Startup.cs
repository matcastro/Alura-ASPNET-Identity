﻿using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;

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
                return new UserManager<UsuarioAplicacao>(userStore);
            });
        }
    }
}