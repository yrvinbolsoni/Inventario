using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntraGriegHomolog.Areas.Telefonia.Controllers
{
    public class InicioController : Controller
    {
        // GET: Telefonia/Inicio
        [Authorize(Roles = "Telefonia")]
        public ActionResult Index()
        {
            return View();
        }
    }
}