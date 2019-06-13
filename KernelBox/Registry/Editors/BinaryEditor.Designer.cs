namespace KernelBox.Registry.Editors
{
    partial class BinaryEditor
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
            this.byteTextBox = new Be.Windows.Forms.HexBox();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Size = new System.Drawing.Size(573, 28);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(477, 281);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(356, 281);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // byteTextBox
            // 
            this.byteTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.byteTextBox.BackColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.byteTextBox.BytesPerLine = 8;
            this.byteTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.byteTextBox.LineInfoForeColor = System.Drawing.Color.Empty;
            this.byteTextBox.LineInfoVisible = true;
            this.byteTextBox.Location = new System.Drawing.Point(13, 104);
            this.byteTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.byteTextBox.Name = "byteTextBox";
            this.byteTextBox.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.byteTextBox.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.byteTextBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.byteTextBox.Size = new System.Drawing.Size(576, 169);
            this.byteTextBox.StringViewVisible = true;
            this.byteTextBox.TabIndex = 15;
            this.byteTextBox.UseFixedBytesPerLine = true;
            // 
            // BinaryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.ClientSize = new System.Drawing.Size(602, 326);
            this.Controls.Add(this.byteTextBox);
            this.Name = "BinaryEditor";
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtName, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.byteTextBox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Be.Windows.Forms.HexBox byteTextBox;
    }
}
