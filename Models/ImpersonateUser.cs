using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Security.Principal;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace oneWin.Models
{
    public class ImpersonateUser : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private IntPtr userHandle;
        public WindowsIdentity Identity = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public ImpersonateUser(string user, string domain, string password)
        {            
            userHandle = new System.IntPtr(0);

            Dispose();

            userHandle = IntPtr.Zero;

           
                // Call LogonUser to get a token for the user
                bool loggedOn = LogonUser(user, domain, password,
                    9 /*(int)LogonType.LOGON32_LOGON_NEW_CREDENTIALS*/,
                    3 /*(int)LogonProvider.LOGON32_PROVIDER_WINNT50*/,
                    ref userHandle);
                if (!loggedOn)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // Begin impersonating the user
                Identity = new WindowsIdentity(userHandle);
                
           
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public void Dispose()
        {
            if (userHandle != IntPtr.Zero)
                CloseHandle(userHandle);

            this.userHandle = System.IntPtr.Zero;

            if (this.Identity != null)
            {
                this.Identity.Dispose();
                this.Identity = null;
            }
        }
    }
}
