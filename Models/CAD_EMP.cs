namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CAD_EMP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CAD_EMP()
        {
            CAD_COLABORADOR = new HashSet<CAD_COLABORADOR>();
            CAD_DEPT = new HashSet<CAD_DEPT>();
            IN_DESKTOP = new HashSet<IN_DESKTOP>();
            IN_LINHA_MOVEL = new HashSet<IN_LINHA_MOVEL>();
            IN_PRINTER = new HashSet<IN_PRINTER>();
            IN_SMARTPHONE = new HashSet<IN_SMARTPHONE>();
            IN_VOIP = new HashSet<IN_VOIP>();
            NFE = new HashSet<NFE>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Empresa")]
        public string descs { get; set; }

        [Required]
        [StringLength(255)]
        public string cnpj { get; set; }

        public string endereco { get; set; }

        [StringLength(14)]
        public string telefone { get; set; }

        [Required]
        [StringLength(50)]
        public string cep { get; set; }

        [DisplayName("Local")]
        public int local_id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_COLABORADOR> CAD_COLABORADOR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAD_DEPT> CAD_DEPT { get; set; }

        public virtual CAD_LOCAL CAD_LOCAL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_DESKTOP> IN_DESKTOP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_LINHA_MOVEL> IN_LINHA_MOVEL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_PRINTER> IN_PRINTER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_SMARTPHONE> IN_SMARTPHONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IN_VOIP> IN_VOIP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NFE> NFE { get; set; }
    }
}
