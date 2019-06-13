using Be.Windows.Forms;
using System;

namespace KernelBox.Registry.Editors
{
    internal partial class BinaryEditor : Registry_ValueEditor
    {
        // 封装好的HexBox类，用于更方便地编辑字节类型
        DynamicByteProvider byteProvider;

        public BinaryEditor(cRegValue value) : base(value)
        {
            InitializeComponent();
            byteProvider = new DynamicByteProvider((byte[])value.Data);
            byteTextBox.ByteProvider = byteProvider;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveValue(byteProvider.Bytes.GetBytes());
        }
    }
}