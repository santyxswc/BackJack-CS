namespace BlackjackForm
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCartasJugador;
        private System.Windows.Forms.Label lblCartasBanca;
        private System.Windows.Forms.Label lblValorJugador;
        private System.Windows.Forms.Label lblValorBanca;
        private System.Windows.Forms.Button btnPedirCarta;
        private System.Windows.Forms.Button btnRetirarse;
        private System.Windows.Forms.FlowLayoutPanel panelCartasJugador;
        private System.Windows.Forms.FlowLayoutPanel panelCartasBanca;


        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.lblCartasJugador = new System.Windows.Forms.Label();
            this.lblCartasBanca = new System.Windows.Forms.Label();
            this.lblValorJugador = new System.Windows.Forms.Label();
            this.lblValorBanca = new System.Windows.Forms.Label();
            this.btnPedirCarta = new System.Windows.Forms.Button();
            this.btnRetirarse = new System.Windows.Forms.Button();
            this.panelCartasJugador = new System.Windows.Forms.FlowLayoutPanel();
            this.panelCartasBanca = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSaldo = new System.Windows.Forms.Label();
            this.txtApuesta = new System.Windows.Forms.TextBox();
            this.btnApostar = new System.Windows.Forms.Button();
            this.apuesta = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCartasJugador
            // 
            this.lblCartasJugador.AutoSize = true;
            this.lblCartasJugador.BackColor = System.Drawing.Color.Transparent;
            this.lblCartasJugador.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblCartasJugador.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCartasJugador.Location = new System.Drawing.Point(63, 167);
            this.lblCartasJugador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCartasJugador.Name = "lblCartasJugador";
            this.lblCartasJugador.Size = new System.Drawing.Size(192, 29);
            this.lblCartasJugador.TabIndex = 0;
            this.lblCartasJugador.Text = "Cartas Jugador:";
            this.lblCartasJugador.Click += new System.EventHandler(this.lblCartasJugador_Click);
            // 
            // lblCartasBanca
            // 
            this.lblCartasBanca.AutoSize = true;
            this.lblCartasBanca.BackColor = System.Drawing.Color.Transparent;
            this.lblCartasBanca.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblCartasBanca.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCartasBanca.Location = new System.Drawing.Point(63, 438);
            this.lblCartasBanca.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCartasBanca.Name = "lblCartasBanca";
            this.lblCartasBanca.Size = new System.Drawing.Size(173, 29);
            this.lblCartasBanca.TabIndex = 1;
            this.lblCartasBanca.Text = "Cartas Banca:";
            this.lblCartasBanca.Click += new System.EventHandler(this.lblCartasBanca_Click);
            // 
            // lblValorJugador
            // 
            this.lblValorJugador.AutoSize = true;
            this.lblValorJugador.BackColor = System.Drawing.Color.Transparent;
            this.lblValorJugador.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblValorJugador.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblValorJugador.Location = new System.Drawing.Point(892, 167);
            this.lblValorJugador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValorJugador.Name = "lblValorJugador";
            this.lblValorJugador.Size = new System.Drawing.Size(177, 29);
            this.lblValorJugador.TabIndex = 2;
            this.lblValorJugador.Text = "Valor Jugador:";
            // 
            // lblValorBanca
            // 
            this.lblValorBanca.AutoSize = true;
            this.lblValorBanca.BackColor = System.Drawing.Color.Transparent;
            this.lblValorBanca.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblValorBanca.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblValorBanca.Location = new System.Drawing.Point(892, 438);
            this.lblValorBanca.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValorBanca.Name = "lblValorBanca";
            this.lblValorBanca.Size = new System.Drawing.Size(158, 29);
            this.lblValorBanca.TabIndex = 3;
            this.lblValorBanca.Text = "Valor Banca:";
            // 
            // btnPedirCarta
            // 
            this.btnPedirCarta.Location = new System.Drawing.Point(66, 761);
            this.btnPedirCarta.Margin = new System.Windows.Forms.Padding(2);
            this.btnPedirCarta.Name = "btnPedirCarta";
            this.btnPedirCarta.Size = new System.Drawing.Size(130, 46);
            this.btnPedirCarta.TabIndex = 4;
            this.btnPedirCarta.Text = "Pedir Carta";
            this.btnPedirCarta.UseVisualStyleBackColor = true;
            this.btnPedirCarta.Click += new System.EventHandler(this.btnPedirCarta_Click);
            // 
            // btnRetirarse
            // 
            this.btnRetirarse.Location = new System.Drawing.Point(324, 761);
            this.btnRetirarse.Margin = new System.Windows.Forms.Padding(2);
            this.btnRetirarse.Name = "btnRetirarse";
            this.btnRetirarse.Size = new System.Drawing.Size(120, 46);
            this.btnRetirarse.TabIndex = 5;
            this.btnRetirarse.Text = "Retirarse";
            this.btnRetirarse.UseVisualStyleBackColor = true;
            this.btnRetirarse.Click += new System.EventHandler(this.btnRetirarse_Click);
            // 
            // panelCartasJugador
            // 
            this.panelCartasJugador.BackColor = System.Drawing.Color.Transparent;
            this.panelCartasJugador.Location = new System.Drawing.Point(63, 239);
            this.panelCartasJugador.Name = "panelCartasJugador";
            this.panelCartasJugador.Size = new System.Drawing.Size(543, 150);
            this.panelCartasJugador.TabIndex = 6;
            this.panelCartasJugador.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCartasJugador_Paint);
            // 
            // panelCartasBanca
            // 
            this.panelCartasBanca.BackColor = System.Drawing.Color.Transparent;
            this.panelCartasBanca.Location = new System.Drawing.Point(63, 545);
            this.panelCartasBanca.Name = "panelCartasBanca";
            this.panelCartasBanca.Size = new System.Drawing.Size(543, 150);
            this.panelCartasBanca.TabIndex = 7;
            this.panelCartasBanca.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCartasBanca_Paint);
            // 
            // lblSaldo
            // 
            this.lblSaldo.AutoSize = true;
            this.lblSaldo.BackColor = System.Drawing.Color.Transparent;
            this.lblSaldo.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F);
            this.lblSaldo.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblSaldo.Location = new System.Drawing.Point(65, 48);
            this.lblSaldo.Name = "lblSaldo";
            this.lblSaldo.Size = new System.Drawing.Size(116, 44);
            this.lblSaldo.TabIndex = 9;
            this.lblSaldo.Text = "Saldo";
            this.lblSaldo.Click += new System.EventHandler(this.lblSaldo_Click);
            // 
            // txtApuesta
            // 
            this.txtApuesta.Location = new System.Drawing.Point(884, 701);
            this.txtApuesta.Multiline = true;
            this.txtApuesta.Name = "txtApuesta";
            this.txtApuesta.Size = new System.Drawing.Size(185, 56);
            this.txtApuesta.TabIndex = 10;
            this.txtApuesta.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnApostar
            // 
            this.btnApostar.Location = new System.Drawing.Point(920, 763);
            this.btnApostar.Name = "btnApostar";
            this.btnApostar.Size = new System.Drawing.Size(109, 44);
            this.btnApostar.TabIndex = 11;
            this.btnApostar.Text = "apostar";
            this.btnApostar.UseVisualStyleBackColor = true;
            this.btnApostar.Click += new System.EventHandler(this.btnApostar_Click_1);
            // 
            // apuesta
            // 
            this.apuesta.AutoSize = true;
            this.apuesta.BackColor = System.Drawing.Color.Transparent;
            this.apuesta.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.25F);
            this.apuesta.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.apuesta.Location = new System.Drawing.Point(948, 69);
            this.apuesta.Name = "apuesta";
            this.apuesta.Size = new System.Drawing.Size(266, 44);
            this.apuesta.TabIndex = 13;
            this.apuesta.Text = "apuesta actual";
            this.apuesta.Click += new System.EventHandler(this.label2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1465, 850);
            this.Controls.Add(this.apuesta);
            this.Controls.Add(this.btnApostar);
            this.Controls.Add(this.txtApuesta);
            this.Controls.Add(this.lblSaldo);
            this.Controls.Add(this.panelCartasBanca);
            this.Controls.Add(this.panelCartasJugador);
            this.Controls.Add(this.btnRetirarse);
            this.Controls.Add(this.btnPedirCarta);
            this.Controls.Add(this.lblValorBanca);
            this.Controls.Add(this.lblValorJugador);
            this.Controls.Add(this.lblCartasBanca);
            this.Controls.Add(this.lblCartasJugador);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "BlackJack";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblSaldo;
        private System.Windows.Forms.TextBox txtApuesta;
        private System.Windows.Forms.Button btnApostar;
        private System.Windows.Forms.Label apuesta;
    }
}
