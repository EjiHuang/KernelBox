using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KernelBox.Registry.Editors
{
    internal partial class StringEditor : KernelBox.Registry.Editors.Registry_ValueEditor
    {
        public StringEditor(cRegValue value) : base(value)
        {
            InitializeComponent();
            textBox.Text = value.Data.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveValue(textBox.Text);
        }
    }
}
