using KernelBox.RegistryEditor;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelBox.Registry
{
    /// <summary>
    /// 注册表操作类方法
    /// </summary>
    class cRegOtherFunction
    {
        /// <summary>
        /// 获取VALUE的名字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string GetRegValueName(string value)
        {
            return value == string.Empty ? "(Default)" : value;
        }

        /// <summary>
        /// 根据字符串路径转换为RegistryKey对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static RegistryKey ParseRootKey(string path)
        {
            RegistryKey key;
            switch (path)
            {
                case "HKEY_CLASSES_ROOT":
                    key = Microsoft.Win32.Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    key = Microsoft.Win32.Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    key = Microsoft.Win32.Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    key = Microsoft.Win32.Registry.Users;
                    break;
                default:
                    key = Microsoft.Win32.Registry.CurrentConfig;
                    break;
            }
            return key;
        }

        internal static void SplitKey(string key,out string hive,out string branch)
        {
            int index = key.IndexOf('\\');
            hive = string.Empty;
            branch = string.Empty;
            if (index.Equals(-1))
            {
                hive = key;
            }
            else
            {
                hive = key.Substring(0, index);
                branch = key.Substring(index + 1);
            }
        }

        /// <summary>
        /// 删除KEY方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool DeleteKey(string key)
        {
            try
            {
                cRegKey child = cRegKey.Parse(key);
                cRegKey parent = cRegKey.Parse(child.Parent, true);
                parent.Key.DeleteSubKeyTree(child.Name);
            }
            catch 
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除VALUE方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool DeleteValue(string key,string value)
        {
            try
            {
                cRegKey regKey = cRegKey.Parse(key, true);
                regKey.Key.DeleteValue(value, false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据KEY生成一个新的KEY名
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string GetNewKeyName(RegistryKey key)
        {
            List<cRegKey> subKeys = cRegistry.GetSubKeys(key);
            bool isfounded = false;
            int suffix = 0;
            string title = string.Empty;

            while (!isfounded)
            {
                suffix++;
                title = "New Key " + suffix;
                isfounded = !subKeys.Exists(subKey => subKey.Name == title);
            }

            return title;
        }

        /// <summary>
        /// 根据KEY生成一个新的VALUE名
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string GetNewValueName(RegistryKey key)
        {
            List<cRegValue> values = cRegistry.GetValues(key);
            bool isfounded = false;
            int suffix = 0;
            string title = string.Empty;

            while (!isfounded)
            {
                suffix++;
                title = "New Value #" + suffix;
                isfounded = !values.Exists(val => val.Name == title);
            }

            return title;
        }
    }
}
