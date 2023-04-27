using iShape.FixBox.Dynamic;
using Unity.Burst;
using Unity.Jobs;

namespace iShape.FixBox.Simulation {

    [BurstCompile]
    public struct SimpleJob: IJob {

        private readonly int startTick;
        private readonly int endTick;
        private World world;

        public SimpleJob(int startTick, int endTick, World world) {
            this.startTick = startTick;
            this.endTick = endTick;
            this.world = world;
        }

        public void Execute() {
            for (int i = startTick; i < endTick; ++i) {
                this.willIterate(i);   
                world.Iterate();
                this.didIterate(i);
            }
        }
        
        // Implement here some physic logic like blast, magnetic field and so on
        private void willIterate(int tick) {
            
        }

        private void didIterate(int tick) {
            
        }
    }

}