using items;

namespace Scientist
{
    public static class Register
    {
        //AbstractObjectType, as well as the class as a whole, must be public and static
        public static AbstractPhysicalObject.AbstractObjectType ShapeSpear;
        public static AbstractPhysicalObject.AbstractObjectType ConcentratedDangleFruit;

        public static void RegisterValues()
        {
            Register.ShapeSpear = new AbstractPhysicalObject.AbstractObjectType("ShapeSpear", true);
            Register.ConcentratedDangleFruit = new AbstractPhysicalObject.AbstractObjectType("ConcentratedDangleFruit", true);
        }

        public static void UnregisterValues()
        {
            AbstractPhysicalObject.AbstractObjectType sharpSpear = ShapeSpear;
            sharpSpear?.Unregister();
            ShapeSpear = null;

            AbstractPhysicalObject.AbstractObjectType concentratedDangleFruit = ConcentratedDangleFruit;
            concentratedDangleFruit?.Unregister();
            ConcentratedDangleFruit = null;
        }
    }
}
