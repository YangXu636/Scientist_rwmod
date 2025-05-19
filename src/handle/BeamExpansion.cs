using Scientist.Effects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scientist.Handle;

public class BeamExpansion : UpdatableAndDeletable
{

    public Dictionary<string, List<Effects.BlizzardBeamWithoutLizard>> blizzardBeamWL = new();

    public BeamExpansion(Room room)
    {
        this.room = room;
        this.blizzardBeamWL = new();
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        List<string> bbwlKeys = blizzardBeamWL.Keys.ToList();
        for (int i = 0; i < bbwlKeys.Count; i++)
        {
            List<Effects.BlizzardBeamWithoutLizard> beams = this.blizzardBeamWL[bbwlKeys[i]];
            for (int j = 0; j < beams.Count; j++)
            {
                if (beams[j].owner is not Interface.ICanBeamHit icbh && !beams[j].shouldRemove) { continue; }
                if (Vector2.Distance(beams[j].owner.bodyChunks[beams[j].ownerBodychunkIndex].pos, beams[j].pos) < beams[j].owner.bodyChunks[beams[j].ownerBodychunkIndex].rad + 2f && !beams[j].shouldRemove) { continue; }
                (beams[j].owner as Interface.ICanBeamHit)?.HitBeamID.Remove(beams[j].beamOwnerID);
                BlizzardBeamWithoutLizard tmp = beams[j];
                beams.Remove(tmp);
                tmp.Destroy();
                j--;
            }
            this.blizzardBeamWL[bbwlKeys[i]] = beams;
        }
    }
}