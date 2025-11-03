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
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const int KEYEVENTF_KEYUP = 0x0002;

        // Holds all currently pressed keys
        private static readonly HashSet<byte> pressedKeys = new HashSet<byte>();

        // Background worker thread
        private static readonly Thread repeatThread;
        private static bool running = true;

        // Key repeat rate (ms)
        private static int repeatDelayMs = 10;

        static Keyboard()
        {
            // Start the background thread
            repeatThread = new Thread(RepeatLoop)
            {
                IsBackground = true
            };
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
                        keybd_event(key, 0, 0, UIntPtr.Zero); // Simulate key-down repeat
                    }
                }
                Thread.Sleep(repeatDelayMs);
            }
        }

        public static void PressKey(byte keyCode)
        {
            lock (pressedKeys)
            {
                if (!pressedKeys.Contains(keyCode))
                {
                    pressedKeys.Add(keyCode);
                    keybd_event(keyCode, 0, 0, UIntPtr.Zero); // Initial key down
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
                    keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Key up
                }
            }
        }

        public static void TapKey(byte keyCode)
        {
            PressKey(keyCode);
            ReleaseKey(keyCode);
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
    }
}
