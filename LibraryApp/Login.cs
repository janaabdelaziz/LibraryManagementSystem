using LibraryManagementSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryApp
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
                Controller controller = new Controller();

            DataTable dt = controller.Login(usertxt.Text, passtxt.Text);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Wrong username or password");
                return;
            }

            bool isActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

            if (!isActive)
            {
                MessageBox.Show("This account is deactivated.");
                return;
            }

            string role = dt.Rows[0]["RoleName"].ToString();

            //if (role == "Admin")
            //{
            //    AdminDashboard admin = new AdminDashboard();
            //    admin.Show();
            //}
            //else if (role == "Librarian")
            //{
            //    LibrarianDashboard librarian = new LibrarianDashboard();
            //    librarian.Show();
            //}
            //else
            if (role == "Member")
            {
                Form1 f = new Form1();;
                f.Show();
            }
            else
            {
                Form1 f = new Form1();
                f.Show();
            }

            this.Hide();
        }

        private void signupbtn_Click(object sender, EventArgs e)
        {
            Controller controller = new Controller();
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
        }
    }
}
