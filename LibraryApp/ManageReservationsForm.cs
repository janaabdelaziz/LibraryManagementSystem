using LibraryManagementSystem;
using System;
using System.Data;
using System.Windows.Forms;

namespace LibraryApp
{
    public class ManageReservationsForm : Form
    {
        private Controller _ctrl = new Controller();
        private DataGridView dgv = new DataGridView();
        private ComboBox cmbMember = new ComboBox();
        private ComboBox cmbBook = new ComboBox();
        private ComboBox cmbCopy = new ComboBox();
        private Button btnAdd = new Button();
        private Button btnCancel = new Button();
        private Button btnRefresh = new Button();

        public ManageReservationsForm()
        {
            this.Text = "Manage Reservations";
            this.Size = new System.Drawing.Size(950, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadComboData();
            LoadReservations();
        }

        private void BuildUI()
        {
            var lblGrid = new Label { Text = "Pending Reservations:", AutoSize = true, Location = new System.Drawing.Point(10, 10), Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };

            dgv.Location = new System.Drawing.Point(10, 35);
            dgv.Size = new System.Drawing.Size(920, 310);
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ── New reservation row ───────────────────────────────
            var lblNew = new Label { Text = "New Reservation:", AutoSize = true, Location = new System.Drawing.Point(10, 358), Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };

            this.Controls.Add(new Label { Text = "Member:", AutoSize = true, Location = new System.Drawing.Point(10, 390) });
            cmbMember.Location = new System.Drawing.Point(80, 387); cmbMember.Width = 200; cmbMember.DropDownStyle = ComboBoxStyle.DropDownList;

            this.Controls.Add(new Label { Text = "Book:", AutoSize = true, Location = new System.Drawing.Point(295, 390) });
            cmbBook.Location = new System.Drawing.Point(340, 387); cmbBook.Width = 230; cmbBook.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBook.SelectedIndexChanged += (s, e) => LoadBorrowedCopies();

            this.Controls.Add(new Label { Text = "Copy:", AutoSize = true, Location = new System.Drawing.Point(585, 390) });
            cmbCopy.Location = new System.Drawing.Point(630, 387); cmbCopy.Width = 150; cmbCopy.DropDownStyle = ComboBoxStyle.DropDownList;

            btnAdd.Text = "➕ Add Reservation"; btnAdd.Location = new System.Drawing.Point(10, 430); btnAdd.Size = new System.Drawing.Size(180, 38);
            btnAdd.BackColor = System.Drawing.Color.SeaGreen; btnAdd.ForeColor = System.Drawing.Color.White;
            btnAdd.Click += BtnAdd_Click;

            btnCancel.Text = "❌ Cancel Selected"; btnCancel.Location = new System.Drawing.Point(205, 430); btnCancel.Size = new System.Drawing.Size(180, 38);
            btnCancel.BackColor = System.Drawing.Color.Firebrick; btnCancel.ForeColor = System.Drawing.Color.White;
            btnCancel.Click += BtnCancel_Click;

            btnRefresh.Text = "🔄 Refresh"; btnRefresh.Location = new System.Drawing.Point(400, 430); btnRefresh.Size = new System.Drawing.Size(110, 38);
            btnRefresh.Click += (s, e) => LoadReservations();

            this.Controls.AddRange(new Control[] { lblGrid, dgv, lblNew, cmbMember, cmbBook, cmbCopy, btnAdd, btnCancel, btnRefresh });
        }

        private void LoadComboData()
        {
            var members = _ctrl.GetActiveMembers();
            cmbMember.DataSource = members ?? new DataTable();
            cmbMember.DisplayMember = "Name"; cmbMember.ValueMember = "UserID";

            var books = _ctrl.SearchBooksAdvanced();
            cmbBook.DataSource = books ?? new DataTable();
            cmbBook.DisplayMember = "Title"; cmbBook.ValueMember = "BookID";
        }

        private void LoadBorrowedCopies()
        {
            if (cmbBook.SelectedValue == null) return;
            int bookID = Convert.ToInt32(cmbCopy.SelectedValue);
            var dt = _ctrl.GetBookCopies(bookID);
            if (dt == null) return;
            // Only non-available copies can be reserved
            var rows = dt.Select("Status <> 'Available'");
            var fdt = dt.Clone();
            foreach (var r in rows) fdt.ImportRow(r);
            cmbCopy.DataSource = fdt; cmbCopy.DisplayMember = "ShelfLocation"; cmbCopy.ValueMember = "CopyID";
        }

        private void LoadReservations()
        {
            dgv.DataSource = _ctrl.Report_ActiveReservations() ?? new DataTable();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbMember.SelectedValue == null || cmbCopy.SelectedValue == null)
            { MessageBox.Show("Please select a member and a copy."); return; }
            try
            {
                _ctrl.AddReservation(Convert.ToInt32(cmbMember.SelectedValue), (int)cmbCopy.SelectedValue);
                MessageBox.Show("Reservation added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadReservations();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a reservation from the grid first."); return; }
            int resID = Convert.ToInt32(dgv.CurrentRow.Cells["ReservationID"].Value);
            if (MessageBox.Show("Cancel this reservation?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                _ctrl.CancelReservation(resID);
                MessageBox.Show("Reservation cancelled.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadReservations();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}