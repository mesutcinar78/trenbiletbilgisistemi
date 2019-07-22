using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SeferMapping : EntityTypeConfiguration<Sefer>
    {
        public SeferMapping()
        {
            ToTable("Seferler");
            HasKey(x => x.SeferID);

            HasRequired(x => x.Rota).WithMany(x => x.Seferler).HasForeignKey(x => x.RotaID);
            HasRequired(x => x.Tren).WithMany(x => x.Seferler).HasForeignKey(x => x.TrenID);
            //eksik
        }
    }
}
