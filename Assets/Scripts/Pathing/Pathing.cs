using UnityEngine;

public static class Pathing
{
    public class StopControl
    {
        public bool stop;
    }

    public static bool Solve(PathingMap pathingMap, Vector2Int sink, StopControl stopControl = null)
    {
        if(pathingMap.InRange(sink) == false)
            throw new System.ArgumentException("argument out of range", "sink");

        {
            int mapLength = pathingMap.map.Length;
            for (int i = 0; i < mapLength; ++i)
                pathingMap.map[i].totalCost = -1;
        }

        int sinkIndex = pathingMap.GetMapIndex(sink);
        pathingMap.map[sinkIndex].totalCost = 0;
        pathingMap.map[sinkIndex].cameFrom = sink;
        BinaryTree<float, Vector2Int> frontier = new BinaryTree<float, Vector2Int>();
        frontier.Initialise();
        frontier.Add(0, sink);
                
        // find the node in frontier that has the lowest heuristic!
        while (frontier.Pop(out _, out Vector2Int currentPosition))
        {
            float totalCost = pathingMap.map[pathingMap.GetMapIndex(currentPosition)].totalCost;

            foreach (PathingMap.Neighbor neighbor in pathingMap.GetNeighbors(currentPosition))
            {
                if (stopControl != null && stopControl.stop)
                    return false;

                int neighborIndex = pathingMap.GetMapIndex(neighbor.position);
                
                if (pathingMap.map[neighborIndex].isWalkable == false)
                    continue;

                float totalNeighborCost = totalCost + neighbor.cost;

                if (pathingMap.map[neighborIndex].totalCost < 0 ||
                    totalNeighborCost < pathingMap.map[neighborIndex].totalCost)
                {
                    pathingMap.map[neighborIndex].cameFrom = currentPosition;
                    pathingMap.map[neighborIndex].totalCost = totalNeighborCost;

                    frontier.Add(pathingMap.map[pathingMap.GetMapIndex(neighbor.position)].totalCost, neighbor.position);
                }
            }
        }

        return true;
    }
}
