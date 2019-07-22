using RepveUOW.Data;
using RepveUOW.Data.Model;
using RepveUOW.Data.Repositories;
using RepveUOW.Data.UnitOfWork;
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
    public partial class Form1 : Form
    {
        private EFBlogContext _dbContext;
        private IUnitOfWork _uow;
        private IRepository<User> _userRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Article> _articleRepository;


        public void TestInitialize()
        {
            _dbContext = new EFBlogContext();
            // EFBlogContext'i kullanıyor olduğumuz için EFUnitOfWork'den türeterek constructor'ına
            // ilgili context'i constructor injection yöntemi ile inject ediyoruz.
            _uow = new EFUnitOfWork(_dbContext);
            _userRepository = new EFRepository<User>(_dbContext);
            _categoryRepository = new EFRepository<Category>(_dbContext);
            _articleRepository = new EFRepository<Article>(_dbContext);
        }

        public Form1()
        {
            InitializeComponent();
            TestInitialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            User user = new User
            {
                FirstName = "Gökhan",
                LastName = "Gökalp",
                CreatedDate = DateTime.Now,
                Email = "gok.gokalp@yahoo.com",
                Password = "123456"
            };
            _userRepository.Add(user);
            int process = _uow.SaveChanges();
        }
    }
}
