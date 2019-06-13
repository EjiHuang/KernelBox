using KernelBox.Registry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KernelBox.Registry.Editors
{
    partial class Registry_ValueEditor : Form
    {
        protected cRegValue Value { get; private set; }

        private Registry_ValueEditor()
        {
            InitializeComponent();
        }

        public Registry_ValueEditor(cRegValue value):this()
        {
            Value = value;
            txtName.Text = value.Name;
            txtName.Modified = false;
        }

        protected void SaveValue(object data)
        {
            Microsoft.Win32.Registry.SetValue(Value.ParentKey.Name, Value.Name, data, Value.Kind);
        }
    }
}
