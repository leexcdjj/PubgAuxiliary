using System.Runtime.InteropServices;
using WindowsInput.Native;

namespace ConsoleApp1;

/// <summary>
/// 原生封装，由于一些操作WindowsInput没有封装，所以自己采用封装
/// </summary>
public class KeyboardHelper
{
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(VirtualKeyCode vKey);
        
    public static bool IsKeyDown(VirtualKeyCode key)
    {
        short state = GetAsyncKeyState(key);
        return (state & 0x8000) != 0;
    }
}