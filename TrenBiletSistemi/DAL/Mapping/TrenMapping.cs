using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TrenMapping:EntityTypeConfiguration<Tren>
    {
        public TrenMapping()
        {
            ToTable("Tren");
            HasKey(x => x.TrenID);

            // HasRequired(x => x.Sefer).WithRequiredPrincipal(x => x.Tren);




        }

    }
}
