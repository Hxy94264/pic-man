using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
/*
 * 速度控制
 * 鬼追捕算法
 * */
namespace Pac_Man
{
    /*map
         1111111111000001
         1010101001111111
         1110100001010001
         1010111101010111
         1010000101010100
         1011111111010101
         1110000001111101
         0011111111000001
         1110100001111111
         1110111111010111
         1000001010010100
         1111111011111111
     */

    public partial class Form1 : Form
    {
        int[][] dir, _map, havePoint, dis;
        int Score, _time, kill, Over;
        int _count,g_count;
        int tv;
        int rx, ry, _r;
        bool _begin;
        Graphics gr;
        CircleQueue cq;
        Pac p;
        Ghost g1, g2, g3;
        public Form1()
        {
            InitializeComponent();
            gr = panel1.CreateGraphics();
            panel1.BackColor = Color.Black;
            //地图
            _map = new int[20][];
            //地图中每个点是否有豆
            havePoint = new int[20][];
            dis = new int[200][];
            for (int i = 0; i < 20; i++)
            {
                _map[i] = new int[20];
                havePoint[i] = new int[20];
            }
            for (int i = 0; i < 200; i++) dis[i] = new int[200];
            StreamReader s = File.OpenText("in.txt");
            for (int i = 1; i <= 12; i++)
            {
                string read = s.ReadLine();
                for (int j = 1; j <= 16; j++) _map[i][j] = read[j - 1] - '0';
            }
            s.Close();
            dir = new int[4][];
            dir[0] = new int[] { -1, 0 };
            dir[1] = new int[] { 1, 0 };
            dir[2] = new int[] { 0, -1 };
            dir[3] = new int[] { 0, 1 };
            //两个反杀豆出现的最小时间间隔
            timer3.Interval = 10000;
            cq = new CircleQueue();
            //FLoyd
            for (int i = 1; i <= 192; i++)
                for (int j = 1; j <= 192; j++) dis[i][j] = -1;
            for (int i = 1; i <= 12; i++)
                for (int j = 1; j <= 16; j++)
                    if (_map[i][j] == 1)
                    {
                        int pos = (i - 1) * 16 + j;
                        dis[pos][pos] = 0;
                        for (int k = 0; k < 4; k++)
                        {
                            int tx = i + dir[k][0], ty = j + dir[k][1];
                            if (_map[tx][ty] == 1)
                            {
                                int tpos = (tx - 1) * 16 + ty;
                                dis[pos][tpos] = 1;
                                dis[tpos][pos] = 1;
                            }
                        }
                    }
            dis[97][112] = dis[112][97] = 1;//两个特殊点
            for (int k = 1; k <= 192; k++)
                for (int i = 1; i <= 192; i++)
                    for (int j = 1; j <= 192; j++) if (dis[i][k] != -1 && dis[k][j] != -1)
                        {
                            if (dis[i][j] == -1) dis[i][j] = dis[i][k] + dis[k][j];
                            else dis[i][j] = Math.Min(dis[i][j], dis[i][k] + dis[k][j]);
                        }
            kill = 0;
            _begin = false;
        }

