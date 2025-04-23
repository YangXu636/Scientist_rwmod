using UnityEngine;

namespace Scientist.Interface;

interface IReflectionBeam : ICanBeamHit
{
    bool CanReflect();

    /// <summary>
    /// ��ȡ���䷽��
    /// </summary>
    /// <param name="incidentPointAngle">bodychunk.posָ����е������</param>
    /// <param name="incidentDirection">�����ķ�������</param>
    /// <param name="intensities">����ǿ��, 0~1</param>
    /// <returns>���䷽����</returns>
    Vector2[] ReflectionAngles(Vector2 incidentPointAngle, Vector2 incidentDirection, out float[] intensities);
}