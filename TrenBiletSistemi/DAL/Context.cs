using DAL.Mapping;
using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Context: DbContext
    {
        //Çıkış saati, varış saati, süre leri time yap

        public Context()
        {

            //BU KISMA KENDİ CONNECTION STRING YAPINIZI EKLEYİNİZ !

            //Database.Connection.ConnectionString = "server = .; database = TrenBiletDb; uid = sa; pwd = 123";
            Database.Connection.ConnectionString = "server = DESKTOP-N13DB8I\\SQLEXPRESS; database = TrenBiletDb; Trusted_Connection = true;";
           // Database.Connection.ConnectionString = "server = (localdb)//mssqllocaldb; database = TrenBiletDb; uid = sa; pwd = 123";


        }

        public DbSet<Bilet> Biletler { get; set; }
        public DbSet<Durak> Duraklar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<KullaniciTip> KullaniciTipleri { get; set; }
        public DbSet<Sefer> Seferler { get; set; }
        public DbSet<Tren> Trenler { get; set; }
        public DbSet<Vagon> Vagonlar { get; set; }
        public DbSet<VagonTipi> VagonTipleri { get; set; }
        public DbSet<Rota> Rotalar { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BiletMapping());
            modelBuilder.Configurations.Add(new DurakMapping());
            modelBuilder.Configurations.Add(new KullaniciMapping());
            modelBuilder.Configurations.Add(new KullaniciTipMapping());
            modelBuilder.Configurations.Add(new SeferMapping());
            modelBuilder.Configurations.Add(new TrenMapping());
            modelBuilder.Configurations.Add(new VagonMapping());
            modelBuilder.Configurations.Add(new VagonTipiMapping());
            modelBuilder.Configurations.Add(new RotaMapping());
            modelBuilder.Configurations.Add(new TrenVagonMapping());

           /* modelBuilder.Entity<Bilet>()
          .HasOptional(x => x.DoluKoltuk).WithMany()
          .HasForeignKey(x => x.KoltukID);
          
            modelBuilder.Entity<DoluKoltuk>()
                .HasRequired(x => x.Bilet).WithMany()
                .HasForeignKey(x => x.KoltukID);*/

            base.OnModelCreating(modelBuilder);
        }
    }

}
