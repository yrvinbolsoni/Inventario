using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntraGriegHomolog.DAL;
using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;

namespace IntraGriegHomolog.Controllers.ComputersControl
{
    [Authorize(Roles = "TI")]
    public class ItemsController : Controller
    {
        private DbIntra db = new DbIntra();
        private ItensDb ItensDal = new ItensDb();


        public List<CAD_ITEM> ListItemPostBack
        {
            get
            {
                return (List<CAD_ITEM>)TempData["ListaItem"];
            }
            set
            {
                TempData["ListaItem"] = value;
            }

        }

        public Parametro UltimoParametro
        {

            get
            {
                return (Parametro)TempData.Peek("UltimoParametro");

            }
            set
            {
                TempData["UltimoParametro"] = value;
            }

        }


        private void ResultadoMensagem(string[] resultado)
        {
            // mensagem erro - estilos 
            if (resultado[0] == "sucesso")
            {
                TempData["Mensagem"] = resultado[1];
                TempData["typeMensagem"] = resultado[2];
            }
            else
            {
                TempData["Mensagem"] = resultado[1];
                TempData["typeMensagem"] = resultado[2];
            }
        }


        // GET: Items
        public ActionResult Index()
        {

            ViewBag.tipo = new SelectList(ItensDal.ListaTipos(), "id", "desc");
            List<CAD_ITEM> list = new List<CAD_ITEM>();
            return View(list);
        }

        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            
            var busca = ItensDal.BuscaItens(Convert.ToInt32(f["Tipo"]) , f["Descricao"] );

            Parametro parametros = new Parametro
            {
                Empresa = Convert.ToInt32(f["Tipo"]),
                UserName = f["Descricao"]

            };

            if (busca.Count == 0)
            {
                TempData["TypeErro"] = "alert alert-danger";
                TempData["erro"] = "A busca não retornou registros ";
            }


            // parametro para cache
            UltimoParametro = parametros;
            ListItemPostBack = busca;


            ViewBag.tipo = new SelectList(ItensDal.ListaTipos(), "id", "desc");
            return View("index", busca);

        }


        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var UltimaBuscaAtualizada = ItensDal.BuscaItens(UltimoParametro.Empresa, UltimoParametro.UserName);
                ViewBag.tipo = new SelectList(ItensDal.ListaTipos(), "id", "desc");
                return View("Index", UltimaBuscaAtualizada);
            }

            return RedirectToAction("Index");
        }


        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.typeID = new SelectList(db.CAD_ITEM_TYPE, "id", "descs");
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,typeID,descs")] CAD_ITEM item)
        {
            if (ModelState.IsValid)
            {

                string[] resultado = ItensDal.CriarItem(item);
                ResultadoMensagem(resultado);
           
                return RedirectToAction("Index");
            }

            ViewBag.typeID = new SelectList(db.CAD_ITEM_TYPE, "id", "descs", item.typeID);
            return View(item);
        }
         
        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAD_ITEM cAD_ITEM = db.CAD_ITEM.Find(id);
            if (cAD_ITEM == null)
            {
                return HttpNotFound();
            }
            return View(cAD_ITEM);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string[] resultado =  ItensDal.Deletar(id);
            ResultadoMensagem(resultado);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
