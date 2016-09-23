//By Gong Xueyuan
//2016.6.17
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    public class Astar
    {
        //Calculate Manhattan distance between cell 1 and cell 2
        static public int calculateH(int x1,int y1,int x2,int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        //Find the path from start cell to end cell in the graph
        static public void findPath(Node[,] graph, Node sNode, Node eNode)
        {
            List<Node> openList = new List<Node>();
            int xn = graph.GetLength(0) - 1, //Length of x dimension of the grid
                yn = graph.GetLength(1) - 1; //Length of y dimension of the grid
            int si = sNode.i, //x index of start cell
                sj = sNode.j, //y index of start cell
                ei = eNode.i, //x index of end cell
                ej = eNode.j; //y index of end cell
            int ci, //x index of current cell
                cj; //y index of current cell
            int ti, //x index of the top cell of current cell
                tj, //y index of the top cell of current cell
                bi, //x index of the bottom cell of current cell
                bj; //y index of the bottom cell of current cell
            int[] ni = new int[8],  //x index of neighbor cells
                  nj = new int[8];  //y index of neighbor cells
            int ng; //New g value
            Node cNode; //Current node

            //Initialize start cell
            graph[si, sj].h = calculateH(si, sj, ei, ej);
            graph[si, sj].f = graph[si, sj].h + graph[si, sj].g;
            openList.Add(graph[si, sj]);

            //Main loop to find the path
            while (openList.Count > 0)  //While the open list is not empty
            {
                openList.Sort();    //Sort the open list so that the node with the smallest f is in the head
                cNode = openList[0];    //Assign the first node in open list to current node
                openList.RemoveAt(0);   //Remove the first node from open list
                //closeList.Add(cNode);   //Add current node to close list
                cNode.listIndex = 2;    //Index the current node that it is in the close list
                ci = cNode.i;
                cj = cNode.j;

                if(cNode.Equals(graph[ei,ej])){ break;  }   //If arrive the end cell, stop

                //Initialize the coordinates of neighbor cells
                bi = (ci - 1) > 0 ? (ci - 1) : 0;
                bj = (cj - 1) > 0 ? (cj - 1) : 0;
                ti = (ci + 1) < xn ? (ci + 1) : xn;
                tj = (cj + 1) < yn ? (cj + 1) : yn;
                ni[0] = ci; nj[0] = bj; //Bottom cell
                ni[1] = ti; nj[1] = cj; //Right cell
                ni[2] = ci; nj[2] = tj; //Top cell
                ni[3] = bi; nj[3] = cj; //Left cell
                ni[4] = bi; nj[4] = bj; //Bottom left cell
                ni[5] = ti; nj[5] = bj; //Bottom right cell
                ni[6] = bi; nj[6] = tj; //Top left cell
                ni[7] = ti; nj[7] = tj; //Top right cell

                //Calculating all neighbor cells
                for (int i = 0; i < 8; ++i)
                {
                    if (graph[ni[i], nj[i]].cType == 0 && graph[ni[i], nj[i]].listIndex != 2)
                    {
                        if (i < 4)
                        {
                            ng = cNode.g + 10;
                        }
                        else
                        {
                            ng = cNode.g + 14;

                            //Avoid to walk diagonally when near the wall
                            if (i == 4 && (graph[ni[0], nj[0]].cType == 1 || graph[ni[3], nj[3]].cType == 1)) { continue; }
                            else if (i == 5 && (graph[ni[0], nj[0]].cType == 1 || graph[ni[1], nj[1]].cType == 1)) { continue; }
                            else if (i == 6 && (graph[ni[2], nj[2]].cType == 1 || graph[ni[3], nj[3]].cType == 1)) { continue; }
                            else if (i == 7 && (graph[ni[1], nj[1]].cType == 1 || graph[ni[2], nj[2]].cType == 1)) { continue; }
                        }

                        if (graph[ni[i], nj[i]].listIndex == 1)
                        {
                            if (ng < graph[ni[i], nj[i]].g)
                            {
                                graph[ni[i], nj[i]].g = ng;
                                graph[ni[i], nj[i]].f = ng + graph[ni[i], nj[i]].h;
                                graph[ni[i], nj[i]].pre = cNode;
                            }
                        }
                        else
                        {
                            graph[ni[i], nj[i]].g = ng;
                            graph[ni[i], nj[i]].h = calculateH(ni[i], nj[i], ei, ej);
                            graph[ni[i], nj[i]].f = ng + graph[ni[i], nj[i]].h;
                            graph[ni[i], nj[i]].pre = cNode;
                            graph[ni[i], nj[i]].listIndex = 1;
                            openList.Add(graph[ni[i], nj[i]]);
                        }
                    }
                }
            }
        }
    }
}
