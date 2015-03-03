using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blitz {
  public class ActiveNode {
    static int poolCount;
    static readonly Stack<ActiveNode> pool = new Stack<ActiveNode>();

    public Short2 Node;
    public ActiveNode Parent;
    public int F, G, H, Slot;

    ActiveNode() {

    }

    public static ActiveNode Create(Short2 node) {
      ActiveNode activeNode;

      if (poolCount > 0) {
        activeNode = pool.Pop();
        poolCount -= 1;
      }
      else {
        activeNode = new ActiveNode();
      }

      activeNode.Node = node;
      activeNode.F = 0;
      activeNode.G = 0;
      activeNode.H = 0;
      activeNode.Slot = 0;
      activeNode.Parent = null;

      return activeNode;
    }

    public static void Recycle(ActiveNode node) {
      pool.Push(node);
    }

    public override string ToString() {
      return Node.ToString();
    }

    public override int GetHashCode() {
      return Node.Packed;
    }

    public override bool Equals(object obj) {
      return ((ActiveNode)obj).Node.Packed == Node.Packed;
    }
  }
}
