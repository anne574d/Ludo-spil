using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Ludo
{
    public class Die : PictureBox
    {
        public Die(GUI parent)
        {
            this.parent = parent;
            parent.Controls.Add(this);

            // picturebox fields
            Size = new Size(75, 75);
            SizeMode = PictureBoxSizeMode.StretchImage;
            Click += OnClick;

            // die fields
            rng = new Random();
            enabled = true;

            // load images
            side1 = (Image)Properties.Resources.ResourceManager.GetObject("die1");
            side2 = (Image)Properties.Resources.ResourceManager.GetObject("die2");
            side3 = (Image)Properties.Resources.ResourceManager.GetObject("die3");
            side4 = (Image)Properties.Resources.ResourceManager.GetObject("die4");
            side5 = (Image)Properties.Resources.ResourceManager.GetObject("die5");
            side6 = (Image)Properties.Resources.ResourceManager.GetObject("die6");

            // give initial value + location
            Value = rng.Next(1, 7);
            changeImage();
            moveAround();
        }

        public void OnClick(object sender, EventArgs e)
        {
            if (enabled)
            {
                Value = rng.Next(1, 7);
                changeImage();
                moveAround();
                Disable();

                parent.DieRolled();
            }
        }

        private void changeImage()
        {
            switch (Value)
            {
                case 1: Image = side1; break;
                case 2: Image = side2; break;
                case 3: Image = side3; break;
                case 4: Image = side4; break;
                case 5: Image = side5; break;
                case 6: Image = side6; break;
            }
        }
        private void moveAround()
        {
            int x, y;
            do
            {
                x = rng.Next(30, 700);
            }
            while (x > 250 && x < 500); // avoid die overlapping fields

            do
            {
                y = rng.Next(30, 700);
            }
            while (y > 250 && y < 500); // avoid die overlapping fields

            Location = new Point(x,y);
        }

        public void Disable()
        {
            enabled = false;
        }
        public void Enable()
        {
            enabled = true;
        }

        public int Value { get; private set; }

        Image side1, side2, side3, side4, side5, side6;
        Random rng;
        bool enabled;
        GUI parent;
    }
}
