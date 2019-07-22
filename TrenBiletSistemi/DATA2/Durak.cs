using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class Durak
    {

        
        public int DurakID { get; set; }

        public string DurakAdi { get; set; }

        /*[ForeignKey("Sefer")]
        public int SeferID { get; set; }*/

        public virtual List<Rota> Rotalar { get; set; }

    }
}
