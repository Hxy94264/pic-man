using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pac_Man
{
    class Pac
    {
        int[][] dir;
        int px, py;

        public  bool fight;//战斗标记
        public int x, y, d;

        public Pac(int _x,int _y,int _d)
        {
            dir = new int[4][];
            dir[0] = new int[] { -1, 0 };
            dir[1] = new int[] { 1, 0 };
            dir[2] = new int[] { 0, -1 };
            dir[3] = new int[] { 0, 1 };
            x = _x; y = _y; d = _d;
            px = x - dir[d][0]; py = y - dir[d][1];
            fight = false;
        }

        public void Next(int[][] _map)
        {
            int tx, ty;
            if (x == 7 && y == 1 && d == 2)
                {
                    tx = x; ty = 16;
                }
                else if (x == 7 && y == 16 && d == 3)
                {
                    tx = x; ty = 1;
                }
                else { tx = x + dir[d][0]; ty = y + dir[d][1]; }
            if (_map[tx][ty] == 1) {
                px = x; py = y;
                x = tx; y = ty;
            }
        }

        public void Paint(Graphics gr, int[][] havePoint)
        {
            int tx, ty;
            gr.FillRectangle(Brushes.Black, 20 + (py - 1) * 40 + 1, 10 + (px - 1) * 40 + 1, 38, 38);
            if (havePoint[px][py] == 1)
                Paint_Bean(px, py, gr);

            tx = 40 * y - 20; ty = 40 * x - 30;

            if (!fight) gr.FillEllipse(Brushes.Yellow, tx + 2, ty + 2, 36, 36);
            else gr.FillEllipse(Brushes.Red, tx + 2, ty + 2, 36, 36);
            switch (d)
            {
                case 0: { gr.FillRectangle(Brushes.Black, tx + 7, ty + 17, 3, 3); break; }
                case 1: { gr.FillRectangle(Brushes.Black, tx + 30, ty + 20, 3, 3); break; }
                case 2: { gr.FillRectangle(Brushes.Black, tx + 17, ty + 7, 3, 3); break; }
                case 3: { gr.FillRectangle(Brushes.Black, tx + 20, ty + 7, 3, 3); break; }
            }
        }

        public void Open(Graphics gr)
        {
            int tx = 40 * y - 20, ty = 40 * x - 30;
            switch (d)
            {
                case 0: { gr.FillPie(Brushes.Black, tx + 1, ty + 1, 36, 36, 240, 60); break; }
                case 1: { gr.FillPie(Brushes.Black, tx + 2, ty + 2, 36, 36, 60, 60); break; }
                case 2: { gr.FillPie(Brushes.Black, tx + 1, ty + 1, 36, 36, 150, 60); break; }
                case 3: { gr.FillPie(Brushes.Black, tx + 2, ty + 2, 36, 36, -30, 60); break; }
            }
        }

        private void Paint_Bean(int x, int y, Graphics gr)
        {
            int tx = 40 * y - 20, ty = 40 * x - 30;
            gr.FillEllipse(Brushes.Yellow, tx + 15, ty + 15, 10, 10);
            gr.FillEllipse(Brushes.Black, tx + 17, ty + 18, 3, 3);
            gr.FillEllipse(Brushes.Black, tx + 20, ty + 18, 3, 3);
        }
    }
}
