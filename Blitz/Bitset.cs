#define SAFE

using System;
using System.Collections.Generic;

namespace Blitz {
  public class Bitset : IEnumerable<bool> {
    public const int SLOT_SIZE = 32;
    public int[] Storage;
    public readonly int Size;

    public Bitset(int size) {
      if (size < 0)
        throw new ArgumentException("Can't be negative", "size");

      if (size % SLOT_SIZE != 0)
        size += SLOT_SIZE - (size % SLOT_SIZE);

      this.Size = size;
      this.Storage = new int[size / SLOT_SIZE];
    }

    public void Set(int bit) {
#if SAFE
      if (bit >= Size)
        throw new ArgumentException("Must be smaller then size", "bit");
#endif

      var i = bit / SLOT_SIZE;
      Storage[i] |= (1 << (bit - (i * SLOT_SIZE)));
    }

    public void Clear(int bit) {
#if SAFE
      if (bit >= Size)
        throw new ArgumentException("Must be smaller then size", "bit");
#endif

      var i = bit / SLOT_SIZE;
      Storage[i] &= ~(1 << (bit - (i * SLOT_SIZE)));
    }

    public bool IsSet(int bit) {
#if SAFE
      if (bit >= Size)
        throw new ArgumentException("Must be smaller then size", "bit");
#endif

      var i = bit / SLOT_SIZE;
      return (Storage[i] & (1 << (bit - (i * SLOT_SIZE)))) != 0;
    }

    public IEnumerator<bool> GetEnumerator() {
      for (var i = 0; i < Size; ++i) {
        yield return IsSet(i);
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return (System.Collections.IEnumerator)GetEnumerator();
    }
  }
}
