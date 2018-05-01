using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using StickyPlusPlus.Core;

namespace StickyPlusPlus {
    public partial class StickyForm : Form {
        private Button newStickyButton;
        private Button changeColorButton;
        private StickyPanel stickyPanel;
        private ColorPanel colorPanel;

        public StickyForm() {
            InitializeComponent();
        }

        private void StickyForm_Load(object sender, EventArgs e) {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }
    }


    public class StickyPanel : TableLayoutPanel {

    }


    public class ColorPanel : TableLayoutPanel {

    }
}
