using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace StickyPlusPlus.Core {
    public class Sticky {
        public static Color LastColor = Color.White;

        private string name;
        private Color color;

        public Sticky() {
            this.color = LastColor;
        }

        public string Name {
            get { return this.name; }
            set { this.name = value; }
        }
    }


    public class StickyContent {
        private List<StickyNode> nodes;

        public void AddNode(int pos) {
            this.nodes.Insert(pos, new StickyNode());
        }

        public void RemoveNode(int pos) {
            this.nodes.RemoveAt(pos);
        }
    }


    public class StickyNode {
        private string next;

        public void AddComponent<Component>(StickyNodeComponent component)
                where Component : StickyNodeComponent {
            throw new NotImplementedException();
        }
    }


    public abstract class StickyNodeComponent {

    }


    public class DateTimeComponent : StickyNodeComponent {
        private DateTime time;

        public DateTimeComponent(DateTime time) {
            this.time = time;
        }

        public DateTime Time {
            get { return this.time; }
        }
    }
}
