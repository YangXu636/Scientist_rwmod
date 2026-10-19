using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Scientist.ScientistHooks
{
    /// <summary>
    /// MouseDrag 软依赖：判断某个 chunk 是否正被鼠标拖拽。
    /// 被拖拽的绳结视为"最高级主动端"（有效质量 ∞），绳子只会被它牵着走，
    /// 另一侧按质量比被拖动 —— 与"被生物抓握"的区别是后者可以被绳子拖动。
    /// 全部走反射，没装 MouseDrag 时 IsDragging 恒为 false。
    /// </summary>
    public static class MouseDragHooks
    {
        private static bool initialized;
        private static FieldInfo dragChunksField;
        private static FieldInfo chunkField;

        /// <summary>该 chunk 当前是否被 MouseDrag 拖动。</summary>
        public static bool IsDragging(BodyChunk chunk)
        {
            if (chunk == null)
            {
                return false;
            }
            EnsureInit();
            if (dragChunksField == null)
            {
                return false;
            }
            try
            {
                if (!(dragChunksField.GetValue(null) is IList list) || list.Count == 0)
                {
                    return false;
                }
                if (chunkField == null)
                {
                    return false;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    object data = list[i];
                    if (data != null && ReferenceEquals(chunkField.GetValue(data), chunk))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // 反射失败（版本差异）：此后不再尝试，按"没在拖拽"处理
                dragChunksField = null;
            }
            return false;
        }

        private static void EnsureInit()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
            try
            {
                Type dragType = Type.GetType("MouseDrag.Drag, MouseDrag");
                if (dragType == null)
                {
                    return;
                }
                dragChunksField = dragType.GetField(
                    "dragChunks",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                Type dataType = dragType.GetNestedType("DragData", BindingFlags.Public | BindingFlags.NonPublic)
                                ?? Type.GetType("MouseDrag.Drag+DragData, MouseDrag");
                if (dataType != null)
                {
                    chunkField = dataType.GetField(
                        "chunk",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
            catch
            {
                dragChunksField = null;
                chunkField = null;
            }
        }
    }
}
