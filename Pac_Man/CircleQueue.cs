using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pac_Man
{
    class CircleQueue
    {
        int[] _x, _y, _t;
        int front, rear;
        public CircleQueue()
        {
            _x = new int[200];
            _y = new int[200];
            _t = new int[200];
            front = rear = 0;
        }
        public void push(int tx, int ty, int tt)
        {
            _x[front] = tx;
            _y[front] = ty;
            _t[front] = tt;
            front = (front + 1) % 200;
        }
        public void pop()
        {
            rear = (rear + 1) % 200;
        }
        public int getX() { return _x[rear]; }
        public int getY() { return _y[rear]; }
        public int getT() { return _t[rear]; }

        public bool isempty() { return rear == front; }
        public void clear() { front = rear = 0; }
    }
}
