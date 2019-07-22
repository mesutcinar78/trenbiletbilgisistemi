using Data;
using System.Data.Entity.ModelConfiguration;

namespace DAL
{
    public class KullaniciTipMapping: EntityTypeConfiguration<KullaniciTip>
    {
        public KullaniciTipMapping()
        {
            ToTable("KullanıcıTipleri");
            Property(x => x.TipAdi).IsRequired();
            HasKey(x => x.TipID);


        }
    }
}
