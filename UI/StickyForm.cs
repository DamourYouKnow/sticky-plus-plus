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

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private TopPanel topPanel;
        private StickyPanel stickyPanel;

        public StickyForm() {
            InitializeComponent();

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
    }


    public class StickyPanel : TableLayoutPanel {

    }


    public class TopPanel : TableLayoutPanel {
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

            // Span entire width.
            this.Height = 25;

            this.Dock = DockStyle.Top;

            this.BackColor = Color.Purple;

            this.MouseMove += this.stickyForm.DragHandler;
        }

        public NewStickyButton NewStickyButton { 
            get { return this.newStickyButton; }
        }
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