        private void MyPaint()
        {
            gr.FillRectangle(Brushes.Black, 0, 0, 680, 500);
            //gr.FillEllipse(Brushes.Red, 270, 60, 20, 20);
            //绘制地图边界(12,16)
            Pen p = new Pen(Color.Blue, 2);
            int w = panel1.Width, h = panel1.Height;
            Point[] BoundaryUp ={
                           new Point(0,h/2),
                           new Point(20,h/2),
                           new Point(20,10),
                           new Point(w-20,10),
                           new Point(w-20,h/2),
                           new Point(w,h/2)
                       };
            Point[] BoundaryDown ={
                           new Point(0,h/2+40),
                           new Point(20,h/2+40),
                           new Point(20,h-10),
                           new Point(w-20,h-10),
                           new Point(w-20,h/2+40),
                           new Point(w,h/2+40)
                       };
            gr.DrawLines(p, BoundaryUp);
            gr.DrawLines(p, BoundaryDown);

            gr.DrawRectangle(p, 60, 50, 40, 40);
            gr.DrawRectangle(p, 60, 130, 40, 120);


            Pen Cover = new Pen(Color.Black, 2);
            gr.DrawRectangle(p, 140, 50, 40, 160);
            gr.DrawRectangle(p, 180, 170, 120, 40);
            gr.DrawLine(Cover, 180, 170, 180, 209);

            //gr.DrawRectangle(p, 220, 50, 160, 80);
            Point[] Polygon1 ={
                                new Point(220,50),
                                new Point(260,50),
                                new Point(260,90),
                                new Point(300,90),
                                new Point(300,50),
                                new Point(380,50),//up-right
                                new Point(380,210),
                                new Point(340,210),
                                new Point(340,130),
                                new Point(220,130),
                            };
            gr.DrawPolygon(p, Polygon1);

            gr.DrawRectangle(p, 140, 250, 240, 40);//380,290
            gr.DrawRectangle(p, 420, 90, 40, 160);
            gr.DrawRectangle(p, 420, 10, 200, 40);

            Point[] Polygon2 ={
                                new Point(500,90),
                                new Point(620,90),
                                new Point(620,130),
                                new Point(540,130),
                                new Point(540,250),
                                new Point(500,250),
                            };
            gr.DrawPolygon(p, Polygon2);
            Point[] Polygon3 ={
                                new Point(580,170),
                                new Point(660,170),
                                new Point(660,210),
                                new Point(620,210),
                                new Point(620,330),
                                new Point(580,330),
                                new Point(420,330),
                                new Point(420,290),
                                new Point(580,290),
                            };
            gr.DrawPolygon(p, Polygon3);
            gr.DrawRectangle(p, 20, 290, 80, 40);
            Point[] Polygon4 ={
                                new Point(140,330),
                                new Point(180,330),
                                new Point(180,410),
                                new Point(260,410),//
                                new Point(260,450),
                                new Point(60,450),
                                new Point(60,410),
                                new Point(140,410),
                            };
            gr.DrawPolygon(p, Polygon4);
            gr.DrawRectangle(p, 220, 330, 160, 40);
            gr.DrawRectangle(p, 300, 410, 40, 80);

            Point[] Polygon5 ={
                                new Point(380,410),
                                new Point(420,410),
                                new Point(420,370),
                                new Point(460,370),
                                new Point(460,450),
                                new Point(380,450)
                            };
            gr.DrawPolygon(p, Polygon5);
            gr.DrawRectangle(p, 500, 370, 40, 80);
            gr.DrawRectangle(p, 580, 410, 80, 40);
            //绘制豆子
            Pen _point = new Pen(Color.White, 1);
            for (int i = 1; i <= 12; i++)
                for (int j = 1; j <= 16; j++) if (_map[i][j] == 1)
                        Paint_Bean(i, j);
            //绘制箭头
            _point.Width = 3;
            gr.DrawLine(_point, 20, 250, 10, 270);
            gr.DrawLine(_point, 10, 270, 20, 290);
            gr.DrawLine(_point, 660, 250, 670, 270);
            gr.DrawLine(_point, 670, 270, 660, 290);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            MyPaint();
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyPaint();
            p = new Pac(12, 9, 0);
            g1 = new Ghost(1, 1);
            g2 = new Ghost(2, 7);
            g3 = new Ghost(1, 16);
            cq.clear();
            Score = 0; _time = 0;
            Over = 0;
            _count = 0;
            g_count = 0;
            _begin = true;
            for (int i = 1; i <= 12; i++)
                for (int j = 1; j <= 16; j++) havePoint[i][j] = _map[i][j];
            //速度初始化
            timer1.Interval = 510;
            timer2.Interval = 510;
            timer1.Start();
            timer2.Start();
            timer3.Start();
            this.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //吃豆人和豆子计时器
            //战斗时间不吃豆子
            if (!p.fight&&havePoint[p.x][p.y] == 1) {
                Score += 5;
                _count++;
                havePoint[p.x][p.y] = 0;
                cq.push(p.x, p.y, _time);
            }
            //每吃够20颗豆子，速度增加
            if (!p.fight&&_count % 20 == 0) timer1.Interval = Math.Max(200, timer1.Interval-10);

            p.Next(_map);

            //反杀豆
            if (_r == 1)
            {
                if (p.x == rx && p.y == ry)
                {
                    _r = 0;
                    p.fight = true;
                    g1.attack = -1;
                    g2.attack = -1;
                    g3.attack = -1;
                    //反杀时间
                    kill = 22;
                    //速度提升
                    tv = timer1.Interval;
                    timer1.Interval = Math.Min(timer1.Interval, timer2.Interval*3/4);
                }
            }
            if (p.fight && kill == 0) {
                timer1.Interval = tv;
                p.fight = false;
                g1.attack = 1;
                g2.attack = 1;
                g3.attack = 1;
            }

            p.Paint(gr, havePoint);
            if(_time%2==1) p.Open(gr);
            //普通豆子恢复
            while (!cq.isempty()&&_time - 60 >= cq.getT()) {
                int tx = cq.getX(), ty = cq.getY();
                cq.pop();
                //如果当前位置有红豆，则黄豆延时
                if (_r == 1 && tx == rx && ty == ry) {
                    cq.push(tx, ty, _time);
                    continue;
                }
                havePoint[cq.getX()][cq.getY()] = 1;
                Paint_Bean(cq.getX(), cq.getY());
            }
            label1.Text = "分数： "+Score.ToString();
            _time++;
            if (kill > 0) kill--;
            GameOver();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            g_count++;
            if (!p.fight&&g_count % 12 == 0)
                timer2.Interval = Math.Max(200, timer2.Interval - 10);
            g1.Next(_map, dis, p.x, p.y, _r, rx, ry, timer1.Interval, timer2.Interval);
            g2.Next(_map, dis, p.x, p.y, _r, rx, ry, timer1.Interval, timer2.Interval);
            //防止鬼重合
            if (g2.x == g1.x && g2.y == g1.y) g2.Pause();
            g3.Next(_map, dis, p.x, p.y, _r, rx, ry, timer1.Interval, timer2.Interval);
            if ((g3.x == g1.x && g3.y == g1.y) || (g2.x == g3.x && g2.y == g3.y)) g3.Pause();

            g1.Cover(gr, havePoint);
            g2.Cover(gr, havePoint);
            g3.Cover(gr, havePoint); 
            g1.Paint(gr);
            g2.Paint(gr);
            g3.Paint(gr);
            GameOver();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (kill > 0) return;
            if (_r == 1) {
                gr.FillRectangle(Brushes.Black, 20 + (ry - 1) * 40 + 1, 10 + (rx - 1) * 40 + 1, 38, 38);
                if (havePoint[rx][ry] == 1)
                    Paint_Bean(rx, ry);
            }
            int r = (new Random()).Next(3);
            switch (r)
            {
                case 0:
                    rx = 2; ry = 7;
                    //(270, 60, 20, 20);
                    break;
                case 1:
                    rx = 1; ry = 16;
                    //(630, 20, 20, 20);
                    break;
                case 2:
                    rx = 12; ry = 16;
                    //(630, 460, 20, 20);
                    break;
            }
            Paint_ReaBean(rx, ry);
            _r = 1;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_begin) return;
            //尝试改变方向
            int pred = p.d;
            switch (e.KeyCode)
            {
                case Keys.Up: p.d = 0; break;
                case Keys.Down: p.d = 1; break;
                case Keys.Left: p.d = 2; break;
                case Keys.Right: p.d = 3; break;
            }
            //若该方向存在障碍物，则恢复原方向
            int tx = p.x + dir[p.d][0], ty = p.y + dir[p.d][1];
            if (p.x == 7 && p.y == 1 && p.d == 2) return;
            if (p.x == 7 && p.y == 16 && p.d == 3) return;
            if (_map[tx][ty] == 0) p.d = pred;
        }

