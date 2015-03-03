using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Blitz {
  public class PathFinder {
    ActiveNodeHeap openList = new ActiveNodeHeap();
    HashSet<int> closedList = new HashSet<int>();
    Dictionary<int, ActiveNode> activeNodes = new Dictionary<int, ActiveNode>();
    List<Short2> path = new List<Short2>();
    List<ActiveNode> activeList = new List<ActiveNode>();

    public bool FindPath(Grid grid, Vector3 from, Vector3 to, out Vector3[] foundPath) {
      Short2[] path;

      if (FindPath(grid, from, to, out path)) {
        foundPath = new Vector3[path.Length];

        if (foundPath.Length == 2) {
          foundPath[0] = from;
          foundPath[1] = to;
          foundPath[1].y = from.y;
        }
        else {
          for (var i = 1; i < path.Length - 1; ++i) {
            foundPath[i] = grid.ToWorld(path[i]);
            foundPath[i].y = from.y;
          }

          foundPath[0] = from;
          foundPath[foundPath.Length - 1] = to;
          foundPath[foundPath.Length - 1].y = from.y;
        }

        return true;
      }
      else {
        foundPath = null;
        return false;
      }
    }

    public bool FindPath(Grid grid, Vector3 from, Vector3 to, out Short2[] foundPath) {
      return FindPath(grid, grid.ToGrid(from), grid.ToGrid(to), out foundPath);
    }

    public bool FindPath(Grid grid, Short2 from, Short2 to, out Short2[] foundPath) {
      var sw = System.Diagnostics.Stopwatch.StartNew();

      foundPath = null;

      var found = false;
      int totalNodesSearched = 0;
      ActiveNode connectionNode = null;

      if (grid.IsClosed(to))
        return false;

      var start = ActiveNode.Create(from);
      openList.Push(start);

      while (openList.NumberOfItems > 0) {
        var current = openList.Pop();
        var numConnections = grid.GetConnections(current.Node);

        for (var i = 0; i < numConnections; ++i) {
          var connection = grid.Connections[i];

          if (grid.IsClosed(connection))
            continue;

          if (closedList.Contains(connection.Packed))
            continue;

          // Old Node
          if (activeNodes.TryGetValue(connection.Packed, out connectionNode)) {
            var newG = Grid.Cost(connectionNode, current);
            if (newG < connectionNode.G) {
              connectionNode.G = newG;
              connectionNode.F = connectionNode.H + newG;
              connectionNode.Parent = current;
              openList.Update(connectionNode);
            }
          }

          // New Node
          else {
            activeNodes[connection.Packed] = connectionNode = ActiveNode.Create(connection);

            connectionNode.H = Grid.Heuristic(connection, to);
            connectionNode.G = Grid.Cost(connectionNode, current);
            connectionNode.F = connectionNode.H + connectionNode.G;
            connectionNode.Parent = current;

            openList.Push(connectionNode);
            activeList.Add(connectionNode);

            ++totalNodesSearched;
          }
        }

        if (current.Node.Packed == to.Packed) {
          while (current != null) {
            path.Add(current.Node);
            current = current.Parent;
          }

          path.Reverse();
          foundPath = path.ToArray();

          if (grid.AutoSmooth) {
            foundPath = grid.Smooth(foundPath);
          }

          found = true;

          break;
        }

        closedList.Add(current.Node.Packed);
      }

      for (var i = 0; i < activeList.Count; ++i) {
        ActiveNode.Recycle(activeList[i]);
      }

      sw.Stop();

      openList.Clear();
      closedList.Clear();
      path.Clear();
      activeList.Clear();
      activeNodes.Clear();

      return found;
    }
  }
}
