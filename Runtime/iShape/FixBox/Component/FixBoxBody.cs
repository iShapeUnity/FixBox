using iShape.FixBox.Dynamic;
using iShape.FixBox.Store;
using UnityEngine;

namespace iShape.FixBox.Component {

    public class FixBoxBody: MonoBehaviour {

        public FixBoxMaterial Material;
        private WeakIndex index;

        private void Start() {
            var body = new Body();
            
            index = FixBoxWorld.Shared.World.AddBody(body);


        }

        private void Update() {
            
        }
    }

}