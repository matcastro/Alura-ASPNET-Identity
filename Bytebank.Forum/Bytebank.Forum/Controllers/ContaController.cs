using Bytebank.Forum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bytebank.Forum.Controllers
{
    public class ContaController : Controller
    {
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(ContaRegistrarViewModel modelo)
        {
            return View();
        }
    }
}