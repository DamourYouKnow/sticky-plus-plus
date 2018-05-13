/*
 * Sticky Plus Plus
 * 
 * Saving the world one GalacticScale, asyncronous, non-blocking 
 * blockchain sticky note at a time.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            this.stickyPanel = new StickyPanel(this);

            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public TopPanel TopPanel {
            get { return this.topPanel; }
        }

        public StickyPanel StickyPanel {
            get { return this.stickyPanel; }
        }

        private void StickyForm_Load(object sender, EventArgs e) {
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.stickyPanel);
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
            this.stickyPanel.FixSize();
        }

        private void drawTopPanelRect() {
            if (this.topPanel == null) return;
            SolidBrush brush = new SolidBrush(Color.Red);
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

        private NewStickyButton newButton;
        private ColorButton colorButton;
        private SettingsButton settingsButton;
        private CloseStickyButton closeButton;
        private StickyForm sticky;

        public TopPanel(StickyForm stickyForm) {
            this.sticky = stickyForm;

            this.RowCount = 1;
            this.ColumnCount = 4;


            this.newButton = new NewStickyButton(this.sticky);
            this.SetCellPosition(this.newButton,
                                 new TableLayoutPanelCellPosition(0, 0));
            this.Controls.Add(this.newButton);

            this.colorButton = new ColorButton(this.sticky);
            this.SetCellPosition(this.colorButton,
                                 new TableLayoutPanelCellPosition(1, 0));
            this.Controls.Add(this.colorButton);

            this.settingsButton = new SettingsButton(this.sticky);
            this.SetCellPosition(this.settingsButton,
                                 new TableLayoutPanelCellPosition(2, 0));
            this.Controls.Add(this.settingsButton);

            this.closeButton = new CloseStickyButton(this.sticky);
            this.SetCellPosition(this.closeButton,
                                 new TableLayoutPanelCellPosition(3, 0));
            this.Controls.Add(this.closeButton);

            this.BackColor = Color.Purple;

            this.Height = 25;
            this.Width = this.sticky.Width - (2 * Offset);
            this.Location = new Point(Offset, Offset);

            this.MouseMove += this.sticky.DragHandler;
        }

        public int RealHeight {
            get { return this.Height + Offset; }
            set { this.Height = value - Offset; }
        }

        public int RealWidth {
            get { return this.Width + (2 * Offset); }
            set { this.Width = value - (2 * Offset); }
        }

        public NewStickyButton NewStickyButton {
            get { return this.newButton; }
        }

        public void FixSize() {
            this.Width = this.sticky.Width - (2 * Offset);
        }
    }


    public class StickyPanel : TableLayoutPanel {
        private const int offset = 3;

        private StickyForm sticky;
        private StickyTextBox textBox;

        public StickyPanel(StickyForm sticky) {
            this.sticky = sticky;
            this.RowCount = 1;
            this.ColumnCount = 1;

            this.textBox = new StickyTextBox(this);
            this.SetCellPosition(this.textBox,
                                 new TableLayoutPanelCellPosition(0, 0));


            this.Location = new Point(offset, this.sticky.TopPanel.RealHeight);
            this.Width = this.sticky.Width - (2 * offset);
            this.Height = this.sticky.Height - this.sticky.TopPanel.RealHeight - offset;

            this.BackColor = Color.LightSkyBlue;
        }

        public int RealHeight {
            get { return this.Height + offset; }
            set { this.Height = value - offset; }
        }

        public int RealWidth {
            get { return this.Width + (2 * offset); }
            set { this.Width = value - (2 * offset); }
        }

        public void FixSize() {
            this.Location = new Point(offset, this.sticky.TopPanel.RealHeight);
            this.Width = this.sticky.Width - (2 * offset);
            this.Height = this.sticky.Height - this.sticky.TopPanel.RealHeight - offset;
        }
    }


    public class ColorPanel : TableLayoutPanel {

    }

    public abstract class StickyButton : Button {
        protected StickyForm sticky;


        public StickyButton(StickyForm sticky) : base() {
            this.sticky = sticky;
            this.Click += new EventHandler(clickHandlerMediator);
        }


        protected abstract void handleClick();

        private void clickHandlerMediator(object sender, EventArgs e) {
            this.handleClick();
        }
    }


    public class SettingsButton : StickyButton {
        public SettingsButton(StickyForm sticky) : base(sticky) {
            this.Text = "SETTINGS";
        }

        protected override void handleClick() {
            throw new NotImplementedException();
        }
    }


    public class ColorButton : StickyButton {
        public ColorButton(StickyForm sticky) : base(sticky) {
            this.Text = "COLOR";
        }

        protected override void handleClick() {
            throw new NotImplementedException();
        }
    }


    public class NewStickyButton : StickyButton {
        public NewStickyButton(StickyForm sticky) : base(sticky) {
            this.Image = Image.FromFile("F:/Programming/sticky-plus-plus/Resources/new.png");
        }

        protected override void handleClick() {
            new StickyForm().Show();
        }

    }


    public class CloseStickyButton : StickyButton {
        public CloseStickyButton(StickyForm sticky) : base(sticky) {
            this.Image = Image.FromFile("F:/Programming/sticky-plus-plus/Resources/close.png");
        }

        protected override void handleClick() {
            this.sticky.Close();
        }
    }


    public class StickyTextBox : TextBox {
        private TextBox textBox;

        public StickyTextBox(StickyPanel panel) {
            this.textBox = new TextBox();

            panel.Controls.Add(this.textBox);
            this.textBox.Multiline = true;
            this.textBox.WordWrap = true;
            this.textBox.AcceptsReturn = true;
            this.textBox.AcceptsTab = true;
            this.textBox.TabIndex = 9;
            this.textBox.Dock = DockStyle.Fill;
        }
    }
}
