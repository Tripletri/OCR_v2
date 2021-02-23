using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OCR_v2
{
    public partial class TrackBarUpDown : UserControl
    {
        public int Value => trackBar1.Value;
        public event Action<object, EventArgs> OnValueChanged;
        public event Action<object, EventArgs> OnDoneButtonClick;
        public event Action<object, EventArgs> OnClose;

        private Color backgroundColor = Color.FromArgb(40, 40, 40);
        private Color controlColor = Color.FromArgb(255, 255, 255);

        public TrackBarUpDown()
        {
            InitializeComponent();
            foreach (Control control in Controls)
                control.KeyPress += (sender, args) => RedirectKeyPressing(args);
            pictureBox1.MouseEnter += (sender, args) => pictureBox1.BackColor = Color.Red;
            pictureBox1.MouseLeave += (sender, args) => pictureBox1.BackColor = Color.Transparent;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int) numericUpDown1.Value;
            OnValueChanged?.Invoke(sender, e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
            OnValueChanged?.Invoke(sender, e);
        }

        private void RedirectKeyPressing(KeyPressEventArgs e) => ((CropAreaForm) Parent).CropAreaForm_KeyPress(null, e);
        private void button1_Click(object sender, EventArgs e) => OnDoneButtonClick?.Invoke(sender, e);
        private void pictureBox1_Click(object sender, EventArgs e) => OnClose?.Invoke(sender, e);



        private void TrackBarUpDown_Load(object sender, EventArgs e)
        {
            BackColor = Color.Transparent;

            trackBar1.BackColor = backgroundColor;

            button1.BackColor = controlColor;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderColor = controlColor;
            button1.FlatAppearance.BorderSize = 0;

            numericUpDown1.BorderStyle = BorderStyle.None;
            numericUpDown1.BackColor = controlColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            const int penWidth = 2;
            const int radius = 30;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(penWidth / 2, penWidth / 2, Width - (int) (penWidth * 1.5),
                Height - (int) (penWidth * 1.5));
            var gp = GetRoundPath(rect, radius);
            Region = new Region(GetRoundPath(new Rectangle(0, 0, Width, Height), radius));
            e.Graphics.DrawPath(new Pen(backgroundColor, penWidth), gp);
            e.Graphics.FillPath(new SolidBrush(backgroundColor), gp);
        }

        private GraphicsPath GetRoundPath(RectangleF rect, int radius)
        {
            var r2 = radius / 2f;
            var graphPath = new GraphicsPath();

            graphPath.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            graphPath.AddLine(rect.X + r2, rect.Y, rect.Width - r2, rect.Y);
            graphPath.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            graphPath.AddLine(rect.X + rect.Width, rect.Y + r2, rect.X + rect.Width, rect.Height - r2);
            graphPath.AddArc(rect.X + rect.Width - radius,
                rect.Y + rect.Height - radius, radius, radius, 0, 90);
            graphPath.AddLine(rect.Width - r2, rect.Bottom, rect.X + r2, rect.Bottom);
            graphPath.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            graphPath.AddLine(rect.X, rect.Height - r2, rect.X, rect.Y + r2);

            graphPath.CloseFigure();
            return graphPath;
        }

        
    }
}