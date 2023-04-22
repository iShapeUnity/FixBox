namespace iShape.FixBox.Dynamic {

    public enum BodyType {
        land = 0,
        player = 1,
        bullet = 2
    }
    
    public static class BodyTypeExtensions {
        public static int Index(this BodyType type) {
            return (int)type;
        }
    }

}