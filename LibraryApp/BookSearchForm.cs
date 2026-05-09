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
        private int _currentUserId;

        public BookSearchForm(int userId)
        {
            InitializeComponent();

            controllerObj = new Controller();
            _currentUserId = userId;
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

        private void btnBorrow_Click(object sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null)
            {
                MessageBox.Show("Please select a copy first.");
                return;
            }

            // make sure your query that fills dgvBooks includes CopyID as a column
            object copyIdObj = dgvBooks.CurrentRow.Cells["CopyID"].Value;

            if (copyIdObj == null || copyIdObj == DBNull.Value)
            {
                MessageBox.Show("Selected row has no copy ID.");
                return;
            }

            int copyId = Convert.ToInt32(copyIdObj);
            DateTime dueDate = dtpDueDate.Value.Date;

            int result = controllerObj.BorrowBook(_currentUserId, copyId, dueDate);

            if (result > 0)
            {
                MessageBox.Show("Book borrowed successfully.");

                // refresh the grid to show updated status
                string titleFilter = txtTitleFilter.Text.Trim();
                dgvBooks.DataSource = controllerObj.SearchBooksByTitle(titleFilter);
            }
            else
            {
                MessageBox.Show("Borrowing failed.");
            }
        }

        private void btnReserve_Click(object sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null)
            {
                MessageBox.Show("Please select a copy first.");
                return;
            }

            object copyIdObj = dgvBooks.CurrentRow.Cells["CopyID"].Value;
            object statusObj = dgvBooks.CurrentRow.Cells["Status"].Value;

            if (copyIdObj == null || copyIdObj == DBNull.Value)
            {
                MessageBox.Show("Selected row has no copy ID.");
                return;
            }

            string status = statusObj?.ToString();

            // Simple rule: only allow reservation if not Available
            if (status == "Available")
            {
                MessageBox.Show("This copy is available; you can borrow it directly instead of reserving.");
                return;
            }

            int copyId = Convert.ToInt32(copyIdObj);

            int result = controllerObj.ReserveBook(_currentUserId, copyId);

            if (result > 0)
            {
                MessageBox.Show("Reservation created successfully.");
                // Refresh results
                string titleFilter = txtTitleFilter.Text.Trim();
                dgvBooks.DataSource = controllerObj.SearchBooksByTitle(titleFilter);
            }
            else
            {
                MessageBox.Show("Reservation failed.");
            }
        }
    }
}
