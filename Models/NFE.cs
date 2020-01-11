namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NFE")]
    public partial class NFE
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string identificador { get; set; }

        public int? emp { get; set; }

        [Required]
        [StringLength(255)]
        public string n_name { get; set; }

        [Required]
        [StringLength(255)]
        public string n_type { get; set; }

        [Required]
        public byte[] n_file { get; set; }

        public int TYPE_ITEM { get; set; }

        public virtual CAD_EMP CAD_EMP { get; set; }

        public virtual CAD_ITEM_TYPE CAD_ITEM_TYPE { get; set; }
    }
}
