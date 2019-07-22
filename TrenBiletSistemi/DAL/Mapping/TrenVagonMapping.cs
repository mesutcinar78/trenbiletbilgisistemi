using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Mapping
{
    public class TrenVagonMapping : EntityTypeConfiguration<TrenVagon>
    {
        public TrenVagonMapping()
        {
            ToTable("TrenVagon");
            HasKey(x => x.ID);

            HasRequired(x => x.Tren).WithMany(x => x.TreninVagonlari).HasForeignKey(x => x.TrenID);
            HasRequired(x => x.Vagon).WithMany(x => x.VagonunTrenleri).HasForeignKey(x => x.VagonID);
        }
    }
}
