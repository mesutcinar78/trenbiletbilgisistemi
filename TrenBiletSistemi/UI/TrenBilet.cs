using DAL;
using DAL.Repositories;
using DAL.UnitOfWork;
using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class TrenBilet : Form
    {
        #region DbRepo
        private Context trenDb;
        private IUnitOfWork uow;
        private IRepository<Kullanici> kullaniciRepo;
        private IRepository<Bilet> biletRepo;
        private IRepository<Durak> durakRepo;
        private IRepository<Rota> rotaRepo;
        private IRepository<Sefer> seferRepo;
        private IRepository<Tren> trenRepo;
        private IRepository<Vagon> vagonRepo;
        private IRepository<TrenVagon> trenVagonRepo;
        private IRepository<VagonTipi> vagonTipiRepo;
        private IRepository<KullaniciTip> kullaniciTipRepo;
        Bilet bilet;
        Sefer sefer;
        bool cinsiyet;
        #endregion

        #region Diğer Değişkenler
        int donusBileti = 0;        //Gidiş dönüş bileti isteniyorsa bu değer 1 olur, gidiş  bileti alınınca 2 olur.
        int gidisSeferID, donusSeferID, cikisId, varisId;
        DateTime gidisTarihi, donusTarihi;
        TimeSpan gidisSaati;
        int biletSinifi; //1 ekonomi 2 business
        int yolcuSayisi;
        int aktifYolcu = 1;
        int secilenKoltuk;
        string sinif;
        Kullanici kullanici;
        decimal fiyat = 70;
        Cinsiyet cns;
        int koltukGidis = 0;   //Dönüş bileti de alınacaksa 1 olur.
        bool biletDurum;        //satın almaysa true rezervasyonsa false
        PictureBox secilenPb;
        bool ayniKullanici = false;//alınan biletleri aynı kullanıcı alıyorsa kız erkek ayrımı yapma.
        bool uyeDegil = false;      //Uye olmadan giriş yapılması durumunu kontrol eder.
        DateTime bugun;
        int secilenBiletId;
        bool adminGirisYapti;
        TimeSpan gidisSeferSaati;       //Dönüş sefer saatinin kontrolü için tutulacak.
        #endregion

        public void DbInitialize()
        {
            trenDb = new Context();
            // EFBlogContext'i kullanıyor olduğumuz için EFUnitOfWork'den türeterek constructor'ına
            // ilgili context'i constructor injection yöntemi ile inject ediyoruz.
            uow = new EFUnitOfWork(trenDb);
            kullaniciRepo = new EFRepository<Kullanici>(trenDb);
            biletRepo = new EFRepository<Bilet>(trenDb);
            durakRepo = new EFRepository<Durak>(trenDb);
            rotaRepo = new EFRepository<Rota>(trenDb);
            seferRepo = new EFRepository<Sefer>(trenDb);
            trenRepo = new EFRepository<Tren>(trenDb);
            vagonRepo = new EFRepository<Vagon>(trenDb);
            trenVagonRepo = new EFRepository<TrenVagon>(trenDb);
            vagonTipiRepo = new EFRepository<VagonTipi>(trenDb);
            kullaniciTipRepo = new EFRepository<KullaniciTip>(trenDb);
        }

        public TrenBilet()
        {
            InitializeComponent();
            DbInitialize();
            bilet = new Bilet();
            sefer = new Sefer();

            #region Durakları Doldur
            var duraklar = durakRepo.GetAll().ToList();

            foreach (var item in duraklar)
            {
                ComboboxItem itemCmb = new ComboboxItem();
                itemCmb.Text = item.DurakAdi;
                itemCmb.Value = item.DurakID;

                cmbNereden.Items.Add(itemCmb);
            }
            #endregion

            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = false;
        }


        private void TrenBilet_Load(object sender, EventArgs e)
        {
            //Tab butonlarını gizle
            TrenTab.Appearance = TabAppearance.FlatButtons;
            TrenTab.ItemSize = new Size(0, 1);
            TrenTab.SizeMode = TabSizeMode.Fixed;

            //Geçmişe dair bilet alınmasını engellemek ve en fazla 14 gün sonrasına bilet vermek için.
            dtGidis.MinDate = DateTime.Today;
            dtGidis.MaxDate = DateTime.Today.AddDays(14);
            dtDonus.MinDate = DateTime.Today;
            dtDonus.MaxDate = DateTime.Today.AddDays(14);

            IlkKayitlariYap();

            lblFiyat.Text = 0.ToString("C");

        }



        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            btnGuvenliCikis.Text = "Çıkış Yap";
            var girisYapan = kullaniciRepo.Get(x => x.Email == txtEposta.Text && x.Sifre == txtSifre.Text);
            if (girisYapan.KullaniciTipID == 2)
            {
                adminGirisYapti = true;
            }
            else
            {
                adminGirisYapti = false;
            }
            if (girisYapan != null)
            {
                btnBiletlerim.Visible = true;
                uyeDegil = false;
                kullanici = new Kullanici();
                kullanici.KullaniciID = girisYapan.KullaniciID;
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;     //Bir sonraki tab'e gönderir.
            }
            else
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
        }

        private void btnUyeOl_Click(object sender, EventArgs e)
        {
            UyeOl uyelik = new UyeOl();
            uyelik.Show();
            this.Hide();
        }

        private void btnDevamEt_Click(object sender, EventArgs e)
        {
            uyeDegil = true;
            btnBiletlerim.Visible = false;
            btnGuvenliCikis.Text = "Geri";
            kullanici = new Kullanici();
            kullanici.KullaniciID = 1;      //Üye olmadan devam edildiği takdirde alınan biletlere kullanıcıID olarak adminin kullaniciID'si verilir.
            TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
        }

        private void cmbNereden_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Nereden comboboxından seçilen durağa göre nereye comboxını doldur. Örneğin İstanbul seçilirse varış durağı olarak İstanbul harici tüm duraklar alınır.
            var duraklar = durakRepo.GetAll().ToList();
            cmbNereye.Items.Clear();

            foreach (var item in duraklar)
            {
                ComboboxItem itemCmb = new ComboboxItem();
                itemCmb.Text = item.DurakAdi;
                itemCmb.Value = item.DurakID;

                if (cmbNereden.Text != itemCmb.Text)
                {
                    cmbNereye.Items.Add(itemCmb);
                }
            }
        }

        private void rdoGidisDonus_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoGidisDonus.Checked)
            {
                dtDonus.Enabled = true;
                //Aşağıdaki iki değişken dönüş seferi seçimi ve dönüş biletlerinin alınması için gereklidir.
                donusBileti = 1;
                koltukGidis = 1;
            }
        }

        private void rdoTekyon_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTekyon.Checked)
            {
                dtDonus.Enabled = false;
                donusBileti = 0;
            }
            else
            {
                dtDonus.Enabled = true;
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            lblYon.Text = "Gidiş Yönü";
            biletDurum = rdoSatinAl.Checked;
            yolcuSayisi = Convert.ToInt32(nmrYolcuSayisi.Value);
            donusBileti = 0;
            if (rdoGidisDonus.Checked)
            {
                donusBileti = 1;

            }
            if (OrtakMetodlar.BosAlanVarMi(grpDurak))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
            else
            {
                lblSeferBilgi.Text = "Lütfen gidiş seferini seçiniz: ";
                cikisId = Convert.ToInt32(((ComboboxItem)cmbNereden.SelectedItem).Value);
                varisId = Convert.ToInt32(((ComboboxItem)cmbNereye.SelectedItem).Value);
                gidisTarihi = new DateTime(dtGidis.Value.Year, dtGidis.Value.Month, dtGidis.Value.Day, 0, 0, 0);
                gidisSaati = DateTime.Now.TimeOfDay;

                if (donusBileti == 1)    //Gidiş dönüş seçilmişse
                {
                    donusTarihi = new DateTime(dtDonus.Value.Year, dtDonus.Value.Month, dtDonus.Value.Day, 0, 0, 0);


                    if (SeferVarMi(cikisId, varisId, gidisTarihi, gidisSaati) && SeferVarMi(varisId, cikisId, donusTarihi, gidisSaati))
                    {
                        TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
                    }
                    else
                    {
                        MessageBox.Show("Sefer(ler) bulunamadı.");
                    }
                }

                else if (donusBileti == 0)    //Tek yön seçilmişse
                {
                    if (SeferVarMi(cikisId, varisId, gidisTarihi, gidisSaati))
                    {
                        TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
                    }
                    else
                    {
                        MessageBox.Show("Sefer bulunamadı.");
                    }
                }

                SeferleriGetir(cikisId, varisId, gidisTarihi, gidisSaati);
            }
        }

        private void pbBusiness_Click(object sender, EventArgs e)
        {
            grpKoltukBusiness.Visible = true;
            grpKoltukEkonomi.Visible = false;

            biletSinifi = 2;
            KoltuklariDoldur(gidisSeferID);

            fiyat = 100;
            lblFiyat.Text = fiyat.ToString("C");


        }

        private void pbEko_Click(object sender, EventArgs e)
        {
            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = true;

            biletSinifi = 1;
            KoltuklariDoldur(gidisSeferID);
            fiyat = 70;
            lblFiyat.Text = fiyat.ToString("C");

        }

        private void koltukSecildi(object sender, EventArgs e)
        {
            //Tıklanan koltuk secilenPb isimli PictureBox'ta tutulur.
            secilenPb = (sender as PictureBox);
            //Seçilen koltuğun numarası koltuğun tag'inde tutulduğundan tag üzerinden direk alınabilir.
            secilenKoltuk = Convert.ToInt32((sender as PictureBox).Tag.ToString());

            cns = new Cinsiyet();
            this.Enabled = false;
            cns.Show();

            //Herhangi bir koltuk seçildiğinde cinsiyet seçimi yapıldıktan sonra işlemler gerçekleşir.
            cns.btnCinsiyet.Click += BtnCinsiyet_Click;



        }

        private void BtnCinsiyet_Click(object sender, EventArgs e)
        {
            cinsiyet = cns.rdoErkek.Checked;
            KoltuklariDoldur(gidisSeferID);
            PictureBox yanKoltuk;
            int yanKoltukNo;

            //Seçilen koltuğun yan koltuğun bulunup yanda oturan birisi varsa, girilen cinsiyetle aynı olup olmadığına bakılır. Farklıysa uyarı verilir.
            if (secilenKoltuk % 2 == 0)
            {
                yanKoltukNo = secilenKoltuk - 1;
                string yanKoltukName = secilenPb.Name.Substring(0, 1).ToString() + (yanKoltukNo).ToString();        //substring ile tıklanan koltuğun name'inin ilk harfi alınır. Yani business ise "b" ekonomi ise "e" değeri alınır. Bu şekilde ilgili pictureboxa ulaşılır.
                yanKoltuk = this.Controls.Find(yanKoltukName, true).FirstOrDefault() as PictureBox;
            }
            else
            {
                yanKoltukNo = secilenKoltuk + 1;
                string yanKoltukName = secilenPb.Name.Substring(0, 1).ToString() + (yanKoltukNo).ToString();
                yanKoltuk = this.Controls.Find(yanKoltukName, true).FirstOrDefault() as PictureBox;
            }


            //Yan koltuk boş değilse işleme başla.. Burada yan koltukta bayan yada erkek oturması durumu image tag özelliğiyle kontrol altında tutulmuştur. 
            //Bir bayana ait koltuğun image tag'i "b", erkeğe ait koltuğun image tag'i ise "e" olur.
            //Eğer iki bilet aynı kişiye aitse kadın erkek yan yana oturabilir.
            if (yanKoltuk.Image.Tag != null)
            {
                if (YanKoltukAyniKisiyeMiAit(yanKoltukNo) || uyeDegil)
                //if (YanKoltukAyniKisiyeMiAit(yanKoltukNo) )
                {
                    if (cinsiyet == false && yanKoltuk.Image.Tag.ToString() == "e")     //Seçilen cinsiyet kadınsa ve yanında erkek oturuyorsa..
                    {
                        MessageBox.Show("Yanyana oturacak yolcuların cinsiyeti aynı olmalıdır.");
                    }
                    else if (cinsiyet == true && yanKoltuk.Image.Tag.ToString() == "b")     //Seçilen cinsiyet erkekse ve yanında kadın oturuyorsa..
                    {
                        MessageBox.Show("Yanyana oturacak yolcuların cinsiyeti aynı olmalıdır.");
                    }
                }
                else   //Cinsiyetler farklı değilse bilet almaya izin verir.
                {
                    if (biletDurum == true)
                        secilenPb.Image = UI.Properties.Resources.seciliSatinAl;
                    else if (biletDurum == false)
                        secilenPb.Image = UI.Properties.Resources.seciliRezerve;
                }


            }
            else   //Yan koltuk boşsa kontrol etmeden bilet almaya izin verir.
            {
                if (biletDurum == true)
                    secilenPb.Image = UI.Properties.Resources.seciliSatinAl;
                else if (biletDurum == false)
                    secilenPb.Image = UI.Properties.Resources.seciliRezerve;
            }

            this.Enabled = true;
            cns.Hide();
            // this.BringToFront();

        }

        private bool YanKoltukAyniKisiyeMiAit(int koltukNumarasi)
        {
            var yanKoltuk = biletRepo.GetAll(x => x.KullaniciID == kullanici.KullaniciID && x.KoltukNo == koltukNumarasi && x.SeferID == gidisSeferID).ToList();
            if (yanKoltuk.Count != 0) return false;
            else
            return true;
        }

        private void KoltuklariDoldur(int seferID)
        {
            //İlgili sefere ait satılmış veya rezerve edilmiş biletleri koltuklara doldur.

            if (biletSinifi == 1)  //Ekonomi
            {
                var alinmisBiletler = biletRepo.GetAll(x => x.SeferID == seferID && x.VagonSinifi == false).ToList();

                foreach (Control koltuk in grpKoltukEkonomi.Controls)
                {
                    if (koltuk is PictureBox && koltuk.Name.StartsWith("kd") == false)      //Koltuk olan pictureboxları al.
                    {
                        ((PictureBox)koltuk).Image = UI.Properties.Resources.bos1;      //Önce tüm koltukları boşalt.
                        ((PictureBox)koltuk).Enabled = true;
                        ((PictureBox)koltuk).Image.Tag = null;
                    }
                }

                foreach (var item in alinmisBiletler)
                {
                    if (item.Cinsiyet == false)  //bayan adına alınan biletlere doluKadin isimli görseli yükle
                    {
                        string pbName = "e" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluKadin;
                        pb.Image.Tag = "b";
                        pb.Enabled = false;
                    }
                    else if (item.Cinsiyet == true)  // erkek adına alınan biletlere doluErkek isimli görseli yükle
                    {
                        string pbName = "e" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluErkek;
                        pb.Image.Tag = "e";
                        pb.Enabled = false;
                    }
                }

            }
            else if (biletSinifi == 2)  //Business || Ekonomiyle ikisinin ayrı ayrı yapılmasının sebebi; ekonomi koltuklarının namei e ile başlar, businessın b ile.
            {
                var alinmisBiletler = biletRepo.GetAll(x => x.SeferID == seferID && x.VagonSinifi == true).ToList();

                foreach (Control koltuk in grpKoltukBusiness.Controls)
                {
                    if (koltuk is PictureBox && koltuk.Name.StartsWith("kd") == false)
                    {
                        ((PictureBox)koltuk).Image = UI.Properties.Resources.bos1;
                        ((PictureBox)koltuk).Enabled = true;
                    }
                }

                foreach (var item in alinmisBiletler)
                {
                    if (item.Cinsiyet == false)  //satın alınan ve bayan
                    {
                        string pbName = "b" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluKadin;
                        pb.Image.Tag = "b";
                        pb.Enabled = false;
                    }
                    else if (item.Cinsiyet == true)  //satın alınan ve erkek
                    {
                        string pbName = "b" + item.KoltukNo;
                        PictureBox pb = this.Controls.Find(pbName, true).FirstOrDefault() as PictureBox;
                        pb.Image = UI.Properties.Resources.doluErkek;
                        pb.Image.Tag = "e";
                        pb.Enabled = false;
                    }
                }
            }
        }

        private void BiletKoltuk_Click(object sender, EventArgs e)
        {

        }

        private void FiyatBelirle(string chkName, int miktar)
        {


            CheckBox tiklanan = this.Controls.Find(chkName, true).FirstOrDefault() as CheckBox;

            if (tiklanan.Checked)
            {
                fiyat += miktar;
            }
            else
            {
                fiyat -= miktar;
            }

            lblFiyat.Text = fiyat.ToString("C");
        }

        private void btnSatinAl_Click(object sender, EventArgs e)
        {
            if (!OrtakMetodlar.BosAlanVarMi(pnlKisi))
            {
                Bilet bilet = new Bilet
                {
                    Ad = txtAd.Text,
                    Soyad = txtSoyad.Text,
                    TcNo = txtTc.Text,
                    BiletDurumu = biletDurum,     //Satın almada true, rezervasyonda false
                    Cinsiyet = cinsiyet,
                    CocukMu = rdoCocuk.Checked,
                    SigortaliMi = chkSigortali.Checked,
                    YemekliMi = chkYemekli.Checked,
                    KoltukNo = secilenKoltuk,
                    YolculukHizmetiVarMi = ckhEkstraHizmet.Checked,
                    SeferID = gidisSeferID,
                    KullaniciID = kullanici.KullaniciID,
                    Fiyat = Convert.ToInt32(fiyat),
                    VagonSinifi = biletSinifi == 1 ? false : true   //biletSinifi = 1 ise ekonomi seçilmiştir. vagon sınıfı false olur. 1 değilse business seçilmiştir. Vagon sınıfı true olur.
                };



                if (biletDurum == true)
                {
                    DialogResult reply = MessageBox.Show("Bilet fiyatı = " + fiyat.ToString("C") + "\nÖdeme işlemini onaylıyor musunuz?",
               "Ödeme işlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (reply == DialogResult.Yes)
                    {
                        biletRepo.Add(bilet);

                        uow.SaveChanges();
                        MessageBox.Show("Bilet satın alma işlemi yapılmıştır.");

                        if (uyeDegil)
                            MessageBox.Show("Bilet numaranız: " + bilet.BiletID.ToString() + " \nLütfen bilet numaranızı kaybetmeyiniz.");

                        KoltuklariDoldur(gidisSeferID);     //bilet satıldıktan sonra yeni haliyle koltukları güncelle ve kullanıcı bilgileri kısmını temizle, yeni bilet alma işlemleri için hazırla.
                        OrtakMetodlar.Temizle(pnlKisi);

                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    biletRepo.Add(bilet);
                    uow.SaveChanges();
                    if (uyeDegil)
                        MessageBox.Show("Bilet numaranız: " + bilet.BiletID.ToString() + " \nLütfen bilet numaranızı kaybetmeyiniz.");
                    MessageBox.Show("Bilet rezervasyon işlemi yapılmıştır.");
                }

                yolcuSayisi--;      //Yolcu sayısı 0'a ulaşana kadar azalt, böylece her yolcuya bilet al.
                aktifYolcu++;       //O anda bilet alınan yolcuyu tut.

                if (yolcuSayisi != 0)
                {
                    ayniKullanici = true;
                    YeniBiletAl();
                }
                else
                {
                    ayniKullanici = false;

                    if (koltukGidis == 1)           //Dönüş bileti isteniyorsa tekrar bilet al
                    {
                        lblYon.Text = "Dönüş Yönü";
                        gidisSeferID = donusSeferID;
                        aktifYolcu = 1;
                        yolcuSayisi = Convert.ToInt32(nmrYolcuSayisi.Value);
                        YeniBiletAl();
                        koltukGidis = 0;
                    }
                    else
                    {
                        if (uyeDegil == true)
                            TrenTab.SelectedIndex = 5;
                        else
                            TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;

                        BiletleriDoldur();
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
        }

        public void BiletleriDoldur()
        {
            var biletler = biletRepo.GetAll(x => x.KullaniciID == kullanici.KullaniciID && (x.BiletID.ToString()).StartsWith(txtBiletId.Text) && x.SilindiMi == false).ToList();
            if (adminGirisYapti)
            {
                biletler = biletRepo.GetAll(x => (x.BiletID.ToString()).StartsWith(txtBiletId.Text) && x.SilindiMi == false).ToList();
            }
            lstBiletler.Items.Clear();
            foreach (var item in biletler)
            {
                string cikisSaat = string.Format("{0:00}:{1:00}", item.Sefer.CikisSaati.Hours, item.Sefer.CikisSaati.Minutes);
                string tarih = string.Format("{0}.{1}.{2}", item.Sefer.Tarih.Day, item.Sefer.Tarih.Month, item.Sefer.Tarih.Year);

                ListViewItem item1 = new ListViewItem(item.BiletID.ToString());
                item1.SubItems.Add(tarih);
                item1.SubItems.Add(item.Ad);
                item1.SubItems.Add(item.Soyad);
                string cikisDurak = durakRepo.GetAll(x => x.DurakID == item.Sefer.Rota.CikisID).Select(x => x.DurakAdi).SingleOrDefault();
                string varisDurak = durakRepo.GetAll(x => x.DurakID == item.Sefer.Rota.VarisID).Select(x => x.DurakAdi).SingleOrDefault();
                item1.SubItems.Add(cikisDurak);
                item1.SubItems.Add(varisDurak);
                item1.SubItems.Add(cikisSaat);
                item1.SubItems.Add(item.SigortaliMi ? "Var" : "Yok");
                item1.SubItems.Add(item.YemekliMi ? "Var" : "Yok");
                item1.SubItems.Add(item.YolculukHizmetiVarMi ? "Var" : "Yok");
                item1.SubItems.Add(item.Fiyat.ToString());
                item1.SubItems.Add(item.BiletDurumu ? "Satın Alındı." : "Rezerve");
                lstBiletler.Items.Add(item1);

            }

        }

        public void YeniBiletAl()
        {
            KoltukSecimiYolcu.Text = aktifYolcu.ToString();
            KoltuklariDoldur(gidisSeferID);
            OrtakMetodlar.Temizle(pnlKisi);
            rdoYetiskin.Checked = true;
            lblFiyat.Text = 0.ToString("C");
            grpKoltukBusiness.Visible = false;
            grpKoltukEkonomi.Visible = false;
        }

        private void TrenTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TrenTab.SelectedIndex == 4)
            {
                BiletleriDoldur();

            }
            else if (TrenTab.SelectedIndex == 1)
            {
                if (uyeDegil)
                    chkBilgiAl.Visible = false;
                else
                    chkBilgiAl.Visible = true;

            }
            else if (TrenTab.SelectedIndex == 6)
            {
                txtBiletNumarasi.Text = txtTcBilet.Text = "";
            }
            else if (TrenTab.SelectedIndex == 3)
            {
                KoltuklariDoldur(gidisSeferID);
            }
        }

        private void btnBiletlerim_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 4;
        }

        private void btnGuvenliCikis_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 0;
            OrtakMetodlar.Temizle(pnlGiris);
        }

        private void btnBiletiAl_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 1;
        }

        private void grpDurak_Enter(object sender, EventArgs e)
        {

        }

        private void btnSorgula_Click(object sender, EventArgs e)
        {
            BiletleriDoldur();
        }
        public bool SeferVarMi(int cikisId, int varisId, DateTime gidisTarihi, TimeSpan gidisSaati)      //aranan seferler varmı diye bakar. sefer yoksa kullanıcıya uyarı verir.
        {
            bugun = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            var rotaId = rotaRepo.GetAll(x => x.CikisID == cikisId && x.VarisID == varisId).Select(x => x.RotaID).SingleOrDefault();

            var seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi).ToList();


            if (gidisTarihi == bugun)       //Gidiş saati bugünse zamanı geçen seferleri gösterme.
            {
                seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && TimeSpan.Compare(x.CikisSaati, gidisSaati) == 1).ToList();

                if (donusBileti == 1)        //Dönüş bileti alınıyorsa getirilecek biletler gidiş biletinden ileri tarihe ve saate ait olmalı.
                {
                    seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && TimeSpan.Compare(x.CikisSaati, gidisSeferSaati) == 1 && TimeSpan.Compare(x.CikisSaati, gidisSaati) == 1).ToList();
                }
            }

            if (biletDurum == false && gidisTarihi == bugun)
            {     //2 saat kala seferleri rezervasyona kapat
                seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && x.CikisSaati.Hours > DateTime.Now.Hour + 2).ToList();

                if (donusBileti == 1)        //Dönüş bileti alınıyorsa getirelecek biletler gidiş biletinden ileri tarihe ve saate ait olmalı.
                {
                    seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && TimeSpan.Compare(x.CikisSaati, gidisSeferSaati) == 1 && x.CikisSaati.Hours > DateTime.Now.Hour + 2).ToList();
                }
            }

            if (seferler.Count == 0)
            {
                return false;
            }
            return true;
        }

        private void btnGeriSefer_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 1;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 2;
        }

        private void btnBiletAra_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 5;
        }

        private void btnBiletNoIleAra_Click(object sender, EventArgs e)
        {
            if (txtBiletNumarasi.Text != null && txtTcBilet.Text != null)
            {
                lblBiletNoSonuc.Text = "";
                try
                {
                    int bNo = int.Parse(txtBiletNumarasi.Text);
                    string tcNo = txtTcBilet.Text;
                    var biletim = biletRepo.GetAll(x => x.BiletID == bNo && x.TcNo == tcNo).SingleOrDefault();

                    string cikisDurak = durakRepo.GetAll(x => x.DurakID == biletim.Sefer.Rota.CikisID).Select(x => x.DurakAdi).SingleOrDefault();
                    string varisDurak = durakRepo.GetAll(x => x.DurakID == biletim.Sefer.Rota.VarisID).Select(x => x.DurakAdi).SingleOrDefault();
                    string cikisSaat = string.Format("{0:00}:{1:00}", biletim.Sefer.CikisSaati.Hours, biletim.Sefer.CikisSaati.Minutes);

                    lblBiletNoSonuc.Text = "Ad: " + biletim.Ad
                        + "\nSoyad: " + biletim.Soyad
                        + "\nÇıkış Durağı: " + cikisDurak
                        + "\nVarış Durağı: " + varisDurak
                        + "\nÇıkış Saati: " + cikisSaat
                        + "\nSigorta: " + (biletim.SigortaliMi ? "Var" : "Yok")
                        + "\nYemek: " + (biletim.YemekliMi ? "Var" : "Yok")
                        + "\nYolculuk Hizmeti: " + (biletim.YolculukHizmetiVarMi ? "Var" : "Yok")
                        + "\nÇocuk: " + (biletim.CocukMu ? "Evet" : "Hayır")
                        + "\nFiyat: " + (biletim.Fiyat.ToString());
                }
                catch
                {
                    MessageBox.Show("Bilet Bulunamadı.");
                }
            }

            else
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
        }

        private void Biletlerim_Click(object sender, EventArgs e)
        {

        }

        private void btnBiletAraGeri_Click(object sender, EventArgs e)
        {
            TrenTab.SelectedIndex = 0;
        }

        public void SeferleriGetir(int cikisId, int varisId, DateTime gidisTarihi, TimeSpan gidisSaati)
        {
            var rotaId = rotaRepo.GetAll(x => x.CikisID == cikisId && x.VarisID == varisId).Select(x => x.RotaID).SingleOrDefault();
            var seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi).ToList();

            if (gidisTarihi == bugun)       //Gidiş saati bugünse zamanı geçen seferleri gösterme.
            {
                seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && TimeSpan.Compare(x.CikisSaati, gidisSaati) == 1).ToList();

                if (donusBileti == 1)        //Dönüş bileti alınıyorsa getirilecek biletler gidiş biletinden ileri tarihe ve saate ait olmalı.
                {
                    seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && TimeSpan.Compare(x.CikisSaati, gidisSeferSaati) == 1 && TimeSpan.Compare(x.CikisSaati, gidisSaati) == 1).ToList();
                }
            }

            if (biletDurum == false && gidisTarihi == bugun)
            {     //2 saat kala seferleri rezervasyona kapat
                seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && x.CikisSaati.Hours > DateTime.Now.Hour + 2).ToList();

                if (donusBileti == 1)        //Dönüş bileti alınıyorsa getirelecek biletler gidiş biletinden ileri tarihe ve saate ait olmalı.
                {
                    seferler = seferRepo.GetAll(x => x.RotaID == rotaId && x.Tarih == gidisTarihi && TimeSpan.Compare(x.CikisSaati, gidisSeferSaati) == 1 && x.CikisSaati.Hours > DateTime.Now.Hour + 2).ToList();
                }
            }

            lstSeferler.Items.Clear();

            foreach (var item in seferler)
            {
                string cikisSaat = string.Format("{0:00}:{1:00}", item.CikisSaati.Hours, item.CikisSaati.Minutes);
                string varisSaat = string.Format("{0:00}:{1:00}", item.VarisSaati.Hours, item.VarisSaati.Minutes);
                string seferSure = string.Format("{0:00}:{1:00}", item.SeferSuresi.Hours, item.SeferSuresi.Minutes);
                string tarih = string.Format("{0}.{1}.{2}", item.Tarih.Day, item.Tarih.Month, item.Tarih.Year);

                ListViewItem item1 = new ListViewItem(item.SeferID.ToString());
                item1.SubItems.Add(cmbNereden.Text);
                item1.SubItems.Add(cmbNereye.Text);
                item1.SubItems.Add(cikisSaat);
                item1.SubItems.Add(varisSaat);
                item1.SubItems.Add(tarih);
                item1.SubItems.Add(seferSure);
                lstSeferler.Items.Add(item1);
            }
        }

        private void btnBiletIptal_Click(object sender, EventArgs e)
        {
            if (biletRepo.Delete(secilenBiletId))
                MessageBox.Show("Bilet Silindi!");
            else
                MessageBox.Show("Silinemedi");
            uow.SaveChanges();

            BiletleriDoldur();

        }

        private void lstBiletler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBiletler.SelectedItems.Count > 0)
            {
                secilenBiletId = Convert.ToInt32(lstBiletler.SelectedItems[0].SubItems[0].Text);
                var secilenBilet = biletRepo.Get(x => x.BiletID == secilenBiletId);
                if (secilenBilet.BiletDurumu == false)
                    btnRSatinAl.Enabled = true;
            }
        }

        private void btnRSatinAl_Click(object sender, EventArgs e)
        {

            Bilet satinAlinacakBilet = biletRepo.GetById(secilenBiletId);
            satinAlinacakBilet.BiletDurumu = true;

            DialogResult reply = MessageBox.Show("Bilet fiyatı = " + satinAlinacakBilet.Fiyat.ToString("C") + "\nÖdeme işlemini onaylıyor musunuz?",
"Ödeme işlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (reply == DialogResult.Yes)
            {
                biletRepo.Update(satinAlinacakBilet);

                uow.SaveChanges();
                MessageBox.Show("Bilet satın alma işlemi yapılmıştır.");
                BiletleriDoldur();
                btnRSatinAl.Enabled = false;

            }



        }

        private void rdoRezerve_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkBilgiAd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBilgiAl.Checked)
            {
                var Kayitlikullanici = kullaniciRepo.Get(x => x.KullaniciID == kullanici.KullaniciID);
                txtTc.Text = Kayitlikullanici.TcNo;
                txtAd.Text = Kayitlikullanici.Ad;
                txtSoyad.Text = Kayitlikullanici.Soyad;
            }
            else
            {
                OrtakMetodlar.Temizle(pnlKisi);
            }
        }

        private void rdoYetiskin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCocuk.Checked == true)
            {
                fiyat -= 35;
            }
            else
            {
                fiyat += 35;
            }
            lblFiyat.Text = fiyat.ToString("C");
        }

        private void chkYemekli_CheckedChanged(object sender, EventArgs e)
        {
            string senderName = ((CheckBox)sender).Name;
            if (biletSinifi == 1)
                FiyatBelirle(senderName, 20);
            else if (biletSinifi == 2)
                FiyatBelirle(senderName, 15);
        }

        private void chkSigortali_CheckedChanged(object sender, EventArgs e)
        {
            string senderName = ((CheckBox)sender).Name;
            FiyatBelirle(senderName, 10);
        }

        private void ckhEkstraHizmet_CheckedChanged(object sender, EventArgs e)
        {
            string senderName = ((CheckBox)sender).Name;
            FiyatBelirle(senderName, 10);
        }

        private void dtGidis_ValueChanged(object sender, EventArgs e)
        {
            dtDonus.MinDate = dtGidis.Value;
            dtDonus.Value = dtDonus.MinDate;

        }

        private void btnIleriSefer_Click(object sender, EventArgs e)
        {
            if (donusBileti == 1)
            {
                lblSeferBilgi.Text = "Lütfen dönüş seferini seçiniz: ";
                //Dönüş bileti isteniyorsa tekrar bu sayfaya dön. dönüş seferlerini listele.
                varisId = Convert.ToInt32(((ComboboxItem)cmbNereden.SelectedItem).Value);
                cikisId = Convert.ToInt32(((ComboboxItem)cmbNereye.SelectedItem).Value);
                SeferleriGetir(cikisId, varisId, donusTarihi, gidisSaati);

                donusBileti = 2;
            }
            else
            {
                TrenTab.SelectedIndex = (TrenTab.SelectedIndex + 1) % TrenTab.TabCount;
            }
        }

        private void lstSeferler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSeferler.SelectedItems.Count > 0)
            {
                if (donusBileti == 1 || donusBileti == 0)
                {
                    gidisSeferSaati = TimeSpan.Parse(lstSeferler.SelectedItems[0].SubItems[4].Text);
                    gidisSeferID = Convert.ToInt32(lstSeferler.SelectedItems[0].SubItems[0].Text);
                }
                else
                {
                    donusSeferID = Convert.ToInt32(lstSeferler.SelectedItems[0].SubItems[0].Text);
                }
            }


        }

        private void BiletSefer_Click(object sender, EventArgs e)
        {
        }

        private void IlkKayitlariYap()
        {
            //Durak yoksa durak ekle.
            var duraklar = durakRepo.GetAll().ToList();
            if (duraklar.Count == 0)
            {
                List<Durak> EklenecekDuraklar = new List<Durak>()
                {
                    new Durak() {DurakAdi = "İstanbul"},
                    new Durak() {DurakAdi = "Sakarya" },
                   new Durak() {DurakAdi = "Ankara"},
                   new Durak() {DurakAdi = "Konya"}
                };
                durakRepo.AddRange(EklenecekDuraklar);
            }
            uow.SaveChanges();

            //Rota yoksa rota ekle.
            var rotalar = rotaRepo.GetAll().ToList();
            if (rotalar.Count == 0)
            {
                List<Rota> EklenecekRotalar = new List<Rota>()
                {
                    new Rota() {CikisID = 1, VarisID = 2},
                    new Rota() {CikisID = 1, VarisID = 3},
                    new Rota() {CikisID = 1, VarisID = 4},
                    new Rota() {CikisID = 2, VarisID = 1},
                    new Rota() {CikisID = 2, VarisID = 3},
                    new Rota() {CikisID = 2, VarisID = 4},
                    new Rota() {CikisID = 3, VarisID = 1},
                    new Rota() {CikisID = 3, VarisID = 2},
                    new Rota() {CikisID = 3, VarisID = 4},
                      new Rota() {CikisID = 4, VarisID = 1},
                    new Rota() {CikisID = 4, VarisID = 2},
                    new Rota() {CikisID = 4, VarisID = 3}
                };
                rotaRepo.AddRange(EklenecekRotalar);
            }
            uow.SaveChanges();

            //VagonTipi yoksa vagontipi ekle.
            var vagonTipleri = vagonTipiRepo.GetAll().ToList();
            if (vagonTipleri.Count == 0)
            {
                List<VagonTipi> EklenecekVagonTipleri = new List<VagonTipi>()
                {
                    new VagonTipi() {TipAdi= "Business"},
                    new VagonTipi() {TipAdi="Ekonomi"}

                };
                vagonTipiRepo.AddRange(EklenecekVagonTipleri);
            }
            uow.SaveChanges();

            //Vagon yoksa vagon ekle.
            var vagonlar = vagonRepo.GetAll().ToList();
            if (vagonlar.Count == 0)
            {
                List<Vagon> EklenecekVagonlar = new List<Vagon>()
                {
                    new Vagon() {VagonID=1, VagonTipiID=1},
                    new Vagon() {VagonID=2, VagonTipiID=2}

                };
                vagonRepo.AddRange(EklenecekVagonlar);
            }
            uow.SaveChanges();

            //Tren yoksa tren ekle.
            var trenler = trenRepo.GetAll().ToList();
            if (trenler.Count == 0)
            {
                List<Tren> EklenecekTrenler = new List<Tren>()
                {
                    new Tren() {TrenID=1},
                    new Tren() {TrenID=2}

                };
                trenRepo.AddRange(EklenecekTrenler);
            }
            uow.SaveChanges();

            var trenVagonlar = trenVagonRepo.GetAll().ToList();
            if (trenVagonlar.Count == 0)
            {
                List<TrenVagon> EklenecekTrenVagonlar = new List<TrenVagon>()
                {
                    new TrenVagon() {TrenID= 1, VagonID=1},
                    new TrenVagon() {TrenID= 1, VagonID=2},
                    new TrenVagon() {TrenID= 2, VagonID=1},
                    new TrenVagon() {TrenID= 2, VagonID=2}
                };
                trenVagonRepo.AddRange(EklenecekTrenVagonlar);
            }
            uow.SaveChanges();

            //Kullanıcı tipi yoksa ekle.
            var kullaniciTipleri = kullaniciTipRepo.GetAll().ToList();
            if (kullaniciTipleri.Count == 0)
            {
                List<KullaniciTip> EklenecekKullaniciTipleri = new List<KullaniciTip>()
                {
                    new KullaniciTip() {TipAdi="Üye"},
                    new KullaniciTip() {TipAdi="Admin"}
                };
                kullaniciTipRepo.AddRange(EklenecekKullaniciTipleri);
            }
            uow.SaveChanges();

            //Kullanici yoksa kullanıcı ekle.
            var kullanicilar = kullaniciRepo.GetAll().ToList();
            if (kullanicilar.Count == 0)
            {
                List<Kullanici> EklenecekKullanicilar = new List<Kullanici>()
                {
                    new Kullanici() {Ad="Admin", Soyad="Admin", Adres="İst", Cinsiyet=false, Email="admin ",
                        Sifre ="admin",KullaniciTipID = 2,TcNo="1", Telefon="0555 555 55 55" },
                    new Kullanici() {Ad="Rabia", Soyad="Karakaya", Adres="İst", Cinsiyet=false, Email="rabia",
                        Sifre ="123",KullaniciTipID = 1,TcNo="2", Telefon="0333 333 33 33" }
                };
                kullaniciRepo.AddRange(EklenecekKullanicilar);
            }
            uow.SaveChanges();

            //İlk atamada 14 günlük sefer eklemesi yapalım..
            var seferlerIlk = seferRepo.GetAll().ToList();
            if (seferlerIlk.Count == 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    DateTime gun = new DateTime(dtGidis.Value.Year, dtGidis.Value.Month, dtGidis.Value.Day + i, 0, 0, 0);
                    List<Sefer> eklenecekSeferler = new List<Sefer>()
                {
                    new Sefer() { CikisSaati=TimeSpan.FromHours(8) , VarisSaati=TimeSpan.FromHours(9), RotaID =1,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(9) , VarisSaati=TimeSpan.FromHours(10), RotaID =2,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(10) , VarisSaati=TimeSpan.FromHours(11), RotaID =3,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(11) , VarisSaati=TimeSpan.FromHours(12), RotaID =4,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(12) , VarisSaati=TimeSpan.FromHours(13), RotaID =5,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(13) , VarisSaati=TimeSpan.FromHours(14), RotaID =6,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                                                  new Sefer() { CikisSaati=TimeSpan.FromHours(14) , VarisSaati=TimeSpan.FromHours(15), RotaID =7,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(15) , VarisSaati=TimeSpan.FromHours(16), RotaID =8,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(16) , VarisSaati=TimeSpan.FromHours(17), RotaID = 9,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(17) , VarisSaati=TimeSpan.FromHours(18), RotaID = 10,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(18) , VarisSaati=TimeSpan.FromHours(19), RotaID = 11,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(19) , VarisSaati=TimeSpan.FromHours(20), RotaID = 12,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },

                                               new Sefer() { CikisSaati=TimeSpan.FromHours(19) , VarisSaati=TimeSpan.FromHours(20), RotaID =1,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(18) , VarisSaati=TimeSpan.FromHours(19), RotaID =2,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(17) , VarisSaati=TimeSpan.FromHours(18), RotaID =3,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(16) , VarisSaati=TimeSpan.FromHours(17), RotaID =4,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(15) , VarisSaati=TimeSpan.FromHours(16), RotaID =5,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(14) , VarisSaati=TimeSpan.FromHours(15), RotaID =6,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 1 },
                                                  new Sefer() { CikisSaati=TimeSpan.FromHours(13) , VarisSaati=TimeSpan.FromHours(14), RotaID =7,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(12) , VarisSaati=TimeSpan.FromHours(13), RotaID =8,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(11) , VarisSaati=TimeSpan.FromHours(12), RotaID = 9,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(10) , VarisSaati=TimeSpan.FromHours(11), RotaID = 10,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(9) , VarisSaati=TimeSpan.FromHours(10), RotaID = 11,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(8) , VarisSaati=TimeSpan.FromHours(9), RotaID = 12,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=gun, TrenID = 2 }
                };
                    seferRepo.AddRange(eklenecekSeferler);
                }
            }
            uow.SaveChanges();


            //Günlük sefer eklemesi
            DateTime yeniGun = new DateTime(dtGidis.Value.Year, dtGidis.Value.Month, dtGidis.Value.Day + 14, 0, 0, 0);

            var seferler = seferRepo.GetAll(x => x.Tarih == yeniGun).ToList();
            if (seferler.Count == 0)
            {
                List<Sefer> eklenecekSeferler = new List<Sefer>()
                {
                    new Sefer() { CikisSaati=TimeSpan.FromHours(8) , VarisSaati=TimeSpan.FromHours(9), RotaID =1,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(9) , VarisSaati=TimeSpan.FromHours(10), RotaID =2,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(10) , VarisSaati=TimeSpan.FromHours(11), RotaID =3,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=DateTime.Now, TrenID = 1 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(11) , VarisSaati=TimeSpan.FromHours(12), RotaID =4,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(12) , VarisSaati=TimeSpan.FromHours(13), RotaID =5,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(13) , VarisSaati=TimeSpan.FromHours(14), RotaID =6,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                                                  new Sefer() { CikisSaati=TimeSpan.FromHours(14) , VarisSaati=TimeSpan.FromHours(15), RotaID =7,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(15) , VarisSaati=TimeSpan.FromHours(16), RotaID =8,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(16) , VarisSaati=TimeSpan.FromHours(17), RotaID =9,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(17) , VarisSaati=TimeSpan.FromHours(18), RotaID =10,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(18) , VarisSaati=TimeSpan.FromHours(19), RotaID =11,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(19) , VarisSaati=TimeSpan.FromHours(20), RotaID =12,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },

                                                    new Sefer() { CikisSaati=TimeSpan.FromHours(19) , VarisSaati=TimeSpan.FromHours(20), RotaID =1,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(18) , VarisSaati=TimeSpan.FromHours(19), RotaID =2,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(17) , VarisSaati=TimeSpan.FromHours(18), RotaID =3,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(16) , VarisSaati=TimeSpan.FromHours(17), RotaID =4,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(15) , VarisSaati=TimeSpan.FromHours(16), RotaID =5,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(14) , VarisSaati=TimeSpan.FromHours(15), RotaID =6,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 1 },
                                                  new Sefer() { CikisSaati=TimeSpan.FromHours(13) , VarisSaati=TimeSpan.FromHours(14), RotaID =7,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                    new Sefer() { CikisSaati=TimeSpan.FromHours(12) , VarisSaati=TimeSpan.FromHours(13), RotaID =8,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                      new Sefer() { CikisSaati=TimeSpan.FromHours(11) , VarisSaati=TimeSpan.FromHours(12), RotaID = 9,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                          new Sefer() { CikisSaati=TimeSpan.FromHours(10) , VarisSaati=TimeSpan.FromHours(11), RotaID = 10,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                                    new Sefer() { CikisSaati=TimeSpan.FromHours(9) , VarisSaati=TimeSpan.FromHours(10), RotaID = 11,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 },
                                              new Sefer() { CikisSaati=TimeSpan.FromHours(8) , VarisSaati=TimeSpan.FromHours(9), RotaID = 12,
                        SeferSuresi = TimeSpan.FromHours(1), Tarih=yeniGun, TrenID = 2 }
                };
                seferRepo.AddRange(eklenecekSeferler);
            }
            uow.SaveChanges();


            //Sefere 2 saat kala rezervasyonları sil
            var tumBiletler = biletRepo.GetAll(x => x.BiletDurumu == false).ToList();
            foreach (var bilet in tumBiletler)
            {
                var sefer = seferRepo.Get(x => x.SeferID == bilet.SeferID);
                var aradakiFark = (sefer.CikisSaati.Hours - DateTime.Now.Hour);
                if (aradakiFark <= 2)
                {
                    biletRepo.Delete(bilet);
                    uow.SaveChanges();
                }
            }

        }
    }
}








