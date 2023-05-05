using iShape.FixBox.Collider;
using iShape.FixBox.Component;
using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace iShape.FixBox.Simulation {

    public abstract class BaseScene: MonoBehaviour, ISerializationCallbackReceiver {
        
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

        protected World world;
        protected int currentTick;
        protected int targetTick;
        private JobHandle jobHandle;
        private bool isJobRun;
        private double timeStep;
        private double startTime;

        private void Awake() {
            var a = FixWidth >> 1;
            var b = FixHeight >> 1;
            var Boundary = new Boundary(new FixVec(-a, -b), new FixVec(a, b));

            world = new World(Boundary, Settings.Settings, new FixVec(FixGravityX, FixGravityY), IsDebug, Allocator.Persistent);
            timeStep = world.timeStep.ToDouble();
            DidWorldCreate();
        }

        private void Update() {
            if (isJobRun && jobHandle.IsCompleted) {
                CompleteJob();
            }
        }

        private void LateUpdate() {
            if (!isJobRun) {
                double gameTime = Time.timeAsDouble - startTime;
                targetTick = (int)(gameTime / timeStep + 0.5);
                if (targetTick > currentTick) {
                    StartJob();
                }
            }
        }
        
        private void OnDestroy() {
            if (isJobRun) {
                jobHandle.Complete();
            }
            world.Dispose();
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
        
        private void OnValidate() {
            if (Settings == null) {
                Settings = Resources.Load<FixBoxSettings>("FixBoxDefaultSettings");
            }
        }
        
        // Serialization

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
        
        // Job

        private void StartJob() {
            this.WillUpdate();
            jobHandle = ExecuteJob();
            isJobRun = true;
        }
        
        private void CompleteJob() {
            jobHandle.Complete();
            isJobRun = false;
            this.DidUpdate();
            currentTick = targetTick;
        }

        protected abstract void DidWorldCreate();

        protected abstract JobHandle ExecuteJob();

        protected abstract void WillUpdate();

        protected abstract void DidUpdate();

    }

}