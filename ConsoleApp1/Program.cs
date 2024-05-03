using ConsoleApp1.Configs;
using WindowsInput;
using WindowsInput.Native;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("正常运行");
            
            // 加载配置
            var systemConfig = ConfigurationManager.LoadConfig();
            
            // 创建动作管理器并启动线程
            ActionManager actionManager = new ActionManager(systemConfig);
            actionManager.Start();
            
            // 持续检测按键状态
            while (true)
            {
                // 如果按下右键，设置 Peek 标志为 true
                if (KeyboardHelper.IsKeyDown(VirtualKeyCode.RIGHT))
                {
                    actionManager.SetPeek(true);
                }

                // 如果按下鼠标侧键，取消 Peek
                if (KeyboardHelper.IsKeyDown(VirtualKeyCode.XBUTTON1))
                {
                    actionManager.CancelPeek();
                }
            }
        }
        
    }
}