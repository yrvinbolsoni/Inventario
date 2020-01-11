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
    public class SmartPhoneDb
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

        public List<SmartphoneColaborador> BuscarSmartPhone(string nameuser, int Empresaid, int Departamento, string Numero, string Modelo, string Imei)
        {

            List<SmartphoneColaborador> result = (from sm in db.IN_SMARTPHONE

                                                  join co in db.CAD_COLABORADOR on sm.id equals co.smartphone into egroup
                                                  from co in egroup.DefaultIfEmpty()

                                                  join e in db.CAD_EMP on sm.EMP equals e.id

                                                  where (!String.IsNullOrEmpty(nameuser) ? (nameuser.ToUpper() == "NULL" ? co.username == null : co.username.Contains(nameuser)) : true)
                                                  where (Empresaid != 0 ? sm.EMP == Empresaid : sm.EMP == sm.EMP)
                                                  where (Departamento != 0 ? co.dept == Departamento : co.dept == co.dept)
                                                  where (!String.IsNullOrEmpty(Numero) ? (Numero.ToUpper() == "NULL" ? sm.linha_movel == null || sm.linha_movel == null : sm.IN_LINHA_MOVEL.DESCS.Contains(Numero) || sm.IN_LINHA_MOVEL1.DESCS.Contains(Numero)) : true)
                                                  where (!String.IsNullOrEmpty(Modelo) ? sm.CAD_ITEM.descs.Contains(Modelo) : true)
                                                  where (!String.IsNullOrEmpty(Imei) ? sm.imei.Contains(Imei) : true) || (!String.IsNullOrEmpty(Imei) ? sm.imei2.Contains(Imei) : true)

                                                  select new
                                                  {
                                                      // colab
                                                      empresa = e.descs,
                                                      Username = co.username,
                                                      departamento = co.CAD_DEPT.descs,
                                                      // smartPhone
                                                      imail = sm.imei,
                                                      id = sm.id,
                                                      linha = sm.IN_LINHA_MOVEL.DESCS,
                                                      linha2 = sm.IN_LINHA_MOVEL1.DESCS,
                                                      model = sm.CAD_ITEM.descs,
                                                      serial_number = sm.serial_number,
                                                      sit = sm.situacao

                                                  }).ToList()
                                                  // mapiamento para a ViewModel
                                                  .Select(x => new SmartphoneColaborador()
                                                  {
                                                      empresa = x.empresa,
                                                      Username = x.Username,
                                                      departamento = x.departamento,
                                                      id = x.id,
                                                      imei = x.imail,
                                                      linha = x.linha,
                                                      linha2 = x.linha2,
                                                      model = x.model,
                                                      serial_number = x.serial_number,
                                                      situacao = x.sit


                                                  }).ToList();

            return result;
        }
        
             public List<Generic> LinhasDisponiveis(long? Linha1, int emp)
             {

            List<IN_LINHA_MOVEL> lista = (from linha in db.IN_LINHA_MOVEL

                                          where !(from smart in db.IN_SMARTPHONE
                                                  where smart.linha_movel != null
                                                  select smart.linha_movel)
                                                 .Contains(linha.ID)

                                          where !(from smart in db.IN_SMARTPHONE
                                                  where smart.linha_movel2 != null
                                                  select smart.linha_movel2)
                                                .Contains(linha.ID)
                                          where linha.EMPID == emp
                                             select linha
                                          ).ToList();


            List<Generic> ListaGenerica = new List<Generic>();
            foreach (var l in lista)
            {
                ListaGenerica.Add(new Generic
                {
                    idLinhaMovel = l.ID,
                    desc = l.DESCS
                });
            }

          
            // Veririfica e  adiciona linha caso tenha uma 
            var Linha1Atual = db.IN_LINHA_MOVEL.Where(x => x.ID == Linha1).FirstOrDefault();

            if (Linha1Atual != null)
            {
                ListaGenerica.Add(new Generic {idLinhaMovel = Linha1Atual.ID , desc = Linha1Atual.DESCS });
            }

            // Verirfica se a linha existe e ordena para deixar em primeiro o valor que tem no banco de dados 
            var verifica = ListaGenerica.Where(x => x.idLinhaMovel == Linha1).FirstOrDefault();

            ListaGenerica.Add(new Generic
            {
                id = 0,
                idLinhaMovel = null,
                desc = "SEM LINHA MOVEL"
            });

            if (verifica != null)
            {
                ListaGenerica = ListaGenerica
                    .OrderByDescending(x => x.idLinhaMovel == Linha1)
                    .ThenByDescending(x => x.idLinhaMovel == Linha1)
                    .ToList();
            }
            else
            {
                ListaGenerica = ListaGenerica
                  .OrderByDescending(x => x.id == 0)
                  .ThenByDescending(x => x.id == 0)
                  .ToList();
            }


            return ListaGenerica;

             }

        public string[] NovoSmartFone(IN_SMARTPHONE smartFone)
        {
            try
            {
                db.IN_SMARTPHONE.Add(smartFone);
                db.SaveChanges();
                SetMensagem("sucesso", "Smartphone cadastrado com sucesso", "success");

            }
            catch (Exception ex )
            {
                return AnaliseError(ex);
            }
            return mensagem;
        }

        public List<CAD_COLABORADOR> DropUserDisponivel(int id , int emp)
        {
            var NomeColaborador = db.CAD_COLABORADOR.Where(x => x.smartphone == id).Select(x => x.username).FirstOrDefault(); //busca o id o colaborador e o nome 

            var lista = db.CAD_COLABORADOR
                  .Where(i => i.smartphone == null || i.username.Equals(NomeColaborador))
                  .Where(x => x.emp == emp)
                  .Where(x => x.STATUS == "A")
                  .OrderByDescending(i => i.desktop)
                  .ToList();

            // adiciona 
            lista.Add(new CAD_COLABORADOR { id = 0, username = "SEM USUARIO" });

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

        public List<CAD_COLABORADOR> DropUserDisponivelPorEmp(int emp)
        {
            var lista = db.CAD_COLABORADOR
                  .Where(i => i.smartphone == null)
                  .Where(x => x.emp == emp)
                  .Where(x => x.STATUS == "A")
                  .OrderByDescending(i => i.desktop)
                  .ToList();

            // adiciona 
            lista.Add(new CAD_COLABORADOR { id = 0, username = "SEM USUARIO" });
          
            return lista;
        }


        public List<Generic> ListaEmpresaIndexNull()
        {
            return DropDownGeneric.DropDownEmpDefault(db.CAD_EMP.ToList());
        }

        public  string[] AdicionarTermo(HttpPostedFileBase postedFile, int id, string ApelidoAnexo)
        {
            // retorno de mensagem 

            if (!String.IsNullOrEmpty(ApelidoAnexo))
            {
                string[] FormatarName = ApelidoAnexo.Split('.');

                ApelidoAnexo = FormatarName[0] + ".pdf";

            }

            var SmartPhone = db.IN_SMARTPHONE.Where(x => x.id == id).FirstOrDefault();

            // convertar arquivo em byte 
            string NameFile = String.Format("{0}-{1}",SmartPhone.serial_number , ApelidoAnexo);
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

                    SmartPhone.TERM_NAME = NameFile;
                    SmartPhone.TERM_TYPE = ContextType;
                    SmartPhone.TERM_FILE = Arquivo;

                    db.Entry(SmartPhone).State = EntityState.Modified;
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

        public string[] EditarSmartfoneColab(IN_SMARTPHONE SmartFone, int UserNameNovo)
        {
            try
            {
                var UserNameAtual = db.CAD_COLABORADOR.Where(l => l.smartphone == SmartFone.id).Select(d => d.id).FirstOrDefault();

                // verificar se houve um mudança de usuario 
                if (UserNameAtual != UserNameNovo)
                {
                    // se o Colaborador nao Tem smarfone atrelado 
                    if (db.CAD_COLABORADOR.Where(d => d.id == UserNameAtual).FirstOrDefault() == null)
                    {
                        CAD_COLABORADOR NewColabSemUser = db.CAD_COLABORADOR.Find(UserNameNovo);
                        NewColabSemUser.smartphone = SmartFone.id;
                        db.Entry(NewColabSemUser).State = EntityState.Modified;
                        db.SaveChanges();
                        SetMensagem("sucesso", " Alterado com sucesso", "success");
                    }

                    // se ele for diferente e tiver um usuario atrelado para o desktop realizar essa operação
                    else
                    {
                        if (UserNameNovo == 0)
                        {

                            var VerificaSePossueSmartFone = db.CAD_COLABORADOR.Where(l => l.smartphone == SmartFone.id).FirstOrDefault();
                            if (VerificaSePossueSmartFone != null)
                            {
                                VerificaSePossueSmartFone.smartphone = null;
                                db.Entry(VerificaSePossueSmartFone).State = EntityState.Modified;
                                db.SaveChanges();
                                SetMensagem("sucesso", " Alterado com sucesso", "success");
                            }

                        }
                        // se nao tiver selecionado SEM USUARIO  esse bloco irar fazer o a troca
                        else
                        {
                            /// deixando o atual colaborador  deste desktop como nulo 
                            CAD_COLABORADOR colab = db.CAD_COLABORADOR.Where(x => x.id == UserNameAtual).FirstOrDefault();
                            colab.smartphone = null;
                            db.Entry(colab).State = EntityState.Modified;
                            db.SaveChanges();

                            /// deixar mudar o id do desktop para o novo colaborador 
                            CAD_COLABORADOR NewColab = db.CAD_COLABORADOR.Where(x => x.id == UserNameNovo).FirstOrDefault();
                            NewColab.smartphone = SmartFone.id;
                            db.Entry(NewColab).State = EntityState.Modified;
                            db.SaveChanges();
                            SetMensagem("sucesso", " Alterado com sucesso", "success");
                        }
                    }
                }

                SetMensagem("sucesso", " Alterado com sucesso", "success");
                // definidon entidade
                var DbEntry = db.Entry(SmartFone);
                db.Entry(SmartFone).State = EntityState.Modified;
                // definindo qual vai ser modificada 
                DbEntry.Property("TERM_NAME").IsModified = false;
                DbEntry.Property("TERM_TYPE").IsModified = false;
                DbEntry.Property("TERM_FILE").IsModified = false;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return AnaliseError(ex);
              

            }

            return mensagem;
        }




        // adicionar nota fiscal 
        public string[] AdicionarNotaFiscal(HttpPostedFileBase postedFile, int id, string ApelidoAnexo)
        {
            var IdTipoMaquina = (from desks in db.IN_SMARTPHONE
                                 join item in db.CAD_ITEM on desks.model equals item.id into egroup
                                 from item in egroup.DefaultIfEmpty()

                                 join itemType in db.CAD_ITEM_TYPE on item.typeID equals itemType.id into Itgroup
                                 from itemType in Itgroup.DefaultIfEmpty()

                                 where desks.id == id
                                 select new
                                 {
                                     id = itemType.id
                                 }).FirstOrDefault();




            if (!String.IsNullOrEmpty(ApelidoAnexo))
            {
                string[] FormatarName = ApelidoAnexo.Split('.');

                ApelidoAnexo = FormatarName[0] + ".pdf";

            }

            var Dados = db.IN_SMARTPHONE.Where(x => x.id == id).FirstOrDefault();

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
                    Nota.identificador = Dados.serial_number;
                    Nota.emp = Dados.EMP;
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


        public string[] RemoverTermo(int id)
        {
            // verificar se existe anexo para este desktop
            var SmartPhone = db.IN_SMARTPHONE.Where(x => x.id.Equals(id)).FirstOrDefault();

            if (SmartPhone != null)
            {
                SmartPhone.TERM_NAME = null;
                SmartPhone.TERM_TYPE = null;
                SmartPhone.TERM_FILE = null;

                db.Entry(SmartPhone).State = EntityState.Modified;
                db.SaveChanges();

                SetMensagem("sucesso", "Termo removido com sucesso", "success");
                return mensagem;
            }
            else
            {
                SetMensagem("sucesso", "Nao existe Termo atrelado a este desktop", "warning");
                return mensagem;
            }

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
                SetMensagem("sucesso", "Nao existe Anexo atrelado a este SmartFone", "warning");
                return mensagem;
            }

        }


    }
}