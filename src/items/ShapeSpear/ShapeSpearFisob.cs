using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace ShapeSpears;

sealed class ShapeSpearFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType ShapeSpear = new("ShapeSpear", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID RedShapeSpear = new("RedShapeSpear", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID OrangeShapeSpear = new("OrangeShapeSpear", true);

    public ShapeSpearFisob() : base(ShapeSpear)
    {
        // Fisobs auto-loads the `icon_ShapeSpear` embedded resource as a texture.
        // See `ShapeSpears.csproj` for how you can add embedded resources to your project.

        // If you want a simple grayscale icon, you can omit the following line.
        Icon = new ShapeSpearIcon();

        SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);

        RegisterUnlock(OrangeShapeSpear, parent: MultiplayerUnlocks.SandboxUnlockID.BigCentipede, data: 70);
        RegisterUnlock(RedShapeSpear, parent: MultiplayerUnlocks.SandboxUnlockID.RedCentipede, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] p = saveData.CustomData.Split(';');

        if (p.Length < 5) {
            p = new string[5];
        }

        var result = new ShapeSpearAbstract(world, saveData.Pos, saveData.ID) {
            hue = float.TryParse(p[0], out var h) ? h : 0,
            saturation = float.TryParse(p[1], out var s) ? s : 1,
            scaleX = float.TryParse(p[2], out var x) ? x : 1,
            scaleY = float.TryParse(p[3], out var y) ? y : 1,
            damage = float.TryParse(p[4], out var r) ? r : 0
        };

        // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see ShapeSpearIcon below).
        if (unlock is SandboxUnlock u) {
            result.hue = u.Data / 1000f;

            if (u.Data == 0) {
                result.scaleX += 0.2f;
                result.scaleY += 0.2f;
            }
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
