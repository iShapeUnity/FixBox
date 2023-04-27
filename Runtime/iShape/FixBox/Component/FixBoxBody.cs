using System.Collections.Generic;
using iShape.FixBox.Collider;
using iShape.FixBox.Dynamic;
using iShape.FixBox.Store;
using iShape.FixFloat;
using Unity.Mathematics;
using UnityEngine;

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
        
        private void OnValidate() {
            if (!Application.isPlaying && Id == -1) {
                Id = GetFirstFreeId();
            }

            if (Material == null) {
                Material = Resources.Load<FixBoxMaterial>("FixBoxDefaultMaterial");
            }
        }

        private Body CreateBody() {
            var body = new Body(Id, BodyType, Material.Material);
            body.Transform = new Dynamic.Transform(new FixVec(fixX, fixY), fixAngle);

            var shape = this.GetShape();
            if (shape.IsNotEmpty) {
                body.Attach(shape);
            }

            return body;
        }

        public void FixBoxCreate(World world) {
            var body = this.CreateBody();
            index = world.AddBody(body);
        }

        public void FixBoxUpdate(World world) {
            var actor = world.GetActor(index);
            index = actor.Index;
            var bodyTr = actor.Body.Transform;
            var pos = bodyTr.Position.ToFloat2();
            var rad = bodyTr.Angle.ToFloat();
            var degrees = math.degrees(rad);
        
            this.transform.position = new Vector3(pos.x, pos.y, 0);
            this.transform.rotation = Quaternion.AngleAxis(degrees, Vector3.forward);
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
                    Debug.LogWarning("Body " + fixBoxBody.gameObject.name + " id was updated to " + id);
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

            float angleInDegrees = t.rotation.eulerAngles.z;
            float angleInRad = math.radians(angleInDegrees);
            fixAngle = angleInRad.ToFix();
        }

        public void OnAfterDeserialize() { }
    }
}