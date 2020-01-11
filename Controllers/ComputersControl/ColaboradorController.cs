using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using IntraGriegHomolog.Models;
using IntraGriegHomolog.DAL;
using IntraGriegHomolog.Models.DropDown;

namespace IntraGriegHomolog.Controllers.ComputersControl
{
    [Authorize(Roles = "TI")]
    public class ColaboradorController : Controller
    {
        private DbIntra db = new DbIntra();
        private ColaboradorDb ColaboradorDb = new ColaboradorDb();

        // Propriedade para guardar o estado da ultima busca para melhor usuabilidade. 
        public List<CAD_COLABORADOR> ListaResultPostBack
        {
            get
            {
                return (List<CAD_COLABORADOR>)TempData["ListaColaborador"];
            }
            set
            {
                TempData["ListaColaborador"] = value;
            }
        }
        // Propriedade para guardar todos os parametros.
        public Parametro UltimoParametro
        {
            get
            {
                return (Parametro)TempData.Peek("ListParametrosColab");
            }
            set
            {
                TempData["ListParametrosColab"] = value;
            }
        }

        // voltar para  lista usando os mesmos parametro ou seja (LISTA ATUALIZADA)
        public ActionResult VoltarList()
        {
            if (UltimoParametro != null)
            {
                var Ultimabusca = ColaboradorDb.BuscaGenericaColab(UltimoParametro.Empresa, UltimoParametro.Departamento, UltimoParametro.UserName);
                ViewBag.emp = new SelectList(ColaboradorDb.ListaEmpresaIndexNull(), "id", "desc");
                return View("Index", Ultimabusca);
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


        public JsonResult GetRamalPorEmpresa(int p)
        {

            return Json(from v in db.IN_VOIP
                        where v.emp == p
                        where !(from c in db.CAD_COLABORADOR select c.ramal)
                        .Contains(v.ramal)
                        select new
                        {
                            Ramal = v.ramal
                        }, JsonRequestBehavior.AllowGet);


        }


        public JsonResult GetDeskPorEmp(int p)
        {
            return Json(from desk in db.IN_DESKTOP
                        where desk.emp == p
                        where !(from c in db.CAD_COLABORADOR select c.desktop)
                        .Contains(desk.id)
                        select new
                        {
                            Id = desk.id,
                            Identificador = desk.identificador
                        }, JsonRequestBehavior.AllowGet);

        }


        // GET: Colaborador
        public ActionResult Index()
        {
            if (ListaResultPostBack != null)
            {
                ViewBag.emp = new SelectList(ColaboradorDb.ListaEmpresaIndexNull(), "id", "desc");
                return View(ListaResultPostBack);
            }
            try
            {
                List<CAD_COLABORADOR> Lista = new List<CAD_COLABORADOR>();
                ViewBag.emp = new SelectList(ColaboradorDb.ListaEmpresaIndexNull(), "id", "desc");
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

            var busca = ColaboradorDb.BuscaGenericaColab(Convert.ToInt32(f["emp"]), Departamento, f["UserName"]);

            Parametro Parametros = new Parametro()
            {
                Empresa = Convert.ToInt32(f["emp"]),
                Departamento = Departamento,
                UserName = f["UserName"],
                Identificador = f["Identificador"]
            };

            if (busca.Count == 0)
            {
                TempData["TypeErro"] = "alert alert-danger";
                TempData["erro"] = "A busca não retornou registros ";
            }
            ViewBag.emp = new SelectList(ColaboradorDb.ListaEmpresaIndexNull(), "id", "desc");

            // cache para parametros e lista 
            UltimoParametro = Parametros;
            ListaResultPostBack = busca;

            return View(busca);
        }



        // GET: Colaborador/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAD_COLABORADOR colab = await db.CAD_COLABORADOR.FindAsync(id);
            if (colab == null)
            {
                return HttpNotFound();
            }
            // busca colaborador com smartfone
            var SmartFone = (from a in db.CAD_COLABORADOR
                             from b in db.IN_SMARTPHONE
                             from c in db.IN_LINHA_MOVEL
                             from d in db.CAD_ITEM
                             where a.smartphone == b.id
                             where b.linha_movel == c.ID
                             where b.model == d.id
                             where a.id == id
                             select new
                             {
                                 SeriaNumber = b.serial_number,
                                 Number = c.DESCS,
                                 Modelo = d.descs,
                             }).FirstOrDefault();

            ViewBag.SmartFone = null;

            if (SmartFone != null)
            {
                ViewBag.Modelo = SmartFone.Modelo;
                ViewBag.Serial = SmartFone.SeriaNumber;
                ViewBag.Numero = SmartFone.Number;


            }

            return View(colab);
        }

        // GET: Colaborador/Create
        public ActionResult Create()
        {


            ViewBag.DropDownSituacao = new SelectList(ColaboradorDb.StatusColab(), "valor", "desc");
            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs");
            ViewBag.dept = new SelectList(db.CAD_DEPT, "id", "descs");
            ViewBag.tipo_u = new SelectList(db.CAD_TIPO_USER, "id", "descs");

            return View();
        }

        // POST:  Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( CAD_COLABORADOR colab)
        {

            string[] resultado = ColaboradorDb.CriarUser(colab);

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



            ViewBag.DropDownSituacao = new SelectList(ColaboradorDb.StatusColab(), "valor", "desc");
            ViewBag.dept = new SelectList(db.CAD_DEPT, "id", "descs", colab.dept);
            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs", colab.emp);
            ViewBag.tipo_u = new SelectList(db.CAD_TIPO_USER, "id", "descs", colab.tipo_u);
            ViewBag.desktop = new SelectList(db.IN_DESKTOP, "id", "identificador", colab.desktop);
            ViewBag.ramal = new SelectList(db.IN_VOIP, "ramal", "ramal", "preencha");
            return View(colab);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            // verifica se o ID foi passado corretamente 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // verifica se o ID existe 
            CAD_COLABORADOR colab = await db.CAD_COLABORADOR.FindAsync(id);
            if (colab == null)
            {
                return HttpNotFound();
            }
            // carregar os DropDown fazendo todas as buscar e ordenação
            LoadDropDown(colab);

            return View(colab);
        }

        private void LoadDropDown(CAD_COLABORADOR colab)
        {

            // preenchendo DropDown
            ViewBag.dept = new SelectList(db.CAD_DEPT.Where(x => x.emp == (colab.emp == null ? 1 : colab.emp)), "id", "descs", colab.dept);
            ViewBag.emp = new SelectList(db.CAD_EMP, "id", "descs", colab.emp);
            ViewBag.tipo_u = new SelectList(db.CAD_TIPO_USER, "id", "descs", colab.tipo_u);

            // busca o desktop do colaborador disponivel da mesma empresa [BuscaDeskDiponivel]
            ViewBag.desktop = new SelectList(BuscaDeskDiponivel(colab), "id", "identificador", colab.desktop);
            ViewBag.DropDownSituacao = new SelectList(ColaboradorDb.StatusColab(), "valor", "desc",colab.STATUS);
            // busca ramal disponivel da mesma empresa do usuario [BuscaRamalDisponivel] 
            ViewBag.ramal = new SelectList(BuscaRamalDisponivel(colab), "ramal", "desc", colab.ramal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( CAD_COLABORADOR colab)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(colab).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    switch (ex.HResult)
                    {
                        case -2146233087:
                            string[] msg = ex.InnerException.InnerException.Message.ToString().Split('*');
                            if (msg.Count() > 1)
                                TempData["erro"] = "Erro " + msg[1];

                            else
                                TempData["erro"] = "Erro " + msg[0];

                            break;

                        default:
                            TempData["erro"] = "Erro " + ex.Message;
                            break;

                    }
                    return RedirectToAction("edit/" + colab.id);

                }


            }
            TempData["erro"] = "Algo deu errado";
            return RedirectToAction("edit/" + colab.id);
        }


        private List<dropDesk> BuscaDeskDiponivel(CAD_COLABORADOR colab)
        {
            string NomeDescricao = "Sem Desktop";

            // pesquisa para buscar os desktop que esta disponivel para uso da empresa cadastrada
            List<dropDesk> ListaDesk = (from desk in db.IN_DESKTOP
                                        where desk.emp == (colab.emp == null ? 1 : colab.emp)
                                        where !(from c in db.CAD_COLABORADOR select c.desktop)
                                        .Contains(desk.id)
                                        select new
                                        {
                                            Id = desk.id,
                                            Identificador = desk.identificador
                                        }).ToList()
                                          .Select(x => new dropDesk
                                          {
                                              id = x.Id,
                                              identificador = x.Identificador
                                          }).ToList();

            // se o colaborador tiver um desktop essa condição irar deixar ele como primeiro 
            if (colab.desktop != null)
            {
                // inserindo colaborador que esta sendo utilizado 
                ListaDesk.Add(new dropDesk
                {
                    id = Convert.ToInt32(colab.desktop),
                    identificador = colab.IN_DESKTOP.identificador,
                });

                // inserindo o valor nulo para poder retirar o desktop do mesmo colaborador 
                ListaDesk.Add(new dropDesk
                {
                    id = null,
                    identificador = NomeDescricao,
                });

            }
            else
            {
                // inserindo o valor nulo para poder retirar o desktop do mesmo colaborador 
                ListaDesk.Add(new dropDesk
                {
                    id = null,
                    identificador = NomeDescricao,
                });

                // ordena para mostrar o valor nulo como primeiro na lista 
                ListaDesk = ListaDesk.OrderByDescending(x => x.id == null)
                                     .ThenBy(x => x.id == null)
                                     .ToList();
            }

            return ListaDesk;
        }

        private List<dropRamal> BuscaRamalDisponivel(CAD_COLABORADOR colab)
        {
            string NomeDescricao = "Sem ramal";

            // buscar para procurar todos os ramais deponivels por empresa que esteja disponivel para sert utilizado 
            // o valor 1 sera usado com default para ser usado quando o usuario nao usuario tiver uma empresa 
            List<dropRamal> ListaDeRamal = (from v in db.IN_VOIP
                                            where v.emp == (colab.emp == null ? 1 : colab.emp)
                                            where !(from c in db.CAD_COLABORADOR select c.ramal)
                                            .Contains(v.ramal)
                                            select new
                                            {
                                                Vramal = v.ramal
                                            }).ToList()
                          .Select(x => new dropRamal
                          {
                              desc = Convert.ToString(x.Vramal),
                              ramal = x.Vramal
                          }).ToList();

            // se o colaborador tiver um ramal essa condição irar deixar ele como primeiro 
            if (colab.ramal != null)
            {
                // inserer o ramal que o usuario já possue 
                ListaDeRamal.Add(new dropRamal
                {
                    desc = Convert.ToString(colab.ramal),
                    ramal = (int)colab.ramal
                });

                // inserir um valor nulo para poder retirar o ramal do mesmo usuario.
                ListaDeRamal.Add(new dropRamal
                {
                    desc = NomeDescricao,
                    ramal = null
                });
            }
            else
            {
                // caso o usuario não tiver ramal o valor nulo.
                ListaDeRamal.Add(new dropRamal
                {
                    desc = NomeDescricao,
                    ramal = null
                });
                // ordenando para deixar o valor nulo como primeiro.
                ListaDeRamal = ListaDeRamal.OrderByDescending(x => x.ramal == null)
                            .ThenBy(x => x.ramal == null)
                            .ToList();

            }

            return ListaDeRamal;
        }


        class dropRamal
        {
            public int? ramal { get; set; }
            public string desc { get; set; }
        };

        class dropDesk
        {
            public int? id { get; set; }
            public string identificador { get; set; }
        };



        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAD_COLABORADOR cAD_COLABORADOR = await db.CAD_COLABORADOR.FindAsync(id);
            if (cAD_COLABORADOR == null)
            {
                return HttpNotFound();
            }
            return View(cAD_COLABORADOR);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CAD_COLABORADOR cAD_COLABORADOR = await db.CAD_COLABORADOR.FindAsync(id);
            db.CAD_COLABORADOR.Remove(cAD_COLABORADOR);
            await db.SaveChangesAsync();
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
