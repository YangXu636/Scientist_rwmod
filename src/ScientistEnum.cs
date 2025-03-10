using Scientist.Items;

namespace Scientist.Enums
{

    public static class ScientistEnum
    {
        public static void RegisterValues()
        {
            Items.RegisterValues();
            SandboxUnlockID.RegisterValues();
            SubBehavID_Fp.RegisterValues();
            Action_Fp.RegisterValues();
            ConversationID_Fp.RegisterValues();
        }

        public static void UnregisterValues()
        {
            Items.UnregisterValues();
            SandboxUnlockID.UnregisterValues();
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
        public static AbstractPhysicalObject.AbstractObjectType Knot;
        public static AbstractPhysicalObject.AbstractObjectType StoneKnife;
        public static AbstractPhysicalObject.AbstractObjectType ExplosivePowder;
        public static AbstractPhysicalObject.AbstractObjectType ExpansionBomb;
        public static AbstractPhysicalObject.AbstractObjectType BreathingBubble;
        public static AbstractPhysicalObject.AbstractObjectType KongmingLantern;
        public static AbstractPhysicalObject.AbstractObjectType TremblingFruit;

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
            Knot = new AbstractPhysicalObject.AbstractObjectType("Knot", true);
            StoneKnife = new AbstractPhysicalObject.AbstractObjectType("StoneKnife", true);
            ExplosivePowder = new AbstractPhysicalObject.AbstractObjectType("ExplosivePowder", true);
            ExpansionBomb = new AbstractPhysicalObject.AbstractObjectType("ExpansionBomb", true);
            BreathingBubble = new AbstractPhysicalObject.AbstractObjectType("BreathingBubble", true);
            KongmingLantern = new AbstractPhysicalObject.AbstractObjectType("KongmingLantern", true);
            TremblingFruit = new AbstractPhysicalObject.AbstractObjectType("TremblingFruit", true);
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

            AbstractPhysicalObject.AbstractObjectType knot = Knot;
            knot?.Unregister();
            Knot = null;

            AbstractPhysicalObject.AbstractObjectType stoneKnife = StoneKnife;
            stoneKnife?.Unregister();
            StoneKnife = null;

            AbstractPhysicalObject.AbstractObjectType explosivePowder = ExplosivePowder;
            explosivePowder?.Unregister();
            ExplosivePowder = null;

            AbstractPhysicalObject.AbstractObjectType expansionBomb = ExpansionBomb;
            expansionBomb?.Unregister();
            ExpansionBomb = null;

            AbstractPhysicalObject.AbstractObjectType breathingBubble = BreathingBubble;
            breathingBubble?.Unregister();
            BreathingBubble = null;

            AbstractPhysicalObject.AbstractObjectType kongmingLantern = KongmingLantern;
            kongmingLantern?.Unregister();
            KongmingLantern = null;

            AbstractPhysicalObject.AbstractObjectType tremblingFruit = TremblingFruit;
            tremblingFruit?.Unregister();
            TremblingFruit = null;
        }
    }

    public class SandboxUnlockID
    {
        public static MultiplayerUnlocks.SandboxUnlockID SharpSpear;
        public static MultiplayerUnlocks.SandboxUnlockID ConcentratedDangleFruit;
        public static MultiplayerUnlocks.SandboxUnlockID PainlessFruit;
        public static MultiplayerUnlocks.SandboxUnlockID ColorfulFruit;
        public static MultiplayerUnlocks.SandboxUnlockID InflatableGlowingShield;
        public static MultiplayerUnlocks.SandboxUnlockID AnesthesiaSpear;
        public static MultiplayerUnlocks.SandboxUnlockID AnesthesiaNeedle;
        public static MultiplayerUnlocks.SandboxUnlockID SmallRock;
        public static MultiplayerUnlocks.SandboxUnlockID Knot;
        public static MultiplayerUnlocks.SandboxUnlockID StoneKnife;
        public static MultiplayerUnlocks.SandboxUnlockID ExplosivePowder;
        public static MultiplayerUnlocks.SandboxUnlockID ExpansionBomb;
        public static MultiplayerUnlocks.SandboxUnlockID BreathingBubble;
        public static MultiplayerUnlocks.SandboxUnlockID KongmingLantern;
        public static MultiplayerUnlocks.SandboxUnlockID TremblingFruit;

