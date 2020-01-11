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

namespace IntraGriegHomolog.Areas.Telefonia.Controllers 
{
    //Definindo grupo para o acesso 
    [Authorize(Roles = "Telefonia")]
    public class LinhaMovelController : Controller
    {
        private DbIntra db = new DbIntra();
        private LinhaMovelDb LinhaMovelDal = new LinhaMovelDb();

        // Propriedade para guardar o estado da ultima busca para melhor usuabilidade. 
        public List<IN_LINHA_MOVEL> ListaResultPostBack
        {
            get
            {
                return (List<IN_LINHA_MOVEL>)TempData["ListaLinha"];
            }
            set
            {
                TempData["ListaLinha"] = value;
            }
        }
        // Propriedade para guardar todos os parametros.
        public Parametro UltimoParametro
        {
            get
            {
                return (Parametro)TempData.Peek("ListParametros");
            }
            set
            {
                TempData["ListParametros"] = value;
            }
        }


        // GET: LinhaMovel
        public ActionResult Index()
        {

            ViewBag.emp = new SelectList(LinhaMovelDal.ListaEmpresaIndexNull(), "id", "desc");
            List<IN_LINHA_MOVEL> linha = new List<IN_LINHA_MOVEL>();
            return View(linha);
        }

        [HttpPost]
        public ActionResult Index(FormCollection f)
        {

            // grava parametros para consuta de cache
            Parametro p = new Parametro()
            {
                Empresa = Convert.ToInt32(f["emp"]),
                Numero = f["Numero"],
                ICCID = f["ICCID"],
            };

            var lista = LinhaMovelDal.BuscaLinhasMovel(p.Empresa, p.Numero, p.ICCID);
            // verifica se deu erro 
            if (lista.Count == 0)
            {
                TempData["typeMensagem"] = "alert alert-danger";
                TempData["Mensagem"] = "A busca não retornou registros ";
            }

            // cache para parametros e lista 
            UltimoParametro = p;
            ListaResultPostBack = lista;

            ViewBag.emp = new SelectList(LinhaMovelDal.ListaEmpresaIndexNull(), "id", "desc");
            return View(lista);
        }

        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var Ultimabusca = LinhaMovelDal.BuscaLinhasMovel(UltimoParametro.Empresa, UltimoParametro.Numero, UltimoParametro.ICCID);

                // traz todas as empresas e uma campo que mostra todas as empresas 
                ViewBag.emp = new SelectList(LinhaMovelDal.ListaEmpresaIndexNull(), "id", "desc");
                return View("Index", Ultimabusca);
            }
            return RedirectToAction("Index");
        }


        // GET: LinhaMovel/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_LINHA_MOVEL linha = db.IN_LINHA_MOVEL.Find(id);
            if (linha == null)
            {
                return HttpNotFound();
            }
            return View(linha);
        }

        // GET: LinhaMovel/Create
        public ActionResult Create()
        {
            ViewBag.EMPID = new SelectList(db.CAD_EMP, "id", "descs");
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs");
            return View();
        }

        // POST: LinhaMovel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EMPID,DESCS,ICCID,tipo_plano,custo_aparelho_plano,situacao")] IN_LINHA_MOVEL linhaMovel)
        {
            if (ModelState.IsValid)
            {
                string[] resultado = LinhaMovelDal.NovaLinha(linhaMovel);
                ResultadoMensagem(resultado);
                return RedirectToAction("Create");
            }
            return View(linhaMovel);
        }

        // GET: LinhaMovel/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_LINHA_MOVEL LinhaMovel = db.IN_LINHA_MOVEL.Find(id);
            if (LinhaMovel == null)
            {
                return HttpNotFound();
            }
            ViewBag.EMPID = new SelectList(db.CAD_EMP, "id", "descs", LinhaMovel.EMPID);
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs", LinhaMovel.situacao);
            return View(LinhaMovel);
        }

        // POST: LinhaMovel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EMPID,DESCS,ICCID,tipo_plano,custo_aparelho_plano,situacao")] IN_LINHA_MOVEL linhaMovel)
        {
            string[] resultado = LinhaMovelDal.EditarLinhaMovel(linhaMovel);

            ResultadoMensagem(resultado);

            return RedirectToAction("Edit/" + linhaMovel.ID);
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

        // GET: LinhaMovel/Delete/5
        public ActionResult Delete(long? id)
        {
            // verifica se foi passado id 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_LINHA_MOVEL linhaMovel = db.IN_LINHA_MOVEL.Find(id);
            // verificar a linha existe a  linha 
            if (linhaMovel == null)
            {
                return HttpNotFound();
            }
            return View(linhaMovel);
        }

        // POST: LinhaMovel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            string[] resultado = LinhaMovelDal.DeletarLinha(id);
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
