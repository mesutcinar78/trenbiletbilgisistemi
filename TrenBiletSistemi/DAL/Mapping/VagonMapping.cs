using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
     public class VagonMapping :EntityTypeConfiguration<Vagon>
    {
        public VagonMapping()
        {
            ToTable("Vagon");
            HasKey(x => x.VagonID);

            HasRequired(x => x.VagonTipi).WithMany(x => x.Vagonlar).HasForeignKey(x => x.VagonTipiID);
        }
    }
}
