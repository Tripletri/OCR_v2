using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OCR_v2
{
    public partial class StartForm : Form
    {
        //private Input input;
        private CropAreaForm cropAreaForm;
        private NotifyIcon notifyIcon;
        public StartForm()
        {
            StartPosition = FormStartPosition.CenterScreen;

            ApplicationHandler.AddKeyBind(new Input((int)KeysModifiers.ALT + (int)KeysModifiers.CTRL, Keys.S, this));

            cropAreaForm = new CropAreaForm();

            notifyIcon = new NotifyIcon
            {
                Visible = false,
                Icon = Properties.Resources.Icon,
                ContextMenuStrip = new ContextMenuStrip()
            };
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (sender, args) => ApplicationHandler.Exit(0));
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            Resize += StartForm_Resize;
            Closing += StartForm_Closing;

            InitializeComponent();
            checkBox1.Checked = true;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Show();
                WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
            }
            else if ((e.Button & MouseButtons.Right) != 0)
            {
                notifyIcon.ContextMenuStrip.Show();
            }
        }

        private void StartForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!checkBox1.Checked) return;
            HideToTray();
            e.Cancel = true;
        }

        private void StartForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                HideToTray();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && !cropAreaForm.Visible)
            {
                cropAreaForm.PreShow();
                Debug.WriteLine("CatchedBtn");
            }
            base.WndProc(ref m);
        }

        public void HideToTray()
        {
            Hide();
            notifyIcon.Visible = true;
        }
    }
}
