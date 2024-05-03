using System.Runtime.InteropServices;
using ConsoleApp1.Configs;
using WindowsInput;
using WindowsInput.Native;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1
{
    public class Program
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(VirtualKeyCode vKey);

        /// <summary>
        /// 是否peek
        /// </summary>
        public static bool IsPeek { get; set; }

        public static InputSimulator Simulator { get; set; } = new();

        public static PeekTypeEnum PeekType { get; set; } = PeekTypeEnum.Unknow;
        
        /// <summary>
        /// 整个系统配置
        /// </summary>
        public static SystemConfig SystemConfiguration { get; set; }

        public static bool IsKeyDown(VirtualKeyCode key)
        {
            short state = GetAsyncKeyState(key);
            return (state & 0x8000) != 0;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("正常运行");
            
            // 配置文件
            Configuration();
            
            Thread sprayThread = new Thread(SprayThread);
            Thread peekThread = new Thread(PeekThread);
            Thread OneButtonPickUpGunThread = new Thread(OneButtonPickUpGun);

            peekThread.Start();
            sprayThread.Start();
            OneButtonPickUpGunThread.Start();

            while (true)
            {
                if (IsKeyDown(VirtualKeyCode.RIGHT))
                {
                    IsPeek = true;
                }

                if (IsKeyDown(VirtualKeyCode.XBUTTON1))
                {
                    IsPeek = false;
                    PeekType = PeekTypeEnum.Unknow;
                }
            }
        }

        #region 配置文件

        /// <summary>
        /// 配置文件初始化
        /// </summary>
        static void Configuration()
        {
            // 初始化配置文件
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            SystemConfiguration  = config.Get<SystemConfig>();
        }

        #endregion
        

        /// <summary>
        /// 一键捡枪
        /// </summary>
        static void OneButtonPickUpGun()
        {
            while (true)
            {
                if (IsKeyDown(VirtualKeyCode.XBUTTON1) && IsKeyDown(VirtualKeyCode.VK_R) &&
                    IsKeyDown(VirtualKeyCode.LBUTTON))
                {
                    // 打开背包
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    Thread.Sleep(SystemConfiguration.Macro.OneButtonPickUpGun.BagSleep);
                    
                    // 拖动第一个
                    foreach (var item in SystemConfiguration.Macro.OneButtonPickUpGun.MoveCoordinates)
                    {
                        Simulator.Mouse.MoveMouseTo(item.MoveStartX, item.MoveStartY);
                        Simulator.Mouse.LeftButtonDown();
                        Simulator.Mouse.MoveMouseTo(item.MoveEndX, item.MoveEndY);
                        Thread.Sleep(SystemConfiguration.Macro.OneButtonPickUpGun.MoveSleep);
                        Simulator.Mouse.LeftButtonUp();
                        
                        // 拖动之间的间隔
                        Thread.Sleep(SystemConfiguration.Macro.OneButtonPickUpGun.MoveIntervalSleep);
                    }

                    // 自动按r进行换枪
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_R);
                }
            }
        }

        /// <summary>
        /// 闪身peek，默认为下蹲状态
        /// </summary>
        static void PeekThread()
        {
            while (true)
            {
                if (IsPeek)
                {
                    // 站起逻辑
                    while (true)
                    {
                        bool isUp;
                        
                        // 先是右键的
                        isUp = PressRight_PeekUp();

                        if (isUp)
                        {
                            PeekType = PeekTypeEnum.Right;
                        }
                        else
                        {
                            isUp = PressLeft_PeekUp();
                            
                            if (isUp)
                            {
                                PeekType = PeekTypeEnum.Left;
                            }
                        }

                        if (isUp)
                        {
                            break;
                        }

                        if (!IsPeek)
                        {
                            break;
                        }
                    }
                    
                    // 防止冗余执行下面代码
                    if (!IsPeek)
                    {
                        continue;
                    }
                    
                    // 站起
                    Simulator.Keyboard.KeyDown(VirtualKeyCode.VK_A);
                    Thread.Sleep(30);
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                    Simulator.Keyboard.KeyUp(VirtualKeyCode.VK_A);

                    // 蹲下逻辑
                    while (true)
                    {
                        // 是否蹲下
                        bool isDown = false;

                        switch (PeekType)
                        {
                            case PeekTypeEnum.Left:
                                isDown = PressLeft_PeekDown();
                                break;
                            case PeekTypeEnum.Right:
                                isDown = PressRight_PeekDown();
                                break;
                        }

                        if (isDown)
                        {
                            break;
                        }

                        if (!IsPeek)
                        {
                            break;
                        }
                    }
                    
                    // 防止冗余执行下面代码
                    if (!IsPeek)
                    {
                        continue;
                    }
                    
                    // 蹲下
                    Simulator.Keyboard.KeyDown(VirtualKeyCode.VK_D);
                    Thread.Sleep(30);
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                    Simulator.Keyboard.KeyUp(VirtualKeyCode.VK_D);
                }
            }
        }

        #region RightPeekFunction
        /// <summary>
        /// 长按右键peek站起
        /// </summary>
        static bool PressRight_PeekUp()
        {
            if (IsKeyDown(VirtualKeyCode.RBUTTON))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 长按右键peek蹲下
        /// </summary>
        static bool PressRight_PeekDown()
        {
            if (!IsKeyDown(VirtualKeyCode.RBUTTON))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region LeftPeekFunction
        
        /// <summary>
        /// 左键peek站起
        /// </summary>
        /// <returns></returns>
        static bool PressLeft_PeekUp()
        {
            if (IsKeyDown(VirtualKeyCode.LBUTTON))
            {
                if (PeekType == PeekTypeEnum.Right)
                {
                    Thread.Sleep(500);
                }

                if (IsKeyDown(VirtualKeyCode.LBUTTON))
                {
                    while (true)
                    {
                        if (!IsKeyDown(VirtualKeyCode.LBUTTON))
                        {
                            return true;
                        }
                    
                    }
                }
                
            }

            return false;
        }
        
        /// <summary>
        /// 左键peek蹲下
        /// </summary>
        /// <returns></returns>
        static bool PressLeft_PeekDown()
        {
            if (IsKeyDown(VirtualKeyCode.LBUTTON))
            {
                while (true)
                {
                    if (!IsKeyDown(VirtualKeyCode.LBUTTON))
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }

        #endregion
        
        /// <summary>
        /// 闪身喷的自动下蹲方法
        /// </summary>
        static void SprayThread()
        {
            while (true)
            {
                // 检测鼠标左键状态
                if ((IsKeyDown(VirtualKeyCode.XBUTTON1) && IsKeyDown(VirtualKeyCode.VK_E) && IsKeyDown(VirtualKeyCode.LBUTTON)) ||
                    (IsKeyDown(VirtualKeyCode.XBUTTON1) && IsKeyDown(VirtualKeyCode.VK_Q) && IsKeyDown(VirtualKeyCode.LBUTTON)))
                {
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);

                    while (true)
                    {
                        if (!IsKeyDown(VirtualKeyCode.LBUTTON))
                        {
                            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                            break;
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}