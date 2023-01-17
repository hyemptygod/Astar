namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public interface ICell
    {
        Vector pos { get; }
        bool walkable { get; }
        int cost { get; }
    }

    public class Node : IComparable<Node>
    {
        public static uint UID = 1;

        public struct Neighbour
        {
            public ICell cell;
            public float cost;
        }

        public ICell cell { get; set; }
        
        public uint uid { get; set; }

        public float F { get; set; }
        public float G { get; set; }
        public float H { get; set; }

        public Node Parent { get; set; }

        public Node(ICell cell)
        {
            this.cell = cell;
            uid = UID++;
        }

        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }

        public IEnumerable<Neighbour> GetNeighbours(IMap map, Zones.Zone zone, NeighbourMode mode)
        {
            if(cell == null)
            {
                yield break;
            }


            Neighbour result = new Neighbour();
            bool check_cell(Vector offset)
            {
                result.cell = map[cell.pos + offset];
                if(result.cell == null || !result.cell.walkable)
                {
                    return false;
                }
                result.cost = result.cell.cost * offset.magnitude;
                return true;
            }

            foreach (var offset in Vector.Neighbours(mode))
            {
                if (check_cell(offset))
                {
                    yield return result;
                }
            }
        }
    }
}
