using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Scientist.Items
{
    /// <summary>
    /// 绳子调试录制：<c>rope_start_debug</c> 开始，<c>rope_end_debug</c> 结束。
    /// 录制期间每帧把房间里所有 <see cref="StringShort"/> 的全局量 + 每个节点的
    /// pos / vel / lastPos / invMass / 深陷计数 / 贴地强度 写进 D:\Temp\rwropedebug.txt。
    /// 只在录制时才有开销；按帧批量写，AutoFlush 保证崩游戏也不丢已写内容。
    /// </summary>
    public static class RopeDebugRecorder
    {
        public const string FilePath = @"D:\Temp\rwropedebug.txt";

        /// <summary>约 60 秒，防止忘记结束把磁盘写满。</summary>
        private const int MaxFrames = 3600;

        public static bool Enabled { get; private set; }

        private static StreamWriter writer;
        private static readonly StringBuilder buffer = new StringBuilder(64 * 1024);
        private static int currentFrame = -1;
        private static int ropeIndex;
        private static int frameCount;
        private static bool writeFailed;

        /// <summary>开始录制；成功返回 null，失败返回错误描述。</summary>
        public static string Start()
        {
            try
            {
                string dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                CloseWriter();
                writer = new StreamWriter(FilePath, false, new UTF8Encoding(false));
                writer.AutoFlush = true;
                writer.WriteLine($"# rw rope debug start {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine("# ### frame 块：rope 行是全局量，i= 行是该绳的节点");
                buffer.Length = 0;
                currentFrame = -1;
                ropeIndex = 0;
                frameCount = 0;
                writeFailed = false;
                Enabled = true;
                return null;
            }
            catch (Exception e)
            {
                CloseWriter();
                Enabled = false;
                return $"打开 {FilePath} 失败：{e.GetType().Name} {e.Message}";
            }
        }

        /// <summary>停止录制并落盘，返回录制到的帧数。</summary>
        public static int Stop()
        {
            int frames = frameCount;
            if (Enabled)
            {
                FlushBuffer();
            }
            CloseWriter();
            Enabled = false;
            return frames;
        }

        /// <summary>由 StringShort.Update 每帧调用（同一帧内多根绳子会被合并成一个 frame 块）。</summary>
        public static void Capture(StringShort rope)
        {
            if (!Enabled || writeFailed || rope == null)
            {
                return;
            }
            try
            {
                int frame = Time.frameCount;
                if (frame != currentFrame)
                {
                    FlushBuffer();
                    currentFrame = frame;
                    ropeIndex = 0;
                    frameCount++;
                    if (frameCount > MaxFrames)
                    {
                        writer?.WriteLine($"# 达到 {MaxFrames} 帧上限，自动停止");
                        Stop();
                        return;
                    }
                }
                if (ropeIndex == 0)
                {
                    buffer.Append("### frame ").Append(frame).Append(' ')
                          .Append(DateTime.Now.ToString("HH:mm:ss.fff")).Append('\n');
                }
                rope.WriteDebugSnapshot(buffer, ropeIndex);
                ropeIndex++;
            }
            catch (Exception e)
            {
                writeFailed = true;
                try
                {
                    writer?.WriteLine($"# 录制异常，已停止：{e.GetType().Name} {e.Message}");
                }
                catch
                {
                    // 写日志也失败就不再尝试
                }
                Stop();
            }
        }

        private static void FlushBuffer()
        {
            if (buffer.Length == 0)
            {
                return;
            }
            try
            {
                writer?.Write(buffer.ToString());
            }
            catch
            {
                writeFailed = true;
                Enabled = false;
            }
            buffer.Length = 0;
        }

        private static void CloseWriter()
        {
            if (writer == null)
            {
                return;
            }
            try
            {
                writer.Flush();
                writer.Dispose();
            }
            catch
            {
                // 关闭失败无需处理：下次 Start 会重新打开
            }
            writer = null;
        }
    }
}
