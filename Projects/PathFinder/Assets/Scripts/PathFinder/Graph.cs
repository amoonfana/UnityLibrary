//By Gong Xueyuan
//2016.6.22
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    public class Graph
    {
        public float xo;  //Origin of x-dimension
        public float yo;  //Origin of y-dimension
        public float xSize; //x size of the cell
        public float ySize; //y size of the cell
        public float xOffset;   //x offset
        public float yOffset;   //y offset
        public int n;   //Number of cells in x-dimension
        public int m;   //Number of cells in y-dimension
        public Node sNode;  //Start cell
        public Node eNode;  //End cell
        public Node[,] grid;   //Grid map

        public Graph(float xo1, float yo1, float xs, float ys, int n1, int m1)
        {
            xo = xo1;
            yo = yo1;
            xSize = xs;
            ySize = ys;
            xOffset = xSize * 0.5f;
            yOffset = ySize * 0.5f;
            n = n1;
            m = m1;
            grid = new Node[n, m];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    grid[i, j] = new Node();
                    grid[i, j].i = i;
                    grid[i, j].j = j;
                }
            }
        }

        //Get the index of current position by the coordinate in the map
        public int cord2Index(float cord, float size, int limit)
        {
            int index = Convert.ToInt32(Math.Floor(cord / size));

            //Limit the index in the domain
            if (index < 0) { index = 0; }
            else if (index >= limit) { index = limit - 1; }

            return index;
        }

        public Vector2 setWallGrid(float x, float y)
        {
            //Get the indices i and j of current mouse position in graph
            int i = cord2Index(x - xo, xSize, n),
                j = cord2Index(y - yo, ySize, m);

            //If the cell is walkable, not the start node and not the end node
            if (grid[i, j].cType == 0 && !grid[i, j].Equals(sNode) && !grid[i, j].Equals(eNode))
            {
                grid[i, j].cType = 1;  //Set the cell unwalkable
            }

            return new Vector2(xo + i * xSize + xOffset, yo + j * ySize + yOffset);
        }

        public Vector2 setWalkGrid(float x, float y)
        {
            //Get the indices i and j of current mouse position in graph
            int i = cord2Index(x - xo, xSize, n),
                j = cord2Index(y - yo, ySize, m);

            grid[i, j].cType = 0;  //Set the cell walkable

            return new Vector2(xo + i * xSize + xOffset, yo + j * ySize + yOffset);
        }

        public Vector2 setStartGrid(float x, float y)
        {
            //Get the indices i and j of current mouse position in graph
            int i = cord2Index(x - xo, xSize, n),
                j = cord2Index(y - yo, ySize, m);

            if (grid[i, j].cType == 0 && !grid[i, j].Equals(eNode))  //If the cell is walkable and not the end node
            {
                sNode = grid[i, j];
            }

            return new Vector2(xo + i * xSize + xOffset, yo + j * ySize + yOffset);
        }

        public Vector2 setEndGrid(float x, float y)
        {
            //Get the indices i and j of current mouse position in graph
            int i = cord2Index(x - xo, xSize, n),
                j = cord2Index(y - yo, ySize, m);

            if (grid[i, j].cType == 0 && !grid[i, j].Equals(sNode))  //If the cell is walkable and not the start node
            {
                eNode = grid[i, j];
            }

            return new Vector2(xo + i * xSize + xOffset, yo + j * ySize + yOffset);
        }

        public void reset()
        {
            sNode = null;
            eNode = null;

            //Clear information in the graph
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    grid[i, j].reset();
                }
            }
        }

        public Stack<Vector2> getPath()
        {
            Stack<Vector2> path = new Stack<Vector2>();

            if (sNode == null || eNode == null) { return path; }

            Astar.findPath(grid, sNode, eNode);

            Node cNode = eNode;
            while (cNode.pre != null)
            {
                path.Push(new Vector2(xo + cNode.i * xSize + xOffset, yo + cNode.j * ySize + yOffset));
                cNode = cNode.pre;
            }

            reset();

            return path;
        }
    }
}
