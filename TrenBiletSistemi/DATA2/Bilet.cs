using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Bilet
    {
        public int BiletID { get; set; }
        public bool BiletDurumu { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TcNo { get; set; }
        public bool Cinsiyet { get; set; }
        public bool YemekliMi { get; set; }
        public bool CocukMu { get; set; }
        public bool SigortaliMi { get; set; }
        public bool YolculukHizmetiVarMi { get; set; }
        public int SeferID { get; set; }
        public int KullaniciID { get; set; }
        public float Fiyat { get; set; }
        public bool VagonSinifi { get; set; }
        public int KoltukNo { get; set; }
       public bool SilindiMi { get; set; }

        public virtual Sefer Sefer { get; set; }
        public virtual Kullanici Kullanici { get; set; }


    }
}
