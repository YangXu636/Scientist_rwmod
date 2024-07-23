using System;
using System.Security.Permissions;
using UnityEngine;

/*
 * This file contains fixes to some common problems when modding Rain World.-����ļ�Ϊ�������mod�����е�һЩ���������ṩ�޸�
 * Unless you know what you're doing, you shouldn't modify anything here.-������֪��������ʲô������Ӧ�ö��������κθ���
 */

//�����������Ӽ���ע�͵���...ûɶ���⣩

// Allows access to private members-�����˽�г�Ա�ķ���
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618


internal static class Extras
{
    private static bool _initialized;

    // Ensure resources are only loaded once and that failing to load them will not break other mods-ȷ����Դ������һ�� ���Ҽ���ʧ�ܵ���Դ��Ӱ������ģ��
    public static On.RainWorld.hook_OnModsInit WrapInit(Action<RainWorld> loadResources)
    {
        return (orig, self) =>
        {
            orig(self);

            try
            {
                if (!_initialized)
                {
                    _initialized = true;
                    loadResources(self);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        };
    }
}