using LibraryManagementSystem;
using System;
using System.Data;
using System.Windows.Forms;

namespace LibraryApp
{
    public class ManageBookCopiesForm : Form
    {
        private Controller _ctrl = new Controller();
        private ComboBox cmbBook = new ComboBox();
        private TextBox txtShelf = new TextBox();
        private DataGridView dgv = new DataGridView();
        private Button btnAdd = new Button();
        private Button btnDelete = new Button();

        public ManageBookCopiesForm()
        {
            this.Text = "Manage Book Copies";
            this.Size = new System.Drawing.Size(720, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadBooks();
        }

        private void BuildUI()
        {
            // Book selector
            this.Controls.Add(new Label { Text = "Select Book:", AutoSize = true, Location = new System.Drawing.Point(10, 13) });
            cmbBook.Location = new System.Drawing.Point(100, 10); cmbBook.Width = 350; cmbBook.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBook.SelectedIndexChanged += (s, e) => LoadCopies();

            // Shelf input
            this.Controls.Add(new Label { Text = "Shelf Location:", AutoSize = true, Location = new System.Drawing.Point(10, 53) });
            txtShelf.Location = new System.Drawing.Point(115, 50); txtShelf.Width = 200;

            // Add button
            btnAdd.Text = "➕ Add Copy"; btnAdd.Location = new System.Drawing.Point(330, 48); btnAdd.Width = 120; btnAdd.Height = 30;
            btnAdd.BackColor = System.Drawing.Color.SeaGreen; btnAdd.ForeColor = System.Drawing.Color.White;
            btnAdd.Click += BtnAdd_Click;

            // Grid
            dgv.Location = new System.Drawing.Point(10, 90); dgv.Size = new System.Drawing.Size(680, 320);
            dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Delete button
            btnDelete.Text = "🗑️ Delete Selected Copy"; btnDelete.Location = new System.Drawing.Point(10, 425); btnDelete.Width = 200; btnDelete.Height = 35;
            btnDelete.BackColor = System.Drawing.Color.Firebrick; btnDelete.ForeColor = System.Drawing.Color.White;
            btnDelete.Click += BtnDelete_Click;

            this.Controls.AddRange(new Control[] { cmbBook, txtShelf, btnAdd, dgv, btnDelete });
        }

        private void LoadBooks()
        {
            var dt = _ctrl.SearchBooksAdvanced();
            cmbBook.DataSource = dt ?? new DataTable();
            cmbBook.DisplayMember = "Title";
            cmbBook.ValueMember = "BookID";
        }

        private void LoadCopies()
        {
            if (cmbBook.SelectedValue == null) return;
            int bookID = Convert.ToInt32(cmbBook.SelectedValue);
            dgv.DataSource = _ctrl.GetBookCopies(bookID) ?? new DataTable();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbBook.SelectedValue == null) { MessageBox.Show("Select a book first."); return; }
            if (string.IsNullOrWhiteSpace(txtShelf.Text)) { MessageBox.Show("Enter a shelf location."); return; }
            try
            {
                _ctrl.AddBookCopy(Convert.ToInt32(cmbBook.SelectedValue), txtShelf.Text.Trim());
                MessageBox.Show("Copy added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtShelf.Clear();
                LoadCopies();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a copy from the grid first."); return; }
            int copyID = Convert.ToInt32(dgv.CurrentRow.Cells["CopyID"].Value);
            if (MessageBox.Show("Delete this copy?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                _ctrl.DeleteBookCopy(copyID);
                MessageBox.Show("Copy deleted.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCopies();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
