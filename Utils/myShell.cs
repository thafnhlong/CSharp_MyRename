using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Do_An_2.Utils
{
    class myShell
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };        [Flags]
        public enum SHGFI : int
        {
            Icon = 0x000000100,
            DisplayName = 0x000000200,
            TypeName = 0x000000400,
            Attributes = 0x000000800,
            IconLocation = 0x000001000,
            ExeType = 0x000002000,
            SysIconIndex = 0x000004000,
            LinkOverlay = 0x000008000,
            Selected = 0x000010000,
            Attr_Specified = 0x000020000,
            LargeIcon = 0x000000000,
            SmallIcon = 0x000000001,
            OpenIcon = 0x000000002,
            ShellIconSize = 0x000000004,
            PIDL = 0x000000008,
            UseFileAttributes = 0x000000010,
            AddOverlays = 0x000000020,
            OverlayIndex = 0x000000040,
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);        [DllImport("shell32.dll", SetLastError = true)]
        static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Environment.SpecialFolder nFolder, ref IntPtr ppidl);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static Icon GetFileIcon(string strPath, bool bSmall)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(shinfo);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;
            IntPtr ptrFile = Marshal.StringToHGlobalAuto(strPath);
            SHGetFileInfo(ptrFile, 0, out shinfo, (uint)cbFileInfo, flags);
            Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
            DestroyIcon(shinfo.hIcon);
            return icon;
        }

        public static Icon GetFolderIcon(bool bSmall)
        {
            var shfi = new SHFILEINFO();
            IntPtr ptrDir = IntPtr.Zero;
            SHGetSpecialFolderLocation(IntPtr.Zero, Environment.SpecialFolder.System, ref ptrDir);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.PIDL | SHGFI.SmallIcon;
            else
                flags = SHGFI.Icon | SHGFI.PIDL | SHGFI.LargeIcon;
            var res = SHGetFileInfo(ptrDir, 0, out shfi, (uint)Marshal.SizeOf(shfi),
            flags);
            Marshal.FreeCoTaskMem(ptrDir);
            var icon = (Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            DestroyIcon(shfi.hIcon);
            return icon;
        }
        public static Icon GetComputerIcon(bool bSmall)
        {
            var shfi = new SHFILEINFO();
            IntPtr ptrDir = IntPtr.Zero;
            SHGetSpecialFolderLocation(IntPtr.Zero, Environment.SpecialFolder.MyComputer, ref ptrDir);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.PIDL | SHGFI.SmallIcon;
            else
                flags = SHGFI.Icon | SHGFI.PIDL | SHGFI.LargeIcon;
            var res = SHGetFileInfo(ptrDir, 0, out shfi, (uint)Marshal.SizeOf(shfi),
            flags);
            Marshal.FreeCoTaskMem(ptrDir);
            var icon = (Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            DestroyIcon(shfi.hIcon);
            return icon;
        }
    }
}
