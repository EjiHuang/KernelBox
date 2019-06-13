using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KernelBox.Registry.Editors
{
    internal partial class MultiStringEditor : Registry_ValueEditor
    {
        public MultiStringEditor(cRegValue value) : base(value)
        {
            InitializeComponent();
            multiStringTextBox.Text = string.Join("\r\n", ((string[])value.Data));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 以"\r\n"为分隔符转换为字符串数组
            SaveValue(multiStringTextBox.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
