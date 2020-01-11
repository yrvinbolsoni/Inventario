using IntraGriegHomolog.Models;
using IntraGriegHomolog.Models.DropDown;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.DAL
{
    public class LinhaMovelDb
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

        // pega todas as empresas e adiciona uma mais com todas as empresas 
        public List<Generic> ListaEmpresaIndexNull()
        {
            return DropDownGeneric.DropDownEmpDefault(db.CAD_EMP.ToList());
        }

        // busca linha baseados em parametros 
        public List<IN_LINHA_MOVEL> BuscaLinhasMovel( int empresa, string numero, string ICCID)
        {
            var ListLinhaMovel = (from linha in db.IN_LINHA_MOVEL
                                  where (!String.IsNullOrEmpty(numero) ? linha.DESCS.Contains(numero) : true)
                                  where (empresa != 0 ? linha.EMPID == empresa : linha.EMPID == linha.EMPID)
                                  where (!String.IsNullOrEmpty(ICCID)?linha.ICCID.Contains(ICCID) :true)
                                  select linha
                                   ).ToList();
            return ListLinhaMovel;
        }

        public string[] EditarLinhaMovel(IN_LINHA_MOVEL linhaMovel)
        {
            try
            {
                db.Entry(linhaMovel).State = EntityState.Modified;
                db.SaveChanges();
                SetMensagem("sucesso", " Alterado com sucesso", "success");

            }
            catch (Exception ex)
            {
                return AnaliseError(ex);
            }

            return mensagem;
        }

        public string[] NovaLinha(IN_LINHA_MOVEL linhaMovel)
        {
            try
            {
                db.IN_LINHA_MOVEL.Add(linhaMovel);
                db.SaveChanges();
                SetMensagem("sucesso", " Linha cadastrada com sucesso", "success");
            }
            catch (Exception ex)
            {
                return AnaliseError(ex);
            }

            return mensagem;
        }

        public string[] DeletarLinha(long id)
        {
            try
            {
                IN_LINHA_MOVEL linhaMovel = db.IN_LINHA_MOVEL.Find(id);
                db.IN_LINHA_MOVEL.Remove(linhaMovel);
                SetMensagem("sucesso", " Linha DELETADA.", "warning");
                db.SaveChanges();
            }
            catch (Exception ex )
            {
                return AnaliseError(ex);
            }
            return mensagem;
        }
    }
}