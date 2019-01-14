using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph {

    Dictionary<Tile, Path_Node<Tile>> nodes;

    public Dictionary<Tile, Path_Node<Tile>> Nodes
    {
        get { return nodes; }
    }

    public Path_TileGraph(Map map)
    {
        nodes = new Dictionary<Tile, Path_Node<Tile>>();

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Width; y++)
            {
                Tile t = map.getTileAt(new Point(x, y));

                if (t != null && t.Cost > 0)
                {
                    Path_Node<Tile> n = new Path_Node<Tile>();
                    n.data = t;
                    nodes.Add(t, n);
                }
            }
        }

        foreach(Tile t in nodes.Keys)
        {
            Path_Node<Tile> n = nodes[t];

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            List<Tile> neighbours = map.getNeighbours(t, true);

            for (int i = 0; i < neighbours.Count; i++)
            {
                if(neighbours[i] != null && neighbours[i].Cost > 0)
                {
                    if(isClippingCorner(map, t, neighbours[i]))
                    {
                        continue;
                    }

                    Path_Edge<Tile> e = new Path_Edge<Tile>();

                    e.cost = neighbours[i].Cost;
                    e.node = nodes[neighbours[i]];

                    edges.Add(e);
                }
            }

            n.edges = edges.ToArray();
        }
    }

    bool isClippingCorner(Map map, Tile curr, Tile neigh)
    {
        int dX = curr.X - neigh.X;
        int dY = curr.Y - neigh.Y;

        if(Mathf.Abs(dX) + Mathf.Abs(dY) == 2)
        {

            if(map.getTileAt(curr.X-dX, curr.Y).Cost==0)
            {
                return true;
            }
            if(map.getTileAt(curr.X, curr.Y-dY).Cost == 0)
            {
                return true;
            }
        }
        return false;
    }
}
