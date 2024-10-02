using items;

namespace Scientist
{
    public static class Register
    {
        //AbstractObjectType, as well as the class as a whole, must be public and static
        public static AbstractPhysicalObject.AbstractObjectType ShapeSpear;
        public static AbstractPhysicalObject.AbstractObjectType ConcentratedDangleFruit;
        public static AbstractPhysicalObject.AbstractObjectType PainlessFruit;

        public static SSOracleBehavior.SubBehavior.SubBehavID MeetScientist;
        public static Conversation.ID Pebbles_Scientist;
        public static SSOracleBehavior.Action MeetScientist_Init;

        public static void RegisterValues()
        {
            Register.ShapeSpear = new AbstractPhysicalObject.AbstractObjectType("ShapeSpear", true);
            Register.ConcentratedDangleFruit = new AbstractPhysicalObject.AbstractObjectType("ConcentratedDangleFruit", true);
            Register.PainlessFruit = new AbstractPhysicalObject.AbstractObjectType("PainlessFruit", true);

            Register.MeetScientist = new SSOracleBehavior.SubBehavior.SubBehavID("MeetScientist", true);
            Register.Pebbles_Scientist = new Conversation.ID("Pebbles_Scientist", true);
            Register.MeetScientist_Init = new SSOracleBehavior.Action("MeetScientist_Init", true);

        }

        public static void UnregisterValues()
        {
            AbstractPhysicalObject.AbstractObjectType sharpSpear = ShapeSpear;
            sharpSpear?.Unregister();
            ShapeSpear = null;

            AbstractPhysicalObject.AbstractObjectType concentratedDangleFruit = ConcentratedDangleFruit;
            concentratedDangleFruit?.Unregister();
            ConcentratedDangleFruit = null;

            AbstractPhysicalObject.AbstractObjectType painlessFruit = PainlessFruit;
            painlessFruit?.Unregister();
            PainlessFruit = null;


            SSOracleBehavior.SubBehavior.SubBehavID meetScientist = MeetScientist;
            meetScientist?.Unregister();
            MeetScientist = null;

            Conversation.ID pebbles_Scientist = Pebbles_Scientist;
            pebbles_Scientist?.Unregister();
            Pebbles_Scientist = null;

            SSOracleBehavior.Action meetScientist_Init = MeetScientist_Init;
            meetScientist_Init?.Unregister();
            MeetScientist_Init = null;
        }
    }
}
