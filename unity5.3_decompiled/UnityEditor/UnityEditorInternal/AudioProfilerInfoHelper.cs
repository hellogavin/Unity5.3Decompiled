namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class AudioProfilerInfoHelper
    {
        public const int AUDIOPROFILER_FLAGS_3D = 1;
        public const int AUDIOPROFILER_FLAGS_COMPRESSED = 0x100;
        public const int AUDIOPROFILER_FLAGS_GROUP = 0x40;
        public const int AUDIOPROFILER_FLAGS_LOOPED = 0x200;
        public const int AUDIOPROFILER_FLAGS_MUTED = 8;
        public const int AUDIOPROFILER_FLAGS_NONBLOCKING = 0x2000;
        public const int AUDIOPROFILER_FLAGS_ONESHOT = 0x20;
        public const int AUDIOPROFILER_FLAGS_OPENMEMORY = 0x400;
        public const int AUDIOPROFILER_FLAGS_OPENMEMORYPOINT = 0x800;
        public const int AUDIOPROFILER_FLAGS_OPENUSER = 0x1000;
        public const int AUDIOPROFILER_FLAGS_PAUSED = 4;
        public const int AUDIOPROFILER_FLAGS_STREAM = 0x80;
        public const int AUDIOPROFILER_FLAGS_VIRTUAL = 0x10;

        private static string FormatDb(float vol)
        {
            if (vol == 0f)
            {
                return "-∞ dB";
            }
            return string.Format("{0:0.00} dB", 20f * Mathf.Log10(vol));
        }

        public static string GetColumnString(AudioProfilerInfoWrapper info, ColumnIndices index)
        {
            bool flag = (info.info.flags & 1) != 0;
            bool flag2 = (info.info.flags & 0x40) != 0;
            switch (index)
            {
                case ColumnIndices.ObjectName:
                    return info.objectName;

                case ColumnIndices.AssetName:
                    return info.assetName;

                case ColumnIndices.Volume:
                    return FormatDb(info.info.volume);

                case ColumnIndices.Audibility:
                    return (!flag2 ? FormatDb(info.info.audibility) : string.Empty);

                case ColumnIndices.PlayCount:
                    return (!flag2 ? info.info.playCount.ToString() : string.Empty);

                case ColumnIndices.Is3D:
                    return (!flag2 ? (!flag ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsPaused:
                    return (!flag2 ? (((info.info.flags & 4) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsMuted:
                    return (!flag2 ? (((info.info.flags & 8) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsVirtual:
                    return (!flag2 ? (((info.info.flags & 0x10) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsOneShot:
                    return (!flag2 ? (((info.info.flags & 0x20) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsLooped:
                    return (!flag2 ? (((info.info.flags & 0x200) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.DistanceToListener:
                    return (!flag2 ? (flag ? ((info.info.distanceToListener < 1000f) ? string.Format("{0:0.00} m", info.info.distanceToListener) : string.Format("{0:0.00} km", info.info.distanceToListener * 0.001f)) : "N/A") : string.Empty);

                case ColumnIndices.MinDist:
                    return (!flag2 ? (flag ? ((info.info.minDist < 1000f) ? string.Format("{0:0.00} m", info.info.minDist) : string.Format("{0:0.00} km", info.info.minDist * 0.001f)) : "N/A") : string.Empty);

                case ColumnIndices.MaxDist:
                    return (!flag2 ? (flag ? ((info.info.maxDist < 1000f) ? string.Format("{0:0.00} m", info.info.maxDist) : string.Format("{0:0.00} km", info.info.maxDist * 0.001f)) : "N/A") : string.Empty);

                case ColumnIndices.Time:
                    return (!flag2 ? string.Format("{0:0.00} s", info.info.time) : string.Empty);

                case ColumnIndices.Duration:
                    return (!flag2 ? string.Format("{0:0.00} s", info.info.duration) : string.Empty);

                case ColumnIndices.Frequency:
                    return (!flag2 ? ((info.info.frequency < 1000f) ? string.Format("{0:0.00} Hz", info.info.frequency) : string.Format("{0:0.00} kHz", info.info.frequency * 0.001f)) : string.Format("{0:0.00} x", info.info.frequency));

                case ColumnIndices.IsStream:
                    return (!flag2 ? (((info.info.flags & 0x80) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsCompressed:
                    return (!flag2 ? (((info.info.flags & 0x100) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsNonBlocking:
                    return (!flag2 ? (((info.info.flags & 0x2000) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsOpenUser:
                    return (!flag2 ? (((info.info.flags & 0x1000) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsOpenMemory:
                    return (!flag2 ? (((info.info.flags & 0x400) == 0) ? "NO" : "YES") : string.Empty);

                case ColumnIndices.IsOpenMemoryPoint:
                    return (!flag2 ? (((info.info.flags & 0x800) == 0) ? "NO" : "YES") : string.Empty);
            }
            return "Unknown";
        }

        public static int GetLastColumnIndex()
        {
            return (!Unsupported.IsDeveloperBuild() ? 15 : 0x16);
        }

        public class AudioProfilerInfoComparer : IComparer<AudioProfilerInfoWrapper>
        {
            public AudioProfilerInfoHelper.ColumnIndices primarySortKey;
            public AudioProfilerInfoHelper.ColumnIndices secondarySortKey;
            public bool sortByDescendingOrder;

            public AudioProfilerInfoComparer(AudioProfilerInfoHelper.ColumnIndices primarySortKey, AudioProfilerInfoHelper.ColumnIndices secondarySortKey, bool sortByDescendingOrder)
            {
                this.primarySortKey = primarySortKey;
                this.secondarySortKey = secondarySortKey;
                this.sortByDescendingOrder = sortByDescendingOrder;
            }

            public int Compare(AudioProfilerInfoWrapper a, AudioProfilerInfoWrapper b)
            {
                int num = this.CompareInternal(a, b, this.primarySortKey);
                return ((num != 0) ? num : this.CompareInternal(a, b, this.secondarySortKey));
            }

            private int CompareInternal(AudioProfilerInfoWrapper a, AudioProfilerInfoWrapper b, AudioProfilerInfoHelper.ColumnIndices key)
            {
                int num = 0;
                switch (key)
                {
                    case AudioProfilerInfoHelper.ColumnIndices.ObjectName:
                        num = a.objectName.CompareTo(b.objectName);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.AssetName:
                        num = a.assetName.CompareTo(b.assetName);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.Volume:
                        num = a.info.volume.CompareTo(b.info.volume);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.Audibility:
                        num = a.info.audibility.CompareTo(b.info.audibility);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.PlayCount:
                        num = a.info.playCount.CompareTo(b.info.playCount);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.Is3D:
                        num = (a.info.flags & 1).CompareTo((int) (b.info.flags & 1));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsPaused:
                        num = (a.info.flags & 4).CompareTo((int) (b.info.flags & 4));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsMuted:
                        num = (a.info.flags & 8).CompareTo((int) (b.info.flags & 8));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsVirtual:
                        num = (a.info.flags & 0x10).CompareTo((int) (b.info.flags & 0x10));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsOneShot:
                        num = (a.info.flags & 0x20).CompareTo((int) (b.info.flags & 0x20));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsLooped:
                        num = (a.info.flags & 0x200).CompareTo((int) (b.info.flags & 0x200));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.DistanceToListener:
                        num = a.info.distanceToListener.CompareTo(b.info.distanceToListener);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.MinDist:
                        num = a.info.minDist.CompareTo(b.info.minDist);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.MaxDist:
                        num = a.info.maxDist.CompareTo(b.info.maxDist);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.Time:
                        num = a.info.time.CompareTo(b.info.time);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.Duration:
                        num = a.info.duration.CompareTo(b.info.duration);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.Frequency:
                        num = a.info.frequency.CompareTo(b.info.frequency);
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsStream:
                        num = (a.info.flags & 0x80).CompareTo((int) (b.info.flags & 0x80));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsCompressed:
                        num = (a.info.flags & 0x100).CompareTo((int) (b.info.flags & 0x100));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsNonBlocking:
                        num = (a.info.flags & 0x2000).CompareTo((int) (b.info.flags & 0x2000));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsOpenUser:
                        num = (a.info.flags & 0x1000).CompareTo((int) (b.info.flags & 0x1000));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsOpenMemory:
                        num = (a.info.flags & 0x400).CompareTo((int) (b.info.flags & 0x400));
                        break;

                    case AudioProfilerInfoHelper.ColumnIndices.IsOpenMemoryPoint:
                        num = (a.info.flags & 0x800).CompareTo((int) (b.info.flags & 0x800));
                        break;
                }
                return (!this.sortByDescendingOrder ? num : -num);
            }
        }

        public enum ColumnIndices
        {
            ObjectName,
            AssetName,
            Volume,
            Audibility,
            PlayCount,
            Is3D,
            IsPaused,
            IsMuted,
            IsVirtual,
            IsOneShot,
            IsLooped,
            DistanceToListener,
            MinDist,
            MaxDist,
            Time,
            Duration,
            Frequency,
            IsStream,
            IsCompressed,
            IsNonBlocking,
            IsOpenUser,
            IsOpenMemory,
            IsOpenMemoryPoint,
            _LastColumn
        }
    }
}

