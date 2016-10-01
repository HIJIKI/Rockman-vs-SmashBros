namespace MapEditor
{
    partial class MainWindow
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
            this.mapDisplay1 = new MapEditor.MapDisplay();
            this.SuspendLayout();
            // 
            // mapDisplay1
            // 
            this.mapDisplay1.Location = new System.Drawing.Point(12, 12);
            this.mapDisplay1.Name = "mapDisplay1";
            this.mapDisplay1.Size = new System.Drawing.Size(500, 350);
            this.mapDisplay1.TabIndex = 0;
            this.mapDisplay1.Text = "mapDisplay1";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 374);
            this.Controls.Add(this.mapDisplay1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private MapDisplay mapDisplay1;
    }
}