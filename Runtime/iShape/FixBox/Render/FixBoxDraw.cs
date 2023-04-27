using iShape.FixBox.Collision;
using iShape.FixBox.Component;
using iShape.FixFloat;
using UnityEngine;
using iShape.Mesh2d;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.FixBox.Render {

    public static class FixBoxDraw {

        private static readonly Color StrokeColor = new Color(0.2f, 0.6f, 0.2f, 1);
        private static readonly Color FillColor = new Color(0.2f, 0.6f, 0.2f, 0.25f);
        private static readonly Color GridSpaceColor = new Color(1.0f, 0.3f, 0.3f, 1f);
        private static readonly StrokeStyle StrokeStyle = new StrokeStyle(0.05f);

        private const string DebugNode = "FixDebug"; 
        
        public static void DrawDebugCollider(this GameObject gameObject) {
            var circleCollider = gameObject.GetComponent<FixBoxCircleCollider>();
            if (circleCollider != null) {
                var radius = circleCollider.FixRadius;
                gameObject.DrawCircle(radius);
                return;
            }
            var rectCollider = gameObject.GetComponent<FixBoxRectCollider>();
            if (rectCollider != null) {
                var width = rectCollider.FixWidth;
                var height = rectCollider.FixHeight;
                gameObject.DrawRect(width, height);
            }
        }

        private static void DrawCircle(this GameObject gameObject, long radius) {
            var colorMesh = new NativeColorMesh(128, Allocator.Temp);

            float r = radius.ToFloat();
            
            var circleStroke = MeshGenerator.StrokeForCircle(float2.zero, r, 32, StrokeStyle, -0.1f, Allocator.Temp);
            colorMesh.AddAndDispose(circleStroke, StrokeColor);
            var radiusStroke = MeshGenerator.StrokeForEdge(float2.zero, new float2(r, 0), StrokeStyle, -0.1f, Allocator.Temp);
            colorMesh.AddAndDispose(radiusStroke, StrokeColor);
            var circleFill = MeshGenerator.FillCircle(float2.zero, r, 32, true, 0, Allocator.Temp);
            colorMesh.AddAndDispose(circleFill, FillColor);

            MeshFilter meshFilter = gameObject.GetDebugChild();
            meshFilter.mesh = colorMesh.Convert();
        }

        private static void DrawRect(this GameObject gameObject, long width, long height) {
            var colorMesh = new NativeColorMesh(128, Allocator.Temp);
            
            float w = width.ToFloat();
            float h = height.ToFloat();
            float2 size = new float2(w, h);
            
            var rectStroke = MeshGenerator.StrokeForRect(float2.zero, size, StrokeStyle, -0.1f, Allocator.Temp);
            colorMesh.AddAndDispose(rectStroke, StrokeColor);
            var rectFill = MeshGenerator.Rect(float2.zero, size, 0, Allocator.Temp);
            colorMesh.AddAndDispose(rectFill, FillColor);

            MeshFilter meshFilter = gameObject.GetDebugChild();
            meshFilter.mesh = colorMesh.Convert();
        }


        private static MeshFilter GetDebugChild(this GameObject gameObject) {
            var debugTransform = gameObject.transform.Find(DebugNode);

            if (debugTransform != null) {
                return debugTransform.GetComponent<MeshFilter>();
            } else {
                var debugObject = new GameObject(DebugNode);
                debugObject.transform.parent = gameObject.transform;
                debugObject.transform.localPosition = Vector3.zero;
                debugObject.transform.localRotation = Quaternion.identity;
                
                MeshFilter meshFilter = debugObject.AddComponent<MeshFilter>();
                
                MeshRenderer meshRenderer = debugObject.AddComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
                meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                
                meshRenderer.material = Resources.Load<Material>("FixBoxDebug");

                return meshFilter;                
            }
        }

        public static void DrawLandGrid(this GameObject gameObject, GridSpace space) {
            MeshFilter meshFilter = gameObject.GetDebugChild();
            if (meshFilter.mesh == null) {
                meshFilter.mesh = new Mesh();
            }

            var colorMesh = new NativeColorMesh(4 * space.Cells.Length, Allocator.Temp);
            var cellSize = space.CellSize;

            var ds = cellSize;

            var n = space.RowCellCount;
            var a = 0.5f * n;

            
            for (int y = 0; y < n; ++y) {
                for (int x = 0; x < n; ++x) {
                    int count = space[x, y].Count();
                    if (count > 0) {
                        var cx = (x - a + 0.5f) * ds.x;
                        var cy = (y - a + 0.5f) * ds.y;

                        var c = space[x, y].Color();

                        var color = new Color(c.x, c.y, c.z, c.w);
                        
                        var rectFill = MeshGenerator.Rect(new float2(cx, cy), cellSize, 0.1f, Allocator.Temp);
                        colorMesh.AddAndDispose(rectFill, color);
                    }
                }   
            }
            
            meshFilter.mesh.SetAndDispose(colorMesh);
        }
        
        public static void RemoveLandGrid(this GameObject gameObject) {
            MeshFilter meshFilter = gameObject.GetDebugChild();
            if (meshFilter.mesh != null) {
                meshFilter.mesh.Clear();
            }
        }
    }

}