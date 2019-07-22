
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Kullanici
    {
        public int KullaniciID { get; set; }
        public int KullaniciTipID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TcNo { get; set; }
        public bool Cinsiyet { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Sifre { get; set; }

        public virtual List<Bilet> Biletler { get; set; }
        public virtual KullaniciTip KullaniciTip { get; set; }
    }
}
