namespace TheCards
{
    partial class frmTheCardsMainMenu
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
            this.btnViewer = new System.Windows.Forms.Button();
            this.btnBuilder = new System.Windows.Forms.Button();
            this.btnCreator = new System.Windows.Forms.Button();
            this.btnPlayer = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.bntExit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewer
            // 
            this.btnViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewer.Location = new System.Drawing.Point(251, 172);
            this.btnViewer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnViewer.Name = "btnViewer";
            this.btnViewer.Size = new System.Drawing.Size(200, 192);
            this.btnViewer.TabIndex = 1;
            this.btnViewer.Text = "Viewer";
            this.btnViewer.UseVisualStyleBackColor = true;
            this.btnViewer.Click += new System.EventHandler(this.btnViewer_Click);
            // 
            // btnBuilder
            // 
            this.btnBuilder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuilder.Location = new System.Drawing.Point(463, 172);
            this.btnBuilder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnBuilder.Name = "btnBuilder";
            this.btnBuilder.Size = new System.Drawing.Size(200, 192);
            this.btnBuilder.TabIndex = 2;
            this.btnBuilder.Text = "Builder";
            this.btnBuilder.UseVisualStyleBackColor = true;
            this.btnBuilder.Visible = false;
            // 
            // btnCreator
            // 
            this.btnCreator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreator.Location = new System.Drawing.Point(39, 172);
            this.btnCreator.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnCreator.Name = "btnCreator";
            this.btnCreator.Size = new System.Drawing.Size(200, 192);
            this.btnCreator.TabIndex = 3;
            this.btnCreator.Text = "Creator";
            this.btnCreator.UseVisualStyleBackColor = true;
            this.btnCreator.Visible = false;
            // 
            // btnPlayer
            // 
            this.btnPlayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayer.Location = new System.Drawing.Point(675, 172);
            this.btnPlayer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnPlayer.Name = "btnPlayer";
            this.btnPlayer.Size = new System.Drawing.Size(200, 192);
            this.btnPlayer.TabIndex = 4;
            this.btnPlayer.Text = "Player";
            this.btnPlayer.UseVisualStyleBackColor = true;
            this.btnPlayer.Visible = false;
            // 
            // btnSettings
            // 
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(887, 199);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(150, 144);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Visible = false;
            // 
            // bntExit
            // 
            this.bntExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntExit.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntExit.Location = new System.Drawing.Point(1049, 222);
            this.bntExit.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.bntExit.Name = "bntExit";
            this.bntExit.Size = new System.Drawing.Size(100, 96);
            this.bntExit.TabIndex = 6;
            this.bntExit.Text = "Exit";
            this.bntExit.UseVisualStyleBackColor = true;
            this.bntExit.Click += new System.EventHandler(this.bntExit_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnCreator);
            this.panel1.Controls.Add(this.bntExit);
            this.panel1.Controls.Add(this.btnBuilder);
            this.panel1.Controls.Add(this.btnSettings);
            this.panel1.Controls.Add(this.btnViewer);
            this.panel1.Controls.Add(this.btnPlayer);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1179, 547);
            this.panel1.TabIndex = 7;
            // 
            // frmTheCardsMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1204, 572);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmTheCardsMainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "The Cards - Main Menu";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnViewer;
        private System.Windows.Forms.Button btnBuilder;
        private System.Windows.Forms.Button btnCreator;
        private System.Windows.Forms.Button btnPlayer;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button bntExit;
        private System.Windows.Forms.Panel panel1;
    }
}

