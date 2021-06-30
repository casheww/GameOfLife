
namespace GameOfLife
{
    partial class GameForm
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSoupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugLabel = new System.Windows.Forms.Label();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.gridSizeLabel = new System.Windows.Forms.Label();
            this.speedControl = new System.Windows.Forms.TrackBar();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speedControl)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.playToolStripMenuItem,
            this.soupToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(888, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.PlayToolStripMenuItem_Click);
            // 
            // soupToolStripMenuItem
            // 
            this.soupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem1,
            this.exportSoupToolStripMenuItem});
            this.soupToolStripMenuItem.Name = "soupToolStripMenuItem";
            this.soupToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.soupToolStripMenuItem.Text = "Soup";
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem1.Text = "New";
            this.newToolStripMenuItem1.Click += new System.EventHandler(this.NewSoup_ToolStripMenuItem_Click);
            // 
            // exportSoupToolStripMenuItem
            // 
            this.exportSoupToolStripMenuItem.Name = "exportSoupToolStripMenuItem";
            this.exportSoupToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportSoupToolStripMenuItem.Text = "Export soup";
            this.exportSoupToolStripMenuItem.Click += new System.EventHandler(this.ExportSoupToolStripMenuItem_Click);
            // 
            // debugLabel
            // 
            this.debugLabel.AutoSize = true;
            this.debugLabel.Location = new System.Drawing.Point(12, 37);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(62, 13);
            this.debugLabel.TabIndex = 1;
            this.debugLabel.Text = "debug label";
            // 
            // gameTimer
            // 
            this.gameTimer.Enabled = true;
            this.gameTimer.Tick += new System.EventHandler(this.GameTimer_Tick);
            // 
            // gridSizeLabel
            // 
            this.gridSizeLabel.AutoSize = true;
            this.gridSizeLabel.Location = new System.Drawing.Point(12, 24);
            this.gridSizeLabel.Name = "gridSizeLabel";
            this.gridSizeLabel.Size = new System.Drawing.Size(70, 13);
            this.gridSizeLabel.TabIndex = 2;
            this.gridSizeLabel.Text = "gridSizeLabel";
            // 
            // speedControl
            // 
            this.speedControl.Location = new System.Drawing.Point(760, 27);
            this.speedControl.Name = "speedControl";
            this.speedControl.Size = new System.Drawing.Size(116, 45);
            this.speedControl.TabIndex = 3;
            this.speedControl.Value = 5;
            this.speedControl.Scroll += new System.EventHandler(this.SpeedControl_Scroll);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 515);
            this.Controls.Add(this.speedControl);
            this.Controls.Add(this.gridSizeLabel);
            this.Controls.Add(this.debugLabel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GameForm";
            this.Text = "Game of Life";
            this.Load += new System.EventHandler(this.GameForm_Click);
            this.ResizeEnd += new System.EventHandler(this.GameForm_ResizeEnd);
            this.Click += new System.EventHandler(this.GameForm_Click);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speedControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Label debugLabel;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.Label gridSizeLabel;
        private System.Windows.Forms.TrackBar speedControl;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportSoupToolStripMenuItem;
    }
}

