using System.Runtime.InteropServices;

namespace Ashfur.Utils;

// everything about this class and all the methods in it are so dumb and pedantic lmao
public class WindowsVersion {
    public static bool IfWindows11() {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
            if (Environment.OSVersion.Version.Major >= 10) {
                if (Environment.OSVersion.Version.Build >= 22000) {
                    return true;
                }
            }
        }

        return false;
    }

    public static string Windows11String() {
        if (IfWindows11()) {
            var version = Environment.OSVersion.Version;
            return $"Windows 11 [{version.Major}.{version.Minor}.{version.Build}.{version.Revision}]";
        }

        return ""; // TODO: Return OS version and shit if it isn't Windows 11.
    }
}