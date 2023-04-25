using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using iShape.FixBox.Dynamic;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxSimulator {

        public static FixBoxSimulator Shared { get; private set; }

        private JobHandle jobHandle;
        public World World;
        public bool isReady => !isRun;
        
        private double startTime;
        private int currentTick;
        private int jobEndTick;
        private readonly double TimeStep;
        private bool isRun;

        public FixBoxSimulator(World world) {
            World = world;
            Shared = this;
            TimeStep = world.TimeStep;
        }

        public void Start() {
            currentTick = 0;
            startTime = Time.timeAsDouble;
            isRun = false;
        }

        public void Update() {
            if (isRun && jobHandle.IsCompleted) {
                StopJob();
            }
        }

        public void LateUpdate() {
            if (!isRun) {
                double gameTime = Time.timeAsDouble - startTime;
                int expectedTick = (int)(gameTime / TimeStep + 0.5);
                int dTick = expectedTick - currentTick;
                if (dTick > 0) {
                    jobEndTick = expectedTick;
                    StartJob(dTick);
                }
            }
        }

        private void StartJob(int count) {
            var iterateJob = new IterateWorld {
                count = count,
                World = World
            }; 
            jobHandle = iterateJob.Schedule();
            isRun = true;
        }
        
        private void StopJob() {
            jobHandle.Complete();
            isRun = false;
            currentTick = jobEndTick;
            // UnityEngine.Debug.Log("End simulation " + jobEndTick);
        }

        public void OnDestroy() {
            jobHandle.Complete();
            isRun = false;
            World.Dispose();
        }
    }
    
    [BurstCompile]
    public struct IterateWorld: IJob {
        
        [ReadOnly]
        public int count;

        public World World;

        public void Execute() {
            for (int i = 0; i < count; ++i) {
                World.Iterate();                
            }
        }
    }

}