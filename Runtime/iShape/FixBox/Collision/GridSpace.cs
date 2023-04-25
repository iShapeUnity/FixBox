using System.Runtime.CompilerServices;
using Unity.Mathematics;
using iShape.FixBox.Collider;
using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Collision {

    public struct BitMask {
        
        private ulong Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitMask(ulong value) {
            Value = value;
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
        public void Intersection(ulong m) {
            Value &= m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next() {
            int index = math.tzcnt(Value);
            Value &= ~(1UL << index);
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() {
            return math.countbits(Value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddSpaceRightMask(ulong rightMask) {
            Value = ((Value & ~rightMask) << 1) | (Value & rightMask);
        }

        public float4 Color() {
            var x = Value;
            if (x != 0) {
                float r = (byte)(x & 0xFF) / 255f;
                x = x >> 8;
                float g = (byte)(x & 0xFF) / 255f;
                x = x >> 8;
                float b = (byte)(x & 0xFF) / 255f;
                return new float4(r, g, b, .8f);
            } else {
                return new float4(0, 0, 0, 0f);
            }
        }
    }

    public struct GridSpace {
        private Boundary Base { get; }
        public NativeArray<BitMask> Cells;
        public readonly int RowCellCount;

        public float2 CellSize => Base.Size.ToFloat2() / RowCellCount;

        private readonly FixVec iScale;
        private readonly int splitRatio;

        public GridSpace(Boundary boundary, int splitRatio, Allocator allocator) {
            this.splitRatio = splitRatio;
            RowCellCount = 1 << splitRatio;
            Base = boundary;
            
            FixVec ds = Base.Size;
            long rowCount = RowCellCount.ToFix();
            long sx = rowCount.Div(ds.x);
            long sy = rowCount.Div(ds.y);
            
            iScale = new FixVec(sx, sy);
            
            int n = RowCellCount << splitRatio;
            Cells = new NativeArray<BitMask>(n, allocator, NativeArrayOptions.UninitializedMemory);
            Clear();
        }

        public void Dispose() {
            Cells.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Index(int x, int y) {
            return (y << splitRatio) + x;
        }

        public BitMask this[int x, int y] => Cells[Index(x, y)];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear() {
            var zeroMask = new BitMask(0);
            for (int i = 0; i < Cells.Length; i++) {
                Cells[i] = zeroMask;
            }
        }

        public void Set(Boundary box, int i) {
            BitMask s = new BitMask(1UL << i);
            IndexBoundary j = box.Index(Base, iScale, RowCellCount);
            
            if (j.IsSimple) {
                int a0 = Index(j.pMin.x, j.pMin.y);
                var cell = Cells[a0];
                cell.Union(s);
                Cells[a0] = cell;
            } else {
                for (int y = j.pMin.y; y <= j.pMax.y; ++y) {
                    for (int x = j.pMin.x; x <= j.pMax.x; ++x) {
                        int ai = Index(x, y);
            
                        var cell = Cells[ai];
                        cell.Union(s);
                        Cells[ai] = cell;
                    }
                }
            }
        }

        public void AddPlace(int index) {
            ulong rightMask = (1UL << index) - 1;
            for (int i = 0; i < Cells.Length; ++i) {
                var cell = Cells[i];
                cell.AddSpaceRightMask(rightMask);
                Cells[i] = cell;
            }
        }

        private void Clear(Boundary box, int i) {
            ulong reverseMask = ~(1UL << i);
            
            IndexBoundary j = box.Index(Base, iScale, RowCellCount);
            
            if (j.IsSimple) {
                int a0 = Index(j.pMin.x, j.pMin.y);
                var cell = Cells[a0];
                cell.Intersection(reverseMask);
                Cells[a0] = cell;
            } else {
                for (int y = j.pMin.y; y <= j.pMax.y; ++y) {
                    for (int x = j.pMin.x; x <= j.pMax.x; ++x) {
                        int ai = Index(x, y);
                        var cell = Cells[ai];
                        cell.Intersection(reverseMask);
                        Cells[ai] = cell;
                    }
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Move(Boundary oldBox, Boundary newBox, int i) {
            this.Clear(oldBox, i);
            this.Set(oldBox, i);
        }

        public BitMask Collide(Boundary boundary) {
            BitMask result = new BitMask(0);
            if (!Base.IsCollide(boundary)) {
                return result;
            }

            var j = boundary.Index(Base, iScale, RowCellCount);

            if (j.IsSimple) {
                result = this[j.pMin.x, j.pMin.y];
            } else {
                for (int y = j.pMin.y; y <= j.pMax.y; ++y) {
                    for (int x = j.pMin.x; x <= j.pMax.x; ++x) {
                        result.Union(this[x, y]);
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

    internal readonly struct Index2d {
        internal readonly int x;
        internal readonly int y;

        internal Index2d(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
    
    internal static class BoundaryExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                pMin = new Index2d(xMin, yMin),
                pMax = new Index2d(xMax, yMax)
            };
        }
    }

}