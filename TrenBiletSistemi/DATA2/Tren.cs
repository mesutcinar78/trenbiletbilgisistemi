using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class Tren
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrenID { get; set; }

        public virtual List<Sefer> Seferler { get; set; }
        public virtual List<TrenVagon> TreninVagonlari { get; set; }
    }
}
