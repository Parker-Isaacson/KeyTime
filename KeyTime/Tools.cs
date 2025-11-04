using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyTime
{
    public class Data(String? fileText = null)
    {
        private readonly String? _fileText = fileText;

        public String? GetFileText() { return _fileText; }
    }

    public static class PercisionTimer
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint uMilliseconds);

        public static void InitHighResolution()
        {
            TimeBeginPeriod(1);
        }

        public static void DeinitHighResolution()
        {
            TimeEndPeriod(1);
        }

        public static void AccurateDelay(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return;
            }
            Stopwatch sw = Stopwatch.StartNew();
            int spinThreshold = Math.Min(2, milliseconds / 10);
            if (milliseconds > spinThreshold)
            {
                Thread.Sleep(milliseconds - spinThreshold);
            }
            while (sw.ElapsedMilliseconds < milliseconds)
            {
                Thread.SpinWait(100);
            }
        }
    }
    public static class Keyboard
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion U;
            public static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        private static readonly HashSet<byte> pressedKeys = new HashSet<byte>();
        private static readonly Thread repeatThread;
        private static bool running = true;
        private static int repeatDelayMs = 10;

        static Keyboard()
        {
            repeatThread = new Thread(RepeatLoop) { IsBackground = true };
            repeatThread.Start();
        }

        private static void RepeatLoop()
        {
            while (running)
            {
                lock (pressedKeys)
                {
                    foreach (var key in pressedKeys)
                    {
                        SendKeyEvent(key, false);
                    }
                }
                Thread.Sleep(repeatDelayMs);
            }
        }

        private static void SendKeyEvent(byte keyCode, bool keyUp)
        {
            ushort scanCode = (ushort)MapVirtualKey(keyCode, 0);

            INPUT input = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keyCode,
                        wScan = scanCode,
                        dwFlags = keyUp ? KEYEVENTF_KEYUP : 0,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            };

            SendInput(1, new INPUT[] { input }, INPUT.Size);
        }

        public static void PressKey(byte keyCode)
        {
            lock (pressedKeys)
            {
                if (!pressedKeys.Contains(keyCode))
                {
                    pressedKeys.Add(keyCode);
                    SendKeyEvent(keyCode, false);
                }
            }
        }

        public static void ReleaseKey(byte keyCode)
        {
            lock (pressedKeys)
            {
                if (pressedKeys.Contains(keyCode))
                {
                    pressedKeys.Remove(keyCode);
                    SendKeyEvent(keyCode, true);
                }
            }
        }

        public static void TapKey(byte keyCode)
        {
            SendKeyEvent(keyCode, false);
            Thread.Sleep(50);
            SendKeyEvent(keyCode, true);
        }

        public static byte CharToVirtualKey(char c)
        {
            if (c >= 'A' && c <= 'Z')
                return (byte)c;
            if (c >= 'a' && c <= 'z')
                return (byte)char.ToUpper(c);
            if (c >= '0' && c <= '9')
                return (byte)c;
            return 0;
        }

        public static void Shutdown()
        {
            running = false;
        }
    }
}
