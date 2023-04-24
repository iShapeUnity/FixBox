using System.Collections.Generic;
using iShape.FixBox.Collider;
using iShape.FixBox.Debug;
using iShape.FixBox.Dynamic;
using iShape.FixBox.Store;
using iShape.FixFloat;
using UnityEngine;
using UnityEngine.Assertions;

namespace iShape.FixBox.Component {

    public class FixBoxBody: MonoBehaviour, ISerializationCallbackReceiver {

        public long Id = -1;
        public BodyType BodyType;
        public FixBoxMaterial Material;
        private BodyIndex index;

        [SerializeField]
        private long fixX;
        [SerializeField]
        private long fixY;
        
        [SerializeField]
        private long fixAngle;
        
#if UNITY_EDITOR
        private void OnValidate() {
            if (!Application.isPlaying && Id == -1) {
                Id = GetFirstFreeId();
            }

            if (Material == null) {
                Material = Resources.Load<FixBoxMaterial>("FixBoxDefault");
            }
        }
#endif
        
        private void Start() {
            Assert.AreNotEqual(-1, Id, "Id is not set");
            var body = new Body(Id, BodyType);
            body.Transform = new Dynamic.Transform(new FixVec(fixX, fixY), fixAngle);

            var shape = this.GetShape();
            if (shape.IsNotEmpty) {
                body.Attach(shape);
            }
            
            index = FixBoxSimulator.Shared.World.AddBody(body);
            if (FixBoxSimulator.Shared.World.IsDebug) {
                this.gameObject.DrawCollider();           
            }
        }
        
        private Shape GetShape() {
            var circleCollider = gameObject.GetComponent<FixBoxCircleCollider>();
            if (circleCollider != null) {
                var radius = circleCollider.FixRadius;
                return new Shape(radius);
            }
            
            var rectCollider = gameObject.GetComponent<FixBoxRectCollider>();
            if (rectCollider != null) {
                var width = rectCollider.FixWidth;
                var height = rectCollider.FixHeight;
                return new Shape(new Size(width, height));
            }

            return Shape.Empty;
        }

        private void Update() {
            if (!FixBoxSimulator.Shared.isReady) {
                return;
            }

            var actor = FixBoxSimulator.Shared.World.GetActor(index);
            index = actor.Index;
            var bodyTr = actor.Body.Transform;
            var pos = bodyTr.Position.ToFloat2();
            var angle = bodyTr.Angle.ToFloat();

            this.transform.position = new Vector3(pos.x, pos.y, 0);
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        private static long GetFirstFreeId() {
            var fixBoxBodies = FindObjectsOfType<FixBoxBody>();
            var fixIdList = new List<FixBoxBody>();
            
            var idSet = new HashSet<long>();
            foreach (FixBoxBody fixBoxBody in fixBoxBodies) {
                var id = fixBoxBody.Id;
                if (id < 0) {
                    fixIdList.Add(fixBoxBody);
                } else {
                    if (idSet.Contains(fixBoxBody.Id)) {
                        fixIdList.Add(fixBoxBody);
                    } else {
                        idSet.Add(id);
                    }
                }
            }
            
            long targetId = NextFreeId(idSet, 0);
            
            // fix others bodies
            if (fixIdList.Count > 0) {
                var id = targetId + 1;
                foreach (FixBoxBody fixBoxBody in fixIdList) {
                    id = NextFreeId(idSet, id);
                    fixBoxBody.Id = id;
                    UnityEngine.Debug.LogWarning("Body " + fixBoxBody.gameObject.name + " id was updated to " + id);
                    id += 1;
                }
            }

            return targetId;
        }

        private static long NextFreeId(HashSet<long> idSet, long lastId) {
            var id = lastId;
            while (idSet.Contains(id)) {
                id++;
            }
            return id;
        }
        
        public void OnBeforeSerialize() {
            var t = transform;
            var pos = t.position;
            fixX = pos.x.ToFix();
            fixY = pos.y.ToFix();
            fixAngle = t.rotation.eulerAngles.z.ToFix();
        }

        public void OnAfterDeserialize() { }
    }
}