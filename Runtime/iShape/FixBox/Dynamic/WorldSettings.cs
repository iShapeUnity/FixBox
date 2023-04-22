namespace iShape.FixBox.Dynamic {

    public readonly struct WorldSettings {

        public readonly int TimeScale;
        public readonly int BodyTimeScale;
        public readonly bool IsBulletVsBullet;
        public readonly bool IsPlayerVsPlayer;
        public readonly int LandCapacity;
        public readonly int PlayerCapacity;
        public readonly int BulletCapacity;
        public readonly int GridSpaceFactor;
        public readonly long FreezeMargin;
        

        public WorldSettings(
            int timeScale = 4,
            int bodyTimeScale = 2,
            bool isBulletVsBullet = false,
            bool isPlayerVsPlayer = false,
            int landCapacity = 64,
            int playerCapacity = 32,
            int bulletCapacity = 64,
            int gridSpaceFactor = 4,
            long freezeMargin = 10
            ) {
            TimeScale = timeScale;
            BodyTimeScale = bodyTimeScale;
            IsBulletVsBullet = isBulletVsBullet;
            IsPlayerVsPlayer = isPlayerVsPlayer;
            LandCapacity = landCapacity;
            PlayerCapacity = playerCapacity;
            BulletCapacity = bulletCapacity;
            GridSpaceFactor = gridSpaceFactor;
            FreezeMargin = freezeMargin;
        }

    }

}