using GlobalLowLevelHooks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GlobalLowLevelHooks.MouseHook;

namespace MouseHookTest
{
    public partial class Form1 : Form
    {
        MouseHook mouseHook;
        bool isLeftDown = false;
        bool isRightDown = false;
        Thread xThread;
        Thread yThread;
        int x = 0;
        int y = 0;

        public Form1()
        {
            InitializeComponent();

            // Create the Mouse Hook
            mouseHook = new MouseHook();
            // Capture the events
            mouseHook.LeftButtonDown += new MouseHookCallback(LeftMouseDown);
            mouseHook.LeftButtonUp += new MouseHookCallback(LeftMouseUp);
            mouseHook.RightButtonDown += new MouseHookCallback(RightMouseDown);
            mouseHook.RightButtonUp += new MouseHookCallback(RightMouseUp);
            //Installing the Mouse Hooks
            mouseHook.Install();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LeftMouseDown(MSLLHOOKSTRUCT mouseStruct)
        {
            isLeftDown = true;
            ThreadClear();
            ThreadInit();
        }

        private void LeftMouseUp(MSLLHOOKSTRUCT mouseStruct)
        {
            isLeftDown = false;
            ThreadClear();
        }

        private void RightMouseDown(MSLLHOOKSTRUCT mouseStruct)
        {
            isRightDown = true;
            ThreadClear();
            ThreadInit();
        }

        private void RightMouseUp(MSLLHOOKSTRUCT mouseStruct)
        {
            isRightDown = false;
            ThreadClear();
        }

        private void ThreadInit()
        {
            xThread = new Thread(() =>
            {
                x = 0;
                int offset = 1;
                while (isLeftDown && isRightDown)
                {
                    label1.InvokeIfRequired(() =>
                    {
                        var cursorData = CorsorExtension.GetCursorPosition();

                        //label1.Text = $"x:{x++}";
                        label1.Text = $"x:{cursorData.X} count:{x++}";
                        Win32.SetCursorPos(cursorData.X + offset, cursorData.Y);
                    });

                    offset = offset * -1;
                    Thread.Sleep(20);
                }
            });
            xThread.Start();

            yThread = new Thread(() =>
            {
                y = 0;
                while (isLeftDown && isRightDown)
                {
                    label2.InvokeIfRequired(() =>
                    {
                        var cursorData = CorsorExtension.GetCursorPosition();

                        //label2.Text = $"y:{y++}";
                        label2.Text = $"y:{cursorData.Y} count:{y++}";
                        Win32.SetCursorPos(cursorData.X, cursorData.Y + 2);
                    });
                    Thread.Sleep(20);
                }
            });
            yThread.Start();
        }

        private void ThreadClear()
        {
            if (xThread != null)
            {
                xThread.Abort();
            }
            xThread = null;

            if (yThread != null)
            {
                yThread.Abort();
            }
            yThread = null;
        }
    }

    //擴充方法
    public static class Extension
    {
        //非同步委派更新UI
        public static void InvokeIfRequired(
            this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)//在非當前執行緒內 使用委派
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
