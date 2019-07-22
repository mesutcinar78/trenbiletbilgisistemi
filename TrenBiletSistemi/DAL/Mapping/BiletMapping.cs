using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BiletMapping : EntityTypeConfiguration<Bilet>
    {
        public BiletMapping()
        {
            ToTable("Biletler");
            HasKey(x => x.BiletID);

            //Property(x => x.BiletDurumu).IsRequired();
            Property(x => x.Ad).IsRequired().HasMaxLength(30);
            Property(x => x.Soyad).IsRequired().HasMaxLength(30);
            Property(x => x.TcNo).IsRequired().HasMaxLength(11);
            Property(x => x.Fiyat).IsRequired();


            HasRequired(x => x.Sefer).WithMany(x => x.Biletler).HasForeignKey(x => x.SeferID);
            HasRequired(x => x.Kullanici).WithMany(x => x.Biletler).HasForeignKey(x => x.KullaniciID);



        }

    }
}
