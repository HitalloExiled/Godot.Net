namespace Godot.Net.Platforms.Windows;

public partial class DisplayServerWindows
{
    public struct LOGCONTEXTW
    {
        public nint lcName;
        public uint lcOptions;
        public uint lcStatus;
        public uint lcLocks;
        public uint lcMsgBase;
        public uint lcDevice;
        public uint lcPktRate;
        public nint lcPktData;
        public nint lcPktMode;
        public nint lcMoveMask;
        public nint lcBtnDnMask;
        public nint lcBtnUpMask;
        public long lcInOrgX;
        public long lcInOrgY;
        public long lcInOrgZ;
        public long lcInExtX;
        public long lcInExtY;
        public long lcInExtZ;
        public long lcOutOrgX;
        public long lcOutOrgY;
        public long lcOutOrgZ;
        public long lcOutExtX;
        public long lcOutExtY;
        public long lcOutExtZ;
        public nint lcSensX;
        public nint lcSensY;
        public nint lcSensZ;
        public bool lcSysMode;
        public int lcSysOrgX;
        public int lcSysOrgY;
        public int lcSysExtX;
        public int lcSysExtY;
        public nint lcSysSensX;
        public nint lcSysSensY;
    };
}
