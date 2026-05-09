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

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Wrong username or password");
                return;
            }

            string status = dt.Rows[0]["Status"].ToString();

            if (status != "Active")
            {
                MessageBox.Show("Account is not active");
                return;
            }

            string role = dt.Rows[0]["RoleID"].ToString();

            if (role == "1")
            {
                AdminDashboard form = new AdminDashboard();
                form.Show();
            }
            else if (role == "3")
            {
                Form1 form = new Form1();
                form.Show();
            }
            else if (role == "4")
            {
                Form1 form = new Form1();
                form.Show();
            }
            else if (role == "2")
            {
                LibrarianDashboard form = new LibrarianDashboard();
                form.Show();
            }

                //this.Hide();
        }

        private void signupbtn_Click(object sender, EventArgs e)
        {
            Controller controller = new Controller();
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
        }

        private void forgotbtn_Click(object sender, EventArgs e)
        {
            ChangePassword chpass = new ChangePassword();
            chpass.Show();
        }
    }
}