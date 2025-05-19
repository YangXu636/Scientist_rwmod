using System.Collections.Generic;
using UnityEngine;

namespace Scientist.Interface;

interface ICanBeamHit
{
    bool CanBlockBeam { get; }

    List<int> HitBeamID { get; set; }

    void HitByBeam(Vector2 hitPointAngle, Vector2 hitAngle, float intensity);
}