namespace IntraGriegHomolog.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbIntra : DbContext
    {
        public DbIntra()
            : base("name=DbIntra1")
        {
        }

        public virtual DbSet<CAD_COLABORADOR> CAD_COLABORADOR { get; set; }
        public virtual DbSet<CAD_DEPT> CAD_DEPT { get; set; }
        public virtual DbSet<CAD_EMP> CAD_EMP { get; set; }
        public virtual DbSet<CAD_ITEM> CAD_ITEM { get; set; }
        public virtual DbSet<CAD_ITEM_TYPE> CAD_ITEM_TYPE { get; set; }
        public virtual DbSet<CAD_LOCAL> CAD_LOCAL { get; set; }
        public virtual DbSet<cad_Situacao> cad_Situacao { get; set; }
        public virtual DbSet<CAD_TIPO_USER> CAD_TIPO_USER { get; set; }
        public virtual DbSet<IN_DESKTOP> IN_DESKTOP { get; set; }
        public virtual DbSet<IN_HISTORY> IN_HISTORY { get; set; }
        public virtual DbSet<IN_LINHA_MOVEL> IN_LINHA_MOVEL { get; set; }
        public virtual DbSet<IN_PRINTER> IN_PRINTER { get; set; }
        public virtual DbSet<IN_SMARTPHONE> IN_SMARTPHONE { get; set; }
        public virtual DbSet<IN_VOIP> IN_VOIP { get; set; }
        public virtual DbSet<NFE> NFE { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CAD_COLABORADOR>()
                .Property(e => e.nome)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_COLABORADOR>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_COLABORADOR>()
                .Property(e => e.passwd)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_COLABORADOR>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_COLABORADOR>()
                .Property(e => e.STATUS)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_DEPT>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_DEPT>()
                .HasMany(e => e.CAD_COLABORADOR)
                .WithOptional(e => e.CAD_DEPT)
                .HasForeignKey(e => e.dept);

            modelBuilder.Entity<CAD_DEPT>()
                .HasMany(e => e.IN_PRINTER)
                .WithOptional(e => e.CAD_DEPT)
                .HasForeignKey(e => e.DEPTID);

            modelBuilder.Entity<CAD_EMP>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_EMP>()
                .Property(e => e.cnpj)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_EMP>()
                .Property(e => e.endereco)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_EMP>()
                .Property(e => e.telefone)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_EMP>()
                .Property(e => e.cep)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.CAD_COLABORADOR)
                .WithOptional(e => e.CAD_EMP)
                .HasForeignKey(e => e.emp);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.CAD_DEPT)
                .WithOptional(e => e.CAD_EMP)
                .HasForeignKey(e => e.emp);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.IN_DESKTOP)
                .WithRequired(e => e.CAD_EMP)
                .HasForeignKey(e => e.emp)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.IN_LINHA_MOVEL)
                .WithRequired(e => e.CAD_EMP)
                .HasForeignKey(e => e.EMPID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.IN_PRINTER)
                .WithRequired(e => e.CAD_EMP)
                .HasForeignKey(e => e.EMPID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.IN_SMARTPHONE)
                .WithRequired(e => e.CAD_EMP)
                .HasForeignKey(e => e.EMP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.IN_VOIP)
                .WithRequired(e => e.CAD_EMP)
                .HasForeignKey(e => e.emp)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_EMP>()
                .HasMany(e => e.NFE)
                .WithOptional(e => e.CAD_EMP)
                .HasForeignKey(e => e.emp);

            modelBuilder.Entity<CAD_ITEM>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_PRINTER)
                .WithRequired(e => e.CAD_ITEM)
                .HasForeignKey(e => e.MODEL)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_DESKTOP)
                .WithOptional(e => e.CAD_ITEM)
                .HasForeignKey(e => e.monitor_client);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_DESKTOP1)
                .WithOptional(e => e.CAD_ITEM1)
                .HasForeignKey(e => e.modelo_client);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_DESKTOP2)
                .WithOptional(e => e.CAD_ITEM2)
                .HasForeignKey(e => e.disco_rigido);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_DESKTOP3)
                .WithOptional(e => e.CAD_ITEM3)
                .HasForeignKey(e => e.mem_ram);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_DESKTOP4)
                .WithOptional(e => e.CAD_ITEM4)
                .HasForeignKey(e => e.sis_oper);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_DESKTOP5)
                .WithOptional(e => e.CAD_ITEM5)
                .HasForeignKey(e => e.pct_office);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_SMARTPHONE)
                .WithRequired(e => e.CAD_ITEM)
                .HasForeignKey(e => e.model)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_ITEM>()
                .HasMany(e => e.IN_VOIP)
                .WithRequired(e => e.CAD_ITEM)
                .HasForeignKey(e => e.modelo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_ITEM_TYPE>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_ITEM_TYPE>()
                .Property(e => e.ITEM_TP)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_ITEM_TYPE>()
                .HasMany(e => e.CAD_ITEM)
                .WithOptional(e => e.CAD_ITEM_TYPE)
                .HasForeignKey(e => e.typeID);

            modelBuilder.Entity<CAD_ITEM_TYPE>()
                .HasMany(e => e.IN_HISTORY)
                .WithRequired(e => e.CAD_ITEM_TYPE)
                .HasForeignKey(e => e.TYPE_ITEM)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_ITEM_TYPE>()
                .HasMany(e => e.NFE)
                .WithRequired(e => e.CAD_ITEM_TYPE)
                .HasForeignKey(e => e.TYPE_ITEM)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_LOCAL>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_LOCAL>()
                .HasMany(e => e.CAD_EMP)
                .WithRequired(e => e.CAD_LOCAL)
                .HasForeignKey(e => e.local_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cad_Situacao>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<cad_Situacao>()
                .HasMany(e => e.IN_DESKTOP)
                .WithRequired(e => e.cad_Situacao)
                .HasForeignKey(e => e.sit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cad_Situacao>()
                .HasMany(e => e.IN_LINHA_MOVEL)
                .WithRequired(e => e.cad_Situacao)
                .HasForeignKey(e => e.situacao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cad_Situacao>()
                .HasMany(e => e.IN_PRINTER)
                .WithRequired(e => e.cad_Situacao)
                .HasForeignKey(e => e.situacao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cad_Situacao>()
                .HasMany(e => e.IN_SMARTPHONE)
                .WithRequired(e => e.cad_Situacao)
                .HasForeignKey(e => e.situacao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cad_Situacao>()
                .HasMany(e => e.IN_VOIP)
                .WithRequired(e => e.cad_Situacao)
                .HasForeignKey(e => e.situacao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CAD_TIPO_USER>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<CAD_TIPO_USER>()
                .HasMany(e => e.CAD_COLABORADOR)
                .WithOptional(e => e.CAD_TIPO_USER)
                .HasForeignKey(e => e.tipo_u);

            modelBuilder.Entity<IN_DESKTOP>()
                .Property(e => e.identificador)
                .IsUnicode(false);

            modelBuilder.Entity<IN_DESKTOP>()
                .Property(e => e.ip)
                .IsUnicode(false);

            modelBuilder.Entity<IN_DESKTOP>()
                .Property(e => e.k_so)
                .IsUnicode(false);

            modelBuilder.Entity<IN_DESKTOP>()
                .Property(e => e.k_office)
                .IsUnicode(false);
 

            modelBuilder.Entity<IN_DESKTOP>()
                .Property(e => e.info)
                .IsUnicode(false);

            modelBuilder.Entity<IN_DESKTOP>()
                .HasMany(e => e.CAD_COLABORADOR)
                .WithOptional(e => e.IN_DESKTOP)
                .HasForeignKey(e => e.desktop);

            modelBuilder.Entity<IN_HISTORY>()
                .Property(e => e.identificador)
                .IsUnicode(false);

            modelBuilder.Entity<IN_HISTORY>()
                .Property(e => e.descs)
                .IsUnicode(false);

            modelBuilder.Entity<IN_HISTORY>()
                .Property(e => e.tipo)
                .IsUnicode(false);

            modelBuilder.Entity<IN_LINHA_MOVEL>()
                .Property(e => e.DESCS)
                .IsUnicode(false);

            modelBuilder.Entity<IN_LINHA_MOVEL>()
                .Property(e => e.ICCID)
                .IsUnicode(false);

            modelBuilder.Entity<IN_LINHA_MOVEL>()
                .Property(e => e.custo_aparelho_plano)
                .IsUnicode(false);

            modelBuilder.Entity<IN_LINHA_MOVEL>()
             .Property(e => e.tipo_plano)
             .IsUnicode(false);

            modelBuilder.Entity<IN_LINHA_MOVEL>()
                .HasMany(e => e.IN_SMARTPHONE)
                .WithOptional(e => e.IN_LINHA_MOVEL)
                .HasForeignKey(e => e.linha_movel);

            modelBuilder.Entity<IN_LINHA_MOVEL>()
                .HasMany(e => e.IN_SMARTPHONE1)
                .WithOptional(e => e.IN_LINHA_MOVEL1)
                .HasForeignKey(e => e.linha_movel2);

            modelBuilder.Entity<IN_PRINTER>()
                .Property(e => e.SERIAL_NO)
                .IsUnicode(false);

            modelBuilder.Entity<IN_PRINTER>()
                .Property(e => e.IP)
                .IsUnicode(false);

            modelBuilder.Entity<IN_PRINTER>()
                .Property(e => e.APELIDO)
                .IsUnicode(false);

            modelBuilder.Entity<IN_PRINTER>()
                .Property(e => e.info)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .Property(e => e.imei)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .Property(e => e.imei2)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .Property(e => e.TERM_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .Property(e => e.TERM_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .Property(e => e.info)
                .IsUnicode(false);

            modelBuilder.Entity<IN_SMARTPHONE>()
                .HasMany(e => e.CAD_COLABORADOR)
                .WithOptional(e => e.IN_SMARTPHONE)
                .HasForeignKey(e => e.smartphone);

            modelBuilder.Entity<IN_VOIP>()
                .Property(e => e.passwd)
                .IsUnicode(false);

            modelBuilder.Entity<IN_VOIP>()
                .Property(e => e.ip)
                .IsUnicode(false);

            modelBuilder.Entity<IN_VOIP>()
                .Property(e => e.INFO)
                .IsUnicode(false);

            modelBuilder.Entity<NFE>()
                .Property(e => e.identificador)
                .IsUnicode(false);

            modelBuilder.Entity<NFE>()
                .Property(e => e.n_name)
                .IsUnicode(false);

            modelBuilder.Entity<NFE>()
                .Property(e => e.n_type)
                .IsUnicode(false);
        }

        public System.Data.Entity.DbSet<IntraGriegHomolog.Models.ViewModels.SmartphoneColaborador> SmartphoneColaboradors { get; set; }
    }
}
