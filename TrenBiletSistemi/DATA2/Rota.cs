using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Rota
    {
        public int RotaID { get; set; }
        public int CikisID { get; set; }
        public int VarisID { get; set; }

        public virtual Durak Durak { get; set; }
        public virtual List<Sefer> Seferler { get; set; }
    }
}
