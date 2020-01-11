using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntraGriegHomolog.Models.ViewModels
{
    public class VoipColaboradorViewModel
    {
            [Key]
            public int ramal { get; set; }
            public string passwd { get; set; }
            public string ip { get; set; }
            public string username { get; set; }
            public string departamento { get; set; }
            public string empresa { get; set; }
            public int  situacao { get; set; }


    }
}