﻿namespace BlackjackForm
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
            this.SuspendLayout();
            // 
            // lblCartasJugador
            // 
            this.lblCartasJugador.AutoSize = true;
            this.lblCartasJugador.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblCartasJugador.Location = new System.Drawing.Point(64, 29);
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
            this.lblCartasBanca.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblCartasBanca.Location = new System.Drawing.Point(64, 300);
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
            this.lblValorJugador.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblValorJugador.Location = new System.Drawing.Point(893, 29);
            this.lblValorJugador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValorJugador.Name = "lblValorJugador";
            this.lblValorJugador.Size = new System.Drawing.Size(177, 29);
            this.lblValorJugador.TabIndex = 2;
            this.lblValorJugador.Text = "Valor Jugador:";
            // 
            // lblValorBanca
            // 
            this.lblValorBanca.AutoSize = true;
            this.lblValorBanca.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.25F);
            this.lblValorBanca.Location = new System.Drawing.Point(893, 300);
            this.lblValorBanca.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValorBanca.Name = "lblValorBanca";
            this.lblValorBanca.Size = new System.Drawing.Size(158, 29);
            this.lblValorBanca.TabIndex = 3;
            this.lblValorBanca.Text = "Valor Banca:";
            // 
            // btnPedirCarta
            // 
            this.btnPedirCarta.Location = new System.Drawing.Point(67, 623);
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
            this.btnRetirarse.Location = new System.Drawing.Point(898, 623);
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
            this.panelCartasJugador.Location = new System.Drawing.Point(64, 101);
            this.panelCartasJugador.Name = "panelCartasJugador";
            this.panelCartasJugador.Size = new System.Drawing.Size(543, 150);
            this.panelCartasJugador.TabIndex = 6;
            this.panelCartasJugador.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCartasJugador_Paint);
            // 
            // panelCartasBanca
            // 
            this.panelCartasBanca.BackColor = System.Drawing.Color.Transparent;
            this.panelCartasBanca.Location = new System.Drawing.Point(64, 407);
            this.panelCartasBanca.Name = "panelCartasBanca";
            this.panelCartasBanca.Size = new System.Drawing.Size(543, 150);
            this.panelCartasBanca.TabIndex = 7;
            this.panelCartasBanca.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCartasBanca_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1291, 721);
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
    }
}
