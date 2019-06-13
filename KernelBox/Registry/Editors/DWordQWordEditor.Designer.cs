namespace KernelBox.Registry.Editors
{
    partial class DWordQWordEditor
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoDecimal = new System.Windows.Forms.RadioButton();
            this.rdoHex = new System.Windows.Forms.RadioButton();
            this.numericTextBox = new KernelBox.Registry.Controls.NumericTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(300, 195);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(179, 195);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoDecimal);
            this.groupBox1.Controls.Add(this.rdoHex);
            this.groupBox1.Location = new System.Drawing.Point(200, 82);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(212, 94);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base";
            // 
            // rdoDecimal
            // 
            this.rdoDecimal.AutoSize = true;
            this.rdoDecimal.Location = new System.Drawing.Point(16, 58);
            this.rdoDecimal.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDecimal.Name = "rdoDecimal";
            this.rdoDecimal.Size = new System.Drawing.Size(96, 22);
            this.rdoDecimal.TabIndex = 2;
            this.rdoDecimal.Text = "&Decimal";
            this.rdoDecimal.UseVisualStyleBackColor = true;
            this.rdoDecimal.CheckedChanged += new System.EventHandler(this.rdoDecimal_CheckedChanged);
            // 
            // rdoHex
            // 
            this.rdoHex.AutoSize = true;
            this.rdoHex.Checked = true;
            this.rdoHex.Location = new System.Drawing.Point(16, 26);
            this.rdoHex.Margin = new System.Windows.Forms.Padding(4);
            this.rdoHex.Name = "rdoHex";
            this.rdoHex.Size = new System.Drawing.Size(132, 22);
            this.rdoHex.TabIndex = 1;
            this.rdoHex.TabStop = true;
            this.rdoHex.Text = "&Hexadecimal";
            this.rdoHex.UseVisualStyleBackColor = true;
            // 
            // numericTextBox
            // 
            this.numericTextBox.AllowDecimal = false;
            this.numericTextBox.AllowGrouping = false;
            this.numericTextBox.AllowNegative = false;
            this.numericTextBox.HexNumber = true;
            this.numericTextBox.Location = new System.Drawing.Point(15, 108);
            this.numericTextBox.MaxLength = 8;
            this.numericTextBox.Name = "numericTextBox";
            this.numericTextBox.Size = new System.Drawing.Size(167, 28);
            this.numericTextBox.TabIndex = 16;
            this.numericTextBox.Text = "0";
            // 
            // DWordQWordEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.ClientSize = new System.Drawing.Size(426, 238);
            this.Controls.Add(this.numericTextBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "DWordQWordEditor";
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtName, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.numericTextBox, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoDecimal;
        private System.Windows.Forms.RadioButton rdoHex;
        private Controls.NumericTextBox numericTextBox;
    }
}
