using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.Models.ViewModels
{
    public class SmartphoneColaborador
    {
        public long id { get; set; }
        [DisplayName("Numero Serial")]
        public string serial_number { get; set; }
        [DisplayName("Modelo")]
        public string  model { get; set; }
        [DisplayName("Imei")]
        public string imei { get; set; }
        [DisplayName("Empresa")]
        public string empresa { get; set; }
        [DisplayName("Linha movel")]
        public string linha { get; set; }
        [DisplayName("Linha movel 2 ")]
        public string linha2 { get; set; }
        [DisplayName("Usuario")]
        public string Username { get; set; }
        [DisplayName("Departamento")]
        public string departamento { get; set; }
        [DisplayName("Situação")]
        public int situacao { get; set; }


        // public DateTime DataCompra { get; set; }
        // public int empid { get; set; }
    }
}