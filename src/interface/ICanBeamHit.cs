using UnityEngine;

namespace Scientist.Interface;

interface ICanBeamHit
{
    bool CanBlockBeam();

    void HitByBeam(Vector2 hitPointAngle, Vector2 hitAngle, float intensity);
}