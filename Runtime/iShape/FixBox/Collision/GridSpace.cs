using System.Runtime.CompilerServices;
using Unity.Mathematics;
using iShape.FixBox.Collider;
using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Collision {

    public struct BitMask {

        public ulong Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitMask(ulong value) {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBit(int index) {
            Value |= 1UL << index;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasNext() {
            return Value > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Union(BitMask m) {
            Value |= m.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next() {
            int index = math.tzcnt(Value);
            Value -= 1UL << index;
            return index;
        }
    }

    public struct GridSpace {

        public NativeArray<BitMask> Cells;
        public Boundary Base;
        public FixVec Scale;
        public int Size;
        public int SplitRatio;

        public GridSpace(int splitRatio, Allocator allocator) {
            SplitRatio = splitRatio;
            Size = 1 << splitRatio;
            Scale = FixVec.Zero;
            Base = Boundary.Zero;
            int n = Size << splitRatio;
            Cells = new NativeArray<BitMask>(n, allocator);
        }

        public void Dispose() {
            Cells.Dispose();
        }

        private int Index(int x, int y) {
            return x << SplitRatio + y;
        }

        public BitMask this[int x, int y] => Cells[Index(x, y)];

        public void Set(NativeArray<Boundary> boxes) {
            Base = new Boundary(boxes);

            FixVec ds = Base.Max - Base.Min;
            long sx = Size * FixNumber.Unit / ds.x;
            long sy = Size * FixNumber.Unit / ds.y;

            Scale = new FixVec(sx, sy);

            for (int i = 0; i < Cells.Length; i++) {
                Cells[i] = new BitMask(0);
            }

            for (int i = 0; i < boxes.Length; i++) {
                BitMask s = new BitMask(1UL << i);

                IndexBoundary j = boxes[i].Index(Base, Scale, Size);

                if (j.IsSimple) {
                    int a0 = Index(j.pMin.x, j.pMin.y);
                    var cell = Cells[a0];
                    cell.Union(s);
                    Cells[a0] = cell;
                } else {
                    for (int x = j.pMin.x; x <= j.pMax.x; x++) {
                        for (int y = j.pMin.y; y <= j.pMax.y; y++) {
                            int ai = Index(x, y);

                            var cell = Cells[ai];
                            cell.Union(s);
                            Cells[ai] = cell;
                        }
                    }
                }
            }
        }

        public BitMask Collide(Boundary boundary) {
            if (!Base.IsCollide(boundary)) {
                return new BitMask(0);
            }

            IndexBoundary j = boundary.Index(Base, Scale, Size);

            BitMask result = new BitMask(0);

            if (j.IsSimple) {
                int i = j.pMin.x << SplitRatio + j.pMin.y;
                result = Cells[i];
            } else {
                for (int x = j.pMin.x; x <= j.pMax.x; x++) {
                    for (int y = j.pMin.y; y <= j.pMax.y; y++) {
                        int i = x << SplitRatio + y;
                        result.Union(Cells[i]);
                    }
                }
            }

            return result;
        }
    }

    internal struct IndexBoundary {

        internal Index2d pMin;
        internal Index2d pMax;

        internal bool IsSimple => pMin.x == pMax.x && pMin.y == pMax.y;
    }

    internal struct Index2d {
        internal int x;
        internal int y;
    }
    
    internal static class BoundaryExtensions {

        internal static IndexBoundary Index(this Boundary boundary, Boundary baseBoundary, FixVec scale, int size) {
            int x0 = (boundary.Min.x - baseBoundary.Min.x).Mul(scale.x).ToInt();
            int y0 = (boundary.Min.y - baseBoundary.Min.y).Mul(scale.y).ToInt();
            int x1 = (boundary.Max.x - baseBoundary.Min.x).Mul(scale.x).ToInt();
            int y1 = (boundary.Max.y - baseBoundary.Min.y).Mul(scale.y).ToInt();

            int xMin = math.max(0, x0);
            int yMin = math.max(0, y0);
            int xMax = math.min(size - 1, x1);
            int yMax = math.min(size - 1, y1);

            return new IndexBoundary {
                pMin = new Index2d { x = xMin, y = yMin },
                pMax = new Index2d { x = xMax, y = yMax }
            };
        }
    }

}