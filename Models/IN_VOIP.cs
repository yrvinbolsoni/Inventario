namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IN_VOIP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IN_VOIP()
        {
            CAD_COLABORADOR = new HashSet<CAD_COLABORADOR>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ramal { get; set; }

        [Required]
        [StringLength(255)]
        public string passwd { get; set; }

        [StringLength(255)]
        public string ip { get; set; }

        public int emp { get; set; }

        public string INFO { get; set; }

        public int situacao { get; set; }

        public int modelo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_COLABORADOR> CAD_COLABORADOR { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        public virtual CAD_ITEM CAD_ITEM { get; set; }

        public virtual cad_Situacao cad_Situacao { get; set; }
    }
}
