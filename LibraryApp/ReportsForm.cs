using LibraryManagementSystem;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryApp
{
    public class ReportsForm : Form
    {
        private Controller _ctrl = new Controller();
        private TabControl tabs = new TabControl();

        public ReportsForm()
        {
            this.Text = "Reports";
            this.Size = new Size(1020, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            tabs.Dock = DockStyle.Fill;
            this.Controls.Add(tabs);
            BuildAllTabs();
        }

        // ── UTILITY: make a simple grid tab ──────────────────────
        private TabPage MakeGridTab(string tabName, string btnLabel, Func<DataTable> loader)
        {
            var tab = new TabPage(tabName);
            var dgv = MakeDgv();
            var btn = MakeBtn(btnLabel);
            btn.Click += (s, e) => dgv.DataSource = loader() ?? new DataTable();
            dgv.DataSource = loader() ?? new DataTable(); // auto-load
            tab.Controls.Add(dgv);
            tab.Controls.Add(btn);
            return tab;
        }

        private DataGridView MakeDgv()
        {
            return new DataGridView
            {
                Location = new Point(0, 44),
                Size = new Size(1000, 580),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.AliceBlue }
            };
        }

        private Button MakeBtn(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
        }

        private void BuildAllTabs()
        {
            tabs.TabPages.Add(BuildDashboardTab());               // M1
            tabs.TabPages.Add(MakeGridTab("📖 Most Borrowed", "Load Most Borrowed Books", () => _ctrl.Report_MostBorrowedBooks()));   // M2
            tabs.TabPages.Add(MakeGridTab("⏱ Avg Duration", "Load Avg Borrowing Duration", () => _ctrl.Report_AvgBorrowDuration()));   // M3
            tabs.TabPages.Add(MakeGridTab("💰 Fine Revenue", "Load Fine Revenue by Month", () => _ctrl.Report_FineRevenue()));         // M4
            tabs.TabPages.Add(MakeGridTab("📉 Overdue Rate", "Load Overdue Statistics", () => _ctrl.Report_OverdueStats()));        // M5
            tabs.TabPages.Add(MakeGridTab("📂 By Category", "Load Books by Category", () => _ctrl.Report_BooksByCategory()));     // M6
            tabs.TabPages.Add(MakeGridTab("👤 Top Members", "Load Top Active Members", () => _ctrl.Report_TopMembers()));          // M7
            tabs.TabPages.Add(MakeGridTab("✍️ Authors", "Load Author Popularity", () => _ctrl.Report_AuthorPopularity()));    // M8
            tabs.TabPages.Add(BuildOverdueBooksTab());            // D1
            tabs.TabPages.Add(BuildBorrowingHistoryTab());        // D2
            tabs.TabPages.Add(MakeGridTab("🔖 Reservations", "Load Active Reservations", () => _ctrl.Report_ActiveReservations())); // D3
        }

        // ── MANAGERIAL REPORT 1: Dashboard ───────────────────────
        private TabPage BuildDashboardTab()
        {
            var tab = new TabPage("📊 Dashboard");
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                WrapContents = true,
                AutoScroll = true
            };

            var btn = MakeBtn("🔄 Refresh Dashboard");
            btn.Dock = DockStyle.None;
            btn.Size = new Size(200, 40);
            btn.Location = new Point(20, 10);

            btn.Click += (s, e) =>
            {
                // Remove old cards (keep button)
                while (panel.Controls.Count > 1)
                    panel.Controls.RemoveAt(1);

                var dt = _ctrl.Report_LibraryStats();
                if (dt == null || dt.Rows.Count == 0) return;
                var row = dt.Rows[0];

                var cards = new (string label, string col, Color color)[]
                {
                    ("📚 Total Books",         "TotalBooks",         Color.SteelBlue),
                    ("📋 Total Copies",        "TotalCopies",        Color.DimGray),
                    ("✅ Available",           "AvailableCopies",    Color.SeaGreen),
                    ("📖 Borrowed",            "BorrowedCopies",     Color.DarkOrange),
                    ("👤 Active Members",      "ActiveMembers",      Color.Purple),
                    ("⚠️ Overdue",            "OverdueCount",       Color.Firebrick),
                    ("🔖 Reservations",       "PendingReservations",Color.Teal),
                    ("💸 Unpaid Fines £",     "TotalUnpaidFines",   Color.DarkRed),
                    ("💰 Revenue £",          "TotalRevenue",       Color.DarkGreen),
                };

                foreach (var (label, col, color) in cards)
                {
                    string val = row[col]?.ToString() ?? "0";
                    var card = new Label
                    {
                        Text = $"{label}\n{val}",
                        Size = new Size(190, 95),
                        BackColor = color,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Margin = new Padding(10),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    panel.Controls.Add(card);
                }
            };

            panel.Controls.Add(btn);
            btn.PerformClick(); // auto-load on open
            tab.Controls.Add(panel);
            return tab;
        }

        // ── DETAILED REPORT 1: Overdue Books ─────────────────────
        private TabPage BuildOverdueBooksTab()
        {
            var tab = new TabPage("🚨 Overdue Books");
            var dgv = MakeDgv();

            // Highlight all rows red (they're all overdue)
            dgv.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
            };

            var btn = MakeBtn("Load Overdue Books");
            btn.Click += (s, e) => dgv.DataSource = _ctrl.Report_OverdueBooks() ?? new DataTable();
            dgv.DataSource = _ctrl.Report_OverdueBooks() ?? new DataTable();

            tab.Controls.Add(dgv);
            tab.Controls.Add(btn);
            return tab;
        }

        // ── DETAILED REPORT 2: Borrowing History with Filters ────
        private TabPage BuildBorrowingHistoryTab()
        {
            var tab = new TabPage("📋 Borrowing History");

            // Filter bar
            var filterPanel = new Panel { Dock = DockStyle.Top, Height = 50 };

            var lblFrom = new Label { Text = "From:", AutoSize = true, Location = new Point(10, 15) };
            var dtpFrom = new DateTimePicker { Location = new Point(55, 12), Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };

            var lblTo = new Label { Text = "To:", AutoSize = true, Location = new Point(190, 15) };
            var dtpTo = new DateTimePicker { Location = new Point(215, 12), Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today };

            var lblStatus = new Label { Text = "Status:", AutoSize = true, Location = new Point(350, 15) };
            var cmbStatus = new ComboBox { Location = new Point(405, 12), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new object[] { "(All)", "Active", "Returned", "Overdue" });
            cmbStatus.SelectedIndex = 0;

            var btn = new Button
            {
                Text = "Load",
                Location = new Point(540, 10),
                Width = 80,
                Height = 30,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };

            var dgv = MakeDgv();
            dgv.Location = new Point(0, 50);
            dgv.Size = new Size(1000, 574);

            btn.Click += (s, e) =>
            {
                string status = cmbStatus.SelectedItem?.ToString() == "(All)" ? null : cmbStatus.SelectedItem?.ToString();
                dgv.DataSource = _ctrl.Report_BorrowingHistory(
                    startDate: dtpFrom.Value.Date,
                    endDate: dtpTo.Value.Date,
                    statusFilter: status) ?? new DataTable();
            };

            // Auto-load
            dgv.DataSource = _ctrl.Report_BorrowingHistory() ?? new DataTable();

            filterPanel.Controls.AddRange(new Control[] { lblFrom, dtpFrom, lblTo, dtpTo, lblStatus, cmbStatus, btn });
            tab.Controls.Add(dgv);
            tab.Controls.Add(filterPanel);
            return tab;
        }
    }
}
