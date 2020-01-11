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
    public class PrinterController : Controller
    {
        private DbIntra db = new DbIntra();
        private PrinterDb PrinterDb = new PrinterDb();

        // parametro para cache
        public List<IN_PRINTER> ListPrinterPostBack
        {
            get
            {
                return (List<IN_PRINTER>)TempData["ListPrinter"];
            }
            set
            {
                TempData["ListPrinter"] = value;
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

        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var UltimaBuscaAtualizada = PrinterDb.BuscaGenericaPrinter(UltimoParametro.Empresa, UltimoParametro.UserName, UltimoParametro.ip, UltimoParametro.Departamento);
                ViewBag.emp = new SelectList(PrinterDb.ListaEmpresaIndexNull(), "id", "desc");
                return View("Index", UltimaBuscaAtualizada);
            }

            return RedirectToAction("Index");
        }

        // GET: Printer
        public ActionResult Index()
        {
            ViewBag.emp = new SelectList(PrinterDb.ListaEmpresaIndexNull(), "id", "desc");
            List<IN_PRINTER> list = new List<IN_PRINTER>();

            return View(list);
        }

        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            int Departamento = (f["Departamentos"] == "-1" ? 0 : Convert.ToInt32(f["departmentsDropdown"]));
            var busca = PrinterDb.BuscaGenericaPrinter(Convert.ToInt32(f["emp"]), f["departamentos"], f["Ip"] , Departamento);

            Parametro parametros = new Parametro
            {
                Empresa = Convert.ToInt32(f["emp"]),
                UserName = f["departamentos"],
                Departamento = Departamento ,
                ip = f["Ip"]

            };

            if (busca.Count == 0)
            {
                TempData["TypeErro"] = "alert alert-danger";
                TempData["erro"] = "A busca não retornou registros ";
            }

            // parametro para cache
            UltimoParametro = parametros;
            ListPrinterPostBack = busca;

            ViewBag.emp  = new SelectList(PrinterDb.ListaEmpresaIndexNull(), "id", "desc");

            return View("index", busca);
 
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


        // GET: Printer/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_PRINTER iN_PRINTER = db.IN_PRINTER.Find(id);
            if (iN_PRINTER == null)
            {
                return HttpNotFound();
            }
            return View(iN_PRINTER);
        }

        // GET: Printer/Create
        public ActionResult Create()
        {
            ViewBag.DEPTID = new SelectList(PrinterDb.BuscarDepartamento(1), "id", "desc");
            ViewBag.EMPID = new SelectList(db.CAD_EMP, "id", "descs");
            ViewBag.MODEL = new SelectList(db.CAD_ITEM.Where(x => x.typeID == 7), "id", "descs");
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs");
            return View();
        }

        // POST: Printer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SERIAL_NO,MODEL,EMPID,DEPTID,IP,APELIDO,info,situacao")] IN_PRINTER printer)
        {
           
                string[] resultado = PrinterDb.NovaImpressora(printer);
            // valida sstatus e mostrar para  o usuario
                ResultadoMensagem(resultado);

            return RedirectToAction("Create");
        }

        // GET: Printer/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_PRINTER printer = db.IN_PRINTER.Find(id);
            if (printer == null)
            {
                return HttpNotFound();
            }
            ViewBag.DEPTID = new SelectList(PrinterDb.BuscarDepartamento(printer.EMPID), "id", "desc", printer.DEPTID);
            ViewBag.EMPID = new SelectList(db.CAD_EMP, "id", "descs", printer.EMPID);
            ViewBag.MODEL = new SelectList(db.CAD_ITEM.Where(x => x.typeID == 7), "id", "descs", printer.MODEL);
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs", printer.situacao);

            return View(printer);
        }


        public JsonResult GetDepartamentos(int p)
        {
            // pega os usuarios que estão com os ramais nulos PRONTO PARA O USO E REFINANDO PELA A EMPRESA 
            return Json(from v in db.CAD_DEPT
                        where v.emp == p
                        select new
                        {
                            id = v.id,
                            descs = v.descs
                        }, JsonRequestBehavior.AllowGet);
        }


        // POST: Printer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SERIAL_NO,MODEL,EMPID,DEPTID,IP,APELIDO,info,situacao")] IN_PRINTER printer)
        {
            string[] resultado = PrinterDb.EditarPrinter(printer);
            ResultadoMensagem(resultado);

            return RedirectToAction("Edit/" + printer.ID);
            
        }

        // GET: Printer/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_PRINTER iN_PRINTER = db.IN_PRINTER.Find(id);
            if (iN_PRINTER == null)
            {
                return HttpNotFound();
            }
            return View(iN_PRINTER);
        }

        // POST: Printer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            string[] resultado = PrinterDb.DeletarPrint(id);
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
