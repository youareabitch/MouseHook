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
        List<Config> configs = new List<Config>();
        Config currentConfig;
        int loadedConfigNo = 0;
        Point cursorData = CorsorExtension.GetCursorPosition();

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

            //init configs
            configs.Add(new Config());
            configs.Add(new Config());
            configs.Add(new Config());

            //init current config
            currentConfig = configs[0];

            //hard code test
            configs[0].Xconfig.Add(new ConfigDetail() { Offset = 0, Rate = 15 });
            configs[0].Xconfig.Add(new ConfigDetail() { Offset = 1, Rate = 15 });
            configs[0].Yconfig.Add(new ConfigDetail() { Offset = 2, Rate = 4 });
            configs[0].Yconfig.Add(new ConfigDetail() { Offset = 0, Rate = 5 });
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
                bool switchFlag = false;
                while (isLeftDown && isRightDown)
                {
                    cursorData = CorsorExtension.GetCursorPosition();
                    label1.InvokeIfRequired(() =>
                    {
                        label1.Text = $"x:{cursorData.X}";
                    });

                    cursorData = CorsorExtension.GetCursorPosition();
                    Win32.SetCursorPos(cursorData.X + currentConfig.Xconfig[Convert.ToInt32(switchFlag)].Offset, cursorData.Y);
                    Thread.Sleep(currentConfig.Xconfig[Convert.ToInt32(switchFlag)].Rate);
                    switchFlag = !switchFlag;
                }
            });
            xThread.Start();

            yThread = new Thread(() =>
            {
                var switchFlag = false;
                while (isLeftDown && isRightDown)
                {
                    label2.InvokeIfRequired(() =>
                    {
                        label2.Text = $"x:{cursorData.X} y:{cursorData.Y}";
                    });

                    cursorData = CorsorExtension.GetCursorPosition();
                    Win32.SetCursorPos(cursorData.X, cursorData.Y + currentConfig.Yconfig[Convert.ToInt32(switchFlag)].Offset);
                    Thread.Sleep(currentConfig.Yconfig[Convert.ToInt32(switchFlag)].Rate);
                    switchFlag = !switchFlag;
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
