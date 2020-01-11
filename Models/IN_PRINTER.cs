namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IN_PRINTER
    {
        public long ID { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Serial")]
        public string SERIAL_NO { get; set; }

        [DisplayName("Modelo")]
        public int MODEL { get; set; }

        [DisplayName("Empresa")]
        public int EMPID { get; set; }

        [DisplayName("Departamento")]
        public int? DEPTID { get; set; }

        [StringLength(50)]
        [DisplayName("Ip")]
        public string IP { get; set; }

        [StringLength(50)]
        [DisplayName("Apelido")]
        public string APELIDO { get; set; }

        [StringLength(255)]
        [DisplayName("Obeservação")]
        public string info { get; set; }

        [DisplayName("Status")]
        public int situacao { get; set; }

        public virtual CAD_DEPT CAD_DEPT { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        public virtual CAD_ITEM CAD_ITEM { get; set; }

        public virtual cad_Situacao cad_Situacao { get; set; }
    }
}
