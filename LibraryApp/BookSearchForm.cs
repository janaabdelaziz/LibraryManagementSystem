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
    public partial class BookSearchForm : Form
    {
        Controller controllerObj;
        public BookSearchForm()
        {
            InitializeComponent();
            controllerObj = new Controller();
        }

        private void BookSearchForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string titleFilter = txtTitleFilter.Text.Trim();
            DataTable dt = controllerObj.SearchBooksByTitle(titleFilter);
            dgvBooks.DataSource = dt;

        }
    }
}
