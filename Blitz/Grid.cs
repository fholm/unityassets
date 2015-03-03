using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blitz {
  [Serializable]
  public class Grid {
    readonly Bitset nodes;

    public bool AutoSmooth = false;

    public readonly Vector3 Center;
    public readonly short Rows;
    public readonly short Cols;
    public readonly int TotalNodes;
    public readonly Short2[] Connections;

    public readonly float Width;
    public readonly float Depth;
    public readonly float Spacing;

    public readonly float HalfWidth;
    public readonly float HalfDepth;

    public readonly float Left;
    public readonly float Right;
    public readonly float Top;
    public readonly float Bottom;

    public Grid(Vector3 center, short rows, short cols, float spacing)
      : this(center, rows, cols, spacing, false) {

    }

    public Grid(Vector3 center, short rows, short cols, float spacing, bool empty) {
      Rows = rows;
      Cols = cols;
      Center = center;
      Spacing = spacing;
      Width = ((float)(Cols - 1)) * Spacing;
      Depth = ((float)(Rows - 1)) * Spacing;
      TotalNodes = Rows * Cols;
      HalfWidth = Width / 2f;
      HalfDepth = Depth / 2f;

      Left = center.x + -HalfWidth;
      Right = center.x + HalfWidth;
      Top = center.z + HalfDepth;
      Bottom = center.z + -HalfDepth;
      Connections = new Short2[8];

      if (!empty) {
        nodes = new Bitset(rows * cols);
      }
    }

    public bool IsClosed(short row, short col) {
      return IsClosed(new Short2 { Row = row, Col = col });
    }

    public bool IsClosed(Short2 node) {
      return nodes.IsSet((node.Row * Cols) + node.Col);
    }

    public void Close(short row, short col) {
      Close(new Short2 { Row = row, Col = col });
    }

    public void Close(Short2 node) {
      nodes.Set((node.Row * Cols) + node.Col);
    }

    //public void Close(int[] nodes) {
    //  Short2 node = new Short2();

    //  for (var i = 0; i < nodes.Length; ++i) {
    //    node.Packed = nodes[i];
    //    this.nodes.Set((node.Row * Cols) + node.Col);
    //  }
    //}

    //public Short2 ToGrid(int packed) {
    //  Short2 node = new Short2();
    //  node.Packed = packed;
    //  return node;
    //}

    public void Open(short row, short col) {
      Open(new Short2 { Row = row, Col = col });
    }

    public void Open(Short2 node) {
      nodes.Clear((node.Row * Cols) + node.Col);
    }

    public Short2 ToGrid(Vector3 world) {
      if (Left < world.x && Right > world.x) {
        if (Bottom < world.z && Top > world.z) {
          var rowDistance = Math.Abs(world.z - Top);
          var colDistance = Math.Abs(world.x - Left);

          var row = (short)Math.Round(rowDistance / Spacing);
          var col = (short)Math.Round(colDistance / Spacing);

          return new Short2 { Row = row, Col = col };
        }
      }

      return new Short2 { Row = -1, Col = -1 };
    }

    public Vector3 ToWorld(short row, short col) {
      var x = Left + (col * Spacing);
      var z = Top - (row * Spacing);
      return new Vector3(x, Center.y, z);
    }

    public Vector3 ToWorld(Short2 pos) {
      return ToWorld(pos.Row, pos.Col);
    }

    public int GetConnections(Short2 node) {
      int r;
      int n = 0;

      // row above: left, mid and right
      r = node.Row - 1;
      getConnection(r, node.Col - 1, ref n);
      getConnection(r, node.Col, ref n);
      getConnection(r, node.Col + 1, ref n);

      // same row: left, right
      getConnection(node.Row, node.Col - 1, ref n);
      getConnection(node.Row, node.Col + 1, ref n);

      // row below: left, mid and right
      r = node.Row + 1;
      getConnection(r, node.Col - 1, ref n);
      getConnection(r, node.Col, ref n);
      getConnection(r, node.Col + 1, ref n);

      return n;
    }

    public bool HasLineOfSight(Short2 a, Short2 b) {
      var x0 = a.Col;
      var y0 = a.Row;
      var x1 = b.Col;
      var y1 = b.Row;

      int dx = Math.Abs(x1 - x0);
      int dy = Math.Abs(y1 - y0);
      int col = x0;
      int row = y0;
      int n = 1 + dx + dy;
      int x_inc = (x1 > x0) ? 1 : -1;
      int y_inc = (y1 > y0) ? 1 : -1;
      int error = dx - dy;

      dx *= 2;
      dy *= 2;

      for (; n > 0; --n) {
        if (nodes.IsSet((row * Cols) + col)) {
          return false;
        }

        if (error > 0) {
          col += x_inc;
          error -= dy;
        }
        else {
          row += y_inc;
          error += dx;
        }
      }

      return true;
    }

    public Short2[] Smooth(Short2[] path) {
      if (path != null && path.Length > 2) {
        List<Short2> smoothed = new List<Short2>();
        Short2 waypoint;
        Short2 previous;

        waypoint = previous = path[0];

        for (var i = 1; i < path.Length; ++i) {
          if (!HasLineOfSight(waypoint, path[i])) {
            smoothed.Add(waypoint);
            waypoint = previous;
          }

          if (i + 1 == path.Length) {
            smoothed.Add(waypoint);
            smoothed.Add(path[i]);
          }

          previous = path[i];
        }

        return smoothed.ToArray();
      }

      return path;
    }

    public static int Cost(ActiveNode node, ActiveNode parent) {
      return parent.G + (IsDiagonal(node.Node, parent.Node) ? 14 : 10);
    }

    public static int Heuristic(Short2 node, Short2 target) {
      int c_dist = node.Col - target.Col;
      int r_dist = node.Row - target.Row;

      if (c_dist < 0) {
        c_dist = -c_dist;
      }

      if (r_dist < 0) {
        r_dist = -r_dist;
      }

      int h = 0;

      if (c_dist > r_dist) {
        h = 14 * r_dist + 10 * (c_dist - r_dist);
      }
      else {
        h = 14 * c_dist + 10 * (r_dist - c_dist);
      }

      if(node.Col != target.Col && node.Row != target.Row) {
        h *= 1;
      }
      else {
        h *= 1;
      }

      return h;
    }

    public static bool IsDiagonal(Short2 a, Short2 b) {
      return a.Row != b.Row && a.Col != b.Col;
    }

    void getConnection(int row, int col, ref int n) {
      if (row >= 0 && col >= 0 && row < Rows && col < Cols) {
        Connections[n].Row = (short)row;
        Connections[n].Col = (short)col;
        ++n;
      }
    }
  }
}