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

namespace IntraGriegHomolog.Areas.Telefonia.Controllers
{
    [Authorize(Roles = "Telefonia")]
    public class SmartPhoneController : Controller
    {
        private DbIntra db = new DbIntra();
        private SmartPhoneDb SmartPhoneDal = new SmartPhoneDb();

        // Propriedade para guardar o estado da ultima busca para melhor usuabilidade. 
        public List<SmartphoneColaborador> SmartFoneColabPostBack
        {
            get
            {
                return (List<SmartphoneColaborador>)TempData["ListaSmartFone"];
            }
            set
            {
                TempData["ListaSmartFone"] = value;
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

        public ActionResult teste()
        {
            return View();
        }

        public ActionResult CadastroItem()
        {
            ViewBag.typeID = new SelectList(db.CAD_ITEM_TYPE.Where(x => x.id == 6), "id", "descs");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CadastroItem([Bind(Include = "id,typeID,descs")] CAD_ITEM item)
        {
            if (ModelState.IsValid)
            {
                db.CAD_ITEM.Add(item);
                db.SaveChanges();

                TempData["TypeErro"] = "alert alert-success";
                TempData["erro"] = "Adicionado com sucesso ";
            }

            ViewBag.typeID = new SelectList(db.CAD_ITEM_TYPE.Where(x => x.id == 6), "id", "descs");

            return View(item);
        }

        public ActionResult HistoricoSmartFone(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var listaHstorico = db.IN_HISTORY.Where(x => x.identificador.Equals(id)).ToList();

            return View(listaHstorico);
        }

        // GET: SmartPhone

        // retornas os dados dos departamentos baseado no dropdown
        public JsonResult getDepartment(int p)
        {
            return Json(db.CAD_DEPT.Where(x => x.emp == p).Select(x => new
            {
                DepartmentID = x.id,
                DepartmentName = x.descs
            }).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult getUserEmpresa(int emp)
        {
            return Json(SmartPhoneDal.DropUserDisponivelPorEmp(emp).Select(x => new
            {
                UsuarioId = x.id,
                UsuarioName = x.username
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getLinhaPorEmp(int emp)
        {
            return Json(SmartPhoneDal.LinhasDisponiveis(null, emp).Select(x => new
            {
                idLinhaMovel = x.idLinhaMovel,
                desc = x.desc
            }).ToList(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index()
        {
            List<SmartphoneColaborador> Lista = new List<SmartphoneColaborador>();
            ViewBag.emp = new SelectList(SmartPhoneDal.ListaEmpresaIndexNull(), "id", "desc");

            return View(Lista);
        }

        [HttpPost]
        public ActionResult Index(FormCollection f)
        {

            // grava parametros para consuta de cache
            Parametro p = new Parametro()
            {
                Empresa = Convert.ToInt32(f["emp"]),
                Departamento = (f["departmentsDropdown"] == "-1" ? 0 : Convert.ToInt32(f["departmentsDropdown"])),
                UserName = f["Username"],
                Numero = f["Numero"],
                ModeloCelular = f["Modelo"],
                Imei = f["Imei"]
            };

            var lista = SmartPhoneDal.BuscarSmartPhone(p.UserName, p.Empresa, p.Departamento, p.Numero, p.ModeloCelular, p.Imei);
            // verifica se deu erro 
            if (lista.Count == 0)
            {
                TempData["TypeErro"] = "alert alert-danger";
                TempData["erro"] = "A busca não retornou registros ";
            }

            // cache para parametros e lista 
            UltimoParametro = p;
            SmartFoneColabPostBack = lista;

            ViewBag.emp = new SelectList(SmartPhoneDal.ListaEmpresaIndexNull(), "id", "desc");
            return View(lista);
        }

        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var Ultimabusca = SmartPhoneDal.BuscarSmartPhone(UltimoParametro.UserName, UltimoParametro.Empresa, UltimoParametro.Departamento, UltimoParametro.Numero, UltimoParametro.ModeloCelular, UltimoParametro.Imei);

                // traz todas as empresas e uma campo que mostra todas as empresas 
                ViewBag.emp = new SelectList(SmartPhoneDal.ListaEmpresaIndexNull(), "id", "desc");
                return View("Index", Ultimabusca);
            }
            return RedirectToAction("Index");
        }


        // GET: SmartPhone/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_SMARTPHONE smartPhone = db.IN_SMARTPHONE.Find(id);
            var notas = db.NFE.Where(x => x.identificador.Equals(smartPhone.serial_number)).ToList();
            if (smartPhone == null)
            {
                return HttpNotFound();
            }

            // verifica se tem dados 
            if (!String.IsNullOrEmpty(smartPhone.TERM_NAME))
            {
                ViewBag.Termo = smartPhone; // se tiver dados colocar o nome das notas 
            }
            if (notas != null)
            {
                ViewBag.Notas = notas; // se tiver dados colocar as   notas 
            }

            return View(smartPhone);
        }

        // GET: SmartPhone/Create
        public ActionResult Create()
        {
            ViewBag.EMP = new SelectList(db.CAD_EMP, "id", "descs");
            ViewBag.model = new SelectList(db.CAD_ITEM.Where(x => x.typeID == 6), "id", "descs");
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs");
            ViewBag.linha_movel = new SelectList(SmartPhoneDal.LinhasDisponiveis(null, 1), "idLinhaMovel", "desc");
            ViewBag.linha_movel2 = new SelectList(SmartPhoneDal.LinhasDisponiveis(null, 1), "idLinhaMovel", "desc");


            return View();
        }

        // POST: SmartPhone/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,serial_number,model,imei,imei2,DATA_COMPRA,EMP,TERM_NAME,TERM_TYPE,TERM_FILE,linha_movel,linha_movel2,info,situacao")] IN_SMARTPHONE smartFone)
        {
            string[] resultado = SmartPhoneDal.NovoSmartFone(smartFone);
            ResultadoMensagem(resultado);

            return RedirectToAction("Create");
        }



        public ActionResult RemoverTermo(int id)
        {
            string[] mensagem = SmartPhoneDal.RemoverTermo(id);
            ResultadoMensagem(mensagem);

            return RedirectToAction("Edit/" + id);
        }

        public ActionResult RemoverNota(int id, int IdSmart)
        {
            string[] mensagem = SmartPhoneDal.RemoverNota(id);
            ResultadoMensagem(mensagem);

            return RedirectToAction("Edit/" + IdSmart);
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

        // Adicionar Termo 
        [HttpPost]
        public ActionResult AdicionarNotaTermo(HttpPostedFileBase postedFileSmartFone, FormCollection f, HttpPostedFileBase postedFileNota)
        {
            int Id = Convert.ToInt32(f["SmartId"]);

            // verifica se foi inserido termo adicione
            if (postedFileSmartFone != null)
            {
                // dados formulario 
                var ApelidoAnexo = f["ApelidoTermo"];

                // adiciona nota 
                string[] resultado = SmartPhoneDal.AdicionarTermo(postedFileSmartFone, Id, ApelidoAnexo);
                ResultadoMensagem(resultado);

            }

            // verifica se foi inserido nota fiscal  , adicione
            if (postedFileNota != null)
            {
                var ApelidoNota = f["ApelidoNota"];

                string[] resultado = SmartPhoneDal.AdicionarNotaFiscal(postedFileNota, Id, ApelidoNota);
                ResultadoMensagem(resultado);

            }

            return RedirectToAction("Edit/" + Id);
        }


        // GET: SmartPhone/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var smartPhone = db.IN_SMARTPHONE.Find(id);
            var notas = db.NFE.Where(x => x.identificador.Equals(smartPhone.serial_number)).ToList();
            if (smartPhone == null)
            {
                return HttpNotFound();
            }

            // verifica se tem dados 
            if (!String.IsNullOrEmpty(smartPhone.TERM_NAME))
            {
                ViewBag.Termo = smartPhone; // se tiver dados colocar o nome das notas 
            }
            if (notas != null)
            {
                ViewBag.Notas = notas; // se tiver dados colocar as   notas 
            }

            TempData["DropUser"] = new SelectList(SmartPhoneDal.DropUserDisponivel((int)id, smartPhone.EMP), "id", "username"); // gera o dropdonw Usuarios  


            ViewBag.EMP = new SelectList(db.CAD_EMP, "id", "descs", smartPhone.EMP);
            ViewBag.model = new SelectList(db.CAD_ITEM, "id", "descs", smartPhone.model);
            ViewBag.situacao = new SelectList(db.cad_Situacao, "id", "descs", smartPhone.situacao);
            ViewBag.linha_movel = new SelectList(SmartPhoneDal.LinhasDisponiveis(smartPhone.linha_movel, smartPhone.EMP), "idLinhaMovel", "desc");
            ViewBag.linha_movel2 = new SelectList(SmartPhoneDal.LinhasDisponiveis(smartPhone.linha_movel2, smartPhone.EMP), "idLinhaMovel", "desc");


            return View(smartPhone);
        }


        // POST: SmartPhone/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IN_SMARTPHONE SmartFone, FormCollection f)
        {
            var UserName = Convert.ToInt32(f["UserName"]);
            string[] resultado = SmartPhoneDal.EditarSmartfoneColab(SmartFone, UserName);

            ResultadoMensagem(resultado);
            return RedirectToAction("Edit/" + SmartFone.id);

        }

        // GET: SmartPhone/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_SMARTPHONE iN_SMARTPHONE = db.IN_SMARTPHONE.Find(id);
            if (iN_SMARTPHONE == null)
            {
                return HttpNotFound();
            }
            return View(iN_SMARTPHONE);
        }

        public void MostrarTermo(int id)
        {
            var dados = db.IN_SMARTPHONE.Where(x => x.id == id).FirstOrDefault(); // pesquisa valor binario 

            Response.Clear();  // limpar reponses anteriores 
            Response.ContentType = dados.TERM_TYPE; // extenção do arquivo 
            Response.AppendHeader("Content-Disposition", "inline; filename =" + dados.TERM_NAME + "");  // nome que vai aparecer para donwload
            Response.BufferOutput = true;
            Response.AddHeader("Content-Length", dados.TERM_FILE.Length.ToString());
            Response.BinaryWrite(dados.TERM_FILE); // convertando aquivo
            Response.End();
        }

        public void MostarNota(int id)
        {
            var dados = db.NFE.Where(x => x.id == id).FirstOrDefault(); // pesquisa valor binario 

            Response.Clear();  // limpar reponses anteriores 
            Response.ContentType = dados.n_type; // extenção do arquivo 
            Response.AppendHeader("Content-Disposition", "inline; filename =" + dados.n_name + "");  // nome que vai aparecer para donwload
            Response.BufferOutput = true;
            Response.AddHeader("Content-Length", dados.n_file.Length.ToString());
            Response.BinaryWrite(dados.n_file); // convertando aquivo
            Response.End();
        }



        // POST: SmartPhone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            IN_SMARTPHONE iN_SMARTPHONE = db.IN_SMARTPHONE.Find(id);
            db.IN_SMARTPHONE.Remove(iN_SMARTPHONE);
            db.SaveChanges();
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
