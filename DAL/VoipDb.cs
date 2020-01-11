using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;
using IntraGriegHomolog.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.DAL
{
    public class VoipDb
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



        public string[] EditarVoipColaborador(int UserNameNovo, IN_VOIP VoipList)
        {

            var UserAtual = db.CAD_COLABORADOR.Where(x => x.ramal == VoipList.ramal).FirstOrDefault();

            try
            {
                // CONDIÇÃO HOUVE ALGUMA MUDANÇA ? NO USUAIROP 
                if ((UserAtual == null ? 0 : UserAtual.id) != UserNameNovo)
                {
                    //// SIM ESSE USUARIO  É NULO ?
                    if (UserAtual == null)
                    {
                        // REGISTRANDO APENAS UM USUARIO PARA UM RAMAL QUE NAO TINHA USUARIO ANTES 
                        CAD_COLABORADOR ColabNovoRamal = db.CAD_COLABORADOR.Find(UserNameNovo);
                        ColabNovoRamal.ramal = VoipList.ramal;
                        db.Entry(ColabNovoRamal).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        // CONDIÇÃO DEJEJA REMOVER O USUARIO 
                        if (UserNameNovo == 0)
                        {

                            // VERIFICA SE ESSE USUARIO REALEMNTE  EXISTE 
                            var PossueUser = db.CAD_COLABORADOR.Where(l => l.ramal == UserAtual.ramal).FirstOrDefault();
                            if (PossueUser != null)
                            {
                                // REMOVER APENAS O USUAIO QUE ESTAVA ATRELADO AO RAMAL 
                                PossueUser.ramal = null;
                                db.Entry(PossueUser).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                        // ANTERAÇÃO DE UM USUARIO QUE JÁ EXISTE PARA OUTRO USUARIO 
                        else
                        {
                            // DEIXAR O USUARIO ANTIGO COMO NULO 
                            CAD_COLABORADOR colabRamalAntigo = db.CAD_COLABORADOR.Find(UserAtual.id);
                            colabRamalAntigo.ramal = null;
                            db.Entry(colabRamalAntigo).State = EntityState.Modified;
                            db.SaveChanges();

                            // INSERINDO O NOVO COLABORDOR NO RAMAL  
                            CAD_COLABORADOR ColabNovoRamal = db.CAD_COLABORADOR.Find(UserNameNovo);
                            ColabNovoRamal.ramal = VoipList.ramal;
                            db.Entry(ColabNovoRamal).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }

                }// FIM TRY CATCH

                // CASO NENHUMA OPÇÃO TENHA SIDO SELECIONADA  CONTINUE A EDITAR  OS SEUS COMPLEMENTOS 
         
                    db.Entry(VoipList).State = EntityState.Modified;
                    db.SaveChanges();
                    SetMensagem("sucesso", "Alterado com sucesso", "success");
                    return mensagem;
                
            }
            catch (Exception ex)
            {
                AnaliseError(ex);
            }

            return mensagem;

        }


        public List<CAD_COLABORADOR> DropUserVoipSemPC(int id , int emp)
        {
            var NomeColaborador = db.CAD_COLABORADOR.Where(x => x.ramal == id).FirstOrDefault(); //busca o id o colaborador e o nome 
             
           

            var lista = db.CAD_COLABORADOR
                  .Where(i => i.ramal == null  && i.emp == emp)
                  .OrderByDescending(i => i.ramal)
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
                lista.Add(NomeColaborador);

              lista = lista.OrderByDescending(x => x.username.Equals(NomeColaborador.username))
             .ThenBy(x => x.username.Equals(NomeColaborador.username))
             .ToList();
            }

            

            return lista;


        }


        /// ///////////////////////////////////////////////////////// BUSCA VOIP \\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public List<VoipColaboradorViewModel> BuscaGenericaVoip(int empresa, string Username, int Ramal, int Departamento)
        {


            var query = (from v in db.IN_VOIP
                         join co in db.CAD_COLABORADOR on v.ramal equals co.ramal into Cgroup
                         from co in Cgroup.DefaultIfEmpty()
                         join e in db.CAD_EMP on v.emp equals e.id
                         where (Departamento != 0) ? co.dept == Departamento : co.dept == co.dept
                         where (empresa != 0) ? v.emp == empresa : co.emp == co.emp
                         where (Ramal != 0) ? v.ramal == Ramal : true
                         where (!String.IsNullOrEmpty(Username)) ? co.username.Contains(Username) : true
                         select new
                         {
                             VRamal = v.ramal,
                             Vpassword = v.passwd,
                             Vip = v.ip,
                             COuserName = co.username,
                             CoDept = co.CAD_DEPT.descs,
                             CoEmpresa = e.descs,
                             Vsituacao = v.situacao
                         }).ToList()
                         .Select(x => new VoipColaboradorViewModel
                         {
                             ramal = x.VRamal,
                             passwd = x.Vpassword,
                             ip = x.Vip,
                             username = x.COuserName,
                             departamento = x.CoDept,
                             empresa = x.CoEmpresa,
                             situacao = x.Vsituacao
                         });

            //var Lista = ListaVoip();
            //  var querrylista =  Lista.Where(x => x.username != null && x.username.Contains(Username));

            return query.ToList();
        }

        public List<Generic> ListaEmpresaIndexNull()
        {
            return DropDownGeneric.DropDownEmpDefault(db.CAD_EMP.ToList());
        }

    }
}