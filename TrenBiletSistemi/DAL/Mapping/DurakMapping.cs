using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DurakMapping : EntityTypeConfiguration<Durak>
    {
        public DurakMapping()
        {
            ToTable("Duraklar");
            HasKey(x => x.DurakID);

            Property(x => x.DurakAdi).IsRequired().HasMaxLength(30);

            //eksik
        }

    }
}
