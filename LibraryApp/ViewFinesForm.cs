using LibraryManagementSystem;
using System;
using System.Data;
using System.Windows.Forms;

namespace LibraryApp
{
    public class ViewFinesForm : Form
    {
        private Controller _ctrl = new Controller();
        private DataGridView dgv = new DataGridView();
        private TextBox txtBorrowID = new TextBox();
        private TextBox txtAmount = new TextBox();
        private Button btnRecord = new Button();
        private Button btnRefresh = new Button();

        public ViewFinesForm()
        {
            this.Text = "View / Record Fines";
            this.Size = new System.Drawing.Size(880, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            var title = new Label { Text = "Borrowing Records & Fines", AutoSize = true, Location = new System.Drawing.Point(10, 10), Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };

            dgv.Location = new System.Drawing.Point(10, 35);
            dgv.Size = new System.Drawing.Size(850, 360);
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow?.DataBoundItem == null) return;
                txtBorrowID.Text = dgv.CurrentRow.Cells["BorrowID"].Value?.ToString();
            };

            this.Controls.Add(new Label { Text = "Borrow ID:", AutoSize = true, Location = new System.Drawing.Point(10, 408) });
            txtBorrowID.Location = new System.Drawing.Point(90, 405); txtBorrowID.Width = 70;

            this.Controls.Add(new Label { Text = "Fine Amount (£):", AutoSize = true, Location = new System.Drawing.Point(180, 408) });
            txtAmount.Location = new System.Drawing.Point(300, 405); txtAmount.Width = 80;

            btnRecord.Text = "💾 Record / Update Fine"; btnRecord.Location = new System.Drawing.Point(10, 440); btnRecord.Size = new System.Drawing.Size(200, 38);
            btnRecord.BackColor = System.Drawing.Color.SteelBlue; btnRecord.ForeColor = System.Drawing.Color.White;
            btnRecord.Click += BtnRecord_Click;

            btnRefresh.Text = "🔄 Refresh"; btnRefresh.Location = new System.Drawing.Point(220, 440); btnRefresh.Size = new System.Drawing.Size(100, 38);
            btnRefresh.Click += (s, e) => LoadData();

            this.Controls.AddRange(new Control[] { title, dgv, txtBorrowID, txtAmount, btnRecord, btnRefresh });
        }

        private void LoadData()
        {
            dgv.DataSource = _ctrl.Report_BorrowingHistory() ?? new DataTable();
        }

        private void BtnRecord_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBorrowID.Text, out int bid)) { MessageBox.Show("Enter a valid Borrow ID."); return; }
            if (!decimal.TryParse(txtAmount.Text, out decimal amt)) { MessageBox.Show("Enter a valid amount."); return; }
            try
            {
                _ctrl.RecordFine(bid, amt);
                MessageBox.Show("Fine recorded/updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}