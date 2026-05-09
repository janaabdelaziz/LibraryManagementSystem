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
    public partial class BorrowingHistoryForm : Form
    {

        private Controller controllerObj;
        private int _currentUserId;

        //constructor
        public BorrowingHistoryForm(int userId)
        {
            InitializeComponent();
            controllerObj = new Controller();
            _currentUserId = userId;
        }

        private void dgvBorrowingHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BorrowingHistoryForm_Load(object sender, EventArgs e)
        {
            DataTable dt = controllerObj.GetBorrowingHistoryForUser(_currentUserId);
            dgvBorrowingHistory.DataSource = dt;
        }
    }
}
