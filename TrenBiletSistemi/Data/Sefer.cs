using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Sefer
    {
        public int SeferID { get; set; }
        public int CikisDurakID { get; set; }
        public int VarisDurakID
        { get; set; }
        public DateTime CikisSaati { get; set; }
        public DateTime VarisSaati { get; set; }
        public DateTime Tarih { get; set; }
        public DateTime SeferSuresi { get; set; }
        public int TrenID { get; set; }
    }
}
