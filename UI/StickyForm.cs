using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using StickyPlusPlus.Core;

namespace StickyPlusPlus {
    public partial class StickyForm : Form {
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, 
                                             int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private TopPanel topPanel;
        private StickyPanel stickyPanel;

        public StickyForm() {
            InitializeComponent();

            this.MinimumSize = new Size(200, 150);
            this.FormBorderStyle = FormBorderStyle.None;
            this.drawTopPanelRect();

            this.topPanel = new TopPanel(this);
            this.stickyPanel = new StickyPanel();

            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void StickyForm_Load(object sender, EventArgs e) {
            this.Controls.Add(this.topPanel);
        }

        public void DragHandler(object Sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                StickyForm.SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            this.drawTopPanelRect();
            this.topPanel.FixSize();
        }

        private void drawTopPanelRect() {
            if (this.topPanel == null) return;
            SolidBrush brush = new SolidBrush(this.topPanel.BackColor);
            Graphics g = this.CreateGraphics();
            g.FillRectangle(brush, 
                            new Rectangle(0, 0, 
                                          this.Width, 
                                          this.topPanel.Height + TopPanel.Offset));
            brush.Dispose();
            g.Dispose();
        }

        protected override void WndProc(ref Message m) {
            const int RESIZE_HANDLE_SIZE = 12;

            switch (m.Msg) {
                case 0x0084/*NCHITTEST*/ :
                    base.WndProc(ref m);

                    if ((int)m.Result == 0x01/*HTCLIENT*/) {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE) {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13/*HTTOPLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12/*HTTOP*/ ;
                            else
                                m.Result = (IntPtr)14/*HTTOPRIGHT*/ ;
                        }
                        else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE)) {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10/*HTLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)2/*HTCAPTION*/ ;
                            else
                                m.Result = (IntPtr)11/*HTRIGHT*/ ;
                        }
                        else {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16/*HTBOTTOMLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15/*HTBOTTOM*/ ;
                            else
                                m.Result = (IntPtr)17/*HTBOTTOMRIGHT*/ ;
                        }
                    }
                    return;
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }
    }


    public class TopPanel : TableLayoutPanel {
        public const int Offset = 3;

        private NewStickyButton newStickyButton;
        private ColorButton colorButton;
        private SettingsButton settingsButton;
        private StickyForm stickyForm;

        public TopPanel(StickyForm stickyForm) {
            this.stickyForm = stickyForm;

            this.RowCount = 1;
            this.ColumnCount = 3;


            this.newStickyButton = new NewStickyButton();
            this.SetCellPosition(this.newStickyButton,
                                 new TableLayoutPanelCellPosition(0, 0));
            this.Controls.Add(this.newStickyButton);

            this.colorButton = new ColorButton();
            this.SetCellPosition(this.colorButton,
                                 new TableLayoutPanelCellPosition(1, 0));
            this.Controls.Add(this.colorButton);

            this.settingsButton = new SettingsButton();
            this.SetCellPosition(this.settingsButton,
                                 new TableLayoutPanelCellPosition(2, 0));
            this.Controls.Add(this.settingsButton);

            this.BackColor = Color.Purple;

            this.Height = 25;
            this.Width = this.stickyForm.Width - (2 * Offset);
            this.Location = new Point(Offset, Offset);

            this.MouseMove += this.stickyForm.DragHandler;
        }

        public NewStickyButton NewStickyButton {
            get { return this.newStickyButton; }
        }

        public void FixSize() {
            this.Width = this.stickyForm.Width - (2 * Offset);
        }
    }


    public class StickyPanel : TableLayoutPanel {

    }


    public class ColorPanel : TableLayoutPanel {

    }


    public class SettingsButton : Button {

    }


    public class ColorButton : Button {

    }


    public class NewStickyButton : Button {
        public NewStickyButton() {
            this.Text = "NEW";
            this.Click += new EventHandler(this.handleClick);
        }

        private void handleClick(object sender, EventArgs e) {
            new StickyForm().Show();
        }
    
    }


    public class CloseButton : Button {

    }
}
