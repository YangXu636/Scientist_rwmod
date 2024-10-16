using items;

namespace Scientist.ScientistEnums
{

    public static class ScientistEnums
    {
        public static void RegisterValues()
        {
            Items.RegisterValues();
            SubBehavID_Fp.RegisterValues();
            Action_Fp.RegisterValues();
            ConversationID_Fp.RegisterValues();
        }

        public static void UnregisterValues()
        {
            Items.UnregisterValues();
            SubBehavID_Fp.UnregisterValues();
            Action_Fp.UnregisterValues();
            ConversationID_Fp.UnregisterValues();
        }
    }

    public static class Items
    {
        //public static AbstractPhysicalObject.AbstractObjectType ScientistIcon;
        public static AbstractPhysicalObject.AbstractObjectType SharpSpear;
        public static AbstractPhysicalObject.AbstractObjectType ConcentratedDangleFruit;
        public static AbstractPhysicalObject.AbstractObjectType PainlessFruit;
        public static AbstractPhysicalObject.AbstractObjectType ColorfulFruit;
        public static AbstractPhysicalObject.AbstractObjectType InflatableGlowingShield;
        public static AbstractPhysicalObject.AbstractObjectType AnesthesiaSpear;
        public static AbstractPhysicalObject.AbstractObjectType AnesthesiaNeedle;
        public static AbstractPhysicalObject.AbstractObjectType SmallRock;

        public static void RegisterValues()
        {
            //ScientistIcon = new AbstractPhysicalObject.AbstractObjectType("ScientistIcon", true);
            SharpSpear = new AbstractPhysicalObject.AbstractObjectType("SharpSpear", true);
            ConcentratedDangleFruit = new AbstractPhysicalObject.AbstractObjectType("ConcentratedDangleFruit", true);
            PainlessFruit = new AbstractPhysicalObject.AbstractObjectType("PainlessFruit", true);
            ColorfulFruit = new AbstractPhysicalObject.AbstractObjectType("ColorfulFruit", true);
            InflatableGlowingShield = new AbstractPhysicalObject.AbstractObjectType("InflatableGlowingShield", true);
            AnesthesiaSpear = new AbstractPhysicalObject.AbstractObjectType("AnesthesiaSpear", true);
            AnesthesiaNeedle = new AbstractPhysicalObject.AbstractObjectType("AnesthesiaNeedle", true);
            SmallRock = new AbstractPhysicalObject.AbstractObjectType("SmallRock", true);
        }

        public static void UnregisterValues()
        {
            /*AbstractPhysicalObject.AbstractObjectType scientistIcon = ScientistIcon;
            scientistIcon?.Unregister();
            ScientistIcon = null;*/

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

            AbstractPhysicalObject.AbstractObjectType smallRock = SmallRock;
            smallRock?.Unregister();
            SmallRock = null;
        }
    }

    public static class SubBehavID_Fp
    {
        public static SSOracleBehavior.SubBehavior.SubBehavID FirstMeetScientist;
        public static SSOracleBehavior.SubBehavior.SubBehavID ThrowOutScientist;

        public static void RegisterValues()
        {
            FirstMeetScientist = new SSOracleBehavior.SubBehavior.SubBehavID("FirstMeetScientist", true);
            ThrowOutScientist = new SSOracleBehavior.SubBehavior.SubBehavID("ThrowOutScientist", true);
        }

        public static void UnregisterValues()
        {
            SSOracleBehavior.SubBehavior.SubBehavID meetScientist = FirstMeetScientist;
            meetScientist?.Unregister();
            FirstMeetScientist = null;

            SSOracleBehavior.SubBehavior.SubBehavID throwOutScientist = ThrowOutScientist;
            throwOutScientist?.Unregister();
            ThrowOutScientist = null;
        }
    }

    public static class Action_Fp
    {
        public static SSOracleBehavior.Action MeetScientist_Init;
        public static SSOracleBehavior.Action MeetScientist_Talk_FirstMeet;
        public static SSOracleBehavior.Action MeetScientist_SeeObject;
        public static SSOracleBehavior.Action MeetScientist_ThrowOut_First;
        public static SSOracleBehavior.Action MeetScientist_ThrowOut_Second;

        public static void RegisterValues()
        {
            MeetScientist_Init = new SSOracleBehavior.Action("MeetScientist_Init", true);
            MeetScientist_Talk_FirstMeet = new SSOracleBehavior.Action("MeetScientist_Talk_FirstMeet", true);
            MeetScientist_SeeObject = new SSOracleBehavior.Action("MeetScientist_SeeObject", true);
            MeetScientist_ThrowOut_First = new SSOracleBehavior.Action("MeetScientist_ThrowOut_First", true);
            MeetScientist_ThrowOut_Second = new SSOracleBehavior.Action("MeetScientist_ThrowOut_Second", true);
        }

        public static void UnregisterValues()
        {
            SSOracleBehavior.Action meetScientist_Init = MeetScientist_Init;
            meetScientist_Init?.Unregister();
            MeetScientist_Init = null;

            SSOracleBehavior.Action meetScientist_Talk_FirstMeet = MeetScientist_Talk_FirstMeet;
            meetScientist_Talk_FirstMeet?.Unregister();
            MeetScientist_Talk_FirstMeet = null;

            SSOracleBehavior.Action meetScientist_SeeObject = MeetScientist_SeeObject;
            meetScientist_SeeObject?.Unregister();
            MeetScientist_SeeObject = null;

            SSOracleBehavior.Action meetScientist_ThrowOut_First = MeetScientist_ThrowOut_First;
            meetScientist_ThrowOut_First?.Unregister();
            MeetScientist_ThrowOut_First = null;

            SSOracleBehavior.Action meetScientist_ThrowOut_Second = MeetScientist_ThrowOut_Second;
            meetScientist_ThrowOut_Second?.Unregister();
            MeetScientist_ThrowOut_Second = null;
        }
    }

    public static class ConversationID_Fp
    {
        public static Conversation.ID Pebbles_Scientist_Meet_First;
        public static Conversation.ID Pebbles_Scientist_ThrowOut;

        public static void RegisterValues()
        {
            Pebbles_Scientist_Meet_First = new Conversation.ID("Pebbles_Scientist_Meet_First", true);
            Pebbles_Scientist_ThrowOut = new Conversation.ID("Pebbles_Scientist_ThrowOut", true);
        }

        public static void UnregisterValues()
        {
            Conversation.ID pebbles_Scientist = Pebbles_Scientist_Meet_First;
            pebbles_Scientist?.Unregister();
            Pebbles_Scientist_Meet_First = null;

            Conversation.ID pebbles_Scientist_ThrowOut = Pebbles_Scientist_ThrowOut;
            pebbles_Scientist_ThrowOut?.Unregister();
            Pebbles_Scientist_ThrowOut = null;
        }
    }
}

