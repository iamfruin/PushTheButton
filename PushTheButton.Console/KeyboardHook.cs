// Type: CommonComponent.globalKeyboardHook
// Assembly: BIG RED BUTTON, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Dream Cheeky\BIG RED BUTTON\BIG RED BUTTON.exe

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PushTheButton.Console
{
    internal class globalKeyboardHook
    {
        public List<Keys> HookedKeys = new List<Keys>();
        private IntPtr hhook = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_SYSKEYDOWN = 260;
        private const int WM_SYSKEYUP = 261;
        private globalKeyboardHook.keyboardHookProc khp;
        private KeyEventHandler _keyDown;
        private KeyEventHandler _keyUp;

        public event KeyEventHandler KeyDown
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                this._keyDown = this._keyDown + value;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                this._keyDown = this._keyDown - value;
            }
        }

        public event KeyEventHandler KeyUp
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                this._keyUp = this._keyUp + value;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                this._keyUp = this._keyUp - value;
            }
        }

        public globalKeyboardHook()
        {
            this.khp = new globalKeyboardHook.keyboardHookProc(this.hookProc);
            this.hook();
        }

        ~globalKeyboardHook()
        {
            this.unhook();
        }

        public void hook()
        {
            this.hhook = globalKeyboardHook.SetWindowsHookEx(13, this.khp, globalKeyboardHook.LoadLibrary("User32"), 0U);
        }

        public void unhook()
        {
            globalKeyboardHook.UnhookWindowsHookEx(this.hhook);
        }

        public int hookProc(int code, int wParam, ref globalKeyboardHook.keyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys keyData = (Keys)lParam.vkCode;
                if (this.HookedKeys.Contains(keyData))
                {
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    if ((wParam == 256 || wParam == 260) && _keyDown != null)
                        _keyDown((object)this, e);
                    else if ((wParam == 257 || wParam == 261) && _keyUp != null)
                        _keyUp((object)this, e);
                    if (e.Handled)
                        return 1;
                }
            }
            return globalKeyboardHook.CallNextHookEx(this.hhook, code, wParam, ref lParam);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, globalKeyboardHook.keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref globalKeyboardHook.keyboardHookStruct lParam);

        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string lpFileName);

        public delegate int keyboardHookProc(int code, int wParam, ref globalKeyboardHook.keyboardHookStruct lParam);

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
    }
}
