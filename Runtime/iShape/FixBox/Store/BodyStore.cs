using System.Runtime.CompilerServices;
using iShape.FixBox.Dynamic;
using Unity.Collections;

namespace iShape.FixBox.Store {

    public struct BodyStore {
        
        public SortedList landList;
        public SortedList playerList;
        public SortedList bulletList;

        public BodyStore(int landCapacity, int playerCapacity, int bulletCapacity, Allocator allocator) {
            landList = new SortedList(landCapacity, allocator);
            playerList = new SortedList(playerCapacity, allocator);
            bulletList = new SortedList(bulletCapacity, allocator);
        }

        public void Dispose() {
            landList.Dispose();
            playerList.Dispose();
            bulletList.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Actor GetActor(BodyIndex bodyIndex) {
            return bodyIndex.Type switch {
                BodyType.land => landList.Get(bodyIndex),
                BodyType.player => playerList.Get(bodyIndex),
                _ => bulletList.Get(bodyIndex)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BodyIndex SetActor(Actor actor) {
            return actor.Index.Type switch {
                BodyType.land => landList.Set(actor),
                BodyType.player => playerList.Set(actor),
                _ => bulletList.Set(actor)
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBody(BodyHandler handler) {
            switch (handler.Body.Type) {
                case BodyType.land:
                    landList.Items[handler.Index] = handler.Body;
                    break;
                case BodyType.player:
                    playerList.Items[handler.Index] = handler.Body;
                    break;
                default:
                    bulletList.Items[handler.Index] = handler.Body;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BodyIndex AddBody(Body body) {
            return body.Type switch {
                BodyType.player => playerList.Add(body),
                BodyType.bullet => bulletList.Add(body),
                _ => landList.Add(body)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveActor(BodyIndex index) {
            switch (index.Type) {
                case BodyType.land:
                    landList.Remove(index);
                    break;
                case BodyType.player:
                    playerList.Remove(index);
                    break;
                default:
                    bulletList.Remove(index);
                    break;
            }
        }
    }

}