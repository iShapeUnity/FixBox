using iShape.FixBox.Collider;
using iShape.FixBox.Render;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxWorld: MonoBehaviour, ISerializationCallbackReceiver {

        public bool IsDebug = true;
        public bool IsDebugGridSpace = true;
        public FixBoxSettings Settings;

        public float2 Gravity = new(0, -10.0f);
        
        [Range(min:0, max: 100)]
        public float Width = 50.0f;
        
        [Range(min:0, max: 100)]
        public float Height = 50.0f;
        
        [Range(min:1, max: 10)]
        public float FreezeMargin = 5.0f;

        [HideInInspector]
        public long FixWidth = 50 * FixNumber.Unit;

        [HideInInspector]
        public long FixHeight = 50 * FixNumber.Unit;
        
        [HideInInspector]
        public long FixFreezeMargin = 5 * FixNumber.Unit;

        [HideInInspector]
        public long FixGravityX;
        
        [HideInInspector]
        public long FixGravityY = -10 * FixNumber.Unit;
        
        private FixBoxSimulator simulator;
        
        private void Awake() {
            var a = FixWidth >> 1;
            var b = FixHeight >> 1;
            var Boundary = new Boundary(new FixVec(-a, -b), new FixVec(a, b));

            simulator = new FixBoxSimulator(new World(Boundary, Settings.Settings, new FixVec(FixGravityX, FixGravityY), IsDebug, Allocator.Persistent));
        }

        private void Start() {
            simulator.Start();
        }

        private void Update() {
            simulator.Update();
            if (IsDebugGridSpace && FixBoxSimulator.Shared.isReady) {
                this.gameObject.DrawLandGrid(simulator.World.LandGrid);
            } else {
                this.gameObject.RemoveLandGrid();
            }
        }
        
        private void LateUpdate() {
            simulator.LateUpdate();
        }

        private void OnDestroy() {
            simulator.OnDestroy();
        }

        private void OnDrawGizmos() {
            Gizmos.matrix = transform.localToWorldMatrix;

            var a = FixWidth.ToFloat();
            var b = FixHeight.ToFloat();
            var m = 2 * FixFreezeMargin.ToFloat();

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(a, b, 0));

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(a + m, b + m, 0));
        }

        public void OnBeforeSerialize() {
            if (!Application.isPlaying) {
                FixWidth = Width.ToFix();
                FixHeight = Height.ToFix();
                FixFreezeMargin = FreezeMargin.ToFix();
                FixGravityX = Gravity.x.ToFix();
                FixGravityY = Gravity.y.ToFix();                
            }
        }

        public void OnAfterDeserialize() {}
        
#if UNITY_EDITOR
        private void OnValidate() {

            if (Settings == null) {
                Settings = Resources.Load<FixBoxSettings>("FixBoxDefaultSettings");
            }
        }
#endif
    }

}