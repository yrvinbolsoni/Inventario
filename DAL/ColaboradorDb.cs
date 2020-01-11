using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.DAL
{
    public class ColaboradorDb
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

         

        public List<Models.CAD_COLABORADOR> BuscaGenericaColab(int Empresaid, int DepartamentP, string nameuser)
        {
            var lista = (from colab in db.CAD_COLABORADOR
                         where (Empresaid != 0 ? colab.emp == Empresaid : colab.emp == colab.emp)
                         where (DepartamentP != 0 ? colab.dept == DepartamentP : colab.dept == colab.dept)
                         where (!String.IsNullOrEmpty(nameuser) ? (nameuser.ToUpper() == "NULL" ? colab.username == null : colab.username.Contains(nameuser)) : true)
                         select colab
                         ).ToList();
            return lista;
        }


        public List<Generic> StatusColab()
        {
            List<Generic> statusList = new List<Generic>();

            statusList.Add(new Generic {id=1 , valor = "A", desc = "ATIVO" });
            statusList.Add(new Generic {id =2, valor = "D", desc = "DESATIVADO" });

            return statusList;

        }

        public string[] CriarUser(CAD_COLABORADOR colab)
        {
            try
            {
                db.CAD_COLABORADOR.Add(colab);
                db.SaveChanges();
                SetMensagem("sucesso", "Cadastrado com sucesso", "success");
            }
            catch (Exception ex)
            {
                AnaliseError(ex);
            }

            return mensagem;
        }




        public List<Generic> ListaEmpresaIndexNull()
        {
            return DropDownGeneric.DropDownEmpDefault(db.CAD_EMP.ToList());
        }
    }
}