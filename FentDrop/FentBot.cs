using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FentDrop
{
    public class FentBot
    {
        public Rectangle rec {  get; set; }
        public Image img {  get; set; }
        public bool esInmune { get; set; }

        public Rectangle hitBox
        {
            get
            {
                return new Rectangle(
                    (rec.X + rec.Width / 2) - 45,
                    (rec.Y + rec.Height / 2) - 45,
                    90,
                    90
                );
            }
        }

        public FentBot(int x, int y, Image imgOriginal)
        {
            rec = new Rectangle(x, y, 120, 120);
            img = new Bitmap(imgOriginal, rec.Size);
        }

        public FentBot() { }

        public void Dibujar(Graphics g)
        {
            g.DrawImage(img, rec);  
        }

        public void MoverDer(int x)
        {
            rec = new Rectangle(rec.X + x, rec.Y, 120, 120);
        }

        public void MoverIzq(int x)
        {
            rec = new Rectangle(rec.X - x, rec.Y, 120, 120);
        }

        public void CambiarPos(int x, int y)
        {
            rec = new Rectangle(x, y, 120, 120);
        }
    }
}
