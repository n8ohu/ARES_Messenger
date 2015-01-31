namespace ARES_Messenger
{
    partial class Contact
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
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.txtCall = new System.Windows.Forms.TextBox();
            this.txtFreq = new System.Windows.Forms.TextBox();
            this.txtGSQTH = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.OK_Button = new System.Windows.Forms.Button();
            this.btnNow = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(22, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(83, 13);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "UTC Date/Time";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(148, 9);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(48, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Call Sign";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(233, 9);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(57, 13);
            this.Label3.TabIndex = 3;
            this.Label3.Text = "Freq (KHz)";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(178, 92);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(91, 13);
            this.Label4.TabIndex = 4;
            this.Label4.Text = "Grid Square/QTH";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(40, 92);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(35, 13);
            this.Label5.TabIndex = 5;
            this.Label5.Text = "Name";
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(7, 25);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(113, 20);
            this.txtDate.TabIndex = 6;
            // 
            // txtCall
            // 
            this.txtCall.Location = new System.Drawing.Point(129, 25);
            this.txtCall.Name = "txtCall";
            this.txtCall.Size = new System.Drawing.Size(85, 20);
            this.txtCall.TabIndex = 7;
            // 
            // txtFreq
            // 
            this.txtFreq.Location = new System.Drawing.Point(220, 25);
            this.txtFreq.Name = "txtFreq";
            this.txtFreq.Size = new System.Drawing.Size(95, 20);
            this.txtFreq.TabIndex = 8;
            // 
            // txtGSQTH
            // 
            this.txtGSQTH.Location = new System.Drawing.Point(104, 108);
            this.txtGSQTH.Name = "txtGSQTH";
            this.txtGSQTH.Size = new System.Drawing.Size(211, 20);
            this.txtGSQTH.TabIndex = 9;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(11, 108);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(87, 20);
            this.txtName.TabIndex = 10;
            // 
            // OK_Button
            // 
            this.OK_Button.Location = new System.Drawing.Point(3, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(118, 23);
            this.OK_Button.TabIndex = 11;
            this.OK_Button.Text = "Add/Edit";
            this.OK_Button.UseVisualStyleBackColor = true;
            // 
            // btnNow
            // 
            this.btnNow.Location = new System.Drawing.Point(7, 44);
            this.btnNow.Name = "btnNow";
            this.btnNow.Size = new System.Drawing.Size(75, 23);
            this.btnNow.TabIndex = 12;
            this.btnNow.Text = "UTC Now";
            this.btnNow.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(5, 139);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(108, 23);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete Contact";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(144, 3);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(57, 23);
            this.Cancel_Button.TabIndex = 14;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.60784F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.39216F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(111, 137);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(204, 29);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // Contact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 178);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNow);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtGSQTH);
            this.Controls.Add(this.txtFreq);
            this.Controls.Add(this.txtCall);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Contact";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Contact";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TextBox txtCall;
        private System.Windows.Forms.TextBox txtFreq;
        private System.Windows.Forms.TextBox txtGSQTH;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button btnNow;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}