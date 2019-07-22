namespace UI
{
    partial class Cinsiyet
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
            this.label11 = new System.Windows.Forms.Label();
            this.rdoErkek = new System.Windows.Forms.RadioButton();
            this.rdoKadin = new System.Windows.Forms.RadioButton();
            this.btnCinsiyet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Calibri", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label11.Location = new System.Drawing.Point(60, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(130, 23);
            this.label11.TabIndex = 2;
            this.label11.Text = "Cinsiyet seçiniz:";
            // 
            // rdoErkek
            // 
            this.rdoErkek.AutoSize = true;
            this.rdoErkek.Checked = true;
            this.rdoErkek.Location = new System.Drawing.Point(35, 63);
            this.rdoErkek.Margin = new System.Windows.Forms.Padding(4);
            this.rdoErkek.Name = "rdoErkek";
            this.rdoErkek.Size = new System.Drawing.Size(65, 21);
            this.rdoErkek.TabIndex = 22;
            this.rdoErkek.TabStop = true;
            this.rdoErkek.Text = "Erkek";
            this.rdoErkek.UseVisualStyleBackColor = true;
            // 
            // rdoKadin
            // 
            this.rdoKadin.AutoSize = true;
            this.rdoKadin.Location = new System.Drawing.Point(157, 63);
            this.rdoKadin.Margin = new System.Windows.Forms.Padding(4);
            this.rdoKadin.Name = "rdoKadin";
            this.rdoKadin.Size = new System.Drawing.Size(65, 21);
            this.rdoKadin.TabIndex = 21;
            this.rdoKadin.Text = "Kadın";
            this.rdoKadin.UseVisualStyleBackColor = true;
            // 
            // btnCinsiyet
            // 
            this.btnCinsiyet.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnCinsiyet.Location = new System.Drawing.Point(38, 109);
            this.btnCinsiyet.Name = "btnCinsiyet";
            this.btnCinsiyet.Size = new System.Drawing.Size(153, 36);
            this.btnCinsiyet.TabIndex = 23;
            this.btnCinsiyet.Text = "Kaydet";
            this.btnCinsiyet.UseVisualStyleBackColor = false;
            this.btnCinsiyet.Click += new System.EventHandler(this.btnCinsiyet_Click);
            // 
            // Cinsiyet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(250, 157);
            this.Controls.Add(this.btnCinsiyet);
            this.Controls.Add(this.rdoErkek);
            this.Controls.Add(this.rdoKadin);
            this.Controls.Add(this.label11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Cinsiyet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cinsiyet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Cinsiyet_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.RadioButton rdoKadin;
        public System.Windows.Forms.RadioButton rdoErkek;
        public System.Windows.Forms.Button btnCinsiyet;
    }
}