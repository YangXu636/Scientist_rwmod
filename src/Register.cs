namespace Scientist
{
    public static class Register
    {
        //AbstractObjectType, as well as the class as a whole, must be public and static
        public static AbstractPhysicalObject.AbstractObjectType ShapeSpear;

        public static void RegisterValues()
        {
            ShapeSpear = new AbstractPhysicalObject.AbstractObjectType("ShapeSpear", true);
        }

        public static void UnregisterValues()
        {
            AbstractPhysicalObject.AbstractObjectType sharpSpear = ShapeSpear;
            sharpSpear?.Unregister();
            ShapeSpear = null;
        }
    }
}
