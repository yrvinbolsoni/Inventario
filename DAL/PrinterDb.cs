using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.DAL
{
    public class PrinterDb
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


        public List<Generic> ListaEmpresaIndexNull()
        {
            return DropDownGeneric.DropDownEmpDefault(db.CAD_EMP.ToList());
        }



        public List<IN_PRINTER> BuscaGenericaPrinter(int empresa, string departamentos, string ip, int Departamento)
        {

            var lista = (from p in db.IN_PRINTER
                         where (empresa != 0 ?p.EMPID == empresa : true)
                         where (!String.IsNullOrEmpty(departamentos)? p.CAD_DEPT.descs.Contains(departamentos) :true)
                         where (!string.IsNullOrEmpty(ip)? p.IP.Contains(ip):true)
                         select p
                         );



            return lista.ToList();
        }


        public List<Generic> BuscarDepartamento(int id)
        {
            var lista = (from d in db.CAD_DEPT
                         where d.emp == id
                         select new Generic
                         {
                            id = d.id,
                            desc = d.descs
                         }).ToList();

            lista.Add(new Generic {id= null , desc = "SEM DEPARTAMENTO" });


            return lista.ToList();
        }

        public string[] EditarPrinter(IN_PRINTER printer)
        {
            try
            {
                db.Entry(printer).State = EntityState.Modified;
                db.SaveChanges();
                SetMensagem("sucesso", "Alterado com sucesso", "success");


            }
            catch (Exception ex)
            {
                AnaliseError(ex);
            }

            return mensagem;
        }

        public string[] NovaImpressora(IN_PRINTER printer)
        {
            try
            {
                db.IN_PRINTER.Add(printer);
                db.SaveChanges();
                SetMensagem("sucesso", "Cadastrado com sucesso", "success");
            }
            catch (Exception ex)
            {
                AnaliseError(ex);
            }

            return mensagem;
        }

        public string[] DeletarPrint(long id)
        {
            try
            {
                IN_PRINTER printer = db.IN_PRINTER.Find(id);
                db.IN_PRINTER.Remove(printer);
                db.SaveChanges();
                SetMensagem("sucesso", "Deletado com sucesso", "warning");

            }
            catch (Exception ex)
            {
                AnaliseError(ex);
            }

            return mensagem;
        }
    }
}