namespace LibraryApp
{
    partial class InsertBook
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
            this.Title = new System.Windows.Forms.Label();
            this.ISBN = new System.Windows.Forms.Label();
            this.noOfCopies = new System.Windows.Forms.Label();
            this.PublisherID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CategoryID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Location = new System.Drawing.Point(60, 37);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(33, 16);
            this.Title.TabIndex = 0;
            this.Title.Text = "Title";
            // 
            // ISBN
            // 
            this.ISBN.AutoSize = true;
            this.ISBN.Location = new System.Drawing.Point(60, 76);
            this.ISBN.Name = "ISBN";
            this.ISBN.Size = new System.Drawing.Size(38, 16);
            this.ISBN.TabIndex = 1;
            this.ISBN.Text = "ISBN";
            // 
            // noOfCopies
            // 
            this.noOfCopies.AutoSize = true;
            this.noOfCopies.Location = new System.Drawing.Point(63, 154);
            this.noOfCopies.Name = "noOfCopies";
            this.noOfCopies.Size = new System.Drawing.Size(90, 16);
            this.noOfCopies.TabIndex = 3;
            this.noOfCopies.Text = "No. Of Copies";
            // 
            // PublisherID
            // 
            this.PublisherID.AutoSize = true;
            this.PublisherID.Location = new System.Drawing.Point(60, 117);
            this.PublisherID.Name = "PublisherID";
            this.PublisherID.Size = new System.Drawing.Size(79, 16);
            this.PublisherID.TabIndex = 2;
            this.PublisherID.Text = "Publisher ID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(66, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "label5";
            // 
            // CategoryID
            // 
            this.CategoryID.AutoSize = true;
            this.CategoryID.Location = new System.Drawing.Point(63, 185);
            this.CategoryID.Name = "CategoryID";
            this.CategoryID.Size = new System.Drawing.Size(78, 16);
            this.CategoryID.TabIndex = 4;
            this.CategoryID.Text = "Category ID";
            this.CategoryID.Click += new System.EventHandler(this.label6_Click);
            // 
            // InsertBook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CategoryID);
            this.Controls.Add(this.noOfCopies);
            this.Controls.Add(this.PublisherID);
            this.Controls.Add(this.ISBN);
            this.Controls.Add(this.Title);
            this.Name = "InsertBook";
            this.Text = "Insert Book";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label ISBN;
        private System.Windows.Forms.Label noOfCopies;
        private System.Windows.Forms.Label PublisherID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label CategoryID;
    }
}