//By Gong Xueyuan
//2016.6.17
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    public class Node : IComparable<Node>
    {
        public int i;   //Record the x coordinate of the node
        public int j;   //Record the y coordinate of the node
        public int g;   //The distance that has walked so far
        public int h;   //The estimated distance about to walk
        public int f;   //Fitness value, which is equal to g+h
        public Node pre;  //The parent node of this node
        public int listIndex;   //0 for null, 1 for open list and 2 for close list
        public int cType; //Cell type, 0 for walkable, 1 for unwalkable

        public Node()
        {
            i = 0;
            j = 0;
            g = 0;
            h = 0;
            f = 0;
            pre = null;
            listIndex = 0;
            cType = 0;
        }

        public void reset()
        {
            g = 0;
            h = 0;
            f = 0;
            pre = null;
            listIndex = 0;
        }

        public int CompareTo(Node node)
        {
            if (f > node.f) { return 1; }
            else if (f < node.f) { return -1; }
            else { return 0; }
        }
    }
}
