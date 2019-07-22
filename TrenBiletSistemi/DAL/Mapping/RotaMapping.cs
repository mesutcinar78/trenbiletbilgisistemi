using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RotaMapping : EntityTypeConfiguration<Rota>
    {
        public RotaMapping()
        {
            ToTable("Rotalar");
            HasKey(x => x.RotaID);

            HasRequired(x => x.Durak).WithMany(x => x.Rotalar).HasForeignKey(x => x.CikisID);
            HasRequired(x => x.Durak).WithMany(x => x.Rotalar).HasForeignKey(x => x.VarisID);
        }
       

    }
}
