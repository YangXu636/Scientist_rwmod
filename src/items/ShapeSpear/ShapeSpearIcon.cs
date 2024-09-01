using Fisobs.Core;
using UnityEngine;
using static MonoMod.InlineRT.MonoModRule;

namespace items.ShapeSpears;

sealed class ShapeSpearIcon : Icon
{
    // Vanilla only gives you one int field to store all your custom data.
    // Here, that int field is used to store the shield's hue, scaled by 1000.
    // So, 0 is red and 70 is orange.
    public override int Data(AbstractPhysicalObject apo)
    {
        return 0;
    }

    public override Color SpriteColor(int data)
    {
        Color result;
        if (data == 0)
        {
            result = new Color(0.5f, 0.5f, 1f);
        }
        else
        {
            result = new Color(1f, 0.5f, 0.5f);
        }
        return result;
    }

    public override string SpriteName(int data)
    {
        // Fisobs autoloads the file in the mod folder named "icon_{Type}.png"
        // To use that, just remove the png suffix: "icon_ShapeSpear"
        return "icon_ShapeSpear";
    }
}
