namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CAD_COLABORADOR
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string nome { get; set; }

        [Required]
        [StringLength(255)]
        public string username { get; set; }

        [Required]
        [StringLength(255)]
        public string passwd { get; set; }

        public int? dept { get; set; }

        public int? emp { get; set; }

        public int? ramal { get; set; }

        public int? desktop { get; set; }

        public int? tipo_u { get; set; }

        [Required]
        [StringLength(255)]
        public string email { get; set; }

        [Required]
        [StringLength(1)]
        public string STATUS { get; set; }

        public long? smartphone { get; set; }

        public virtual IN_DESKTOP IN_DESKTOP { get; set; }

        public virtual IN_VOIP IN_VOIP { get; set; }

        public virtual CAD_TIPO_USER CAD_TIPO_USER { get; set; }

        public virtual CAD_DEPT CAD_DEPT { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        public virtual IN_SMARTPHONE IN_SMARTPHONE { get; set; }
    }
}
