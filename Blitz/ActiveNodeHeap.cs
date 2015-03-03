using System;

namespace Blitz {
  class ActiveNodeHeap {
    int numberOfItems;
    ActiveNode[] binaryHeap;

    public ActiveNodeHeap(int numberOfElements)
      : base() {
      binaryHeap = new ActiveNode[numberOfElements];
      numberOfItems = 1;
    }

    public ActiveNodeHeap()
      : this(1024) {

    }

    public void Clear() {
      numberOfItems = 1;
    }

    public int NumberOfItems {
      get {
        return numberOfItems - 1;
      }
    }

    public void Update(ActiveNode node) {
      var bubbleIndex = node.Slot;

      while (bubbleIndex != 1) {
        var parentIndex = bubbleIndex / 2;
        if (binaryHeap[parentIndex].F > node.F) {
          binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
          binaryHeap[parentIndex] = node;

          binaryHeap[parentIndex].Slot = parentIndex;
          binaryHeap[bubbleIndex].Slot = bubbleIndex;

          bubbleIndex = parentIndex;
        }
        else
          break;
      }
    }

    public void Push(ActiveNode node) {
      if (numberOfItems == binaryHeap.Length) {
        var newHeap = new ActiveNode[binaryHeap.Length * 2];
        Array.Copy(binaryHeap, newHeap, binaryHeap.Length);
        binaryHeap = newHeap;
      }

      var bubbleIndex = numberOfItems;
      binaryHeap[bubbleIndex] = node;
      node.Slot = bubbleIndex;

      while (bubbleIndex != 1) {
        var parentIndex = bubbleIndex / 2;
        if (binaryHeap[parentIndex].F > node.F) {
          binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
          binaryHeap[parentIndex] = node;

          binaryHeap[parentIndex].Slot = parentIndex;
          binaryHeap[bubbleIndex].Slot = bubbleIndex;

          bubbleIndex = parentIndex;
        }
        else
          break;
      }

      ++numberOfItems;
    }

    public ActiveNode Pop() {
      numberOfItems--;

      var returnItem = binaryHeap[1];
      binaryHeap[1] = binaryHeap[numberOfItems];

      var swapItem = 1;
      var parent = 1;

      do {
        parent = swapItem;

        if ((2 * parent + 1) <= numberOfItems) {
          // Both children exist
          if (binaryHeap[parent].F >= binaryHeap[2 * parent].F) {
            swapItem = 2 * parent;
          }
          if (binaryHeap[swapItem].F >= binaryHeap[2 * parent + 1].F) {
            swapItem = 2 * parent + 1;
          }
        }
        else if ((2 * parent) <= numberOfItems) {
          // Only one child exists
          if (binaryHeap[parent].F >= binaryHeap[2 * parent].F) {
            swapItem = 2 * parent;
          }
        }

        // One if the parent's children are smaller or equal, swap them
        if (parent != swapItem) {
          var tmpIndex = binaryHeap[parent];

          binaryHeap[parent] = binaryHeap[swapItem];
          binaryHeap[swapItem] = tmpIndex;

          binaryHeap[parent].Slot = parent;
          binaryHeap[swapItem].Slot = swapItem;
        }

      } while (parent != swapItem);

      return returnItem;
    }
  }
}