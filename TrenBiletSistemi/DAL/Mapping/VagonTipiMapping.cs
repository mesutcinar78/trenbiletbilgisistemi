using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class VagonTipiMapping :EntityTypeConfiguration<VagonTipi>
    {
        public VagonTipiMapping()
        {
            ToTable("VagonTipleri");
            HasKey(x => x.TipID);
            Property(x => x.TipAdi).IsRequired();
        }
    }
}
