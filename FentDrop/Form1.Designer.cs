namespace FentDrop
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerPersonaje = new System.Windows.Forms.Timer(this.components);
            this.timerjuego = new System.Windows.Forms.Timer(this.components);
            this.timerFrenesi = new System.Windows.Forms.Timer(this.components);
            this.timerParpadeo = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerPersonaje
            // 
            this.timerPersonaje.Interval = 16;
            this.timerPersonaje.Tick += new System.EventHandler(this.timerPersonaje_Tick);
            // 
            // timerjuego
            // 
            this.timerjuego.Interval = 16;
            this.timerjuego.Tick += new System.EventHandler(this.timerjuego_Tick);
            // 
            // timerFrenesi
            // 
            this.timerFrenesi.Interval = 1000;
            this.timerFrenesi.Tick += new System.EventHandler(this.timerFrenesi_Tick);
            // 
            // timerParpadeo
            // 
            this.timerParpadeo.Interval = 500;
            this.timerParpadeo.Tick += new System.EventHandler(this.timerParpadeo_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1432, 836);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "FentDrop";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerPersonaje;
        private System.Windows.Forms.Timer timerjuego;
        private System.Windows.Forms.Timer timerFrenesi;
        private System.Windows.Forms.Timer timerParpadeo;
    }
}

