using DAL;
using DAL.Repositories;
using DAL.UnitOfWork;
using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class UyeOl : Form
    {
        private Context trenDb;
        private IUnitOfWork uow;
        private IRepository<Kullanici> kullaniciRepo;

        public UyeOl()
        {
            InitializeComponent();
        }

        private void UyeOl_Load(object sender, EventArgs e)
        {
            trenDb = new Context();
            uow = new EFUnitOfWork(trenDb);
            kullaniciRepo = new EFRepository<Kullanici>(trenDb);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TrenBilet tb = new TrenBilet();
            tb.Show();
            this.Hide();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (OrtakMetodlar.BosAlanVarMi(grpUye))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
            }
            else
            {
                if (kullaniciRepo.Get(x => x.Email == txtEposta.Text) != null)
                {
                    MessageBox.Show("Girdiğiniz e-posta ile kayıtlı bir kullanıcı bulunmaktadır.");
                }
                else if (kullaniciRepo.Get(x => x.TcNo == txtTcNo.Text) != null)
                {
                    MessageBox.Show("Girdiğiniz TC no ile kayıtlı bir kullanıcı bulunmaktadır.");
                }
                else
                {
                    Kullanici kullanici = new Kullanici
                    {
                        KullaniciTipID = 1,
                        Ad = txtAd.Text,
                        Soyad = txtSoyad.Text,
                        TcNo = txtTcNo.Text,
                        Cinsiyet = rdoErkek.Checked,
                        Adres = txtAdres.Text,
                        Email = txtEposta.Text,
                        Sifre = txtSifre.Text,
                        Telefon = txtTelefon.Text
                    };

                    kullaniciRepo.Add(kullanici);
                    int islem = uow.SaveChanges();

                    MessageBox.Show("Üyelik işlemi başarıyla gerçekleştirilmiştir.");
                    TrenBilet tb = new TrenBilet();
                    tb.Show();
                    this.Hide();
                }
            }
        }

  


    }
}
