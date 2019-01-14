using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Peak
{
    public class MftScanner
    {
        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);
        private const uint GenericRead = 0x80000000;
        private const int FileShareRead = 0x1;
        private const int FileShareWrite = 0x2;
        private const int OpenExisting = 3;
        private const int FileReadAttributes = 0x80;
        private const int FileNameIinformation = 9;
        private const int FileFlagBackupSemantics = 0x2000000;
        private const int FileOpenForBackupIntent = 0x4000;
        private const int FileOpenByFileId = 0x2000;
        private const int FileOpen = 0x1;
        private const int ObjCaseInsensitive = 0x40;
        private const int FsctlEnumUsnData = 0x900b3;

        [StructLayout(LayoutKind.Sequential)]
        private struct MftEnumData
        {
            public long StartFileReferenceNumber;
            public long LowUsn;
            public long HighUsn;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UsnRecord
        {
            public int RecordLength;
            public short MajorVersion;
            public short MinorVersion;
            public long FileReferenceNumber;
            public long ParentFileReferenceNumber;
            public long Usn;
            public long TimeStamp;
            public int Reason;
            public int SourceInfo;
            public int SecurityId;
            public FileAttributes FileAttributes;
            public short FileNameLength;
            public short FileNameOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IoStatusBlock
        {
            public int Status;
            public int Information;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UnicodeString
        {
            public short Length;
            public short MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ObjectAttributes
        {
            public int Length;
            public IntPtr RootDirectory;
            public IntPtr ObjectName;
            public int Attributes;
            public int SecurityDescriptor;
            public int SecurityQualityOfService;
        }

        //// MFT_ENUM_DATA
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(IntPtr hDevice, int dwIoControlCode, ref MftEnumData lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 CloseHandle(IntPtr lpObject);

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int NtCreateFile(ref IntPtr fileHandle, int desiredAccess, ref ObjectAttributes objectAttributes, ref IoStatusBlock ioStatusBlock, int allocationSize, int fileAttribs, int sharedAccess, int creationDisposition, int createOptions, int eaBuffer,
            int eaLength);

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int NtQueryInformationFile(IntPtr fileHandle, ref IoStatusBlock ioStatusBlock, IntPtr fileInformation, int length, int fileInformationClass);

        private IntPtr _m_HCj;
        private IntPtr _mBuffer;
        private int _mBufferSize;

        private string _mDriveLetter;

        private class FsNode
        {
            public long Frn;
            public long ParentFrn;
            public string FileName;

            public bool IsFile;
            public FsNode(long lFrn, long lParentFsn, string sFileName, bool bIsFile)
            {
                Frn = lFrn;
                ParentFrn = lParentFsn;
                FileName = sFileName;
                IsFile = bIsFile;
            }
        }

        private IntPtr OpenVolume(string szDriveLetter)
        {

            IntPtr hCj = default(IntPtr);
            //// volume handle

            _mDriveLetter = szDriveLetter;
            hCj = CreateFile(@"\\.\" + szDriveLetter, GenericRead, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);

            return hCj;

        }


        private void Cleanup()
        {
            if (_m_HCj != IntPtr.Zero)
            {
                // Close the volume handle.
                CloseHandle(_m_HCj);
                _m_HCj = InvalidHandleValue;
            }

            if (_mBuffer != IntPtr.Zero)
            {
                // Free the allocated memory
                Marshal.FreeHGlobal(_mBuffer);
                _mBuffer = IntPtr.Zero;
            }

        }


        public IEnumerable<String> EnumerateFiles(string szDriveLetter)
        {
            try
            {
                var usnRecord = default(UsnRecord);
                var mft = default(MftEnumData);
                var dwRetBytes = 0;
                var cb = 0;
                var dicFrnLookup = new Dictionary<long, FsNode>();
                var bIsFile = false;

                // This shouldn't be called more than once.
                if (_mBuffer.ToInt32() != 0)
                {
                    throw new Exception("invalid buffer");
                }

                // Assign buffer size
                _mBufferSize = 65536;
                //64KB

                // Allocate a buffer to use for reading records.
                _mBuffer = Marshal.AllocHGlobal(_mBufferSize);

                // correct path
                szDriveLetter = szDriveLetter.TrimEnd('\\');

                // Open the volume handle 
                _m_HCj = OpenVolume(szDriveLetter);

                // Check if the volume handle is valid.
                if (_m_HCj == InvalidHandleValue)
                {
                    string errorMsg = "Couldn't open handle to the volume.";
                    if (!IsAdministrator())
                        errorMsg += "Current user is not administrator";

                    throw new Exception(errorMsg);
                }

                mft.StartFileReferenceNumber = 0;
                mft.LowUsn = 0;
                mft.HighUsn = long.MaxValue;

                do
                {
                    if (DeviceIoControl(_m_HCj, FsctlEnumUsnData, ref mft, Marshal.SizeOf(mft), _mBuffer, _mBufferSize, ref dwRetBytes, IntPtr.Zero))
                    {
                        cb = dwRetBytes;
                        // Pointer to the first record
                        IntPtr pUsnRecord = new IntPtr(_mBuffer.ToInt32() + 8);

                        while ((dwRetBytes > 8))
                        {
                            // Copy pointer to USN_RECORD structure.
                            usnRecord = (UsnRecord)Marshal.PtrToStructure(pUsnRecord, usnRecord.GetType());

                            // The filename within the USN_RECORD.
                            string fileName = Marshal.PtrToStringUni(new IntPtr(pUsnRecord.ToInt32() + usnRecord.FileNameOffset), usnRecord.FileNameLength / 2);

                            bIsFile = !usnRecord.FileAttributes.HasFlag(FileAttributes.Directory);
                            dicFrnLookup.Add(usnRecord.FileReferenceNumber, new FsNode(usnRecord.FileReferenceNumber, usnRecord.ParentFileReferenceNumber, fileName, bIsFile));

                            // Pointer to the next record in the buffer.
                            pUsnRecord = new IntPtr(pUsnRecord.ToInt32() + usnRecord.RecordLength);

                            dwRetBytes -= usnRecord.RecordLength;
                        }

                        // The first 8 bytes is always the start of the next USN.
                        mft.StartFileReferenceNumber = Marshal.ReadInt64(_mBuffer, 0);


                    }
                    else
                    {
                        break; // TODO: might not be correct. Was : Exit Do

                    }

                } while (!(cb <= 8));

                // Resolve all paths for Files
                foreach (FsNode oFsNode in dicFrnLookup.Values.Where(o => o.IsFile))
                {
                    string sFullPath = oFsNode.FileName;
                    FsNode oParentFsNode = oFsNode;

                    while (dicFrnLookup.TryGetValue(oParentFsNode.ParentFrn, out oParentFsNode))
                    {
                        sFullPath = string.Concat(oParentFsNode.FileName, @"\", sFullPath);
                    }
                    sFullPath = string.Concat(szDriveLetter, @"\", sFullPath);

                    yield return sFullPath;
                }
            }
            finally
            {
                //// cleanup
                Cleanup();
            }
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}