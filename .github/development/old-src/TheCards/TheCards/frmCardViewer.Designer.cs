namespace TheCards
{
    partial class frmCardViewer
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
            this.btnExit = new System.Windows.Forms.Button();
            this.pnlViewer = new System.Windows.Forms.Panel();
            this.lblCardNumberTotal = new System.Windows.Forms.Label();
            this.lblCardNumberOf = new System.Windows.Forms.Label();
            this.lblCardNumberStart = new System.Windows.Forms.Label();
            this.lblCardNumberThroughSymbol = new System.Windows.Forms.Label();
            this.lblCardNumberEnd = new System.Windows.Forms.Label();
            this.bntDisplay = new System.Windows.Forms.Button();
            this.btnPageForward = new System.Windows.Forms.Button();
            this.btnPageBack = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblCardSize = new System.Windows.Forms.Label();
            this.lblCardSizeDecrease = new System.Windows.Forms.Label();
            this.lblCardSizeIncrease = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Location = new System.Drawing.Point(1227, 701);
            this.btnExit.Margin = new System.Windows.Forms.Padding(2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(25, 25);
            this.btnExit.TabIndex = 0;
            this.btnExit.Text = "E";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // pnlViewer
            // 
            this.pnlViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlViewer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlViewer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlViewer.Location = new System.Drawing.Point(11, 11);
            this.pnlViewer.Margin = new System.Windows.Forms.Padding(2);
            this.pnlViewer.Name = "pnlViewer";
            this.pnlViewer.Size = new System.Drawing.Size(1241, 686);
            this.pnlViewer.TabIndex = 2;
            this.pnlViewer.SizeChanged += new System.EventHandler(this.pnlCardViewerContainer_SizeChanged);
            // 
            // lblCardNumberTotal
            // 
            this.lblCardNumberTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardNumberTotal.Location = new System.Drawing.Point(667, 701);
            this.lblCardNumberTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardNumberTotal.Name = "lblCardNumberTotal";
            this.lblCardNumberTotal.Size = new System.Drawing.Size(58, 25);
            this.lblCardNumberTotal.TabIndex = 13;
            this.lblCardNumberTotal.Text = "99999";
            this.lblCardNumberTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCardNumberOf
            // 
            this.lblCardNumberOf.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardNumberOf.Location = new System.Drawing.Point(640, 701);
            this.lblCardNumberOf.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardNumberOf.Name = "lblCardNumberOf";
            this.lblCardNumberOf.Size = new System.Drawing.Size(23, 24);
            this.lblCardNumberOf.TabIndex = 12;
            this.lblCardNumberOf.Text = "of";
            this.lblCardNumberOf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCardNumberStart
            // 
            this.lblCardNumberStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardNumberStart.Location = new System.Drawing.Point(513, 702);
            this.lblCardNumberStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardNumberStart.Name = "lblCardNumberStart";
            this.lblCardNumberStart.Size = new System.Drawing.Size(58, 23);
            this.lblCardNumberStart.TabIndex = 8;
            this.lblCardNumberStart.Text = "99999";
            this.lblCardNumberStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCardNumberThroughSymbol
            // 
            this.lblCardNumberThroughSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardNumberThroughSymbol.Location = new System.Drawing.Point(563, 700);
            this.lblCardNumberThroughSymbol.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardNumberThroughSymbol.Name = "lblCardNumberThroughSymbol";
            this.lblCardNumberThroughSymbol.Size = new System.Drawing.Size(22, 24);
            this.lblCardNumberThroughSymbol.TabIndex = 11;
            this.lblCardNumberThroughSymbol.Text = "-";
            this.lblCardNumberThroughSymbol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCardNumberEnd
            // 
            this.lblCardNumberEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardNumberEnd.Location = new System.Drawing.Point(577, 703);
            this.lblCardNumberEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardNumberEnd.Name = "lblCardNumberEnd";
            this.lblCardNumberEnd.Size = new System.Drawing.Size(59, 21);
            this.lblCardNumberEnd.TabIndex = 10;
            this.lblCardNumberEnd.Text = "99999";
            this.lblCardNumberEnd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bntDisplay
            // 
            this.bntDisplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntDisplay.Location = new System.Drawing.Point(1198, 701);
            this.bntDisplay.Margin = new System.Windows.Forms.Padding(2);
            this.bntDisplay.Name = "bntDisplay";
            this.bntDisplay.Size = new System.Drawing.Size(25, 25);
            this.bntDisplay.TabIndex = 5;
            this.bntDisplay.Text = "D";
            this.bntDisplay.UseVisualStyleBackColor = true;
            this.bntDisplay.Click += new System.EventHandler(this.TEST_Click);
            // 
            // btnPageForward
            // 
            this.btnPageForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPageForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPageForward.Location = new System.Drawing.Point(729, 702);
            this.btnPageForward.Margin = new System.Windows.Forms.Padding(2);
            this.btnPageForward.Name = "btnPageForward";
            this.btnPageForward.Size = new System.Drawing.Size(25, 25);
            this.btnPageForward.TabIndex = 6;
            this.btnPageForward.Text = ">";
            this.btnPageForward.UseVisualStyleBackColor = true;
            this.btnPageForward.Click += new System.EventHandler(this.btnPageForward_Click);
            // 
            // btnPageBack
            // 
            this.btnPageBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPageBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPageBack.Location = new System.Drawing.Point(464, 701);
            this.btnPageBack.Margin = new System.Windows.Forms.Padding(2);
            this.btnPageBack.Name = "btnPageBack";
            this.btnPageBack.Size = new System.Drawing.Size(25, 25);
            this.btnPageBack.TabIndex = 7;
            this.btnPageBack.Text = "<";
            this.btnPageBack.UseVisualStyleBackColor = true;
            this.btnPageBack.Click += new System.EventHandler(this.btnPageBack_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.Location = new System.Drawing.Point(99, 703);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 25);
            this.trackBar1.TabIndex = 14;
            // 
            // lblCardSize
            // 
            this.lblCardSize.AutoSize = true;
            this.lblCardSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardSize.Location = new System.Drawing.Point(13, 705);
            this.lblCardSize.Name = "lblCardSize";
            this.lblCardSize.Size = new System.Drawing.Size(67, 17);
            this.lblCardSize.TabIndex = 15;
            this.lblCardSize.Text = "Card size";
            // 
            // lblCardSizeDecrease
            // 
            this.lblCardSizeDecrease.AutoSize = true;
            this.lblCardSizeDecrease.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCardSizeDecrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardSizeDecrease.Location = new System.Drawing.Point(83, 701);
            this.lblCardSizeDecrease.Name = "lblCardSizeDecrease";
            this.lblCardSizeDecrease.Size = new System.Drawing.Size(17, 24);
            this.lblCardSizeDecrease.TabIndex = 16;
            this.lblCardSizeDecrease.Text = "-";
            // 
            // lblCardSizeIncrease
            // 
            this.lblCardSizeIncrease.AutoSize = true;
            this.lblCardSizeIncrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCardSizeIncrease.Location = new System.Drawing.Point(198, 703);
            this.lblCardSizeIncrease.Name = "lblCardSizeIncrease";
            this.lblCardSizeIncrease.Size = new System.Drawing.Size(21, 24);
            this.lblCardSizeIncrease.TabIndex = 17;
            this.lblCardSizeIncrease.Text = "+";
            // 
            // frmCardViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1264, 730);
            this.Controls.Add(this.lblCardNumberEnd);
            this.Controls.Add(this.lblCardSizeIncrease);
            this.Controls.Add(this.lblCardSizeDecrease);
            this.Controls.Add(this.lblCardSize);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.lblCardNumberTotal);
            this.Controls.Add(this.btnPageBack);
            this.Controls.Add(this.btnPageForward);
            this.Controls.Add(this.lblCardNumberThroughSymbol);
            this.Controls.Add(this.bntDisplay);
            this.Controls.Add(this.lblCardNumberOf);
            this.Controls.Add(this.pnlViewer);
            this.Controls.Add(this.lblCardNumberStart);
            this.Controls.Add(this.btnExit);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmCardViewer";
            this.Text = "The Cards";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel pnlViewer;
        private System.Windows.Forms.Button bntDisplay;
        private System.Windows.Forms.Button btnPageForward;
        private System.Windows.Forms.Button btnPageBack;
        private System.Windows.Forms.Label lblCardNumberStart;
        private System.Windows.Forms.Label lblCardNumberEnd;
        private System.Windows.Forms.Label lblCardNumberThroughSymbol;
        private System.Windows.Forms.Label lblCardNumberOf;
        private System.Windows.Forms.Label lblCardNumberTotal;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lblCardSize;
        private System.Windows.Forms.Label lblCardSizeDecrease;
        private System.Windows.Forms.Label lblCardSizeIncrease;
    }
}