namespace IntraGriegHomolog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IN_HISTORY
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Identificador")]
        public string identificador { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Descrição")]
        public string descs { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Tipo")]
        public string tipo { get; set; }

        [DisplayName("Data Hora")]
        public DateTime dt { get; set; }

        [DisplayName("Tipo Item")]
        public int TYPE_ITEM { get; set; }

        public virtual CAD_ITEM_TYPE CAD_ITEM_TYPE { get; set; }
    }
}
