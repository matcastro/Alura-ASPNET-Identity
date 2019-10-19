using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytebank.Forum.Models
{
    public class UsuarioAplicacao : IdentityUser
    {
        public String FullName { get; set; }
    }
}