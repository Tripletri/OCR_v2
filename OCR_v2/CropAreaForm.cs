using System;
using System.Drawing;
using System.Windows.Forms;
using Tesseract;

namespace OCR_v2
{
    public partial class CropAreaForm : Form
    {
        private readonly Timer mainTimer = new Timer();
        private Bitmap background;
        private Point startLoc;
        private Point endLoc;
        private bool isMouseDown = false;
        private Rectangle cropArea;
        private TrackBarUpDown trackBarUpDown = new TrackBarUpDown { Visible = false };
        private Bitmap original;
        private TesseractEngine ocr;
        private bool need2Clear = false;
        private WebForm webForm;
        private readonly Uri baseTranslatorUri = new Uri(@"https://translate.google.com/?sl=en&tl=ru&text=");
        private const string tessdataPath = "tessdata";

        private const int cropAreaOutlineWidth = 3;
        private const int frameWidth = 3;
#if DEBUG
        private int frames;
#endif

        public CropAreaForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            KeyPress += CropAreaForm_KeyPress;
            MouseMove += CropAreaForm_MouseMove;

            MouseDown += CropAreaForm_MouseDown;
            MouseUp += CropAreaForm_MouseUp;

            mainTimer.Tick += MainTimer_Tick;
            mainTimer.Interval = 4;
            mainTimer.Start();

            trackBarUpDown.OnValueChanged += TrackBarUpDown_OnValueChanged;
            trackBarUpDown.OnDoneButtonClick += TrackBarUpDownOnDoneButtonClick;
            trackBarUpDown.OnClose += (o, args) => HideCropArea();
            trackBarUpDown.Cursor = Cursors.Default;

            Controls.Add(trackBarUpDown);

            try
            {
                ocr = new TesseractEngine(tessdataPath, "eng");
            }
            catch
            {
                MessageBox.Show("Failed to initialise tesseract engine.", "Error((",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ApplicationHandler.Exit(2);
            }

            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;
            webForm = new WebForm(this) { TopMost = true, Height = height / 2, Width = width / 2 };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            Cursor = Cursors.Cross;
        }

        private void TrackBarUpDownOnDoneButtonClick(object arg1, EventArgs arg2)
        {
            var tmpBmp = new Bitmap(cropArea.Width, cropArea.Height);
            var g = Graphics.FromImage(tmpBmp);
            g.DrawImage(background, new Rectangle(0, 0, tmpBmp.Width, tmpBmp.Height), cropArea, GraphicsUnit.Pixel);

            Cursor.Current = Cursors.WaitCursor;
            var processed = ocr.Process(tmpBmp);
            var text = processed.GetText();
            processed.Dispose();

            webForm.SetBrowserUrl(baseTranslatorUri + text);
            webForm.Show();
            webForm.WindowState = FormWindowState.Normal;
            Cursor.Current = Cursors.Default;
        }

        private void TrackBarUpDown_OnValueChanged(object arg1, EventArgs arg2)
        {
            ProcessCropArea();
        }

        private void CropAreaForm_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            var tbLoc = new Point(cropArea.Location.X + cropArea.Width / 2 - trackBarUpDown.Width / 2,
                cropArea.Y - trackBarUpDown.Height - (int)Math.Floor(cropAreaOutlineWidth / 2d));
            trackBarUpDown.Location = tbLoc;
            trackBarUpDown.Visible = true;
            Cursor.Current = Cursors.Default;
            ProcessCropArea();
        }

        public void ProcessCropArea()
        {
            var tmpBmp = new Bitmap(original);
            var imgProcs = new ImageProcessor(tmpBmp);
            imgProcs.Threshold((byte)trackBarUpDown.Value, cropArea);
            imgProcs.UpdateBitmap();
            background = tmpBmp;
            Invalidate();
        }

        private void CropAreaForm_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            startLoc = e.Location;
            endLoc = startLoc;
            PrepareToRedrawArea();
        }

        private void PrepareToRedrawArea()
        {
            trackBarUpDown.Visible = false;
            need2Clear = true;
            background?.Dispose();
            background = new Bitmap(original);
        }

        private void CropAreaForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown) return;
            endLoc = e.Location;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (!isMouseDown) return;
            Invalidate();
        }

        public void CropAreaForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Escape)
                HideCropArea();
        }

        public void HideCropArea()
        {
            Visible = false;
        }

        public void PreShow()
        {
            var sc = new Screenshoter();
            original?.Dispose();
            original = sc.MakeScreenshot();
            PrepareToRedrawArea();
            ShowOnTop();
        }

        public void ShowOnTop()
        {
            TopMost = true;
            Show();
#if DEBUG
            TopMost = false;
#endif
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(background, Point.Empty);

            if (!need2Clear)
            {
                var x = endLoc.X < startLoc.X ? endLoc.X : startLoc.X;
                var y = endLoc.Y < startLoc.Y ? endLoc.Y : startLoc.Y;
                var width = Math.Abs(endLoc.X - startLoc.X);
                var height = Math.Abs(endLoc.Y - startLoc.Y);
                cropArea = new Rectangle(x, y, width, height);
                var outlineBrush = new SolidBrush(Color.FromArgb(120, Color.DeepSkyBlue));
                var innerBrush = new SolidBrush(Color.FromArgb(255, Color.DeepSkyBlue));
                e.Graphics.DrawRectangle(new Pen(outlineBrush, cropAreaOutlineWidth),
                    cropArea);
                e.Graphics.DrawRectangle(new Pen(innerBrush, 1),
                    cropArea);
            }

#if DEBUG
            e.Graphics.DrawString(frames.ToString(), new Font(FontFamily.GenericMonospace, 25f), Brushes.Green,
                PointF.Empty);
            frames++;
#endif
            e.Graphics.DrawRectangle(new Pen(Color.Red, frameWidth),
                new Rectangle(frameWidth / 2, frameWidth / 2, background.Width - frameWidth,
                    background.Height - frameWidth));
            need2Clear = false;
        }
    }
}