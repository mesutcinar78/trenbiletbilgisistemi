
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class Sefer
    {
        public int SeferID { get; set; }

        //[ForeignKey("Durak")]
        //public int CikisDurakID { get; set; }

        public int RotaID{ get; set; }
        [Column(TypeName = "time")]
        public TimeSpan CikisSaati { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan VarisSaati { get; set; }
        
        public DateTime Tarih { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan SeferSuresi { get; set; }
        public int TrenID { get; set; }

        public virtual Rota Rota { get; set; }
        public virtual List<Bilet> Biletler { get; set; }
        public virtual Tren Tren { get; set; }
    }
}
