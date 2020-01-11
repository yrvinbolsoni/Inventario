using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
    public class DesktopController : Controller
    {
        private DbIntra db = new DbIntra();

        private DesktopDb DeskDal = new DesktopDb();


        // Propriedade para guardar o estado da ultima busca para melhor usuabilidade. 
        public List<DesktopColaboradorViewModel> ListDeskColabPostBack
        {
            get
            {
                return (List<DesktopColaboradorViewModel>)TempData["listaDeskColaborador"];
            }
            set
            {
                TempData["listaDeskColaborador"] = value;
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

        // voltar para  lista usando os mesmos parametro ou seja (LISTA ATUALIZADA)
        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var Ultimabusca = DeskDal.BuscaGenericaDeskTop(UltimoParametro.Empresa, UltimoParametro.Departamento, UltimoParametro.UserName, UltimoParametro.Identificador, UltimoParametro.KeyOffice, UltimoParametro.KeySo);
                ViewBag.emp = new SelectList(DeskDal.ListaEmpresaIndexNull(), "id", "desc");
                return View("Index", Ultimabusca);
            }
            return RedirectToAction("Index");
        }

        // retornas os dados dos departamentos baseado no dropdown
        public JsonResult getDepartment(int p)
        {
            return Json(db.CAD_DEPT.Where(x => x.emp == p).Select(x => new
            {
                DepartmentID = x.id,
                DepartmentName = x.descs
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Desktop
        public ActionResult Index()
        {
            if (ListDeskColabPostBack != null)
            {
                ViewBag.emp = new SelectList(DeskDal.ListaEmpresaIndexNull(), "id", "desc");
                return View(ListDeskColabPostBack);
            }
            try
            {
                List<DesktopColaboradorViewModel> Lista = new List<DesktopColaboradorViewModel>();
                ViewBag.emp = new SelectList(DeskDal.ListaEmpresaIndexNull(), "id", "desc");
                return View(Lista);
            }
            catch (Exception ex)
            {
                TempData["erro"] = ex.Message.ToString();
                throw;
            }


        }


        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            int Departamento = (f["departmentsDropdown"] == "-1" ? 0 : Convert.ToInt32(f["departmentsDropdown"]));

            var busca = DeskDal.BuscaGenericaDeskTop(Convert.ToInt32(f["emp"]), Departamento, f["UserName"], f["Identificador"], f["KeyOffice"], f["KeySo"]);

            Parametro Parametros = new Parametro()
            {
                Empresa = Convert.ToInt32(f["emp"]),
                Departamento = Departamento,
                UserName = f["UserName"],
                Identificador = f["Identificador"],
                KeyOffice = f["KeyOffice"],
                KeySo = f["KeySo"]
            };

            if (busca.Count == 0)
            {
                TempData["TypeErro"] = "alert alert-danger";
                TempData["erro"] = "A busca não retornou registros ";
            }

            ViewBag.emp = new SelectList(DeskDal.ListaEmpresaIndexNull(), "id", "desc");

            // cache para parametros e lista 
            UltimoParametro = Parametros;
            ListDeskColabPostBack = busca;

            return View(busca);
        }

        // GET: Desktop/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_DESKTOP desk =  db.IN_DESKTOP.Find(id);
            var dados = db.NFE.Where(x => x.identificador == desk.identificador).ToList();

            // verifica se tem dados 
            if (dados.Count > 0) 
            {
                ViewBag.Nota = dados; // se tiver dados colocar o nome das notas 
            }
            else
            {
                ViewBag.Nota = null;
            }

            if (desk == null)
            {
                return HttpNotFound();
            }

            // adiciona usuaio na view
            CAD_COLABORADOR colab = db.CAD_COLABORADOR.Where(x => x.desktop == id).FirstOrDefault();
            ViewBag.Colaborador = colab;

            return View(desk);
        }

      
          // ***********************************  CADASTROS   
        public ActionResult Create()
        {
            LoadFormCreate();
            return View();
        }

        private void LoadFormCreate()
        {
            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs");
            ViewBag.DropDownMonitor = new SelectList(DeskDal.DropdownGenerico(3, "SEM MONITOR"), "id", "desc");
            ViewBag.DropDownDesktop = new SelectList((DeskDal.DropModeloDesk()), "id", "descs");
            ViewBag.DropDownHD = new SelectList(DeskDal.DropdownGenerico(5, "SEM DISCO RIGIDO"), "id", "desc");
            ViewBag.DropDownMemoria = new SelectList(DeskDal.DropdownGenerico(4, "SEM MEMORIA RAM"), "id", "desc");
            ViewBag.sis_oper = new SelectList(db.CAD_ITEM.Where(x => x.typeID == 8), "id", "descs");
            ViewBag.pct_office = new SelectList(db.CAD_ITEM.Where(x => x.typeID == 9), "id", "descs");
            ViewBag.DropDownSituacao = new SelectList(db.cad_Situacao, "id", "descs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,identificador,ip,emp,dt_compra,k_so,k_office,situacao,info,monitor_client,modelo_client,disco_rigido,mem_ram,sis_oper,pct_office,sit")] IN_DESKTOP desk)
        {
            // configuração caso o usuario não tenha inserido nota neste caso o sistema ira inserir o desktop sem nota 
                if (ModelState.IsValid)
                {
                    string[] resultado = DeskDal.CreateDesktop(desk);
                    ResultadoMensagem(resultado);

                if (resultado[0]== "falha")
                    {
                        LoadFormCreate();
                        return View("Create", desk);
                    }
                    //busca o id que acabou de ser cadastro para continuar cadastrando
                    var IdCadastrado = db.IN_DESKTOP.Where(x => x.identificador == desk.identificador).Select(x => x.id).FirstOrDefault();
                    return RedirectToAction("CreateSecond/" + IdCadastrado);
                }
                else
                {
                    LoadFormCreate();
                    return View();
                }
        }

        // SEGUNDO CADASTRO 
        public ActionResult CreateSecond(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // busca do cadastro
            IN_DESKTOP desk = db.IN_DESKTOP.Find(id);
            var dados = db.NFE.Where(x => x.identificador == desk.identificador).ToList();
           
            // verifica se tem dados 
            if (dados.Count > 0) 
                // se tiver dados colocar o nome das notas 
                ViewBag.Nota = dados; 
            else
                ViewBag.Nota = null;

            return View(desk);
        }

        // ADD  E REMOVER NOTA DO SEGUNDO CADASTRO CADASTRO 
        [HttpPost]
        public ActionResult AdicionarCadastralNota(HttpPostedFileBase postedFile, FormCollection f)
        {
            // dados formulario 
            int Id = Convert.ToInt32(f["DeskId"]);
            var ApelidoAnexo = f["ApelidoNota"];

            // adiciona nota 
            string[] resultado = DeskDal.AdicionarNota(postedFile, Id, ApelidoAnexo);
            // valida resultado e mostra mesagem para usuario
            ResultadoMensagem(resultado);
            return RedirectToAction("CreateSecond/" + Id);
        }

        public ActionResult RemoverAnexosCadastral(int id, int IdDesk)
        {
            // remove nota
            string[] mensagem = DeskDal.RemoverNota(id);
            // valida resultado e mostra mesagem para usuario
            ResultadoMensagem(mensagem);
            return RedirectToAction("CreateSecond/" + IdDesk);

        }

        // ***********************************  FIM  CADASTROS  


        // GET: Desktop/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IN_DESKTOP desktop = db.IN_DESKTOP.Find(id);
            if (desktop == null)
            {
                return HttpNotFound();
            }

            var dados = db.NFE.Where(x => x.identificador == desktop.identificador).ToList();

            // verifica se tem dados 
            if (dados.Count > 0) 
            {
                ViewBag.Nota = dados; // se tiver dados colocar o nome das notas 
            }
            else
            {
                ViewBag.Nota = null;
            }

         

            // pega o usuario atrelado e os que estao disponivels 
            TempData["DropUser"] = new SelectList(DeskDal.DropUser((int)id, (int)desktop.emp), "id", "username"); // gera o dropdonw Usuarios  
            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs", desktop.emp);
            ViewBag.DropDownMonitor = new SelectList(DeskDal.DropdownGenerico(3,"SEM MONITOR"), "id", "desc", desktop.monitor_client);
            ViewBag.DropDownDesktop = new SelectList((DeskDal.DropModeloDesk()), "id", "descs", desktop.modelo_client);
            ViewBag.DropDownHD = new SelectList(DeskDal.DropdownGenerico(5,"SEM DISCO RIGIDO"), "id", "desc", desktop.disco_rigido);
            ViewBag.DropDownMemoria = new SelectList(DeskDal.DropdownGenerico(4, "SEM MEMORIA RAM"), "id", "desc", desktop.mem_ram);
            ViewBag.sis_oper = new SelectList(DeskDal.DropdownGenerico(8, "SEM SISTEMA OPERACIONAL"), "id", "desc", desktop.sis_oper);
            ViewBag.pct_office = new SelectList(DeskDal.DropdownGenerico(9 , "SEM PACOTE OFFICE"), "id", "desc", desktop.pct_office);
            ViewBag.DropDownSituacao = new SelectList(db.cad_Situacao, "id", "descs", desktop.sit);

            return View(desktop);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection f, IN_DESKTOP desk)
        {

            // convertendo dados do formulario 
            var UserNameNovo = Convert.ToInt32(f["UserNameNovo"]);

            // verficar se posso retirar 
            var removerAnexo = Convert.ToBoolean(f["RemoverAnexo"]);

            // editar colaborador 
            string[] resultado = DeskDal.EditarDeskColaborador(desk, UserNameNovo);

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

            return RedirectToAction("Edit/" + desk.id);
        }

        // notas fiscais 
        [HttpPost]
        public ActionResult AdicionarNota(HttpPostedFileBase postedFile, FormCollection f)
        {
            // dados formulario 
            int Id = Convert.ToInt32(f["DeskId"]);
            var ApelidoAnexo = f["ApelidoNota"];

            // adiciona nota 
            string[] resultado = DeskDal.AdicionarNota(postedFile, Id, ApelidoAnexo);

            ResultadoMensagem(resultado);
            return RedirectToAction("Edit/" + Id);
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

        public ActionResult RemoverAnexos(int id, int IdDesk)
        {
            string[] mensagem = DeskDal.RemoverNota(id);
            ResultadoMensagem(mensagem);

            return RedirectToAction("Edit/" + IdDesk);
        }


        public void MostarArquivo(int id)
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

        // fim notas fiscais 
        public ActionResult HistoricoDesk(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var listaHstorico = db.IN_HISTORY.Where(x => x.identificador.Equals(id)).ToList();

            return View(listaHstorico);
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
