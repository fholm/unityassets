using System.Runtime.InteropServices;

namespace Blitz {
  [StructLayout(LayoutKind.Explicit)]
  public struct Short2 {
    [FieldOffset(0)]
    public short Row;

    [FieldOffset(2)]
    public short Col;

    [FieldOffset(0)]
    public int Packed;

    public Short2(short row, short col) {
      Packed = 0;
      Row = row;
      Col = col;
    }

    public override string ToString() {
      return string.Format("Row: {0}, Col: {1}", Row, Col);
    }
  }
}