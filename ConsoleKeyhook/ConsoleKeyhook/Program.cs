    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    namespace ConsoleKeyhook
    {
        class Hooky
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook,
                LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

            private static extern IntPtr GetModuleHandle(string lpModuleName);
            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100;
            private const int WM_KEYUP = 0x0101;
            private static LowLevelKeyboardProc _proc = HookCallback;
            private static IntPtr _hookID = IntPtr.Zero;
            private static bool SENDING_KEYS = false;

            private static string[] KeysToReplace = new string[26];
            private static string[] Replacements = new string[26];

            public static void Main()
            {
                for (int x = 0; x < 26; x++)
                {
                    //KeysToReplace[x] = ((char)x).ToString();
                    switch ((char)x+65)
                    {
                        case 'A': Replacements[x] = "Ayy"; break;
                        case 'B': Replacements[x] = "Bee"; break;
                        case 'C': Replacements[x] = "See"; break;
                        case 'D': Replacements[x] = "Dee"; break;
                        case 'E': Replacements[x] = "Eee"; break;
                        case 'F': Replacements[x] = "Eff"; break;
                        case 'G': Replacements[x] = "Gee"; break;
                        case 'H': Replacements[x] = "Aitch"; break;
                        case 'I': Replacements[x] = "Eye"; break;
                        case 'J': Replacements[x] = "Jay"; break;
                        case 'K': Replacements[x] = "Kay"; break;
                        case 'L': Replacements[x] = "El"; break;
                        case 'M': Replacements[x] = "Em"; break;
                        case 'N': Replacements[x] = "En"; break;
                        case 'O': Replacements[x] = "Oh"; break;
                        case 'P': Replacements[x] = "Pee"; break;
                        case 'Q': Replacements[x] = "Cue"; break;
                        case 'R': Replacements[x] = "Arr"; break;
                        case 'S': Replacements[x] = "Ess"; break;
                        case 'T': Replacements[x] = "Tea"; break;
                        case 'U': Replacements[x] = "Yew"; break;
                        case 'V': Replacements[x] = "Vee"; break;
                        case 'W': Replacements[x] = "Doubleyew"; break;
                        case 'X': Replacements[x] = "Ex"; break;
                        case 'Y': Replacements[x] = "Why"; break;
                        case 'Z': Replacements[x] = "Zee"; break;
                        //... and so on
                    }
                }
                _hookID = SetHook(_proc);
                Application.Run();
            }

            private static IntPtr SetHook(LowLevelKeyboardProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

            private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (!SENDING_KEYS)
                {
                    if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) //KeyDown
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        string theKey = ((Keys)vkCode).ToString();
                        if (theKey == "Escape")
                        {
                            UnhookWindowsHookEx(_hookID);
                            Environment.Exit(0);
                        }
                        return (IntPtr)1;
                    }
                    else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) //KeyUP
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        string theKey = ((Keys)vkCode).ToString();
                        if (theKey == "Escape")
                        {
                            UnhookWindowsHookEx(_hookID);
                            Environment.Exit(0);
                        }
                        string t;
                        if (theKey.Length == 1)
                        {
                            try
                            {
                                Console.WriteLine(theKey);
                                t = Replacements[((int)(theKey.ToCharArray()[0])) - 65];
                                Console.WriteLine(((int)(theKey.ToCharArray()[0])).ToString());
                                theKey = t;
                            }
                            catch { }
                        }
                        SENDING_KEYS = true;
                        SendKeys.Send(theKey);
                        SENDING_KEYS = false;
                        return (IntPtr)1;
                    }
                }
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }
    }