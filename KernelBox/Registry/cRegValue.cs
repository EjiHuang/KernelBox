using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace KernelBox.Registry
{
    /// <summary>
    /// 用于记录VALUE的类
    /// </summary>
    class cRegValue
    {
        string name;

        // 记录当前VALUE是否为Default
        public bool IsDefault
        {
            get
            {
                return name == String.Empty;
            }
        }

        // 保存VALUE的Name
        public string Name
        {
            get
            {
                if (IsDefault)
                    return "(Default)";
                else
                    return name;
            }
            set
            {
                name = value;
            }
        }

        // 注册表中存储值时所用的数据类型
        public RegistryValueKind Kind { get; set; }

        // VALUE的数据
        public object Data { get; set; }

        // 上一级的Key
        public RegistryKey ParentKey { get; private set; }

        // 构造函数，初始化数据
        public cRegValue(string name,RegistryValueKind kind,object data)
        {
            this.name = name;
            Kind = kind;
            Data = data;
        }

        public cRegValue(RegistryKey parentKey, string valueName):
            this(valueName, parentKey.GetValueKind(valueName), parentKey.GetValue(valueName))
        {
            ParentKey = parentKey;
        }
        
        // 覆盖ToString方法
        public override string ToString()
        {
            return ToString(Kind, Data);
        }

        // 当参数只有ValueData时，需判断数据类型，再做字符串返回
        public static string ToString(object valueData)
        {
            if (valueData is byte[])
                return Encoding.ASCII.GetString((byte[])valueData);
            else
                return valueData.ToString();
        }

        // 当参数为两个时，需判断RegistryValueKind枚举值来判断ValueData的类型，再做字符串返回
        public static string ToString(RegistryValueKind valueKind, object valueData)
        {
            string data;
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    data = ByteArrayToString((byte[])valueData);
                    break;
                case RegistryValueKind.MultiString:
                    data = String.Join(" ", (string[])valueData);
                    break;
                case RegistryValueKind.DWord:
                    data = ((UInt32)((Int32)valueData)).ToString();
                    break;
                case RegistryValueKind.QWord:
                    data = ((UInt64)((Int64)valueData)).ToString();
                    break;
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    data = valueData.ToString();
                    break;
                case RegistryValueKind.Unknown:
                default:
                    data = String.Empty;
                    break;
            }
            return data;
        }

        // 字节数组转换为16进制字符串
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", " ");
        }

        // 直接覆盖GetHashCode函数，返回Name的哈希码
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
