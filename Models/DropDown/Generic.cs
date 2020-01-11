using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.Models.DropDown
{
    public class Generic
    {
        public int? id { get; set; }

        public long? idLinhaMovel { get; set; }

        public string desc { get; set; }
        public string valor { get; set; }

        // *******************************************************************  DropDown lista  HD **************************************************

        // retorna todas as empresas com um opação a mais TODAS AS EMPRESAS
        public List<Generic> DropDownEmpDefault(List<CAD_EMP> emp)
        {
            var ListaEmpresa = emp;
            List<Generic> NovaLista = new List<Generic>();
            NovaLista.Add(new Generic { id = 0, desc = "Todas as Empresas" });

            foreach (var item in ListaEmpresa)
            {
                NovaLista.Add(new Generic { id = item.id, desc = item.descs });
            }

            return NovaLista.ToList();

        }


        
        // ******************************************************************* FIM   **************************************************
        //
        //
        //
        // ******************************************************************* Situacao  **************************************************

        
    }
}