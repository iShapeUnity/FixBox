using iShape.FixBox.Dynamic;
using iShape.FixFloat;
using UnityEngine;

namespace iShape.FixBox.Component {

    [CreateAssetMenu(fileName = "FixBoxSettings", menuName = "FixBox/Settings")]
    public class FixBoxSettings: ScriptableObject {
        
        [Range(min: 2, max: 8)]
        public int TimeScale = 4;
        [Range(min: 1, max: 4)]
        public int BodyTimeScale = 2;
        public bool IsBulletVsBullet = false;
        public bool IsPlayerVsPlayer = false;
        [Range(min: 16, max: 64)]
        public int LandCapacity = 64;
        [Range(min: 1, max: 64)]
        public int PlayerCapacity = 32;
        [Range(min: 1, max: 64)]
        public int BulletCapacity = 64;
        [Range(min: 3, max: 6)]
        public int GridSpaceFactor = 4;
        [Range(min: 1, max: 10)]
        public float FreezeMargin;
        
        public WorldSettings Settings => new WorldSettings(
            TimeScale,
            BodyTimeScale,
            IsBulletVsBullet,
            IsPlayerVsPlayer,
            LandCapacity,
            PlayerCapacity,
            BulletCapacity,
            GridSpaceFactor,
            FreezeMargin.ToFix()
            );
    }

}