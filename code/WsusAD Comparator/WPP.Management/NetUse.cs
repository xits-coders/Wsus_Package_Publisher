﻿using System;
using System.Runtime.InteropServices;
using WPP.Security;

using DWORD = System.UInt32;
using LPWSTR = System.String;
using NET_API_STATUS = System.UInt32;

namespace WPP.Management
{
    public class NetUse
    {
        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseAdd(
            LPWSTR UncServerName,
            DWORD Level,
            ref USE_INFO_2 Buf,
            out DWORD ParmError);

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseDel(
            LPWSTR UncServerName,
            LPWSTR UseName,
            DWORD ForceCond);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal LPWSTR ui2_local;
            internal LPWSTR ui2_remote;
            internal LPWSTR ui2_password;
            internal DWORD ui2_status;
            internal DWORD ui2_asg_type;
            internal DWORD ui2_refcount;
            internal DWORD ui2_usecount;
            internal LPWSTR ui2_username;
            internal LPWSTR ui2_domainname;
        }


        public bool Mount(string drive, string networkPath, Credential credential)
        {
            try
            {
                if (!string.IsNullOrEmpty(drive))
                {
                    NetUseDel("", drive, 2);
                }
                else
                {
                    NetUseDel("", networkPath, 2);
                }

                USE_INFO_2 useInfo = new USE_INFO_2();
                useInfo.ui2_local = drive;
                useInfo.ui2_remote = networkPath;
                useInfo.ui2_password = credential.Password;
                useInfo.ui2_asg_type = 0;    //disk drive
                useInfo.ui2_usecount = 1;
                useInfo.ui2_username = credential.Username;
                //useInfo.ui2_domainname = "";

                uint paramErrorIndex;
                uint returnCode = NetUseAdd(null, 2, ref useInfo, out paramErrorIndex);
                if (returnCode != 0)
                {
                    return false;
                }
            }
            catch (Exception) { return false; }
            return true;
        }

        public void UnMount(string networkPath_or_Drive)
        {
            try
            {
                NetUseDel("", networkPath_or_Drive, 2);
            }
            catch (Exception) { }
        }

    }
}
