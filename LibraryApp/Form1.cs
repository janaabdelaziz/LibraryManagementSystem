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
    
    public partial class Form1 : Form
    {
        Controller controllerObj;
        public Form1()
        {
            InitializeComponent();
            controllerObj = new Controller();
        }

        private void btnCountUsers_Click(object sender, EventArgs e)
        {
            int count = controllerObj.CountUsers();
            MessageBox.Show("Number of users = " + count);
        }

        private void btnOpenBookSearch_Click(object sender, EventArgs e)
        {
            int currentUserId = 1;  // use a real existing UserID for now
            BookSearchForm f = new BookSearchForm(currentUserId);
            f.Show();
        }

        private void btnMyHistory_Click(object sender, EventArgs e)
        {
            // TODO: replace 1 with an actual UserID from your USERS table
            int currentUserId = 1;

            BorrowingHistoryForm f = new BorrowingHistoryForm(currentUserId);
            f.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
