namespace KernelBox.Registry.Editors
{
    partial class MultiStringEditor
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
            this.multiStringTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // multiStringTextBox
            // 
            this.multiStringTextBox.AcceptsReturn = true;
            this.multiStringTextBox.Location = new System.Drawing.Point(16, 106);
            this.multiStringTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.multiStringTextBox.Multiline = true;
            this.multiStringTextBox.Name = "multiStringTextBox";
            this.multiStringTextBox.Size = new System.Drawing.Size(396, 167);
            this.multiStringTextBox.TabIndex = 15;
            // 
            // MultiStringEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.ClientSize = new System.Drawing.Size(426, 326);
            this.Controls.Add(this.multiStringTextBox);
            this.Name = "MultiStringEditor";
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtName, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.multiStringTextBox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox multiStringTextBox;
    }
}
