using System.Runtime.CompilerServices;
using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Geometry {

    public static class ConvexExtension {
        
        public static bool IsPointInsideConvexPolygon(this NativeArray<FixVec> polygon, FixVec point)
        {
            FixVec p0 = polygon[0];
            FixVec p1 = polygon[1];

            if (Orientation(p0, p1, point) < 0)
            {
                return false;
            }

            int low = 1;
            int high = polygon.Length - 1;

            while (high - low > 1)
            {
                int mid = (low + high) / 2;
                FixVec pm = polygon[mid];
                if (Orientation(p0, pm, point) < 0)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }

            return IsTriangleContain(p0, polygon[low], polygon[high], point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Orientation(FixVec a, FixVec b, FixVec c)
        {
            return (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsTriangleContain(FixVec a, FixVec b, FixVec c, FixVec p)
        {
            float s0 = Orientation(a, b, p);
            float s1 = Orientation(b, c, p);
            float s2 = Orientation(c, a, p);

            return s0 >= 0 && s1 >= 0 && s2 >= 0;
        }
    }

}