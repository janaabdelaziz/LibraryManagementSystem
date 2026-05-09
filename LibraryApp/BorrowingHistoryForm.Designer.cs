namespace LibraryApp
{
    partial class BorrowingHistoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvBorrowingHistory = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBorrowingHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBorrowingHistory
            // 
            this.dgvBorrowingHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBorrowingHistory.Location = new System.Drawing.Point(30, 32);
            this.dgvBorrowingHistory.Name = "dgvBorrowingHistory";
            this.dgvBorrowingHistory.RowHeadersWidth = 51;
            this.dgvBorrowingHistory.RowTemplate.Height = 24;
            this.dgvBorrowingHistory.Size = new System.Drawing.Size(598, 383);
            this.dgvBorrowingHistory.TabIndex = 0;
            this.dgvBorrowingHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBorrowingHistory_CellContentClick);
            // 
            // BorrowingHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 450);
            this.Controls.Add(this.dgvBorrowingHistory);
            this.Name = "BorrowingHistoryForm";
            this.Text = "BorrowingHistoryForm";
            this.Load += new System.EventHandler(this.BorrowingHistoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBorrowingHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBorrowingHistory;
    }
}