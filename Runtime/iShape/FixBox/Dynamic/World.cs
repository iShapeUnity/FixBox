using System.Runtime.CompilerServices;
using iShape.FixBox.Collider;
using iShape.FixBox.Collision;
using iShape.FixBox.Store;
using iShape.FixFloat;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.FixBox.Dynamic {

    public struct World {
        
        private readonly WorldSettings Settings;
        public readonly Boundary FreezeBoundary;
        public FixVec Gravity;

        internal BodyStore bodyStore;
        public GridSpace LandGrid;

        public double TimeStep => (1L << Settings.TimeScale).ToDouble();
        private readonly int timeScale;
        private readonly int bodyTimeScale;
        private readonly int bodySteps;

        public World(Boundary boundary, WorldSettings settings, FixVec gravity, bool isDebug, Allocator allocator) {
            FreezeBoundary = new Boundary(boundary.Min - new FixVec(settings.FreezeMargin, settings.FreezeMargin), boundary.Max + new FixVec(settings.FreezeMargin, settings.FreezeMargin));
            Settings = settings;
            Gravity = gravity;
            
            bodyStore = new BodyStore(settings.LandCapacity, settings.PlayerCapacity, settings.BulletCapacity, allocator);
            LandGrid = new GridSpace(boundary, settings.GridSpaceFactor, allocator);
            
            timeScale = 10 - Settings.TimeScale;
            bodyTimeScale = 10 - math.min(Settings.TimeScale, Settings.BodyTimeScale);
            bodySteps = 1 << (Settings.TimeScale - Settings.BodyTimeScale);
        }

        public void Dispose() {
            bodyStore.Dispose();
            LandGrid.Dispose();
        }

        public void Iterate() {
            // update lands first

            var lands = bodyStore.landList.Items;
            for (int i = 0; i < lands.Length; ++i) {
                var body = lands[i];
                if (!body.Velocity.isZero) {

                    body.Iterate(timeScale);
                    lands[i] = body;
                    // TODO update landGrid
                }
            }

            // update players

            var players = bodyStore.playerList.Items;
            var bullets = bodyStore.bulletList.Items;

            for (int s = 0; s < bodySteps; ++s) {
                // update players

                for (int j = 0; j < players.Length; ++j) {
                    var player = players[j];

                    player.Iterate(bodyTimeScale, Gravity);

                    players[j] = player;

                    // collide with lands

                    var indexMask = LandGrid.Collide(player.Boundary);

                    while (indexMask.HasNext()) {
                        int i = indexMask.Next();
                        var land = lands[i];

                        if (ImpactSolver.CollideDynamicAndStatic(ref player, land)) {
                            players[j] = player;
                        }
                    }
                }
/*
                // update bullets

                for (int j = 0; j < bullets.Length; ++j) {
                    var bullet = bullets[j];

                    bullet.Iterate(bodyTimeScale, Gravity);

                    bullets[j] = bullet;

                    // TODO collide with land
                }

                // collide player vs player
*/
                if (Settings.IsPlayerVsPlayer && players.Length > 1) {
                    // TODO optimize
                    for (int j = 0; j < players.Length - 1; ++j) {
                        var playerA = players[j];
                        for (int i = j + 1; i < players.Length; ++i) {
                            var playerB = players[i];

                            // collide with player
                            if (ImpactSolver.CollideDynamicAndDynamic(ref playerA, ref playerB)) {
                                players[j] = playerA;
                                players[i] = playerB;
                            }
                        }
                    }
                }

                // collide player vs bullet
/*
                if (players.Length > 0 && bullets.Length > 0) {

                    // TODO optimize
                    for (int j = 0; j < players.Length; ++j) {
                        var player = players[j];
                        for (int i = 0; i < bullets.Length; ++i) {
                            var bullet = bullets[i];

                            // collide with player

                            this.Collide(new BodyHandler(j, player), new BodyHandler(i, bullet));
                        }
                    }
                }

                // collide bullet vs bullet

                if (Settings.IsBulletVsBullet && bullets.Length > 1) {
                    // TODO optimize
                    for (int j = 0; j < bullets.Length - 1; ++j) {
                        var btA = bullets[j];
                        for (int i = j + 1; i < bullets.Length; ++i) {
                            var btB = bullets[i];

                            // collide with player
                            this.Collide(new BodyHandler(j, btA), new BodyHandler(i, btB));
                        }
                    }
                }
*/
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Actor GetActor(BodyIndex bodyIndex) {
            return bodyStore.GetActor(bodyIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Actor GetActor(long id, BodyType bodyType) {
            return bodyStore.GetActor(new BodyIndex(id, -1, -1, bodyType));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActor(Actor actor) {
            bodyStore.SetActor(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BodyIndex AddBody(Body body) {
            var index = bodyStore.AddBody(body);

            var isLand = body.Type == BodyType.land && body.Shape.IsNotEmpty;
            if (isLand) {
                if (!bodyStore.IsLast(index)) {
                    LandGrid.AddPlace(index.Index);
                }
                LandGrid.Set(body.Boundary, index.Index);
            }

            return index;
        }

    }

}