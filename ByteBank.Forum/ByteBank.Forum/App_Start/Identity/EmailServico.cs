using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace ByteBank.Forum.App_Start.Identity
{
    public class EmailServico : IIdentityMessageService
    {
        private readonly string EMAIL_ORIGEM = ConfigurationManager.AppSettings["email_servico:email_remetente"];
        private readonly string EMAIL_SENHA = ConfigurationManager.AppSettings["email_servico:email_senha"];
        public async Task SendAsync(IdentityMessage message)
        {
            
        }
    }
}