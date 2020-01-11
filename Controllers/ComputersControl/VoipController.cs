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
using IntraGriegHomolog.Models.ViewModels;

namespace IntraGriegHomolog.Controllers.ComputersControl
{
    [Authorize(Roles = "TI")]
    public class VoipController : Controller
    {
        private DbIntra db = new DbIntra();

        private VoipDb VoipDal = new VoipDb();

        public List<VoipColaboradorViewModel> ListVoipColabPostBack
        {
            get
            {
                return (List<VoipColaboradorViewModel>)TempData["ListVoipColab"];
            }
            set
            {
                TempData["ListVoipColab"] = value;
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

        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var UltimaBuscaAtualizada = VoipDal.BuscaGenericaVoip(UltimoParametro.Empresa, UltimoParametro.UserName, UltimoParametro.Ramal, UltimoParametro.Departamento);
                ViewBag.emp = new SelectList(VoipDal.ListaEmpresaIndexNull(), "id", "desc");
                return View("Index", UltimaBuscaAtualizada);
            }
            return RedirectToAction("Index");
        }

        public JsonResult getDepartment(int p)
        {
            return Json(db.CAD_DEPT.Where(x => x.emp == p).Select(x => new
            {
                DepartmentID = x.id,
                DepartmentName = x.descs
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Voip
        public ActionResult Index()
        {
            ViewBag.emp = new SelectList(VoipDal.ListaEmpresaIndexNull(), "id", "desc");
            List<VoipColaboradorViewModel> list = new List<VoipColaboradorViewModel>();

            return View(list);
        }


        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            int ramal = (f["Ramal"] == "" ? 0 : Convert.ToInt32(f["Ramal"]));
            int Departamentos = (f["departmentsDropdown"] == "-1" ? 0 : Convert.ToInt32(f["departmentsDropdown"]));
            var busca = VoipDal.BuscaGenericaVoip(Convert.ToInt32(f["emp"]), f["UserName"], ramal, Departamentos);

            Parametro parametros = new Parametro
            {
                Empresa = Convert.ToInt32(f["emp"]),
                UserName = f["UserName"],
                Ramal = ramal,
                Departamento = Departamentos
            };

            if (busca.Count == 0)
            {
                TempData["TypeErro"] = "alert alert-danger";
                TempData["erro"] = "A busca não retornou registros ";
            }

            // parametro para cache
            UltimoParametro = parametros;
            ListVoipColabPostBack = busca;

            ViewBag.emp = new SelectList(VoipDal.ListaEmpresaIndexNull(), "id", "desc");
            return View("index", busca);
        }

        // * * * * * * * * * * * * * * *  * * * *  *   *   *  *   *   *  *  *   *   *   *   * fim index 


        //  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *   *   *   * editar


        public JsonResult GetUser(int p)
        {
            // pega os usuarios que estão com os ramais nulos PRONTO PARA O USO E REFINANDO PELA A EMPRESA 
            return Json(from v in db.CAD_COLABORADOR
                        where v.ramal == null
                        where v.emp == p
                        select new
                        {
                            id = v.id,
                            username = v.username
                        },JsonRequestBehavior.AllowGet);
        }


        // GET: Voip/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_VOIP Voip =   db.IN_VOIP.Find(id);
            // Situacao dropdonw especifico 
            ViewBag.DropDownSituacao = new SelectList(db.cad_Situacao, "id", "descs", Voip.situacao); 

            if (Voip == null)
            {
                return HttpNotFound();
            }

            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs", Voip.emp);
            ViewBag.modelo = new SelectList(db.CAD_ITEM.Where(x => x.typeID ==10 ), "id", "descs", Voip.modelo);
            TempData["DropUserSemPC"] = new SelectList(VoipDal.DropUserVoipSemPC(Voip.ramal , Voip.emp), "id", "username"); // gera o dropdonw Usuarios  


            return View(Voip);
        }

        // POST: Voip/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection f, IN_VOIP VoipList)
        {
            // usuario selecioando no formulario 
            var  UserNameNovo = Convert.ToInt32(f["UserNameNovo"]);

            // EDITAR COLABORADOR ATREELADO AO RAMAL OU INFORMAÇOES DE RAMAIS 
            string[] resultado = VoipDal.EditarVoipColaborador(UserNameNovo, VoipList);

            ResultadoMensagem(resultado);
            return RedirectToAction("Edit/" + VoipList.ramal);

        }
 
        // GET: Voip/Create
        public ActionResult Create()
        {
            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs");
            ViewBag.modelo = new SelectList(db.CAD_ITEM.Where(x => x.typeID == 10), "id", "descs");
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ramal,passwd,ip,emp,INFO,situacao,modelo")] IN_VOIP iN_VOIP)
        {
            if (ModelState.IsValid)
            {
                db.IN_VOIP.Add(iN_VOIP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs", iN_VOIP.emp);
            ViewBag.modelo = new SelectList(db.CAD_ITEM, "id", "descs", iN_VOIP.modelo);
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs", iN_VOIP.situacao);
            return View(iN_VOIP);
        }

        public ActionResult Historico(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ListaHistorico = db.IN_HISTORY.Where(x => x.identificador.Equals(id)).ToList();

            return View(ListaHistorico);

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
