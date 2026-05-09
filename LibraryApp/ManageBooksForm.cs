
using LibraryManagementSystem;
using System;
using System.Data;
using System.Windows.Forms;

namespace LibraryApp
{
    public partial class ManageBooksForm : Form
    {
        private Controller _ctrl = new Controller();
        private int _selectedBookID = -1;

        // Controls
        private DataGridView dgv = new DataGridView();
        private TextBox txtTitle = new TextBox();
        private TextBox txtISBN = new TextBox();
        private TextBox txtYear = new TextBox();
        private TextBox txtSearch = new TextBox();
        private ComboBox cmbCategory = new ComboBox();
        private ComboBox cmbPublisher = new ComboBox();
        private Button btnSearch = new Button();
        private Button btnClear = new Button();
        private Button btnAdd = new Button();
        private Button btnUpdate = new Button();
        private Button btnDelete = new Button();

        public ManageBooksForm()
        {
            this.Text = "Manage Books";
            this.Size = new System.Drawing.Size(960, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadCombos();
            LoadBooks();
        }

        private void BuildUI()
        {
            // ── Search bar ───────────────────────────────────────
            var lblSearch = new Label { Text = "Search by title:", AutoSize = true, Location = new System.Drawing.Point(10, 13) };
            txtSearch.Location = new System.Drawing.Point(115, 10); txtSearch.Width = 220;
            btnSearch.Text = "Search"; btnSearch.Location = new System.Drawing.Point(345, 8); btnSearch.Width = 80;
            btnSearch.BackColor = System.Drawing.Color.SteelBlue; btnSearch.ForeColor = System.Drawing.Color.White;
            btnSearch.Click += (s, e) => LoadBooks(txtSearch.Text.Trim());
            btnClear.Text = "Clear"; btnClear.Location = new System.Drawing.Point(435, 8); btnClear.Width = 70;
            btnClear.Click += (s, e) => { txtSearch.Clear(); LoadBooks(); };

            // ── Grid ─────────────────────────────────────────────
            dgv.Location = new System.Drawing.Point(10, 42);
            dgv.Size = new System.Drawing.Size(930, 310);
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionChanged += Dgv_SelectionChanged;

            // ── Input fields ─────────────────────────────────────
            int lx = 10, vx = 130, y = 370, gap = 38;

            Label MkLbl(string t, int yy) => new Label { Text = t, AutoSize = true, Location = new System.Drawing.Point(lx, yy + 4), Font = new System.Drawing.Font("Segoe UI", 9) };

            void Row(string lbl, Control c)
            {
                c.Location = new System.Drawing.Point(vx, y); c.Width = 280;
                this.Controls.Add(MkLbl(lbl, y)); this.Controls.Add(c); y += gap;
            }

            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPublisher.DropDownStyle = ComboBoxStyle.DropDownList;

            Row("Title:", txtTitle);
            Row("ISBN:", txtISBN);
            Row("Year:", txtYear);
            Row("Category:", cmbCategory);
            Row("Publisher:", cmbPublisher);

            // ── Buttons ───────────────────────────────────────────
            int by = 560;
            btnAdd.Text = "➕ Add Book"; btnAdd.Location = new System.Drawing.Point(10, by); btnAdd.Size = new System.Drawing.Size(130, 40); btnAdd.BackColor = System.Drawing.Color.SeaGreen; btnAdd.ForeColor = System.Drawing.Color.White;
            btnUpdate.Text = "✏️ Update Book"; btnUpdate.Location = new System.Drawing.Point(150, by); btnUpdate.Size = new System.Drawing.Size(130, 40); btnUpdate.BackColor = System.Drawing.Color.SteelBlue; btnUpdate.ForeColor = System.Drawing.Color.White;
            btnDelete.Text = "🗑️ Delete Book"; btnDelete.Location = new System.Drawing.Point(290, by); btnDelete.Size = new System.Drawing.Size(130, 40); btnDelete.BackColor = System.Drawing.Color.Firebrick; btnDelete.ForeColor = System.Drawing.Color.White;

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            this.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, btnClear,
                dgv, btnAdd, btnUpdate, btnDelete
            });
        }

        private void LoadCombos()
        {
            var cats = _ctrl.GetAllCategories();
            cmbCategory.DataSource = cats; cmbCategory.DisplayMember = "CategoryName"; cmbCategory.ValueMember = "CategoryID";

            var pubs = _ctrl.GetAllPublishers();
            cmbPublisher.DataSource = pubs; cmbPublisher.DisplayMember = "Name"; cmbPublisher.ValueMember = "PublisherID";
        }

        private void LoadBooks(string search = null)
        {
            var dt = _ctrl.SearchBooksAdvanced(title: string.IsNullOrEmpty(search) ? null : search);
            dgv.DataSource = dt ?? new DataTable();
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = dgv.CurrentRow;
            _selectedBookID = Convert.ToInt32(row.Cells["BookID"].Value);
            txtTitle.Text = row.Cells["Title"].Value?.ToString();
            txtISBN.Text = row.Cells["ISBN"].Value?.ToString();
            txtYear.Text = row.Cells["PublicationYear"].Value?.ToString();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("Title is required.", "Validation"); return false; }
            if (!int.TryParse(txtYear.Text, out _)) { MessageBox.Show("Year must be a number.", "Validation"); return false; }
            return true;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            try
            {
                _ctrl.AddBook(
                    txtTitle.Text.Trim(),
                    txtISBN.Text.Trim(),
                    int.Parse(txtYear.Text.Trim()),
                    Convert.ToInt32(cmbCategory.SelectedValue),
                    Convert.ToInt32(cmbPublisher.SelectedValue));
                MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadBooks();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedBookID == -1) { MessageBox.Show("Please select a book from the grid first."); return; }
            if (!ValidateInputs()) return;
            try
            {
                _ctrl.UpdateBook(
                    _selectedBookID,
                    txtTitle.Text.Trim(),
                    txtISBN.Text.Trim(),
                    int.Parse(txtYear.Text.Trim()),
                    Convert.ToInt32(cmbCategory.SelectedValue),
                    Convert.ToInt32(cmbPublisher.SelectedValue));
                MessageBox.Show("Book updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadBooks();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedBookID == -1) { MessageBox.Show("Please select a book first."); return; }
            if (MessageBox.Show("Delete this book and ALL its copies?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                _ctrl.DeleteBook(_selectedBookID);
                MessageBox.Show("Book deleted.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _selectedBookID = -1;
                txtTitle.Clear(); txtISBN.Clear(); txtYear.Clear();
                LoadBooks();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ManageBooksForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ManageBooksForm";
            this.Load += new System.EventHandler(this.ManageBooksForm_Load);
            this.ResumeLayout(false);

        }

        private void ManageBooksForm_Load(object sender, EventArgs e)
        {

        }
    }
}
