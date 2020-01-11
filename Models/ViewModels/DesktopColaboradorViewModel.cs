using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.Models.ViewModels
{
    public class DesktopColaboradorViewModel
    {
        public int id { get; set; }
        public int? IdEmpresa { get; set; }
        public string username { get; set; }
        public string Empresa { get; set; }
        public string EmpresaColaborador { get; set; }
        public string So { get; set; }
        public string Departamento { get; set; }
        public string Identificador { get; set; }
        public string Modelo { get; set; }
        public string Offices { get; set; }
        public string Keyofice { get; set; }
        public string KeySo { get; set; }
        public bool UserRemove { get; set; }
        public int? v_so { get; set; }
        public int? v_office { get; set; }


        public string ip { get; set; }
        public string hd { get; set; }
        public string memoria { get; set; }
        public string monitor { get; set; }
        public string modeloDesk { get; set; }

        public int situacao { get; set; }
        public DateTime? dt_compra { get; set; }
        public int? empDesk { get; set; }

    }
}