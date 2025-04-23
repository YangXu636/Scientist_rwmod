using UnityEngine;

namespace Scientist.Interface;

interface IReflectionBeam : ICanBeamHit
{
    bool CanReflect();

    /// <summary>
    /// 获取反射方向
    /// </summary>
    /// <param name="incidentPointAngle">bodychunk.pos指向击中点的向量</param>
    /// <param name="incidentDirection">光柱的方向向量</param>
    /// <param name="intensities">反射强度, 0~1</param>
    /// <returns>反射方向组</returns>
    Vector2[] ReflectionAngles(Vector2 incidentPointAngle, Vector2 incidentDirection, out float[] intensities);
}