using System;
using System.Windows.Forms;

namespace LibraryApp
{
    public class LibrarianDashboard : Form
    {
        public LibrarianDashboard()
        {
            this.Text = "Librarian Dashboard";
            this.Size = new System.Drawing.Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
            BuildUI();
        }

        private void BuildUI()
        {
            var title = new Label
            {
                Text = "📚 Librarian Panel",
                Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(200, 30)
            };

            string[] labels = { "Manage Books", "Manage Book Copies", "Issue a Book", "Return a Book", "Manage Reservations", "View / Record Fines", "📊 Reports" };
            Action[] actions = {
                () => new ManageBooksForm().ShowDialog(),
                () => new ManageBookCopiesForm().ShowDialog(),
                () => new IssueBookForm().ShowDialog(),
                () => new ReturnBookForm().ShowDialog(),
                () => new ManageReservationsForm().ShowDialog(),
                () => new ViewFinesForm().ShowDialog(),
                () => new ReportsForm().ShowDialog()
            };

            int y = 100;
            for (int i = 0; i < labels.Length; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Text = labels[i],
                    Size = new System.Drawing.Size(280, 48),
                    Location = new System.Drawing.Point(210, y),
                    Font = new System.Drawing.Font("Segoe UI", 11),
                    BackColor = i == 6 ? System.Drawing.Color.DarkSlateBlue : System.Drawing.Color.SteelBlue,
                    ForeColor = System.Drawing.Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btn.Click += (s, e) => actions[idx]();
                this.Controls.Add(btn);
                y += 58;
            }

            this.Controls.Add(title);
        }
    }
}
