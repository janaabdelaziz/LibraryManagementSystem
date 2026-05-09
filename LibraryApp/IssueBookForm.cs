using LibraryManagementSystem;
using System;
using System.Data;
using System.Windows.Forms;

namespace LibraryApp
{
    public class IssueBookForm : Form
    {
        private Controller _ctrl = new Controller();
        private ComboBox cmbMember = new ComboBox();
        private ComboBox cmbBook = new ComboBox();
        private ComboBox cmbCopy = new ComboBox();
        private DateTimePicker dtpDue = new DateTimePicker();
        private Label lblCopyInfo = new Label();
        private Button btnIssue = new Button();

        public IssueBookForm()
        {
            this.Text = "Issue a Book";
            this.Size = new System.Drawing.Size(480, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadMembers();
            LoadBooks();
        }

        private void BuildUI()
        {
            int lx = 20, vx = 150, y = 30, gap = 48;

            void Row(string lbl, Control c)
            {
                this.Controls.Add(new Label { Text = lbl, AutoSize = true, Location = new System.Drawing.Point(lx, y + 4), Font = new System.Drawing.Font("Segoe UI", 10) });
                c.Location = new System.Drawing.Point(vx, y); c.Width = 290;
                this.Controls.Add(c); y += gap;
            }

            cmbMember.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBook.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCopy.DropDownStyle = ComboBoxStyle.DropDownList;
            dtpDue.MinDate = DateTime.Today.AddDays(1);
            dtpDue.Value = DateTime.Today.AddDays(14);

            cmbBook.SelectedIndexChanged += (s, e) => LoadAvailableCopies();

            Row("Member:", cmbMember);
            Row("Book:", cmbBook);
            Row("Copy:", cmbCopy);
            Row("Due Date:", dtpDue);

            lblCopyInfo.Location = new System.Drawing.Point(lx, y);
            lblCopyInfo.AutoSize = true;
            lblCopyInfo.ForeColor = System.Drawing.Color.DarkGreen;
            lblCopyInfo.Font = new System.Drawing.Font("Segoe UI", 9);
            y += 25;

            btnIssue.Text = "✅ Issue Book";
            btnIssue.Location = new System.Drawing.Point(vx, y);
            btnIssue.Size = new System.Drawing.Size(160, 42);
            btnIssue.BackColor = System.Drawing.Color.SteelBlue;
            btnIssue.ForeColor = System.Drawing.Color.White;
            btnIssue.Font = new System.Drawing.Font("Segoe UI", 11);
            btnIssue.Click += BtnIssue_Click;

            this.Controls.Add(lblCopyInfo);
            this.Controls.Add(btnIssue);
        }

        private void LoadMembers()
        {
            var dt = _ctrl.GetActiveMembers();
            cmbMember.DataSource = dt ?? new DataTable();
            cmbMember.DisplayMember = "FullName";
            cmbMember.ValueMember = "UserID";
        }

        private void LoadBooks()
        {
            var dt = _ctrl.SearchBooksAdvanced();
            cmbBook.DataSource = dt ?? new DataTable();
            cmbBook.DisplayMember = "Title";
            cmbBook.ValueMember = "BookID";
        }

        private void LoadAvailableCopies()
        {
            if (cmbBook.SelectedValue == null) return;

            int bookID;
            if (!int.TryParse(cmbBook.SelectedValue.ToString(), out bookID)) return;

            var dt = _ctrl.GetBookCopies(bookID);
            if (dt == null) { lblCopyInfo.Text = "No copies found."; return; }

            var available = dt.Select("Status = 'Available'");
            var filtered = dt.Clone();
            foreach (var r in available) filtered.ImportRow(r);

            cmbCopy.DataSource = filtered;
            cmbCopy.DisplayMember = "ShelfLocation";
            cmbCopy.ValueMember = "CopyID";
            lblCopyInfo.Text = $"{available.Length} available copies for this book.";
        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            if (cmbMember.SelectedValue == null || cmbCopy.SelectedValue == null)
            { MessageBox.Show("Please select a member and a copy."); return; }

            int userID = Convert.ToInt32(cmbMember.SelectedValue);
            int copyID = Convert.ToInt32(cmbCopy.SelectedValue);

            try
            {
                _ctrl.IssueBook(userID, copyID, dtpDue.Value.Date);
                MessageBox.Show($"Book issued successfully!\nDue date: {dtpDue.Value:dd/MM/yyyy}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAvailableCopies();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // IssueBookForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "IssueBookForm";
            this.Load += new System.EventHandler(this.IssueBookForm_Load);
            this.ResumeLayout(false);

        }

        private void IssueBookForm_Load(object sender, EventArgs e)
        {

        }
    }
}