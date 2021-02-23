using System;
using System.Windows.Forms;

namespace OCR_v2
{
    public partial class WebForm : Form
    {
        public Form CropAreaForm { get; }
        public WebForm()
        {
            InitializeComponent();
            CropAreaForm = (CropAreaForm)Parent;
            Closing += WebForm_Closing;
            KeyPress += WebForm_KeyPress;
            webBrowser1.PreviewKeyDown += WebBrowser1_PreviewKeyDown;
        }

        private void WebForm_KeyPress(object sender, KeyPressEventArgs e) =>
            ((CropAreaForm) CropAreaForm).CropAreaForm_KeyPress(sender, e);

        private void WebForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        public WebForm(Form parentForm) : this() =>
            CropAreaForm = parentForm;

        private void WebBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) =>
            ((CropAreaForm)CropAreaForm).CropAreaForm_KeyPress(sender, new KeyPressEventArgs((char)e.KeyCode));

        public void SetBrowserUrl(string url) => webBrowser1.Url = new Uri(url);
    }
}
