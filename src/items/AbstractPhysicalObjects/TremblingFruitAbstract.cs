using Rewired.ControllerExtensions;
using RWCustom;
using Scientist;
using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	public class TremblingFruitAbstract : AbstractPhysicalObject
    {
        public Color[] snailShellColors;

        public TremblingFruitAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, Color[] snailShellColors = null) : base(world, Scientist.Enums.Items.TremblingFruit, realizedObject, pos, ID)
        {
            UnityEngine.Random.InitState(this.ID.RandomSeed);
            this.snailShellColors = snailShellColors ?? (new Color[2] { Custom.HSL2RGB(0.9f, Mathf.Lerp(0.7f, 1f, 1f - Mathf.Pow(UnityEngine.Random.value, 3f)), Mathf.Lerp(0f, 0.3f, Mathf.Pow(UnityEngine.Random.value, 2f))), Custom.HSL2RGB(Mathf.Lerp(0.85f, 0.9f, Mathf.Pow(UnityEngine.Random.value, 3f)), Mathf.Lerp(0.7f, 1f, 1f - Mathf.Pow(UnityEngine.Random.value, 3f)), Mathf.Lerp(0.05f, 1f, Mathf.Pow(UnityEngine.Random.value, 3f))) });
        }

        public override void Realize()
		{
            this.realizedObject ??= new Items.TremblingFruit(this, this.world);
        }

        public override string ToString()
        {
            return $"{base.ToString()}<oA>{this.snailShellColors[0].ToHex()}<oA>{this.snailShellColors[1].ToHex()}";
        }
    }
}
