using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.DAL
{
    public class ItensDb
    {

        private DbIntra db = new DbIntra();
        private string[] mensagem = new string[3];



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


        // Pega a lista de tipos e adiciona Todos os tipos para  consultas
        public List<Generic> ListaTipos()
        {

            var ListaTipos = (from i in db.CAD_ITEM_TYPE
                              select new Generic
                              {
                                  id = i.id,
                                  desc = i.descs
                              }).ToList();

            ListaTipos.Add(new Generic { id = 0, desc = "Todos os tipos" });


            ListaTipos = ListaTipos
                         .OrderByDescending(x => x.id == 0)
                         .ToList();

            return ListaTipos;

        }

        public List<CAD_ITEM> BuscaItens(int Tipo, string NomeDescricao)
        {

            var lista = (from i in db.CAD_ITEM
                         where (Tipo == 0 ? i.typeID == i.typeID : i.typeID == Tipo)
                         where (!String.IsNullOrEmpty(NomeDescricao) ? i.descs.Contains(NomeDescricao) : true)
                         select i
                         ).ToList();

            return lista;
        }



        public string[] Deletar(int id )
        {

            try
            {
                CAD_ITEM item = db.CAD_ITEM.Find(id);
                db.CAD_ITEM.Remove(item);
                db.SaveChanges();
                SetMensagem("sucesso", "Deletado com sucesso", "success");
            }
            catch (Exception ex )
            {

                AnaliseError(ex);
            }

            return mensagem;

        }

        public string[] CriarItem(CAD_ITEM item)
        {
            try
            {
                db.CAD_ITEM.Add(item);
                db.SaveChanges();
                SetMensagem("sucesso", "Cadastrado com sucesso", "success");
            }
            catch (Exception ex)
            {
                AnaliseError(ex);
            }

            return mensagem;
        }
    }
}