        private bool GameOver()
        {
            if (p.fight)
            {
                if (p.x == g1.x && p.y == g1.y)
                {
                    Score += 300;
                    g1.x = -1; g1.y = -1;
                    g1.Alive = false;
                    tv += 5;
                }
                if (p.x == g2.x && p.y == g2.y)
                {
                    Score += 300;
                    g2.x = -1; g2.y = -1;
                    g2.Alive = false;
                    tv += 5;
                }
                if (p.x == g3.x && p.y == g3.y)
                {
                    Score += 300;
                    g3.x = -1; g3.y = -1;
                    g3.Alive = false;
                    tv += 5;
                }
            }
            else {
                if ((!g1.Alive && p.x == g1.x && p.y == g1.y) || (g2.Alive && p.x == g2.x && p.y == g2.y) || (g3.Alive && p.x == g3.x && p.y == g3.y))
                {
                    Over++;
                    if (Over==1) { MessageBox.Show("输了！！", "GameOver"); }
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    _begin = false;
                    return true;
                }
            }
            if (!g1.Alive && !g2.Alive && !g3.Alive)
            {
                Over++;
                if(Over==1) MessageBox.Show("赢了！！", "GameOver");
                timer1.Stop();
                timer2.Stop();
                timer3.Stop();
                _begin = false;
                return true;
            }
            return false;
        }

        private void Paint_Bean(int x, int y)
        {
            int tx = 40 * y - 20, ty = 40 * x - 30;
            gr.FillEllipse(Brushes.Yellow, tx + 15, ty + 15, 10, 10);
            gr.FillEllipse(Brushes.Black, tx + 17, ty + 18, 3, 3);
            gr.FillEllipse(Brushes.Black, tx + 20, ty + 18, 3, 3);
        }

        private void Paint_ReaBean(int x, int y)
        {
            int tx = 40 * y - 10, ty = 40 * x - 20;
            Pen t = new Pen(Color.Black, 2);
            gr.FillEllipse(Brushes.Red, tx, ty, 20, 20);
            gr.DrawLine(t, tx+4, ty+5, tx+7, ty+8);
            gr.DrawLine(t, tx+16, ty+5, tx+13, ty+8);
        }

        private void 游戏说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1.鬼的速度回随着时间增加而增加，且加速度也在增加\n"
                +"2.吃豆人的能力并不会比鬼差，随着所吃豆子的增加，速度也会提高\n"
                +"3.红色豆子会让吃豆人瞬间变得强壮，甚至可以吃鬼\n"
                +"4.红色豆子每隔一段时间随机刷出，但有存在时间限制\n"
                +"5.利用好时空之门，同时注意鬼也可以穿越时空之门","游戏说明");
        }

        private void 游戏技巧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1.鬼在躲避吃豆人追杀的过程中，因为慌乱，可能会撞在一起，\n"
            +"此时他们很可能忽略节操而选择抱在一起逃跑，如果能抓住这段时机，可以一次解决掉两只鬼\n"
            +"2.鬼不会傻到去抓可以比自己更快到达红豆子处的吃豆人\n"
            +"3.鬼不是豆子，吃了会让吃豆人消化不良导致速度有所减慢，留一些逃跑的时间", "游戏技巧");
        }

    }
}
