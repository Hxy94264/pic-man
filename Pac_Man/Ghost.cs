using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pac_Man
{
    class Ghost
    {
        int[][] dir;
        int px, py;
        public int x, y, attack;//进攻标志
        public bool Alive;

        public Ghost(int _x, int _y)
        {
            dir = new int[4][];
            dir[0] = new int[] { -1, 0 };
            dir[1] = new int[] { 1, 0 };
            dir[2] = new int[] { 0, -1 };
            dir[3] = new int[] { 0, 1 };
            px = py = -1;
            x = _x; y = _y;
            attack = 1;
            Alive = true;
        }

        public void Next(int[][] _map, int[][] dis, int tarx, int tary, int _r, int rx, int ry, int tp, int tg)
        {
            if (!Alive) return;
            int tar = (tarx - 1) * 16 + tary;
            if (_r == 1) {
                int posr = (rx - 1) * 16 + ry;
                int posg = (x - 1) * 16 + y;
                int p_r = dis[tar][posr] * tp;
                int g_r = dis[posg][posr] * tg;
                if (dis[tar][posr] <= 2)
                    if (g_r > p_r)
                        attack = -1;
                    else
                        attack = 1;
                else
                    attack = 1;
            }
            int Min = Int32.MaxValue, pos=0;
            for (int i = 0; i < 4; i++) {
                int tx = x + dir[i][0], ty = y + dir[i][1];
                if (_map[tx][ty] == 0) continue;
                if (dis[(tx - 1) * 16 + ty][tar] * attack < Min)
                    Min = dis[pos=(tx - 1) * 16 + ty][tar] * attack;
                else if (dis[(tx - 1) * 16 + ty][tar] * attack == Min) {
                    int ran = (new Random()).Next(100);
                    if (ran < 50) pos = (tx - 1) * 16 + ty;
                }
            }
            //时空之门 97 112
            if (x == 7 && y == 1)
                if (dis[112][tar] * attack < Min) pos = 112;
                else if (dis[112][tar] * attack == Min) {
                    int ran = (new Random()).Next(100);
                    if(ran<50) pos = 112;
                }
            if (x == 7 && y == 16)
                if (dis[97][tar] * attack < Min) pos = 97;
                else if (dis[112][tar] * attack == Min)
                {
                    int ran = (new Random()).Next(100);
                    if (ran < 50)  pos = 97;
                }
            px = x; py = y;
            x = (pos - 1) / 16+1;
            y = pos - 16 * (x-1);
        }

        public void Pause() {
            x = px; y = py;
        }

        public void Paint(Graphics gr)
        {
            if (!Alive) return;
            int tx, ty;
            tx = y * 40 - 20; ty = x * 40 - 30;

            gr.FillEllipse(Brushes.White, tx + 2, ty + 2, 36, 36);
            gr.FillRectangle(Brushes.Black, tx + 10, ty + 10, 3, 3);
            gr.FillRectangle(Brushes.Black, tx + 27, ty + 10, 3, 3);
            gr.FillEllipse(Brushes.Black, tx + 16, ty + 17, 8, 13);
        }

        public void Cover(Graphics gr, int[][] havePoint)
        {
            if (!Alive) return;
            gr.FillRectangle(Brushes.Black, 20 + (py - 1) * 40 + 1, 10 + (px - 1) * 40 + 1, 38, 38);
            if (!(px == -1 && py == -1) && havePoint[px][py] == 1)
                Paint_Bean(px, py, gr);
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
