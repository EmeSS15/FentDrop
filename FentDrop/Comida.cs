using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FentDrop
{
    public class Comida
    {
        public Rectangle rec { get; set; }
        public Image img { get; set; }
        public bool esBuena { get; set; }
        public bool esFenta { get; set; }
        public bool UnoSi { get; set; }
        static Pen penF = new Pen(Color.HotPink, 4);

        public Rectangle adornoFenta
        {
            get
            {
                return new Rectangle(
                    (rec.X + rec.Width / 2) - 90,
                    (rec.Y + rec.Height / 2) - 90,
                    180, 
                    180
                );
            }
        }

        public Rectangle hitBox
        {
            get
            {
                return new Rectangle(
                    (rec.X + rec.Width / 2) - 35,
                    (rec.Y + rec.Height / 2) - 35,
                    70,
                    70
                );
            }
        }

        public Comida(int x, int y, Image imgOriginal) 
        {
            rec = new Rectangle(x, y, 110, 110);
            img = new Bitmap(imgOriginal, rec.Size);
        }

        public Comida()
        {

        }

        public void Dibujar(Graphics g)
        {
            g.DrawImage(img, rec);

            if(esFenta && UnoSi)
            {
                g.DrawEllipse(penF, adornoFenta);
            }
        }

        public void MoverAbajo(int y)
        {
            rec = new Rectangle(rec.X, rec.Y + y, 110, 110);
        }
    }
}
