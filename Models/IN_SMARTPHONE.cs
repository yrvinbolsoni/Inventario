namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IN_SMARTPHONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IN_SMARTPHONE()
        {
            CAD_COLABORADOR = new HashSet<CAD_COLABORADOR>();
        }

        public long id { get; set; }

        [Required]
        [StringLength(255)]

        [DisplayName("Número Serial")]
        public string serial_number { get; set; }
        [DisplayName("Modelo")]
        public int model { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Imei")]
        public string imei { get; set; }

        [StringLength(255)]
        [DisplayName("Imei 2")]
        public string imei2 { get; set; }

        [DisplayName("Data compra")]
        public DateTime DATA_COMPRA { get; set; }

        [DisplayName("Empresa")]
        public int EMP { get; set; }

        [StringLength(50)]
        [DisplayName("Nome Termo")]
        public string TERM_NAME { get; set; }

        [StringLength(50)]
        [DisplayName("Tipo Termo")]
        public string TERM_TYPE { get; set; }

        public byte[] TERM_FILE { get; set; }
        [DisplayName("Linha movel")]
        public long? linha_movel { get; set; }

        [DisplayName("Linha movel 2 ")]
        public long? linha_movel2 { get; set; }

        [StringLength(255)]
        public string info { get; set; }
        [DisplayName("Situação")]
        public int situacao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_COLABORADOR> CAD_COLABORADOR { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        public virtual CAD_ITEM CAD_ITEM { get; set; }

        public virtual cad_Situacao cad_Situacao { get; set; }

        public virtual IN_LINHA_MOVEL IN_LINHA_MOVEL { get; set; }

        public virtual IN_LINHA_MOVEL IN_LINHA_MOVEL1 { get; set; }
    }
}
