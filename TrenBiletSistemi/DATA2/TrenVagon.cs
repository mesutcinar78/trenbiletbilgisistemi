using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class TrenVagon
    {
        public int ID { get; set; }
        public int TrenID { get; set; }
        public int VagonID { get; set; }

        public virtual Tren Tren { get; set; }
        public virtual Vagon Vagon { get; set; }
    }
}
