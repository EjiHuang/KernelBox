using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelBox.Registry
{
    /// <summary>
    /// 用于记录KEY信息的类
    /// </summary>
    class cRegKey
    {
        public string Name { get; private set; }
        public RegistryKey Key { get; private set; }
        public string Parent { get; private set; }

        public cRegKey(string name,RegistryKey key)
        {
            this.Name = name;
            this.Key = key;
            int index = key.Name.Length - name.Length - 1;
            if (index > 0)
                this.Parent = key.Name.Substring(0, index);
        }

        public cRegKey(RegistryKey key):
            this(key.Name.Contains('\\')?key.Name.Substring(key.Name.LastIndexOf('\\')) : key.Name, key)
        {

        }

        // 通过KEY的路径，转换为cRegKey对象
        public static cRegKey Parse(string keyPath)
        {
            return Parse(keyPath, false);
        }

        // 通过KEY的路径，转换为cRegKey对象
        public static cRegKey Parse(string keyPath, bool writable)
        {
            string[] tokens = keyPath.Split(new char[] { '\\' }, 2);
            RegistryKey rootKey = cRegOtherFunction.ParseRootKey(tokens[0]);
            if (tokens.Length.Equals(1))
            {
                return new cRegKey(rootKey);
            }

            string path = tokens[1];
            string name = keyPath.Substring(keyPath.LastIndexOf('\\') + 1);
            try
            {
                var key = rootKey.OpenSubKey(path, writable);
                if (key.Equals(null))
                {
                    return null;
                }
                return new cRegKey(name, key);
            }
            catch
            {
                return null;
            }
        }
    }
}
