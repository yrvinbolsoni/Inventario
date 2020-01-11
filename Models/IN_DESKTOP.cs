namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IN_DESKTOP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IN_DESKTOP()
        {
            CAD_COLABORADOR = new HashSet<CAD_COLABORADOR>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string identificador { get; set; }

        [StringLength(255)]
        public string ip { get; set; }

        public int emp { get; set; }

        [DisplayName("Data da Compra")]
        public DateTime? dt_compra { get; set; }

        [StringLength(255)]
        [DisplayName("Chave Sistema Operacional")]
        public string k_so { get; set; }

        [StringLength(255)]
        [DisplayName("Chave Microsoft Office")]
        public string k_office { get; set; }
 

        public string info { get; set; }
        [DisplayName("Monitor")]
        public int? monitor_client { get; set; }

        [DisplayName("Modelo")]
        public int? modelo_client { get; set; }

        [DisplayName("HD")]
        public int? disco_rigido { get; set; }

        [DisplayName("Memoria")]
        public int? mem_ram { get; set; }

        [DisplayName("Chave Sistema Operacional")]
        public int? sis_oper { get; set; }

        [DisplayName("Chave Microsoft Office")]
        public int? pct_office { get; set; }

        [DisplayName("Situação")]
        public int sit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_COLABORADOR> CAD_COLABORADOR { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        public virtual CAD_ITEM CAD_ITEM { get; set; }

        public virtual CAD_ITEM CAD_ITEM1 { get; set; }

        public virtual CAD_ITEM CAD_ITEM2 { get; set; }

        public virtual CAD_ITEM CAD_ITEM3 { get; set; }

        public virtual CAD_ITEM CAD_ITEM4 { get; set; }

        public virtual CAD_ITEM CAD_ITEM5 { get; set; }

        public virtual cad_Situacao cad_Situacao { get; set; }
    }
}
