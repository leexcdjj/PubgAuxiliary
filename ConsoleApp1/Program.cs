using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace ConsoleApp1
{
    public class Program
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(VirtualKeyCode vKey);

        /// <summary>
        /// 是否peek
        /// </summary>
        public static bool IsPeek { get; set; } = false;

        public static InputSimulator Simulator { get; set; } = new InputSimulator();

        public static PeekTypeEnum PeekType { get; set; } = PeekTypeEnum.Unknow;


        public static bool IsKeyDown(VirtualKeyCode key)
        {
            short state = GetAsyncKeyState(key);
            return (state & 0x8000) != 0;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("正常运行");

            Thread sprayThread = new Thread(SprayThread);
            Thread peekThread = new Thread(PeekThread);

            peekThread.Start();
            sprayThread.Start();

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
                        bool isUp = false;
                        
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
                    Console.WriteLine("站起 a");
                    Thread.Sleep(30);
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                    Console.WriteLine("站起 c");
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
                    Console.WriteLine("蹲下 d");
                    Thread.Sleep(30);
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                    Console.WriteLine("蹲下 c");
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
                    Console.WriteLine("闪身 c");

                    while (true)
                    {
                        if (!IsKeyDown(VirtualKeyCode.LBUTTON))
                        {
                            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                            Console.WriteLine("闪身 c");
                            break;
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}