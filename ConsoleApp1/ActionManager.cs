using ConsoleApp1.Configs;
using WindowsInput;
using WindowsInput.Native;

namespace ConsoleApp1;

public class ActionManager
{
    private SystemConfig _systemConfig;
    private Thread _peekThread;
    private Thread _sprayThread;
    private Thread _oneButtonPickUpGunThread;
    private bool IsPeek { get; set; }
    private PeekTypeEnum PeekType { get; set; }
    public static InputSimulator Simulator { get; set; } = new();

    public ActionManager(SystemConfig systemConfig)
    {
        _systemConfig = systemConfig;
        _peekThread = new Thread(PeekThread);
        _sprayThread = new Thread(SprayThread);
        _oneButtonPickUpGunThread = new Thread(OneButtonPickUpGun);
    }

    public void Start()
    {
        _peekThread.Start();
        _sprayThread.Start();
        _oneButtonPickUpGunThread.Start();
    }

    public void SetPeek(bool value)
    {
        IsPeek = value;
    }

    public void CancelPeek()
    {
        IsPeek = false;
        PeekType = PeekTypeEnum.Unknow;
    }
    
    #region RightPeekFunction
    /// <summary>
    /// 长按右键peek站起
    /// </summary>
    private bool PressRight_PeekUp()
    {
        if (KeyboardHelper.IsKeyDown(VirtualKeyCode.RBUTTON))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 长按右键peek蹲下
    /// </summary>
    private bool PressRight_PeekDown()
    {
        if (!KeyboardHelper.IsKeyDown(VirtualKeyCode.RBUTTON))
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
    private bool PressLeft_PeekUp()
    {
        if (KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
        {
            if (PeekType == PeekTypeEnum.Right)
            {
                Thread.Sleep(500);
            }

            if (KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
            {
                while (true)
                {
                    if (!KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
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
    private bool PressLeft_PeekDown()
    {
        if (KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
        {
            while (true)
            {
                if (!KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
                {
                    return true;
                }
                    
            }
        }

        return false;
    }

    #endregion
    
    /// <summary>
    /// Peek逻辑
    /// </summary>
    private void PeekThread()
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
    
    /// <summary>
    /// 闪身喷逻辑
    /// </summary>
    private void SprayThread()
    {
        while (true)
        {
            // 检测鼠标左键状态
            if ((KeyboardHelper.IsKeyDown(VirtualKeyCode.XBUTTON1) && KeyboardHelper.IsKeyDown(VirtualKeyCode.VK_E) && KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON)) ||
                (KeyboardHelper.IsKeyDown(VirtualKeyCode.XBUTTON1) && KeyboardHelper.IsKeyDown(VirtualKeyCode.VK_Q) && KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON)))
            {
                Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);

                while (true)
                {
                    if (!KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
                    {
                        Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_C);
                        break;
                    }
                }
            }

            Thread.Sleep(10);
        }
    }
    
    /// <summary>
    /// 一键捡枪逻辑
    /// </summary>
    private void OneButtonPickUpGun()
    {
        while (true)
        {
            if (KeyboardHelper.IsKeyDown(VirtualKeyCode.XBUTTON1) && KeyboardHelper.IsKeyDown(VirtualKeyCode.VK_R) &&
                KeyboardHelper.IsKeyDown(VirtualKeyCode.LBUTTON))
            {
                // 打开背包
                Simulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                Thread.Sleep(_systemConfig.Macro.OneButtonPickUpGun.BagSleep);
                    
                // 拖动第一个
                foreach (var item in _systemConfig.Macro.OneButtonPickUpGun.MoveCoordinates)
                {
                    Simulator.Mouse.MoveMouseTo(item.MoveStartX, item.MoveStartY);
                    Simulator.Mouse.LeftButtonDown();
                    Simulator.Mouse.MoveMouseTo(item.MoveEndX, item.MoveEndY);
                    Thread.Sleep(_systemConfig.Macro.OneButtonPickUpGun.MoveSleep);
                    Simulator.Mouse.LeftButtonUp();
                        
                    // 拖动之间的间隔
                    Thread.Sleep(_systemConfig.Macro.OneButtonPickUpGun.MoveIntervalSleep);
                }

                // 自动按r进行换枪
                Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_R);
            }
        }
    }
    
}