using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KernelBox.Registry.Editors
{
    internal partial class DWordQWordEditor : Registry_ValueEditor
    {
        public DWordQWordEditor(cRegValue value) : base(value)
        {
            InitializeComponent();
            string data;
            if (value.Kind.Equals(RegistryValueKind.DWord))
            {
                data = ((int)value.Data).ToString("x");
                numericTextBox.Text = data;
            }
            else
            {
                data = ((long)value.Data).ToString("x");
                numericTextBox.Text = data;
            }
        }

        // 10进制单选框被选中
        private void rdoDecimal_CheckedChanged(object sender, EventArgs e)
        {
            numericTextBox.HexNumber = rdoHex.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Value.Kind.Equals(RegistryValueKind.DWord))
            {
                SaveValue((int)numericTextBox.UIntValue);
            }
            else
            {
                SaveValue((long)numericTextBox.ULongValue);
            }
        }
    }
}
