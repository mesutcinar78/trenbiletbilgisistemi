using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class KullaniciMapping : EntityTypeConfiguration<Kullanici>
    {
        public KullaniciMapping()
        {
            ToTable("Kullanıcılar");
            HasKey(x => x.KullaniciID);
            Property(x => x.Ad).IsRequired().HasMaxLength(30);
            Property(x => x.Soyad).IsRequired().HasMaxLength(30);
            Property(x => x.TcNo).IsRequired();
            Property(x => x.Adres).IsRequired().HasMaxLength(250);
            Property(x => x.Email).IsRequired().HasMaxLength(40);
            Property(x => x.Telefon).IsRequired().HasMaxLength(25);

            HasRequired(x => x.KullaniciTip).WithMany(x => x.Kullanicilar).HasForeignKey(x => x.KullaniciTipID);

        }
    }
}
