using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace items.ShapeSpears;

sealed class ShapeSpearFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType ShapeSpear = new("ShapeSpear", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID ShapeSpearSbid = new("RedShapeSpear", true);

    public ShapeSpearFisob() : base(ShapeSpear)
    {
        // Fisobs auto-loads the `icon_ShapeSpear` embedded resource as a texture.
        // See `ShapeSpears.csproj` for how you can add embedded resources to your project.

        // If you want a simple grayscale icon, you can omit the following line.
        Icon = new ShapeSpearIcon();
        SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);
        RegisterUnlock(ShapeSpearSbid, parent: MultiplayerUnlocks.SandboxUnlockID.BigCentipede, data: 0);
    }

#nullable enable
    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] p = saveData.CustomData.Split(';');
        if (p.Length < 1) {
            p = new string[1];
        }
        var result = new ShapeSpearAbstract(world, null, saveData.Pos, saveData.ID) {
            damageTimes = 1.5f
        };

        // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see ShapeSpearIcon below).
        if (unlock != null) {
            result.damageTimes = (float)unlock.Data;
        }
        return result;
    }

    private static readonly ShapeSpearProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
        // The Mosquitoes example demonstrates this.
        return properties;
    }
}
