using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;
using IntraGriegHomolog.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.DAL
{
    public class DesktopDb
    {
        private DbIntra db = new DbIntra();
        private Generic DropDownGeneric = new Generic();
        private string[] mensagem = new string[3];
       
        // mensagens 
        private void SetMensagem(string status, string msg, string style)
        {
            // define mensagens para view

            mensagem[0] = status;
            mensagem[1] = msg;
            mensagem[2] = String.Format("alert alert-{0}  alert-dismissible", style);
        }

        public string[] AnaliseError(Exception ex)
        {
            switch (ex.HResult)
            {
                case -2146233087:
                    string[] msg = ex.InnerException.InnerException.Message.ToString().Split('*');
                    if (msg.Count() > 1)
                        SetMensagem("falha", msg[1], "danger");
                    else
                        SetMensagem("falha", msg[0], "danger");
                    break;

                default:
                    SetMensagem("falha", ex.Message, "danger");
                    break;

            }

            return mensagem;
        }
        // mensagens 

        public string[] CreateDesktop(IN_DESKTOP desk)
        {
            // configuração caso o usuario não tenha inserido nota neste caso o sistema ira inserir o desktop sem nota 
            try
            {
                    // grava no db
                    db.IN_DESKTOP.Add(desk);
                    db.SaveChanges();
                   
                   // Cadastrado com sucesso , redireciona para pagina index e mostra mensagem de sucesso 
                    SetMensagem("sucesso", "Cadastrado com sucesso", "success");
            }
            catch (Exception ex)
            {
                // algo errado no memento da gravação faz o tratamento de erro e mostra na view , os erros podem ser chavem duplicada etc ..
                AnaliseError(ex);
            }
            return mensagem;
        }


        public string[] RemoverNota(int id)
        {
            // verificar se existe anexo para este desktop
            var dados = db.NFE.Where(x => x.id.Equals(id)).FirstOrDefault();  

            if (dados != null)
            {
                db.NFE.Remove(dados);
                db.SaveChanges();
                SetMensagem("sucesso", "Anexo removido com sucesso", "success");
                return mensagem;
            }
            else  
            {
                SetMensagem("sucesso", "Nao existe Anexo atrelado a este desktop", "warning");
                return mensagem;
            }

        }

        // adicionar nota fiscals
        public string[] AdicionarNota(HttpPostedFileBase postedFile, int Id, string ApelidoAnexo)
        {
            var IdTipoMaquina = (from desks in db.IN_DESKTOP
                                    join item in db.CAD_ITEM on desks.modelo_client equals item.id into egroup
                                    from item in egroup.DefaultIfEmpty()

                                    join itemType in db.CAD_ITEM_TYPE on item.typeID equals itemType.id into Itgroup
                                    from itemType in Itgroup.DefaultIfEmpty()

                                    where desks.id == Id
                                    select new
                                    {
                                        id = itemType.id
                                    }).FirstOrDefault();


         

            if (!String.IsNullOrEmpty(ApelidoAnexo))
            {
                string[] FormatarName = ApelidoAnexo.Split('.');

                ApelidoAnexo = FormatarName[0] + ".pdf";

            }

            var Dados = db.IN_DESKTOP.Where(x => x.id == Id).FirstOrDefault();

            // convertar arquivo em byte 
            string NameFile = ApelidoAnexo;
            string ContextType = postedFile.ContentType;
            string FileExtension = Path.GetExtension(postedFile.FileName);
            int FileSize = postedFile.ContentLength;

            if (FileExtension.ToLower() == ".pdf")
            {
                if (FileSize <= 1000000)
                {
                    Stream stream = postedFile.InputStream;
                    BinaryReader binaryReader = new BinaryReader(stream);
                    byte[] Arquivo = binaryReader.ReadBytes((int)stream.Length); // arquivo convertindo em byte 

                    NFE Nota = new NFE();
                    Nota.n_name = NameFile;
                    Nota.n_type = ContextType;
                    Nota.n_file = Arquivo;
                    Nota.identificador = Dados.identificador;
                    Nota.emp = Dados.emp;
                    Nota.TYPE_ITEM = IdTipoMaquina.id;

                    db.NFE.Add(Nota);
                    db.SaveChanges();

                    SetMensagem("sucesso", "Anexo inserido com sucesso", "success");
                    return mensagem;
                }
                else
                {
                    SetMensagem("sucesso", "* Tamanho maximo de 1MB", "danger");
                    return mensagem;
                }

            }
            else
            {
                SetMensagem("sucesso", "* somente .pdf", "danger");
                return mensagem;
            }
        }


        public string[] EditarDeskColaborador(IN_DESKTOP desk, int UserNameNovo)
        {
            try
            {


                var UserNameAtual = db.CAD_COLABORADOR.Where(l => l.desktop == desk.id).Select(d => d.id).FirstOrDefault();

                // verificar se houve um mudança de usuario 
                if (UserNameAtual != UserNameNovo)
                {
                    // se o Colaborador nao nem maquina atrelado 
                    if (db.CAD_COLABORADOR.Where(d => d.id == UserNameAtual).FirstOrDefault() == null)
                    {
                        CAD_COLABORADOR NewColabSemUser = db.CAD_COLABORADOR.Find(UserNameNovo);
                        NewColabSemUser.desktop = desk.id;
                        db.Entry(NewColabSemUser).State = EntityState.Modified;
                        db.SaveChanges();
                        SetMensagem("sucesso"," Alterado com sucesso", "success");
                    }

                    // se ele for diferente e tiver um usuario atrelado para o desktop realizar essa operação
                    else
                    {
                        if (UserNameNovo == 0)
                        {

                            var VerificarSePossueDesk = db.CAD_COLABORADOR.Where(l => l.desktop == desk.id).FirstOrDefault();
                            if (VerificarSePossueDesk != null)
                            {
                                // CAD_COLABORADOR colab = db.CAD_COLABORADOR.Find(UserNameAtual);
                                VerificarSePossueDesk.desktop = null;
                                db.Entry(VerificarSePossueDesk).State = EntityState.Modified;
                                db.SaveChanges();
                                SetMensagem("sucesso", " Alterado com sucesso", "success");
                            }

                        }
                        // se nao tiver selecionado SEM USUARIO  esse bloco irar fazer o a troca
                        else
                        {
                            /// deixando o atual colaborador  deste desktop como nulo 
                            CAD_COLABORADOR colab = db.CAD_COLABORADOR.Where(x => x.id == UserNameAtual).FirstOrDefault();
                            colab.desktop = null;
                            db.Entry(colab).State = EntityState.Modified;
                            db.SaveChanges();

                            /// deixar mudar o id do desktop para o novo colaborador 
                            CAD_COLABORADOR NewColab = db.CAD_COLABORADOR.Where(x => x.id == UserNameNovo).FirstOrDefault();
                            NewColab.desktop = desk.id;
                            db.Entry(NewColab).State = EntityState.Modified;
                            db.SaveChanges();
                            SetMensagem("sucesso", " Alterado com sucesso", "success");
                        }
                    }
                }

                SetMensagem("sucesso", " Alterado com sucesso", "success");
                db.Entry(desk).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return AnaliseError(ex);
            }

            return mensagem;
        }

       


        // modelo dos desktops  , porque dessa forma pegas os dois modelo de desk e notbook  id 
        public List<CAD_ITEM> DropModeloDesk()
        {
            var DeskDrop = (from c in db.CAD_ITEM
                            where c.typeID == 1 || c.typeID == 2
                            select c
                            
                                 ).ToList();

            var  listaOrdenada  = DeskDrop.OrderByDescending(x => x.typeID == 1)
                .ThenBy(x => x.typeID)
                .ToList();
                                   
            return listaOrdenada;
        }

        public List<Generic> DropdownGenerico(int id, string descricao)
        {

            // ADICIONA um item null e uma descrição ex SEM ALGUMA COISA e faz o devido filtro
            // busca  
            var lista = (from item in db.CAD_ITEM
                         where item.typeID == id
                         select new Generic
                         {
                             id = item.id,
                             desc = item.descs,
                             valor = item.descs
                         }).ToList();


            // adiciona  
            lista.Add(new Generic { id = null, desc = descricao });


            // ordena  
            List<Generic> ListaOrdenada = lista
                                  .OrderByDescending(x => x.id == null)
                                  .ThenBy(x => x.id)
                                  .ToList();

            return ListaOrdenada;
        }


        public List<Generic> ListaEmpresaIndexNull()
        {
            return DropDownGeneric.DropDownEmpDefault(db.CAD_EMP.ToList());
        }


        public List<DesktopColaboradorViewModel> BuscaGenericaDeskTop(int empresaid, int DepartamentoP, string UserName, string Identificador, string KeyOffice, string KeySo)
        {
            var lista = BuscaporEmpresaDesktop(empresaid, DepartamentoP, UserName, Identificador, KeyOffice, KeySo);

            return lista;
        }



        // busca Lista Generica
        public List<DesktopColaboradorViewModel> BuscaporEmpresaDesktop(int Empresaid, int DepartamentP, string nameuser, string identificadorP, string KeyofficeP, string keysoP)
        {

            var result = (from desk in db.IN_DESKTOP
                          join co in db.CAD_COLABORADOR on desk.id equals co.desktop into egroup
                          from co in egroup.DefaultIfEmpty()

                          join e in db.CAD_EMP on desk.emp equals e.id

                          join Monitor in db.CAD_ITEM on desk.monitor_client equals Monitor.id into Ogrup
                          from Monitor in Ogrup.DefaultIfEmpty()

                          join So in db.CAD_ITEM on desk.sis_oper equals So.id into sgrup
                          from So in sgrup.DefaultIfEmpty()

                          join Modelo in db.CAD_ITEM on desk.modelo_client equals Modelo.id into Dgroup
                          from Modelo in Dgroup.DefaultIfEmpty()

                          join Office in db.CAD_ITEM on desk.pct_office equals Office.id into Fgroup
                          from Office in Fgroup.DefaultIfEmpty()

                          join Memoria in db.CAD_ITEM on desk.mem_ram equals Memoria.id into Mgroup
                          from Memoria in Mgroup.DefaultIfEmpty()

                          join Hd in db.CAD_ITEM on desk.mem_ram equals Hd.id into Hgroup
                          from Hd in Hgroup.DefaultIfEmpty()

                          where (Empresaid != 0 ? desk.emp == Empresaid : desk.emp == desk.emp)
                          where (DepartamentP != 0 ? co.dept == DepartamentP : co.dept == co.dept)
                          where (!String.IsNullOrEmpty(identificadorP) ? desk.identificador.Contains(identificadorP) : true)
                          where (!String.IsNullOrEmpty(KeyofficeP) ? desk.k_office.Contains(KeyofficeP) : true)
                          where (!String.IsNullOrEmpty(keysoP) ? desk.k_so.Contains(keysoP) : true)
                          where (!String.IsNullOrEmpty(nameuser) ? (nameuser.ToUpper() == "NULL" ? co.username == null : co.username.Contains(nameuser)) : true)
                          select new
                          {
                              Id = desk.id,
                              Empresa = e.descs,
                              EmpresaId = desk.emp,
                              UserName = co.username,
                              Departamento = co.CAD_DEPT.descs,
                              Identificador = desk.identificador,
                              Modelo = Modelo.descs,
                              Offices = Office.descs,
                              So = So.descs,
                              KeyOfice = desk.k_office,
                              KeySo = desk.k_so,
                              memoria = Memoria.descs,
                              situacao = desk.sit,
                              monitor = Monitor.descs,
                              ip = desk.ip,
                              hd = Hd.descs,
                              dataCompra = desk.dt_compra,
                              empresaDesk = desk.emp
                          }).ToList()
                     .Select(x => new DesktopColaboradorViewModel()
                     {
                         id = x.Id,
                         empDesk = x.empresaDesk,
                         dt_compra = x.dataCompra,
                         memoria = x.memoria,
                         situacao = x.situacao,
                         monitor = x.monitor,
                         ip = x.ip,
                         hd = x.hd,
                         Empresa = x.Empresa,
                         IdEmpresa = x.EmpresaId,
                         username = x.UserName,
                         So = x.So,
                         Departamento = x.Departamento,
                         Identificador = x.Identificador,
                         modeloDesk = x.Modelo,
                         Offices = x.Offices,
                         Keyofice = x.KeyOfice,
                         KeySo = x.KeySo
                     }).ToList();

            return result;

        }

        // arrumar tudo daqui pra baixo

        public List<DesktopColaboradorViewModel> ListaJoin() // retorna todos os computadores usando o recurso do join 
        {
            //LEFT OUTER JOIN CAD_OFFICE O ON D.V_OFFICE = O.ID
            //LEFT OUTER JOIN CAD_SO S ON D.V_SO = S.ID;

            var result = (from desk in db.IN_DESKTOP
                          join co in db.CAD_COLABORADOR on desk.id equals co.desktop into egroup

                          from co in egroup.DefaultIfEmpty()
                          join e in db.CAD_EMP on desk.emp equals e.id

                          join Monitor in db.CAD_ITEM on desk.monitor_client equals Monitor.id into Ogrup
                          from Monitor in Ogrup.DefaultIfEmpty()

                          join So in db.CAD_ITEM on desk.sis_oper equals So.id into sgrup
                          from So in sgrup.DefaultIfEmpty()

                          join Modelo in db.CAD_ITEM on desk.modelo_client equals Modelo.id into Dgroup
                          from Modelo in Dgroup.DefaultIfEmpty()

                          join Office in db.CAD_ITEM on desk.pct_office equals Office.id into Fgroup
                          from Office in Fgroup.DefaultIfEmpty()

                          join Memoria in db.CAD_ITEM on desk.mem_ram equals Memoria.id into Mgroup
                          from Memoria in Mgroup.DefaultIfEmpty()

                          join Hd in db.CAD_ITEM on desk.mem_ram equals Hd.id into Hgroup
                          from Hd in Hgroup.DefaultIfEmpty()
                          select new
                          {
                              Id = desk.id,
                              Empresa = e.descs,
                              EmpresaId = desk.emp,
                              UserName = co.username,
                              Departamento = co.CAD_DEPT.descs,
                              Identificador = desk.identificador,
                              Modelo = Modelo.descs,
                              Offices = Office.descs,
                              So = So.descs,
                              KeyOfice = desk.k_office,
                              KeySo = desk.k_so,
                              memoria = Memoria.descs,
                              situacao = desk.sit,
                              monitor = Monitor.descs,
                              ip = desk.ip,
                              hd = Hd.descs,
                              dataCompra = desk.dt_compra,
                              empresaDesk = desk.emp
                          }).ToList()
                     .Select(x => new DesktopColaboradorViewModel()
                     {
                         id = x.Id,
                         empDesk = x.empresaDesk,
                         dt_compra = x.dataCompra,
                         memoria = x.memoria,
                         situacao = x.situacao,
                         monitor = x.monitor,
                         ip = x.ip,
                         hd = x.hd,
                         Empresa = x.Empresa,
                         IdEmpresa = x.EmpresaId,
                         username = x.UserName,
                         So = x.So,
                         Departamento = x.Departamento,
                         Identificador = x.Identificador,
                         modeloDesk = x.Modelo,
                         Offices = x.Offices,
                         Keyofice = x.KeyOfice,
                         KeySo = x.KeySo
                     }).ToList();

            return result;
        }
        

        public List<DesktopColaboradorViewModel> BuscaComputadoresJoin(int id)
        {
            var lista = ListaJoin().Where(x => x.id == id).ToList();
            return lista;
        }

        public List<CAD_COLABORADOR> DropUser(int id, int Emp)
        {
            var NomeColaborador = BuscaComputadoresJoin((int)id).Select(x => x.username).First(); //busca o id o colaborador e o nome 

            var lista = db.CAD_COLABORADOR
                  .Where(x => x.emp == (Emp == 0 ? 1 : Emp))
                  .Where(i => i.desktop == null || i.username.Equals(NomeColaborador))
                  .OrderByDescending(i => i.desktop)
                  .ToList();

            // adiciona 
            lista.Add( new CAD_COLABORADOR {id= 0 , username = "SEM USUARIO" } );

            // sem colaborador deixar o sem usuario em primeiro 
            if (NomeColaborador == null)
            {
               // ordena
               lista = lista
                    .OrderByDescending(x => x.id == 0)
                    .ThenBy(x => x.id)
                    .ToList();
            }

            // com colaborador deixar ele como primeiro 
            if (NomeColaborador != null)
            {
                lista = lista
            .OrderByDescending(x => x.username.Equals(NomeColaborador))
            .ThenBy(x => x.username.Equals(NomeColaborador))
            .ToList();
            }
            return lista;

        }

    }
}