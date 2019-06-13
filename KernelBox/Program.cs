using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KernelBox
{
    static class Program
    {
        static DriverManager DrvMgr = new DriverManager();
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Initialize();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            Quit();
        }

        // 主程序的初始化操作
        static void Initialize()
        {
            // 初始化驱动服务
            DrvMgr.InitializeDriver();
        }

        // 主程序的结束操作
        static void Quit()
        {
            DrvMgr.RemoveDrvier();
        }
    }
}
