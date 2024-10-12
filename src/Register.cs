using items;

namespace Scientist
{
    public static class Register
    {
        //AbstractObjectType, as well as the class as a whole, must be public and static
        public static AbstractPhysicalObject.AbstractObjectType ScientistIcon;
        public static AbstractPhysicalObject.AbstractObjectType SharpSpear;
        public static AbstractPhysicalObject.AbstractObjectType ConcentratedDangleFruit;
        public static AbstractPhysicalObject.AbstractObjectType PainlessFruit;
        public static AbstractPhysicalObject.AbstractObjectType ColorfulFruit;
        public static AbstractPhysicalObject.AbstractObjectType InflatableGlowingShield;
        public static AbstractPhysicalObject.AbstractObjectType AnesthesiaSpear;
        public static AbstractPhysicalObject.AbstractObjectType AnesthesiaNeedle;

        public static SSOracleBehavior.SubBehavior.SubBehavID MeetScientist;
        public static Conversation.ID Pebbles_Scientist;
        public static SSOracleBehavior.Action MeetScientist_Init;

        public static void RegisterValues()
        {
            Register.ScientistIcon = new AbstractPhysicalObject.AbstractObjectType("ScientistIcon", true);
            Register.SharpSpear = new AbstractPhysicalObject.AbstractObjectType("SharpSpear", true);
            Register.ConcentratedDangleFruit = new AbstractPhysicalObject.AbstractObjectType("ConcentratedDangleFruit", true);
            Register.PainlessFruit = new AbstractPhysicalObject.AbstractObjectType("PainlessFruit", true);
            Register.ColorfulFruit = new AbstractPhysicalObject.AbstractObjectType("ColorfulFruit", true);
            Register.InflatableGlowingShield = new AbstractPhysicalObject.AbstractObjectType("InflatableGlowingShield", true);
            Register.AnesthesiaSpear = new AbstractPhysicalObject.AbstractObjectType("AnesthesiaSpear", true);
            Register.AnesthesiaNeedle = new AbstractPhysicalObject.AbstractObjectType("AnesthesiaNeedle", true);

            Register.MeetScientist = new SSOracleBehavior.SubBehavior.SubBehavID("MeetScientist", true);
            Register.Pebbles_Scientist = new Conversation.ID("Pebbles_Scientist", true);
            Register.MeetScientist_Init = new SSOracleBehavior.Action("MeetScientist_Init", true);

        }

        public static void UnregisterValues()
        {
            AbstractPhysicalObject.AbstractObjectType scientistIcon = ScientistIcon;
            scientistIcon?.Unregister();
            ScientistIcon = null;

            AbstractPhysicalObject.AbstractObjectType sharpSpear = SharpSpear;
            sharpSpear?.Unregister();
            SharpSpear = null;

            AbstractPhysicalObject.AbstractObjectType concentratedDangleFruit = ConcentratedDangleFruit;
            concentratedDangleFruit?.Unregister();
            ConcentratedDangleFruit = null;

            AbstractPhysicalObject.AbstractObjectType painlessFruit = PainlessFruit;
            painlessFruit?.Unregister();
            PainlessFruit = null;

            AbstractPhysicalObject.AbstractObjectType colorfulFruit = ColorfulFruit;
            colorfulFruit?.Unregister();
            ColorfulFruit = null;

            AbstractPhysicalObject.AbstractObjectType inflatableGlowingShield = InflatableGlowingShield;
            inflatableGlowingShield?.Unregister();
            InflatableGlowingShield = null;

            AbstractPhysicalObject.AbstractObjectType anesthesiaSpear = AnesthesiaSpear;
            anesthesiaSpear?.Unregister();
            AnesthesiaSpear = null;

            AbstractPhysicalObject.AbstractObjectType anesthesiaNeedle = AnesthesiaNeedle;
            anesthesiaNeedle?.Unregister();
            AnesthesiaNeedle = null;


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
