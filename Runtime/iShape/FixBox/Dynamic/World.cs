using System.Runtime.CompilerServices;
using iShape.FixBox.Collider;
using iShape.FixBox.Collision;
using iShape.FixBox.Store;
using iShape.FixFloat;
using Unity.Collections;

namespace iShape.FixBox.Dynamic {

    public struct World {
        public readonly WorldSettings Settings;
        public readonly Boundary CollisionBoundary;
        public readonly Boundary FreezeBoundary;
        public readonly bool IsDebug;
        public long Tick { get; private set; }

        internal BodyStore bodyStore;
        internal GridSpace landGrid;
        
        public World(Boundary boundary, WorldSettings settings, bool isDebug, Allocator allocator) {
            CollisionBoundary = boundary;
            FreezeBoundary = new Boundary(boundary.Min - new FixVec(settings.FreezeMargin, settings.FreezeMargin), boundary.Max + new FixVec(settings.FreezeMargin, settings.FreezeMargin));
            Settings = settings;
            IsDebug = isDebug;

            Tick = 0;

            
            bodyStore = new BodyStore(settings.LandCapacity, settings.PlayerCapacity, settings.BulletCapacity, allocator);
            landGrid = new GridSpace(settings.GridSpaceFactor, allocator);
        }

        public void Dispose() {
            bodyStore.Dispose();
            landGrid.Dispose();
        }

        public void Iterate() {
            var timeScale = 10 - Settings.TimeScale;

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

            var bodyTimeScale = 10 - Settings.BodyTimeScale;
            var bodySteps = 1 << (Settings.TimeScale - Settings.BodyTimeScale);

            var players = bodyStore.playerList.Items;
            var bullets = bodyStore.bulletList.Items;

            for (int s = 0; s < bodySteps; ++s) {
                // update players

                for (int j = 0; j < players.Length; ++j) {

                    var player = players[j];

                    player.Iterate(bodyTimeScale);

                    players[j] = player;

                    // collide with lands

                    var indexMask = landGrid.Collide(player.Boundary);

                    while (indexMask.HasNext()) {
                        int i = indexMask.Next();
                        var land = lands[i];

                        this.Collide(new BodyHandler(j, player), new BodyHandler(i, land));
                    }
                }

                // update bullets

                for (int j = 0; j < players.Length; ++j) {
                    var bullet = bullets[j];

                    bullet.Iterate(bodyTimeScale);

                    bullets[j] = bullet;

                    // TODO collide with land
                }

                // collide player vs player

                if (Settings.IsPlayerVsPlayer && players.Length > 1) {
                    // TODO optimize
                    for (int j = 0; j < players.Length - 1; ++j) {
                        var plA = players[j];
                        for (int i = j + 1; i < players.Length; ++i) {
                            var plB = players[i];

                            // collide with player

                            this.Collide(new BodyHandler(j, plA), new BodyHandler(i, plB));
                        }
                    }
                }

                // collide player vs bullet

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
            }
            
            Tick += 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Actor GetActor(WeakIndex weakIndex) {
            return bodyStore.GetActor(weakIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActor(Actor actor) {
            bodyStore.SetActor(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WeakIndex AddBody(Body body) {
            return bodyStore.AddBody(body);
        }
    }

}