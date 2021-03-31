using System.Collections.Generic;
using UnityEngine;
using System.Threading;

[System.Serializable]
public class PathingJob
{    
    public PathingNode[,] map;
    public Vector3 mapOrigin;
    public Vector3 mapNodeSize = Vector3.one;
    public Vector3 start;
    public Vector3 end;

    private bool foundPath;
    private List<Vector3> path;

    private int runningThreads;
    private Pathing.StopControl stopControl;

    public PathingJob(
        PathingNode[,] map, 
        Vector3 mapOrigin, Vector3 mapNodeSize, 
        Vector3 start, Vector3 end)
    {
        this.map = map;
        this.mapOrigin = mapOrigin;
        this.mapNodeSize = mapNodeSize;
        this.start = start;
        this.end = end;
        stopControl = new Pathing.StopControl();
    }

    /// <summary>
    /// Tries once to start a thread. If there's another running this returns false.
    /// </summary>
    public bool TryStart()
    {
        lock (stopControl)
        {
            if (0 < runningThreads)
                return false;
            else
                ++runningThreads;
        }

        stopControl.stop = false;
        new Thread(Solve).Start();
        return true;
    }

    /// <summary>
    /// Sits still until all threads are finished then starts a new thread.
    /// </summary>
    public void WaitForStart()
    {
        while (TryStart() == false)
        {
            Thread.Yield();
        }
    }

    public bool IsFinished(out bool foundPath, out List<Vector3> path)
    {
        bool result = true;
        lock(stopControl)
        {
            if (0 < runningThreads)
                result = false;
        }

        if(result)
        {
            foundPath = this.foundPath;
            path = this.path;
            return true;
        }
        else
        {
            foundPath = default;
            path = default;
            return false;
        }
    }

    public bool GetResults(out List<Vector3> path)
    {        
        path = this.path;
        return foundPath;
    }

    public bool Stop()
    {
        lock (stopControl)
        {
            if (0 == runningThreads)
                return false;
        }

        stopControl.stop = true;
        return true;
    }

    private void Solve()
    {        
        foundPath = Pathing.Solve((PathingNode[,])map.Clone(), mapOrigin, mapNodeSize, start, end, out path, stopControl);
        lock (stopControl)
        {
            --runningThreads;
        }
    }
}
