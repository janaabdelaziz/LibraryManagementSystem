using LibraryManagementSystem;
using System;
using System.Data;
using System.Windows.Forms;

namespace LibraryApp
{
    public class ReturnBookForm : Form
    {
        private Controller _ctrl = new Controller();
        private DataGridView dgv = new DataGridView();
        private Label lblResult = new Label();
        private Button btnReturn = new Button();
        private Button btnRefresh = new Button();

        public ReturnBookForm()
        {
            this.Text = "Return a Book";
            this.Size = new System.Drawing.Size(920, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            var title = new Label { Text = "Active Borrowings — select a row then click Return", AutoSize = true, Location = new System.Drawing.Point(10, 10), Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };

            dgv.Location = new System.Drawing.Point(10, 38);
            dgv.Size = new System.Drawing.Size(890, 360);
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Highlight overdue rows in pink
            dgv.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var row = dgv.Rows[e.RowIndex];
                if (row.DataBoundItem == null) return;
                var drv = (DataRowView)row.DataBoundItem;
                int days = Convert.ToInt32(drv["DaysOverdue"]);
                row.DefaultCellStyle.BackColor = days > 0
                    ? System.Drawing.Color.MistyRose
                    : System.Drawing.Color.White;
            };

            lblResult.Location = new System.Drawing.Point(10, 408);
            lblResult.AutoSize = true;
            lblResult.ForeColor = System.Drawing.Color.Firebrick;
            lblResult.Font = new System.Drawing.Font("Segoe UI", 10);

            btnReturn.Text = "📥 Process Return";
            btnReturn.Location = new System.Drawing.Point(10, 435);
            btnReturn.Size = new System.Drawing.Size(180, 40);
            btnReturn.BackColor = System.Drawing.Color.SteelBlue;
            btnReturn.ForeColor = System.Drawing.Color.White;
            btnReturn.Font = new System.Drawing.Font("Segoe UI", 11);
            btnReturn.Click += BtnReturn_Click;

            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Location = new System.Drawing.Point(200, 435);
            btnRefresh.Size = new System.Drawing.Size(100, 40);
            btnRefresh.Click += (s, e) => LoadData();

            this.Controls.AddRange(new Control[] { title, dgv, lblResult, btnReturn, btnRefresh });
        }

        private void LoadData()
        {
            dgv.DataSource = _ctrl.GetActiveBorrowings() ?? new DataTable();
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a borrowing record first."); return; }

            var drv = (DataRowView)dgv.CurrentRow.DataBoundItem;
            int borrowID = Convert.ToInt32(drv["BorrowID"]);
            int daysOver = Convert.ToInt32(drv["DaysOverdue"]);
            string member = drv["MemberName"]?.ToString();
            string book = drv["BookTitle"]?.ToString();

            string msg = $"Return \"{book}\" borrowed by {member}?";
            if (daysOver > 0)
                msg += $"\n\n⚠️ Overdue by {daysOver} day(s). A fine of £{daysOver:F2} will be added.";

            if (MessageBox.Show(msg, "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var result = _ctrl.ReturnBook(borrowID);
                if (result != null && result.Rows.Count > 0)
                {
                    decimal fine = Convert.ToDecimal(result.Rows[0]["FineAmount"]);
                    string info = result.Rows[0]["Message"]?.ToString();
                    lblResult.ForeColor = fine > 0 ? System.Drawing.Color.Firebrick : System.Drawing.Color.SeaGreen;
                    lblResult.Text = fine > 0 ? $"Fine generated: £{fine:F2}  —  {info}" : info;
                    MessageBox.Show($"Return processed.\n{info}" + (fine > 0 ? $"\nFine: £{fine:F2}" : ""),
                        "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
