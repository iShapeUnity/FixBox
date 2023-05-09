using System.Collections.Generic;
using System.Linq;
using iShape.FixBox.Component;
using iShape.FixBox.Render;
using Unity.Jobs;

namespace iShape.FixBox.Simulation {

    public class SimpleScene: BaseScene {

        protected List<FixBoxBody> fixBodies; 

        protected override void WillWorldCreate() { }

        protected override void DidWorldCreate() {
            var objs = FindObjectsOfType<FixBoxBody>();
            fixBodies = objs.OrderBy(fb => fb.Id).ToList();
            for (int i = 0; i < fixBodies.Count; ++i) {
                var fixBody = fixBodies[i];
                fixBody.FixBoxCreate(world);
                if (IsDebug) {
                    fixBody.gameObject.DrawDebugCollider();
                }
            }
        }

        protected override JobHandle ExecuteJob() {
            var job = new SimpleJob(currentTick, targetTick, world); 
            return job.Schedule();
        }

        protected override void WillUpdate() {

        }

        protected override void DidUpdate() {
            for (int i = 0; i < fixBodies.Count; ++i) {
                fixBodies[i].FixBoxUpdate(world); 
            }         
        }
    }

}