using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelBox.Registry
{
    /// <summary>
    /// 这个类用于处理一些扩展的方法，用于简化主类代码量
    /// </summary>
    static class cRegExtensions
    {
        /// <summary>
        /// 获取Data的类型，并以字符串形式返回
        /// </summary>
        /// <param name="valueKind"></param>
        /// <returns></returns>
        public static string ToDataType(this RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    return "REG_BINARY";
                case RegistryValueKind.DWord:
                    return "REG_DWORD";
                case RegistryValueKind.ExpandString:
                    return "REG_EXPAND_SZ";
                case RegistryValueKind.MultiString:
                    return "REG_MULTI_SZ";
                case RegistryValueKind.QWord:
                    return "REG_QWORD";
                case RegistryValueKind.String:
                    return "REG_SZ";
                case RegistryValueKind.Unknown:
                    return "REG_UNKNOWN";
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// 给类型数据创建默认值
        /// </summary>
        /// <param name="valueKind"></param>
        /// <returns></returns>
        public static object GetDefaultData(this RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    return new byte[0];
                case RegistryValueKind.DWord:
                    return 0;
                case RegistryValueKind.ExpandString:
                    return String.Empty;
                case RegistryValueKind.MultiString:
                    return new string[0];
                case RegistryValueKind.QWord:
                    return (long)0;
                case RegistryValueKind.String:
                    return String.Empty;
                case RegistryValueKind.Unknown:
                default:
                    return null;
            }
        }
    }
}