        public static void RegisterValues()
        {
            SharpSpear = new MultiplayerUnlocks.SandboxUnlockID("SharpSpear", true);
            ConcentratedDangleFruit = new MultiplayerUnlocks.SandboxUnlockID("ConcentratedDangleFruit", true);
            PainlessFruit = new MultiplayerUnlocks.SandboxUnlockID("PainlessFruit", true);
            ColorfulFruit = new MultiplayerUnlocks.SandboxUnlockID("PainlessFruit", true);
            InflatableGlowingShield = new MultiplayerUnlocks.SandboxUnlockID("InflatableGlowingShield", true);
            AnesthesiaSpear = new MultiplayerUnlocks.SandboxUnlockID("AnesthesiaSpear", true);
            AnesthesiaNeedle = new MultiplayerUnlocks.SandboxUnlockID("AnesthesiaNeedle", true); 
            SmallRock = new MultiplayerUnlocks.SandboxUnlockID("SmallRock", true);
            Knot = new MultiplayerUnlocks.SandboxUnlockID("Knot", true);
            StoneKnife = new MultiplayerUnlocks.SandboxUnlockID("StoneKnife", true);
            ExplosivePowder = new MultiplayerUnlocks.SandboxUnlockID("ExplosivePowder", true);
            ExpansionBomb = new MultiplayerUnlocks.SandboxUnlockID("ExpansionBomb", true);
            BreathingBubble = new MultiplayerUnlocks.SandboxUnlockID("BreathingBubble", true);
            KongmingLantern = new MultiplayerUnlocks.SandboxUnlockID("KongmingLantern", true);
            TremblingFruit = new MultiplayerUnlocks.SandboxUnlockID("TremblingFruit", true);
        }

        // Token: 0x06004043 RID: 16451 RVA: 0x0047A3D0 File Offset: 0x004785D0
        public static void UnregisterValues()
        {
            MultiplayerUnlocks.SandboxUnlockID sharpSpear = SharpSpear;
            sharpSpear?.Unregister();
            SharpSpear = null;

            MultiplayerUnlocks.SandboxUnlockID concentratedDangleFruit = ConcentratedDangleFruit;
            concentratedDangleFruit?.Unregister();
            ConcentratedDangleFruit = null;

            MultiplayerUnlocks.SandboxUnlockID painlessFruit = PainlessFruit;
            painlessFruit?.Unregister();
            PainlessFruit = null;

            MultiplayerUnlocks.SandboxUnlockID colorfulFruit = ColorfulFruit;
            colorfulFruit?.Unregister();
            ColorfulFruit = null;

            MultiplayerUnlocks.SandboxUnlockID anesthesiaSpear = AnesthesiaSpear;
            anesthesiaSpear?.Unregister();
            AnesthesiaSpear = null;

            MultiplayerUnlocks.SandboxUnlockID anesthesiaNeedle = AnesthesiaNeedle;
            anesthesiaNeedle?.Unregister();
            AnesthesiaNeedle = null;

            MultiplayerUnlocks.SandboxUnlockID smallRock = SmallRock;
            smallRock?.Unregister();
            SmallRock = null;

            MultiplayerUnlocks.SandboxUnlockID knot = Knot;
            knot?.Unregister();
            Knot = null;

            MultiplayerUnlocks.SandboxUnlockID stoneKnife = StoneKnife;
            stoneKnife?.Unregister();
            StoneKnife = null;

            MultiplayerUnlocks.SandboxUnlockID explosivePowder = ExplosivePowder;
            explosivePowder?.Unregister();
            ExplosivePowder = null;

            MultiplayerUnlocks.SandboxUnlockID expansionBomb = ExpansionBomb;
            expansionBomb?.Unregister();
            ExpansionBomb = null;

            MultiplayerUnlocks.SandboxUnlockID breathingBubble = BreathingBubble;
            breathingBubble?.Unregister();
            BreathingBubble = null;

            MultiplayerUnlocks.SandboxUnlockID kongmingLantern = KongmingLantern;
            kongmingLantern?.Unregister();
            KongmingLantern = null;

            MultiplayerUnlocks.SandboxUnlockID tremblingFruit = TremblingFruit;
            tremblingFruit?.Unregister();
            TremblingFruit = null;
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

