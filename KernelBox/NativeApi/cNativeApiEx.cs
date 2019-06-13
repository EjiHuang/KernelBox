using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KernelBox
{
    /// <summary>
    /// 记录win32函数所用到的数据类型和关键字
    /// </summary>
    class NativeApiEx
    {
        [Flags]
        public enum ACCESS_MASK : uint
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            STANDARD_RIGHTS_READ = 0x00020000,
            STANDARD_RIGHTS_WRITE = 0x00020000,
            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            STANDARD_RIGHTS_ALL = 0x001F0000,

            SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

            ACCESS_SYSTEM_SECURITY = 0x01000000,

            MAXIMUM_ALLOWED = 0x02000000,

            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,

            DESKTOP_READOBJECTS = 0x00000001,
            DESKTOP_CREATEWINDOW = 0x00000002,
            DESKTOP_CREATEMENU = 0x00000004,
            DESKTOP_HOOKCONTROL = 0x00000008,
            DESKTOP_JOURNALRECORD = 0x00000010,
            DESKTOP_JOURNALPLAYBACK = 0x00000020,
            DESKTOP_ENUMERATE = 0x00000040,
            DESKTOP_WRITEOBJECTS = 0x00000080,
            DESKTOP_SWITCHDESKTOP = 0x00000100,

            WINSTA_ENUMDESKTOPS = 0x00000001,
            WINSTA_READATTRIBUTES = 0x00000002,
            WINSTA_ACCESSCLIPBOARD = 0x00000004,
            WINSTA_CREATEDESKTOP = 0x00000008,
            WINSTA_WRITEATTRIBUTES = 0x00000010,
            WINSTA_ACCESSGLOBALATOMS = 0x00000020,
            WINSTA_EXITWINDOWS = 0x00000040,
            WINSTA_ENUMERATE = 0x00000100,
            WINSTA_READSCREEN = 0x00000200,

            WINSTA_ALL_ACCESS = 0x0000037F
        }

        /// <summary>
        /// Severity of the error, and action taken, if this service fails 
        /// to start.
        /// </summary>
        public enum SERVICE_ERROR
        {
            /// <summary>
            /// The startup program ignores the error and continues the startup
            /// operation.
            /// </summary>
            SERVICE_ERROR_IGNORE = 0x00000000,

            /// <summary>
            /// The startup program logs the error in the event log but continues
            /// the startup operation.
            /// </summary>
            SERVICE_ERROR_NORMAL = 0x00000001,

            /// <summary>
            /// The startup program logs the error in the event log. If the 
            /// last-known-good configuration is being started, the startup 
            /// operation continues. Otherwise, the system is restarted with 
            /// the last-known-good configuration.
            /// </summary>
            SERVICE_ERROR_SEVERE = 0x00000002,

            /// <summary>
            /// The startup program logs the error in the event log, if possible.
            /// If the last-known-good configuration is being started, the startup
            /// operation fails. Otherwise, the system is restarted with the 
            /// last-known good configuration.
            /// </summary>
            SERVICE_ERROR_CRITICAL = 0x00000003,
        }

        [Flags]
        public enum SCM_ACCESS : uint
        {
            /// <summary>
            /// Required to connect to the service control manager.
            /// </summary>
            SC_MANAGER_CONNECT = 0x00001,

            /// <summary>
            /// Required to call the CreateService function to create a service
            /// object and add it to the database.
            /// </summary>
            SC_MANAGER_CREATE_SERVICE = 0x00002,

            /// <summary>
            /// Required to call the EnumServicesStatusEx function to list the 
            /// services that are in the database.
            /// </summary>
            SC_MANAGER_ENUMERATE_SERVICE = 0x00004,

            /// <summary>
            /// Required to call the LockServiceDatabase function to acquire a 
            /// lock on the database.
            /// </summary>
            SC_MANAGER_LOCK = 0x00008,

            /// <summary>
            /// Required to call the QueryServiceLockStatus function to retrieve 
            /// the lock status information for the database.
            /// </summary>
            SC_MANAGER_QUERY_LOCK_STATUS = 0x00010,

            /// <summary>
            /// Required to call the NotifyBootConfigStatus function.
            /// </summary>
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x00020,

            /// <summary>
            /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access 
            /// rights in this table.
            /// </summary>
            SC_MANAGER_ALL_ACCESS = ACCESS_MASK.STANDARD_RIGHTS_REQUIRED |
                SC_MANAGER_CONNECT |
                SC_MANAGER_CREATE_SERVICE |
                SC_MANAGER_ENUMERATE_SERVICE |
                SC_MANAGER_LOCK |
                SC_MANAGER_QUERY_LOCK_STATUS |
                SC_MANAGER_MODIFY_BOOT_CONFIG,

            GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ |
                SC_MANAGER_ENUMERATE_SERVICE |
                SC_MANAGER_QUERY_LOCK_STATUS,

            GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE |
                SC_MANAGER_CREATE_SERVICE |
                SC_MANAGER_MODIFY_BOOT_CONFIG,

            GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE |
                SC_MANAGER_CONNECT | SC_MANAGER_LOCK,

            GENERIC_ALL = SC_MANAGER_ALL_ACCESS,
        }
        
        /// <summary>
        /// Access to the service. Before granting the requested access, the
        /// system checks the access token of the calling process. 
        /// </summary>
        [Flags]
        public enum SERVICE_ACCESS : uint
        {
            /// <summary>
            /// Required to call the QueryServiceConfig and 
            /// QueryServiceConfig2 functions to query the service configuration.
            /// </summary>
            SERVICE_QUERY_CONFIG = 0x00001,

            /// <summary>
            /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function 
            /// to change the service configuration. Because this grants the caller 
            /// the right to change the executable file that the system runs, 
            /// it should be granted only to administrators.
            /// </summary>
            SERVICE_CHANGE_CONFIG = 0x00002,

            /// <summary>
            /// Required to call the QueryServiceStatusEx function to ask the service 
            /// control manager about the status of the service.
            /// </summary>
            SERVICE_QUERY_STATUS = 0x00004,

            /// <summary>
            /// Required to call the EnumDependentServices function to enumerate all 
            /// the services dependent on the service.
            /// </summary>
            SERVICE_ENUMERATE_DEPENDENTS = 0x00008,

            /// <summary>
            /// Required to call the StartService function to start the service.
            /// </summary>
            SERVICE_START = 0x00010,

            /// <summary>
            ///     Required to call the ControlService function to stop the service.
            /// </summary>
            SERVICE_STOP = 0x00020,

            /// <summary>
            /// Required to call the ControlService function to pause or continue 
            /// the service.
            /// </summary>
            SERVICE_PAUSE_CONTINUE = 0x00040,

            /// <summary>
            /// Required to call the EnumDependentServices function to enumerate all
            /// the services dependent on the service.
            /// </summary>
            SERVICE_INTERROGATE = 0x00080,

            /// <summary>
            /// Required to call the ControlService function to specify a user-defined
            /// control code.
            /// </summary>
            SERVICE_USER_DEFINED_CONTROL = 0x00100,

            /// <summary>
            /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
            /// </summary>
            SERVICE_ALL_ACCESS = (ACCESS_MASK.STANDARD_RIGHTS_REQUIRED |
                SERVICE_QUERY_CONFIG |
                SERVICE_CHANGE_CONFIG |
                SERVICE_QUERY_STATUS |
                SERVICE_ENUMERATE_DEPENDENTS |
                SERVICE_START |
                SERVICE_STOP |
                SERVICE_PAUSE_CONTINUE |
                SERVICE_INTERROGATE |
                SERVICE_USER_DEFINED_CONTROL),

            GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ |
                SERVICE_QUERY_CONFIG |
                SERVICE_QUERY_STATUS |
                SERVICE_INTERROGATE |
                SERVICE_ENUMERATE_DEPENDENTS,

            GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE |
                SERVICE_CHANGE_CONFIG,

            GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE |
                SERVICE_START |
                SERVICE_STOP |
                SERVICE_PAUSE_CONTINUE |
                SERVICE_USER_DEFINED_CONTROL,

            /// <summary>
            /// Required to call the QueryServiceObjectSecurity or 
            /// SetServiceObjectSecurity function to access the SACL. The proper
            /// way to obtain this access is to enable the SE_SECURITY_NAME 
            /// privilege in the caller's current access token, open the handle 
            /// for ACCESS_SYSTEM_SECURITY access, and then disable the privilege.
            /// </summary>
            ACCESS_SYSTEM_SECURITY = ACCESS_MASK.ACCESS_SYSTEM_SECURITY,

            /// <summary>
            /// Required to call the DeleteService function to delete the service.
            /// </summary>
            DELETE = ACCESS_MASK.DELETE,

            /// <summary>
            /// Required to call the QueryServiceObjectSecurity function to query
            /// the security descriptor of the service object.
            /// </summary>
            READ_CONTROL = ACCESS_MASK.READ_CONTROL,

            /// <summary>
            /// Required to call the SetServiceObjectSecurity function to modify
            /// the Dacl member of the service object's security descriptor.
            /// </summary>
            WRITE_DAC = ACCESS_MASK.WRITE_DAC,

            /// <summary>
            /// Required to call the SetServiceObjectSecurity function to modify 
            /// the Owner and Group members of the service object's security 
            /// descriptor.
            /// </summary>
            WRITE_OWNER = ACCESS_MASK.WRITE_OWNER,
        }

        /// <summary>
        /// Service types.
        /// </summary>
        [Flags]
        public enum SERVICE_TYPE : uint
        {
            /// <summary>
            /// Driver service.
            /// </summary>
            SERVICE_KERNEL_DRIVER = 0x00000001,

            /// <summary>
            /// File system driver service.
            /// </summary>
            SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,

            /// <summary>
            /// Service that runs in its own process.
            /// </summary>
            SERVICE_WIN32_OWN_PROCESS = 0x00000010,

            /// <summary>
            /// Service that shares a process with one or more other services.
            /// </summary>
            SERVICE_WIN32_SHARE_PROCESS = 0x00000020,

            /// <summary>
            /// The service can interact with the desktop.
            /// </summary>
            SERVICE_INTERACTIVE_PROCESS = 0x00000100,
        }

        /// <summary>
        /// Service start options
        /// </summary>
        public enum SERVICE_START : uint
        {
            /// <summary>
            /// A device driver started by the system loader. This value is valid
            /// only for driver services.
            /// </summary>
            SERVICE_BOOT_START = 0x00000000,

            /// <summary>
            /// A device driver started by the IoInitSystem function. This value 
            /// is valid only for driver services.
            /// </summary>
            SERVICE_SYSTEM_START = 0x00000001,

            /// <summary>
            /// A service started automatically by the service control manager 
            /// during system startup. For more information, see Automatically 
            /// Starting Services.
            /// </summary>         
            SERVICE_AUTO_START = 0x00000002,

            /// <summary>
            /// A service started by the service control manager when a process 
            /// calls the StartService function. For more information, see 
            /// Starting Services on Demand.
            /// </summary>
            SERVICE_DEMAND_START = 0x00000003,

            /// <summary>
            /// A service that cannot be started. Attempts to start the service
            /// result in the error code ERROR_SERVICE_DISABLED.
            /// </summary>
            SERVICE_DISABLED = 0x00000004,
        }

        [Flags]
        public enum ServiceType {
            SERVICE_KERNEL_DRIVER = 0x1,
            SERVICE_FILE_SYSTEM_DRIVER = 0x2,
            SERVICE_WIN32_OWN_PROCESS = 0x10,
            SERVICE_WIN32_SHARE_PROCESS = 0x20,
            SERVICE_INTERACTIVE_PROCESS = 0x100,
            SERVICETYPE_NO_CHANGE = SERVICE_NO_CHANGE,
            SERVICE_WIN32 = (SERVICE_WIN32_OWN_PROCESS | SERVICE_WIN32_SHARE_PROCESS),
            SERVICE_NO_CHANGE = 0xffff //this value is found in winsvc.h
        }

        [Flags]
        public enum SERVICE_TYPES : int
        {
            SERVICE_KERNEL_DRIVER = 0x00000001,
            SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,
            SERVICE_WIN32_OWN_PROCESS = 0x00000010,
            SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
            SERVICE_INTERACTIVE_PROCESS = 0x00000100
        }

        [Flags]
        public enum SERVICE_STATE : int
        {
            SERVICE_ACTIVE = 0x00000001,
            SERVICE_INACTIVE = 0x00000002,
            SERVICE_STATE_ALL = SERVICE_ACTIVE | SERVICE_INACTIVE
        }

        [Flags]
        public enum SERVICE_CONTROL : uint
        {
            STOP = 0x00000001,
            PAUSE = 0x00000002,
            CONTINUE = 0x00000003,
            INTERROGATE = 0x00000004,
            SHUTDOWN = 0x00000005,
            PARAMCHANGE = 0x00000006,
            NETBINDADD = 0x00000007,
            NETBINDREMOVE = 0x00000008,
            NETBINDENABLE = 0x00000009,
            NETBINDDISABLE = 0x0000000A,
            DEVICEEVENT = 0x0000000B,
            HARDWAREPROFILECHANGE = 0x0000000C,
            POWEREVENT = 0x0000000D,
            SESSIONCHANGE = 0x0000000E
        }

        [Flags]
        public enum EMethod : uint
        {
            Buffered = 0,
            InDirect = 1,
            OutDirect = 2,
            Neither = 3
        }

        [Flags]
        public enum EFileDevice : uint
        {
            Beep = 0x00000001,
            CDRom = 0x00000002,
            CDRomFileSytem = 0x00000003,
            Controller = 0x00000004,
            Datalink = 0x00000005,
            Dfs = 0x00000006,
            Disk = 0x00000007,
            DiskFileSystem = 0x00000008,
            FileSystem = 0x00000009,
            InPortPort = 0x0000000a,
            Keyboard = 0x0000000b,
            Mailslot = 0x0000000c,
            MidiIn = 0x0000000d,
            MidiOut = 0x0000000e,
            Mouse = 0x0000000f,
            MultiUncProvider = 0x00000010,
            NamedPipe = 0x00000011,
            Network = 0x00000012,
            NetworkBrowser = 0x00000013,
            NetworkFileSystem = 0x00000014,
            Null = 0x00000015,
            ParallelPort = 0x00000016,
            PhysicalNetcard = 0x00000017,
            Printer = 0x00000018,
            Scanner = 0x00000019,
            SerialMousePort = 0x0000001a,
            SerialPort = 0x0000001b,
            Screen = 0x0000001c,
            Sound = 0x0000001d,
            Streams = 0x0000001e,
            Tape = 0x0000001f,
            TapeFileSystem = 0x00000020,
            Transport = 0x00000021,
            Unknown = 0x00000022,
            Video = 0x00000023,
            VirtualDisk = 0x00000024,
            WaveIn = 0x00000025,
            WaveOut = 0x00000026,
            Port8042 = 0x00000027,
            NetworkRedirector = 0x00000028,
            Battery = 0x00000029,
            BusExtender = 0x0000002a,
            Modem = 0x0000002b,
            Vdm = 0x0000002c,
            MassStorage = 0x0000002d,
            Smb = 0x0000002e,
            Ks = 0x0000002f,
            Changer = 0x00000030,
            Smartcard = 0x00000031,
            Acpi = 0x00000032,
            Dvd = 0x00000033,
            FullscreenVideo = 0x00000034,
            DfsFileSystem = 0x00000035,
            DfsVolume = 0x00000036,
            Serenum = 0x00000037,
            Termsrv = 0x00000038,
            Ksec = 0x00000039,
            // From Windows Driver Kit 7
            Fips = 0x0000003A,
            Infiniband = 0x0000003B,
            Vmbus = 0x0000003E,
            CryptProvider = 0x0000003F,
            Wpd = 0x00000040,
            Bluetooth = 0x00000041,
            MtComposite = 0x00000042,
            MtTransport = 0x00000043,
            Biometric = 0x00000044,
            Pmi = 0x00000045
        }

        [Flags]
        public enum EIOControlCode : uint
        {
            // STORAGE
            StorageCheckVerify = (EFileDevice.MassStorage << 16) | (0x0200 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageCheckVerify2 = (EFileDevice.MassStorage << 16) | (0x0200 << 2) | EMethod.Buffered | (0 << 14), // FileAccess.Any
            StorageMediaRemoval = (EFileDevice.MassStorage << 16) | (0x0201 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageEjectMedia = (EFileDevice.MassStorage << 16) | (0x0202 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageLoadMedia = (EFileDevice.MassStorage << 16) | (0x0203 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageLoadMedia2 = (EFileDevice.MassStorage << 16) | (0x0203 << 2) | EMethod.Buffered | (0 << 14),
            StorageReserve = (EFileDevice.MassStorage << 16) | (0x0204 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageRelease = (EFileDevice.MassStorage << 16) | (0x0205 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageFindNewDevices = (EFileDevice.MassStorage << 16) | (0x0206 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageEjectionControl = (EFileDevice.MassStorage << 16) | (0x0250 << 2) | EMethod.Buffered | (0 << 14),
            StorageMcnControl = (EFileDevice.MassStorage << 16) | (0x0251 << 2) | EMethod.Buffered | (0 << 14),
            StorageGetMediaTypes = (EFileDevice.MassStorage << 16) | (0x0300 << 2) | EMethod.Buffered | (0 << 14),
            StorageGetMediaTypesEx = (EFileDevice.MassStorage << 16) | (0x0301 << 2) | EMethod.Buffered | (0 << 14),
            StorageResetBus = (EFileDevice.MassStorage << 16) | (0x0400 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageResetDevice = (EFileDevice.MassStorage << 16) | (0x0401 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageGetDeviceNumber = (EFileDevice.MassStorage << 16) | (0x0420 << 2) | EMethod.Buffered | (0 << 14),
            StoragePredictFailure = (EFileDevice.MassStorage << 16) | (0x0440 << 2) | EMethod.Buffered | (0 << 14),
            StorageObsoleteResetBus = (EFileDevice.MassStorage << 16) | (0x0400 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            StorageObsoleteResetDevice = (EFileDevice.MassStorage << 16) | (0x0401 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            StorageQueryProperty = (EFileDevice.MassStorage << 16) | (0x0500 << 2) | EMethod.Buffered | (0 << 14),
            // DISK
            DiskGetDriveGeometry = (EFileDevice.Disk << 16) | (0x0000 << 2) | EMethod.Buffered | (0 << 14),
            DiskGetDriveGeometryEx = (EFileDevice.Disk << 16) | (0x0028 << 2) | EMethod.Buffered | (0 << 14),
            DiskGetPartitionInfo = (EFileDevice.Disk << 16) | (0x0001 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskGetPartitionInfoEx = (EFileDevice.Disk << 16) | (0x0012 << 2) | EMethod.Buffered | (0 << 14),
            DiskSetPartitionInfo = (EFileDevice.Disk << 16) | (0x0002 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetDriveLayout = (EFileDevice.Disk << 16) | (0x0003 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskSetDriveLayout = (EFileDevice.Disk << 16) | (0x0004 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskVerify = (EFileDevice.Disk << 16) | (0x0005 << 2) | EMethod.Buffered | (0 << 14),
            DiskFormatTracks = (EFileDevice.Disk << 16) | (0x0006 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskReassignBlocks = (EFileDevice.Disk << 16) | (0x0007 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskPerformance = (EFileDevice.Disk << 16) | (0x0008 << 2) | EMethod.Buffered | (0 << 14),
            DiskIsWritable = (EFileDevice.Disk << 16) | (0x0009 << 2) | EMethod.Buffered | (0 << 14),
            DiskLogging = (EFileDevice.Disk << 16) | (0x000a << 2) | EMethod.Buffered | (0 << 14),
            DiskFormatTracksEx = (EFileDevice.Disk << 16) | (0x000b << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskHistogramStructure = (EFileDevice.Disk << 16) | (0x000c << 2) | EMethod.Buffered | (0 << 14),
            DiskHistogramData = (EFileDevice.Disk << 16) | (0x000d << 2) | EMethod.Buffered | (0 << 14),
            DiskHistogramReset = (EFileDevice.Disk << 16) | (0x000e << 2) | EMethod.Buffered | (0 << 14),
            DiskRequestStructure = (EFileDevice.Disk << 16) | (0x000f << 2) | EMethod.Buffered | (0 << 14),
            DiskRequestData = (EFileDevice.Disk << 16) | (0x0010 << 2) | EMethod.Buffered | (0 << 14),
            DiskControllerNumber = (EFileDevice.Disk << 16) | (0x0011 << 2) | EMethod.Buffered | (0 << 14),
            DiskSmartGetVersion = (EFileDevice.Disk << 16) | (0x0020 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskSmartSendDriveCommand = (EFileDevice.Disk << 16) | (0x0021 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskSmartRcvDriveData = (EFileDevice.Disk << 16) | (0x0022 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskUpdateDriveSize = (EFileDevice.Disk << 16) | (0x0032 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGrowPartition = (EFileDevice.Disk << 16) | (0x0034 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetCacheInformation = (EFileDevice.Disk << 16) | (0x0035 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskSetCacheInformation = (EFileDevice.Disk << 16) | (0x0036 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskDeleteDriveLayout = (EFileDevice.Disk << 16) | (0x0040 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskFormatDrive = (EFileDevice.Disk << 16) | (0x00f3 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskSenseDevice = (EFileDevice.Disk << 16) | (0x00f8 << 2) | EMethod.Buffered | (0 << 14),
            DiskCheckVerify = (EFileDevice.Disk << 16) | (0x0200 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskMediaRemoval = (EFileDevice.Disk << 16) | (0x0201 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskEjectMedia = (EFileDevice.Disk << 16) | (0x0202 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskLoadMedia = (EFileDevice.Disk << 16) | (0x0203 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskReserve = (EFileDevice.Disk << 16) | (0x0204 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskRelease = (EFileDevice.Disk << 16) | (0x0205 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskFindNewDevices = (EFileDevice.Disk << 16) | (0x0206 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskGetMediaTypes = (EFileDevice.Disk << 16) | (0x0300 << 2) | EMethod.Buffered | (0 << 14),
            DiskSetPartitionInfoEx = (EFileDevice.Disk << 16) | (0x0013 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetDriveLayoutEx = (EFileDevice.Disk << 16) | (0x0014 << 2) | EMethod.Buffered | (0 << 14),
            DiskSetDriveLayoutEx = (EFileDevice.Disk << 16) | (0x0015 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskCreateDisk = (EFileDevice.Disk << 16) | (0x0016 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetLengthInfo = (EFileDevice.Disk << 16) | (0x0017 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            // CHANGER
            ChangerGetParameters = (EFileDevice.Changer << 16) | (0x0000 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerGetStatus = (EFileDevice.Changer << 16) | (0x0001 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerGetProductData = (EFileDevice.Changer << 16) | (0x0002 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerSetAccess = (EFileDevice.Changer << 16) | (0x0004 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            ChangerGetElementStatus = (EFileDevice.Changer << 16) | (0x0005 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            ChangerInitializeElementStatus = (EFileDevice.Changer << 16) | (0x0006 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerSetPosition = (EFileDevice.Changer << 16) | (0x0007 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerExchangeMedium = (EFileDevice.Changer << 16) | (0x0008 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerMoveMedium = (EFileDevice.Changer << 16) | (0x0009 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerReinitializeTarget = (EFileDevice.Changer << 16) | (0x000A << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerQueryVolumeTags = (EFileDevice.Changer << 16) | (0x000B << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            // FILESYSTEM
            FsctlRequestOplockLevel1 = (EFileDevice.FileSystem << 16) | (0 << 2) | EMethod.Buffered | (0 << 14),
            FsctlRequestOplockLevel2 = (EFileDevice.FileSystem << 16) | (1 << 2) | EMethod.Buffered | (0 << 14),
            FsctlRequestBatchOplock = (EFileDevice.FileSystem << 16) | (2 << 2) | EMethod.Buffered | (0 << 14),
            FsctlOplockBreakAcknowledge = (EFileDevice.FileSystem << 16) | (3 << 2) | EMethod.Buffered | (0 << 14),
            FsctlOpBatchAckClosePending = (EFileDevice.FileSystem << 16) | (4 << 2) | EMethod.Buffered | (0 << 14),
            FsctlOplockBreakNotify = (EFileDevice.FileSystem << 16) | (5 << 2) | EMethod.Buffered | (0 << 14),
            FsctlLockVolume = (EFileDevice.FileSystem << 16) | (6 << 2) | EMethod.Buffered | (0 << 14),
            FsctlUnlockVolume = (EFileDevice.FileSystem << 16) | (7 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDismountVolume = (EFileDevice.FileSystem << 16) | (8 << 2) | EMethod.Buffered | (0 << 14),
            FsctlIsVolumeMounted = (EFileDevice.FileSystem << 16) | (10 << 2) | EMethod.Buffered | (0 << 14),
            FsctlIsPathnameValid = (EFileDevice.FileSystem << 16) | (11 << 2) | EMethod.Buffered | (0 << 14),
            FsctlMarkVolumeDirty = (EFileDevice.FileSystem << 16) | (12 << 2) | EMethod.Buffered | (0 << 14),
            FsctlQueryRetrievalPointers = (EFileDevice.FileSystem << 16) | (14 << 2) | EMethod.Neither | (0 << 14),
            FsctlGetCompression = (EFileDevice.FileSystem << 16) | (15 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetCompression = (EFileDevice.FileSystem << 16) | (16 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            FsctlMarkAsSystemHive = (EFileDevice.FileSystem << 16) | (19 << 2) | EMethod.Neither | (0 << 14),
            FsctlOplockBreakAckNo2 = (EFileDevice.FileSystem << 16) | (20 << 2) | EMethod.Buffered | (0 << 14),
            FsctlInvalidateVolumes = (EFileDevice.FileSystem << 16) | (21 << 2) | EMethod.Buffered | (0 << 14),
            FsctlQueryFatBpb = (EFileDevice.FileSystem << 16) | (22 << 2) | EMethod.Buffered | (0 << 14),
            FsctlRequestFilterOplock = (EFileDevice.FileSystem << 16) | (23 << 2) | EMethod.Buffered | (0 << 14),
            FsctlFileSystemGetStatistics = (EFileDevice.FileSystem << 16) | (24 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetNtfsVolumeData = (EFileDevice.FileSystem << 16) | (25 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetNtfsFileRecord = (EFileDevice.FileSystem << 16) | (26 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetVolumeBitmap = (EFileDevice.FileSystem << 16) | (27 << 2) | EMethod.Neither | (0 << 14),
            FsctlGetRetrievalPointers = (EFileDevice.FileSystem << 16) | (28 << 2) | EMethod.Neither | (0 << 14),
            FsctlMoveFile = (EFileDevice.FileSystem << 16) | (29 << 2) | EMethod.Buffered | (0 << 14),
            FsctlIsVolumeDirty = (EFileDevice.FileSystem << 16) | (30 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetHfsInformation = (EFileDevice.FileSystem << 16) | (31 << 2) | EMethod.Buffered | (0 << 14),
            FsctlAllowExtendedDasdIo = (EFileDevice.FileSystem << 16) | (32 << 2) | EMethod.Neither | (0 << 14),
            FsctlReadPropertyData = (EFileDevice.FileSystem << 16) | (33 << 2) | EMethod.Neither | (0 << 14),
            FsctlWritePropertyData = (EFileDevice.FileSystem << 16) | (34 << 2) | EMethod.Neither | (0 << 14),
            FsctlFindFilesBySid = (EFileDevice.FileSystem << 16) | (35 << 2) | EMethod.Neither | (0 << 14),
            FsctlDumpPropertyData = (EFileDevice.FileSystem << 16) | (37 << 2) | EMethod.Neither | (0 << 14),
            FsctlSetObjectId = (EFileDevice.FileSystem << 16) | (38 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetObjectId = (EFileDevice.FileSystem << 16) | (39 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDeleteObjectId = (EFileDevice.FileSystem << 16) | (40 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetReparsePoint = (EFileDevice.FileSystem << 16) | (41 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetReparsePoint = (EFileDevice.FileSystem << 16) | (42 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDeleteReparsePoint = (EFileDevice.FileSystem << 16) | (43 << 2) | EMethod.Buffered | (0 << 14),
            FsctlEnumUsnData = (EFileDevice.FileSystem << 16) | (44 << 2) | EMethod.Neither | (0 << 14),
            FsctlSecurityIdCheck = (EFileDevice.FileSystem << 16) | (45 << 2) | EMethod.Neither | (FileAccess.Read << 14),
            FsctlReadUsnJournal = (EFileDevice.FileSystem << 16) | (46 << 2) | EMethod.Neither | (0 << 14),
            FsctlSetObjectIdExtended = (EFileDevice.FileSystem << 16) | (47 << 2) | EMethod.Buffered | (0 << 14),
            FsctlCreateOrGetObjectId = (EFileDevice.FileSystem << 16) | (48 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetSparse = (EFileDevice.FileSystem << 16) | (49 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetZeroData = (EFileDevice.FileSystem << 16) | (50 << 2) | EMethod.Buffered | (FileAccess.Write << 14),
            FsctlQueryAllocatedRanges = (EFileDevice.FileSystem << 16) | (51 << 2) | EMethod.Neither | (FileAccess.Read << 14),
            FsctlEnableUpgrade = (EFileDevice.FileSystem << 16) | (52 << 2) | EMethod.Buffered | (FileAccess.Write << 14),
            FsctlSetEncryption = (EFileDevice.FileSystem << 16) | (53 << 2) | EMethod.Neither | (0 << 14),
            FsctlEncryptionFsctlIo = (EFileDevice.FileSystem << 16) | (54 << 2) | EMethod.Neither | (0 << 14),
            FsctlWriteRawEncrypted = (EFileDevice.FileSystem << 16) | (55 << 2) | EMethod.Neither | (0 << 14),
            FsctlReadRawEncrypted = (EFileDevice.FileSystem << 16) | (56 << 2) | EMethod.Neither | (0 << 14),
            FsctlCreateUsnJournal = (EFileDevice.FileSystem << 16) | (57 << 2) | EMethod.Neither | (0 << 14),
            FsctlReadFileUsnData = (EFileDevice.FileSystem << 16) | (58 << 2) | EMethod.Neither | (0 << 14),
            FsctlWriteUsnCloseRecord = (EFileDevice.FileSystem << 16) | (59 << 2) | EMethod.Neither | (0 << 14),
            FsctlExtendVolume = (EFileDevice.FileSystem << 16) | (60 << 2) | EMethod.Buffered | (0 << 14),
            FsctlQueryUsnJournal = (EFileDevice.FileSystem << 16) | (61 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDeleteUsnJournal = (EFileDevice.FileSystem << 16) | (62 << 2) | EMethod.Buffered | (0 << 14),
            FsctlMarkHandle = (EFileDevice.FileSystem << 16) | (63 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSisCopyFile = (EFileDevice.FileSystem << 16) | (64 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSisLinkFiles = (EFileDevice.FileSystem << 16) | (65 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            FsctlHsmMsg = (EFileDevice.FileSystem << 16) | (66 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            FsctlNssControl = (EFileDevice.FileSystem << 16) | (67 << 2) | EMethod.Buffered | (FileAccess.Write << 14),
            FsctlHsmData = (EFileDevice.FileSystem << 16) | (68 << 2) | EMethod.Neither | (FileAccess.ReadWrite << 14),
            FsctlRecallFile = (EFileDevice.FileSystem << 16) | (69 << 2) | EMethod.Neither | (0 << 14),
            FsctlNssRcontrol = (EFileDevice.FileSystem << 16) | (70 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            // VIDEO
            VideoQuerySupportedBrightness = (EFileDevice.Video << 16) | (0x0125 << 2) | EMethod.Buffered | (0 << 14),
            VideoQueryDisplayBrightness = (EFileDevice.Video << 16) | (0x0126 << 2) | EMethod.Buffered | (0 << 14),
            VideoSetDisplayBrightness = (EFileDevice.Video << 16) | (0x0127 << 2) | EMethod.Buffered | (0 << 14)
        }

        public enum DEVICE_TYPE : ulong
        {
            FILE_DEVICE_BEEP = 0x01,
            FILE_DEVICE_CD_ROM = 0x02,
            FILE_DEVICE_CD_ROM_FILE_SYSTEM = 0x03,
            FILE_DEVICE_CONTROLLER = 0x04,
            FILE_DEVICE_DATALINK = 0x05,
            FILE_DEVICE_DFS = 0x06,
            FILE_DEVICE_DISK = 0x07, // IOCTL_DISK_BASE
            FILE_DEVICE_DISK_FILE_SYSTEM = 0x08,
            FILE_DEVICE_FILE_SYSTEM = 0x09,
            FILE_DEVICE_INPORT_PORT = 0x0a,
            FILE_DEVICE_KEYBOARD = 0x0b,
            FILE_DEVICE_MAILSLOT = 0x0c,
            FILE_DEVICE_MIDI_IN = 0x0d,
            FILE_DEVICE_MIDI_OUT = 0x0e,
            FILE_DEVICE_MOUSE = 0x0f,
            FILE_DEVICE_MULTI_UNC_PROVIDER = 0x10,
            FILE_DEVICE_NAMED_PIPE = 0x11,
            FILE_DEVICE_NETWORK = 0x12,
            FILE_DEVICE_NETWORK_BROWSER = 0x13,
            FILE_DEVICE_NETWORK_FILE_SYSTEM = 0x14,
            FILE_DEVICE_NULL = 0x15,
            FILE_DEVICE_PARALLEL_PORT = 0x16,
            FILE_DEVICE_PHYSICAL_NETCARD = 0x17,
            FILE_DEVICE_PRINTER = 0x18,
            FILE_DEVICE_SCANNER = 0x19,
            FILE_DEVICE_SERIAL_MOUSE_PORT = 0x1a,
            FILE_DEVICE_SERIAL_PORT = 0x1b,
            FILE_DEVICE_SCREEN = 0x1c,
            FILE_DEVICE_SOUND = 0x1d,
            FILE_DEVICE_STREAMS = 0x1e,
            FILE_DEVICE_TAPE = 0x1f,
            FILE_DEVICE_TAPE_FILE_SYSTEM = 0x20,
            FILE_DEVICE_TRANSPORT = 0x21,
            FILE_DEVICE_UNKNOWN = 0x22,
            FILE_DEVICE_VIDEO = 0x23,
            FILE_DEVICE_VIRTUAL_DISK = 0x24,
            FILE_DEVICE_WAVE_IN = 0x25,
            FILE_DEVICE_WAVE_OUT = 0x26,
            FILE_DEVICE_8042_PORT = 0x27,
            FILE_DEVICE_NETWORK_REDIRECTOR = 0x28,
            FILE_DEVICE_BATTERY = 0x29,
            FILE_DEVICE_BUS_EXTENDER = 0x2a,
            FILE_DEVICE_MODEM = 0x2b,
            FILE_DEVICE_VDM = 0x2c,
            FILE_DEVICE_MASS_STORAGE = 0x2d, // IOCTL_STORAGE_BASE
            FILE_DEVICE_SMB = 0x2e,
            FILE_DEVICE_KS = 0x2f,
            FILE_DEVICE_CHANGER = 0x30, // IOCTL_CHANGER_BASE
            FILE_DEVICE_SMARTCARD = 0x31,
            FILE_DEVICE_ACPI = 0x32,
            FILE_DEVICE_DVD = 0x33,
            FILE_DEVICE_FULLSCREEN_VIDEO = 0x34,
            FILE_DEVICE_DFS_FILE_SYSTEM = 0x35,
            FILE_DEVICE_DFS_VOLUME = 0x36,
            FILE_DEVICE_SERENUM = 0x37,
            FILE_DEVICE_TERMSRV = 0x38,
            FILE_DEVICE_KSEC = 0x39,
            FILE_DEVICE_FIPS = 0x3A,
            FILE_DEVICE_INFINIBAND = 0x3B,
            FILE_DEVICE_VMBUS = 0x3E,
            FILE_DEVICE_CRYPT_PROVIDER = 0x3F,
            FILE_DEVICE_WPD = 0x40,
            FILE_DEVICE_BLUETOOTH = 0x41,
            FILE_DEVICE_MT_COMPOSITE = 0x42,
            FILE_DEVICE_MT_TRANSPORT = 0x43,
            FILE_DEVICE_BIOMETRIC = 0x44,
            FILE_DEVICE_PMI = 0x45,
            FILE_DEVICE_EHSTOR = 0x46,
            FILE_DEVICE_DEVAPI = 0x47,
            FILE_DEVICE_GPIO = 0x48,
            FILE_DEVICE_USBEX = 0x49,
            FILE_DEVICE_CONSOLE = 0x50,
            FILE_DEVICE_NFP = 0x51,
            FILE_DEVICE_SYSENV = 0x52,
            FILE_DEVICE_VIRTUAL_BLOCK = 0x53,
            FILE_DEVICE_POINT_OF_SERVICE = 0x54,
            FILE_DEVICE_STORAGE_REPLICATION = 0x55,
            FILE_DEVICE_TRUST_ENV = 0x56 // IOCTL_VOLUME_BASE
        }

        public enum SYSTEM_INFORMATION_CLASS
        {
            SystemBasicInformation = 0x0000,
            SystemProcessorInformation = 0x0001,
            SystemPerformanceInformation = 0x0002,
            SystemTimeOfDayInformation = 0x0003,
            SystemPathInformation = 0x0004,
            SystemProcessInformation = 0x0005,
            SystemCallCountInformation = 0x0006,
            SystemDeviceInformation = 0x0007,
            SystemProcessorPerformanceInformation = 0x0008,
            SystemFlagsInformation = 0x0009,
            SystemCallTimeInformation = 0x000A,
            SystemModuleInformation = 0x000B,
            SystemLocksInformation = 0x000C,
            SystemStackTraceInformation = 0x000D,
            SystemPagedPoolInformation = 0x000E,
            SystemNonPagedPoolInformation = 0x000F,
            SystemHandleInformation = 0x0010,
            SystemObjectInformation = 0x0011,
            SystemPageFileInformation = 0x0012,
            SystemVdmInstemulInformation = 0x0013,
            SystemVdmBopInformation = 0x0014,
            SystemFileCacheInformation = 0x0015,
            SystemPoolTagInformation = 0x0016,
            SystemInterruptInformation = 0x0017,
            SystemDpcBehaviorInformation = 0x0018,
            SystemFullMemoryInformation = 0x0019,
            SystemLoadGdiDriverInformation = 0x001A,
            SystemUnloadGdiDriverInformation = 0x001B,
            SystemTimeAdjustmentInformation = 0x001C,
            SystemSummaryMemoryInformation = 0x001D,
            SystemMirrorMemoryInformation = 0x001E,
            SystemPerformanceTraceInformation = 0x001F,
            SystemCrashDumpInformation = 0x0020,
            SystemExceptionInformation = 0x0021,
            SystemCrashDumpStateInformation = 0x0022,
            SystemKernelDebuggerInformation = 0x0023,
            SystemContextSwitchInformation = 0x0024,
            SystemRegistryQuotaInformation = 0x0025,
            SystemExtendServiceTableInformation = 0x0026,
            SystemPrioritySeperation = 0x0027,
            SystemVerifierAddDriverInformation = 0x0028,
            SystemVerifierRemoveDriverInformation = 0x0029,
            SystemProcessorIdleInformation = 0x002A,
            SystemLegacyDriverInformation = 0x002B,
            SystemCurrentTimeZoneInformation = 0x002C,
            SystemLookasideInformation = 0x002D,
            SystemTimeSlipNotification = 0x002E,
            SystemSessionCreate = 0x002F,
            SystemSessionDetach = 0x0030,
            SystemSessionInformation = 0x0031,
            SystemRangeStartInformation = 0x0032,
            SystemVerifierInformation = 0x0033,
            SystemVerifierThunkExtend = 0x0034,
            SystemSessionProcessInformation = 0x0035,
            SystemLoadGdiDriverInSystemSpace = 0x0036,
            SystemNumaProcessorMap = 0x0037,
            SystemPrefetcherInformation = 0x0038,
            SystemExtendedProcessInformation = 0x0039,
            SystemRecommendedSharedDataAlignment = 0x003A,
            SystemComPlusPackage = 0x003B,
            SystemNumaAvailableMemory = 0x003C,
            SystemProcessorPowerInformation = 0x003D,
            SystemEmulationBasicInformation = 0x003E,
            SystemEmulationProcessorInformation = 0x003F,
            SystemExtendedHandleInformation = 0x0040,
            SystemLostDelayedWriteInformation = 0x0041,
            SystemBigPoolInformation = 0x0042,
            SystemSessionPoolTagInformation = 0x0043,
            SystemSessionMappedViewInformation = 0x0044,
            SystemHotpatchInformation = 0x0045,
            SystemObjectSecurityMode = 0x0046,
            SystemWatchdogTimerHandler = 0x0047,
            SystemWatchdogTimerInformation = 0x0048,
            SystemLogicalProcessorInformation = 0x0049,
            SystemWow64SharedInformationObsolete = 0x004A,
            SystemRegisterFirmwareTableInformationHandler = 0x004B,
            SystemFirmwareTableInformation = 0x004C,
            SystemModuleInformationEx = 0x004D,
            SystemVerifierTriageInformation = 0x004E,
            SystemSuperfetchInformation = 0x004F,
            SystemMemoryListInformation = 0x0050, // SYSTEM_MEMORY_LIST_INFORMATION
            SystemFileCacheInformationEx = 0x0051,
            SystemThreadPriorityClientIdInformation = 0x0052,
            SystemProcessorIdleCycleTimeInformation = 0x0053,
            SystemVerifierCancellationInformation = 0x0054,
            SystemProcessorPowerInformationEx = 0x0055,
            SystemRefTraceInformation = 0x0056,
            SystemSpecialPoolInformation = 0x0057,
            SystemProcessIdInformation = 0x0058,
            SystemErrorPortInformation = 0x0059,
            SystemBootEnvironmentInformation = 0x005A,
            SystemHypervisorInformation = 0x005B,
            SystemVerifierInformationEx = 0x005C,
            SystemTimeZoneInformation = 0x005D,
            SystemImageFileExecutionOptionsInformation = 0x005E,
            SystemCoverageInformation = 0x005F,
            SystemPrefetchPatchInformation = 0x0060,
            SystemVerifierFaultsInformation = 0x0061,
            SystemSystemPartitionInformation = 0x0062,
            SystemSystemDiskInformation = 0x0063,
            SystemProcessorPerformanceDistribution = 0x0064,
            SystemNumaProximityNodeInformation = 0x0065,
            SystemDynamicTimeZoneInformation = 0x0066,
            SystemCodeIntegrityInformation = 0x0067,
            SystemProcessorMicrocodeUpdateInformation = 0x0068,
            SystemProcessorBrandString = 0x0069,
            SystemVirtualAddressInformation = 0x006A,
            SystemLogicalProcessorAndGroupInformation = 0x006B,
            SystemProcessorCycleTimeInformation = 0x006C,
            SystemStoreInformation = 0x006D,
            SystemRegistryAppendString = 0x006E,
            SystemAitSamplingValue = 0x006F,
            SystemVhdBootInformation = 0x0070,
            SystemCpuQuotaInformation = 0x0071,
            SystemNativeBasicInformation = 0x0072,
            SystemErrorPortTimeouts = 0x0073,
            SystemLowPriorityIoInformation = 0x0074,
            SystemBootEntropyInformation = 0x0075,
            SystemVerifierCountersInformation = 0x0076,
            SystemPagedPoolInformationEx = 0x0077,
            SystemSystemPtesInformationEx = 0x0078,
            SystemNodeDistanceInformation = 0x0079,
            SystemAcpiAuditInformation = 0x007A,
            SystemBasicPerformanceInformation = 0x007B,
            SystemQueryPerformanceCounterInformation = 0x007C,
            SystemSessionBigPoolInformation = 0x007D,
            SystemBootGraphicsInformation = 0x007E,
            SystemScrubPhysicalMemoryInformation = 0x007F,
            SystemBadPageInformation = 0x0080,
            SystemProcessorProfileControlArea = 0x0081,
            SystemCombinePhysicalMemoryInformation = 0x0082,
            SystemEntropyInterruptTimingInformation = 0x0083,
            SystemConsoleInformation = 0x0084,
            SystemPlatformBinaryInformation = 0x0085,
            SystemThrottleNotificationInformation = 0x0086,
            SystemHypervisorProcessorCountInformation = 0x0087,
            SystemDeviceDataInformation = 0x0088,
            SystemDeviceDataEnumerationInformation = 0x0089,
            SystemMemoryTopologyInformation = 0x008A,
            SystemMemoryChannelInformation = 0x008B,
            SystemBootLogoInformation = 0x008C,
            SystemProcessorPerformanceInformationEx = 0x008D,
            SystemSpare0 = 0x008E,
            SystemSecureBootPolicyInformation = 0x008F,
            SystemPageFileInformationEx = 0x0090,
            SystemSecureBootInformation = 0x0091,
            SystemEntropyInterruptTimingRawInformation = 0x0092,
            SystemPortableWorkspaceEfiLauncherInformation = 0x0093,
            SystemFullProcessInformation = 0x0094,
            MaxSystemInfoClass = 0x0095
        }

        public enum NtStatus : uint
        {
            // Success
            Success = 0x00000000,
            Wait0 = 0x00000000,
            Wait1 = 0x00000001,
            Wait2 = 0x00000002,
            Wait3 = 0x00000003,
            Wait63 = 0x0000003f,
            Abandoned = 0x00000080,
            AbandonedWait0 = 0x00000080,
            AbandonedWait1 = 0x00000081,
            AbandonedWait2 = 0x00000082,
            AbandonedWait3 = 0x00000083,
            AbandonedWait63 = 0x000000bf,
            UserApc = 0x000000c0,
            KernelApc = 0x00000100,
            Alerted = 0x00000101,
            Timeout = 0x00000102,
            Pending = 0x00000103,
            Reparse = 0x00000104,
            MoreEntries = 0x00000105,
            NotAllAssigned = 0x00000106,
            SomeNotMapped = 0x00000107,
            OpLockBreakInProgress = 0x00000108,
            VolumeMounted = 0x00000109,
            RxActCommitted = 0x0000010a,
            NotifyCleanup = 0x0000010b,
            NotifyEnumDir = 0x0000010c,
            NoQuotasForAccount = 0x0000010d,
            PrimaryTransportConnectFailed = 0x0000010e,
            PageFaultTransition = 0x00000110,
            PageFaultDemandZero = 0x00000111,
            PageFaultCopyOnWrite = 0x00000112,
            PageFaultGuardPage = 0x00000113,
            PageFaultPagingFile = 0x00000114,
            CrashDump = 0x00000116,
            ReparseObject = 0x00000118,
            NothingToTerminate = 0x00000122,
            ProcessNotInJob = 0x00000123,
            ProcessInJob = 0x00000124,
            ProcessCloned = 0x00000129,
            FileLockedWithOnlyReaders = 0x0000012a,
            FileLockedWithWriters = 0x0000012b,

            // Informational
            Informational = 0x40000000,
            ObjectNameExists = 0x40000000,
            ThreadWasSuspended = 0x40000001,
            WorkingSetLimitRange = 0x40000002,
            ImageNotAtBase = 0x40000003,
            RegistryRecovered = 0x40000009,

            // Warning
            Warning = 0x80000000,
            GuardPageViolation = 0x80000001,
            DatatypeMisalignment = 0x80000002,
            Breakpoint = 0x80000003,
            SingleStep = 0x80000004,
            BufferOverflow = 0x80000005,
            NoMoreFiles = 0x80000006,
            HandlesClosed = 0x8000000a,
            PartialCopy = 0x8000000d,
            DeviceBusy = 0x80000011,
            InvalidEaName = 0x80000013,
            EaListInconsistent = 0x80000014,
            NoMoreEntries = 0x8000001a,
            LongJump = 0x80000026,
            DllMightBeInsecure = 0x8000002b,

            // Error
            Error = 0xc0000000,
            Unsuccessful = 0xc0000001,
            NotImplemented = 0xc0000002,
            InvalidInfoClass = 0xc0000003,
            InfoLengthMismatch = 0xc0000004,
            AccessViolation = 0xc0000005,
            InPageError = 0xc0000006,
            PagefileQuota = 0xc0000007,
            InvalidHandle = 0xc0000008,
            BadInitialStack = 0xc0000009,
            BadInitialPc = 0xc000000a,
            InvalidCid = 0xc000000b,
            TimerNotCanceled = 0xc000000c,
            InvalidParameter = 0xc000000d,
            NoSuchDevice = 0xc000000e,
            NoSuchFile = 0xc000000f,
            InvalidDeviceRequest = 0xc0000010,
            EndOfFile = 0xc0000011,
            WrongVolume = 0xc0000012,
            NoMediaInDevice = 0xc0000013,
            NoMemory = 0xc0000017,
            NotMappedView = 0xc0000019,
            UnableToFreeVm = 0xc000001a,
            UnableToDeleteSection = 0xc000001b,
            IllegalInstruction = 0xc000001d,
            AlreadyCommitted = 0xc0000021,
            AccessDenied = 0xc0000022,
            BufferTooSmall = 0xc0000023,
            ObjectTypeMismatch = 0xc0000024,
            NonContinuableException = 0xc0000025,
            BadStack = 0xc0000028,
            NotLocked = 0xc000002a,
            NotCommitted = 0xc000002d,
            InvalidParameterMix = 0xc0000030,
            ObjectNameInvalid = 0xc0000033,
            ObjectNameNotFound = 0xc0000034,
            ObjectNameCollision = 0xc0000035,
            ObjectPathInvalid = 0xc0000039,
            ObjectPathNotFound = 0xc000003a,
            ObjectPathSyntaxBad = 0xc000003b,
            DataOverrun = 0xc000003c,
            DataLate = 0xc000003d,
            DataError = 0xc000003e,
            CrcError = 0xc000003f,
            SectionTooBig = 0xc0000040,
            PortConnectionRefused = 0xc0000041,
            InvalidPortHandle = 0xc0000042,
            SharingViolation = 0xc0000043,
            QuotaExceeded = 0xc0000044,
            InvalidPageProtection = 0xc0000045,
            MutantNotOwned = 0xc0000046,
            SemaphoreLimitExceeded = 0xc0000047,
            PortAlreadySet = 0xc0000048,
            SectionNotImage = 0xc0000049,
            SuspendCountExceeded = 0xc000004a,
            ThreadIsTerminating = 0xc000004b,
            BadWorkingSetLimit = 0xc000004c,
            IncompatibleFileMap = 0xc000004d,
            SectionProtection = 0xc000004e,
            EasNotSupported = 0xc000004f,
            EaTooLarge = 0xc0000050,
            NonExistentEaEntry = 0xc0000051,
            NoEasOnFile = 0xc0000052,
            EaCorruptError = 0xc0000053,
            FileLockConflict = 0xc0000054,
            LockNotGranted = 0xc0000055,
            DeletePending = 0xc0000056,
            CtlFileNotSupported = 0xc0000057,
            UnknownRevision = 0xc0000058,
            RevisionMismatch = 0xc0000059,
            InvalidOwner = 0xc000005a,
            InvalidPrimaryGroup = 0xc000005b,
            NoImpersonationToken = 0xc000005c,
            CantDisableMandatory = 0xc000005d,
            NoLogonServers = 0xc000005e,
            NoSuchLogonSession = 0xc000005f,
            NoSuchPrivilege = 0xc0000060,
            PrivilegeNotHeld = 0xc0000061,
            InvalidAccountName = 0xc0000062,
            UserExists = 0xc0000063,
            NoSuchUser = 0xc0000064,
            GroupExists = 0xc0000065,
            NoSuchGroup = 0xc0000066,
            MemberInGroup = 0xc0000067,
            MemberNotInGroup = 0xc0000068,
            LastAdmin = 0xc0000069,
            WrongPassword = 0xc000006a,
            IllFormedPassword = 0xc000006b,
            PasswordRestriction = 0xc000006c,
            LogonFailure = 0xc000006d,
            AccountRestriction = 0xc000006e,
            InvalidLogonHours = 0xc000006f,
            InvalidWorkstation = 0xc0000070,
            PasswordExpired = 0xc0000071,
            AccountDisabled = 0xc0000072,
            NoneMapped = 0xc0000073,
            TooManyLuidsRequested = 0xc0000074,
            LuidsExhausted = 0xc0000075,
            InvalidSubAuthority = 0xc0000076,
            InvalidAcl = 0xc0000077,
            InvalidSid = 0xc0000078,
            InvalidSecurityDescr = 0xc0000079,
            ProcedureNotFound = 0xc000007a,
            InvalidImageFormat = 0xc000007b,
            NoToken = 0xc000007c,
            BadInheritanceAcl = 0xc000007d,
            RangeNotLocked = 0xc000007e,
            DiskFull = 0xc000007f,
            ServerDisabled = 0xc0000080,
            ServerNotDisabled = 0xc0000081,
            TooManyGuidsRequested = 0xc0000082,
            GuidsExhausted = 0xc0000083,
            InvalidIdAuthority = 0xc0000084,
            AgentsExhausted = 0xc0000085,
            InvalidVolumeLabel = 0xc0000086,
            SectionNotExtended = 0xc0000087,
            NotMappedData = 0xc0000088,
            ResourceDataNotFound = 0xc0000089,
            ResourceTypeNotFound = 0xc000008a,
            ResourceNameNotFound = 0xc000008b,
            ArrayBoundsExceeded = 0xc000008c,
            FloatDenormalOperand = 0xc000008d,
            FloatDivideByZero = 0xc000008e,
            FloatInexactResult = 0xc000008f,
            FloatInvalidOperation = 0xc0000090,
            FloatOverflow = 0xc0000091,
            FloatStackCheck = 0xc0000092,
            FloatUnderflow = 0xc0000093,
            IntegerDivideByZero = 0xc0000094,
            IntegerOverflow = 0xc0000095,
            PrivilegedInstruction = 0xc0000096,
            TooManyPagingFiles = 0xc0000097,
            FileInvalid = 0xc0000098,
            InstanceNotAvailable = 0xc00000ab,
            PipeNotAvailable = 0xc00000ac,
            InvalidPipeState = 0xc00000ad,
            PipeBusy = 0xc00000ae,
            IllegalFunction = 0xc00000af,
            PipeDisconnected = 0xc00000b0,
            PipeClosing = 0xc00000b1,
            PipeConnected = 0xc00000b2,
            PipeListening = 0xc00000b3,
            InvalidReadMode = 0xc00000b4,
            IoTimeout = 0xc00000b5,
            FileForcedClosed = 0xc00000b6,
            ProfilingNotStarted = 0xc00000b7,
            ProfilingNotStopped = 0xc00000b8,
            NotSameDevice = 0xc00000d4,
            FileRenamed = 0xc00000d5,
            CantWait = 0xc00000d8,
            PipeEmpty = 0xc00000d9,
            CantTerminateSelf = 0xc00000db,
            InternalError = 0xc00000e5,
            InvalidParameter1 = 0xc00000ef,
            InvalidParameter2 = 0xc00000f0,
            InvalidParameter3 = 0xc00000f1,
            InvalidParameter4 = 0xc00000f2,
            InvalidParameter5 = 0xc00000f3,
            InvalidParameter6 = 0xc00000f4,
            InvalidParameter7 = 0xc00000f5,
            InvalidParameter8 = 0xc00000f6,
            InvalidParameter9 = 0xc00000f7,
            InvalidParameter10 = 0xc00000f8,
            InvalidParameter11 = 0xc00000f9,
            InvalidParameter12 = 0xc00000fa,
            MappedFileSizeZero = 0xc000011e,
            TooManyOpenedFiles = 0xc000011f,
            Cancelled = 0xc0000120,
            CannotDelete = 0xc0000121,
            InvalidComputerName = 0xc0000122,
            FileDeleted = 0xc0000123,
            SpecialAccount = 0xc0000124,
            SpecialGroup = 0xc0000125,
            SpecialUser = 0xc0000126,
            MembersPrimaryGroup = 0xc0000127,
            FileClosed = 0xc0000128,
            TooManyThreads = 0xc0000129,
            ThreadNotInProcess = 0xc000012a,
            TokenAlreadyInUse = 0xc000012b,
            PagefileQuotaExceeded = 0xc000012c,
            CommitmentLimit = 0xc000012d,
            InvalidImageLeFormat = 0xc000012e,
            InvalidImageNotMz = 0xc000012f,
            InvalidImageProtect = 0xc0000130,
            InvalidImageWin16 = 0xc0000131,
            LogonServer = 0xc0000132,
            DifferenceAtDc = 0xc0000133,
            SynchronizationRequired = 0xc0000134,
            DllNotFound = 0xc0000135,
            IoPrivilegeFailed = 0xc0000137,
            OrdinalNotFound = 0xc0000138,
            EntryPointNotFound = 0xc0000139,
            ControlCExit = 0xc000013a,
            PortNotSet = 0xc0000353,
            DebuggerInactive = 0xc0000354,
            CallbackBypass = 0xc0000503,
            PortClosed = 0xc0000700,
            MessageLost = 0xc0000701,
            InvalidMessage = 0xc0000702,
            RequestCanceled = 0xc0000703,
            RecursiveDispatch = 0xc0000704,
            LpcReceiveBufferExpected = 0xc0000705,
            LpcInvalidConnectionUsage = 0xc0000706,
            LpcRequestsNotAllowed = 0xc0000707,
            ResourceInUse = 0xc0000708,
            ProcessIsProtected = 0xc0000712,
            VolumeDirty = 0xc0000806,
            FileCheckedOut = 0xc0000901,
            CheckOutRequired = 0xc0000902,
            BadFileType = 0xc0000903,
            FileTooLarge = 0xc0000904,
            FormsAuthRequired = 0xc0000905,
            VirusInfected = 0xc0000906,
            VirusDeleted = 0xc0000907,
            TransactionalConflict = 0xc0190001,
            InvalidTransaction = 0xc0190002,
            TransactionNotActive = 0xc0190003,
            TmInitializationFailed = 0xc0190004,
            RmNotActive = 0xc0190005,
            RmMetadataCorrupt = 0xc0190006,
            TransactionNotJoined = 0xc0190007,
            DirectoryNotRm = 0xc0190008,
            CouldNotResizeLog = 0xc0190009,
            TransactionsUnsupportedRemote = 0xc019000a,
            LogResizeInvalidSize = 0xc019000b,
            RemoteFileVersionMismatch = 0xc019000c,
            CrmProtocolAlreadyExists = 0xc019000f,
            TransactionPropagationFailed = 0xc0190010,
            CrmProtocolNotFound = 0xc0190011,
            TransactionSuperiorExists = 0xc0190012,
            TransactionRequestNotValid = 0xc0190013,
            TransactionNotRequested = 0xc0190014,
            TransactionAlreadyAborted = 0xc0190015,
            TransactionAlreadyCommitted = 0xc0190016,
            TransactionInvalidMarshallBuffer = 0xc0190017,
            CurrentTransactionNotValid = 0xc0190018,
            LogGrowthFailed = 0xc0190019,
            ObjectNoLongerExists = 0xc0190021,
            StreamMiniversionNotFound = 0xc0190022,
            StreamMiniversionNotValid = 0xc0190023,
            MiniversionInaccessibleFromSpecifiedTransaction = 0xc0190024,
            CantOpenMiniversionWithModifyIntent = 0xc0190025,
            CantCreateMoreStreamMiniversions = 0xc0190026,
            HandleNoLongerValid = 0xc0190028,
            NoTxfMetadata = 0xc0190029,
            LogCorruptionDetected = 0xc0190030,
            CantRecoverWithHandleOpen = 0xc0190031,
            RmDisconnected = 0xc0190032,
            EnlistmentNotSuperior = 0xc0190033,
            RecoveryNotNeeded = 0xc0190034,
            RmAlreadyStarted = 0xc0190035,
            FileIdentityNotPersistent = 0xc0190036,
            CantBreakTransactionalDependency = 0xc0190037,
            CantCrossRmBoundary = 0xc0190038,
            TxfDirNotEmpty = 0xc0190039,
            IndoubtTransactionsExist = 0xc019003a,
            TmVolatile = 0xc019003b,
            RollbackTimerExpired = 0xc019003c,
            TxfAttributeCorrupt = 0xc019003d,
            EfsNotAllowedInTransaction = 0xc019003e,
            TransactionalOpenNotAllowed = 0xc019003f,
            TransactedMappingUnsupportedRemote = 0xc0190040,
            TxfMetadataAlreadyPresent = 0xc0190041,
            TransactionScopeCallbacksNotSet = 0xc0190042,
            TransactionRequiredPromotion = 0xc0190043,
            CannotExecuteFileInTransaction = 0xc0190044,
            TransactionsNotFrozen = 0xc0190045,

            MaximumNtStatus = 0xffffffff
        }

        public enum MachineType : ushort
        {
            Native = 0,
            I386 = 0x014c,
            Itanium = 0x0200,
            x64 = 0x8664
        }

        public enum MagicType : ushort
        {
            IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b,
            IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b
        }

        public enum SubSystemType : ushort
        {
            IMAGE_SUBSYSTEM_UNKNOWN = 0,
            IMAGE_SUBSYSTEM_NATIVE = 1,
            IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
            IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
            IMAGE_SUBSYSTEM_POSIX_CUI = 7,
            IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
            IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
            IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
            IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
            IMAGE_SUBSYSTEM_EFI_ROM = 13,
            IMAGE_SUBSYSTEM_XBOX = 14

        }

        public enum DllCharacteristicsType : ushort
        {
            RES_0 = 0x0001,
            RES_1 = 0x0002,
            RES_2 = 0x0004,
            RES_3 = 0x0008,
            IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE = 0x0040,
            IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
            IMAGE_DLL_CHARACTERISTICS_NX_COMPAT = 0x0100,
            IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
            IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
            IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
            RES_4 = 0x1000,
            IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
            IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
        }





        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct ENUM_SERVICE_STATUS
        {
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string pServiceName;

            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string pDisplayName;

            public SERVICE_STATUS ServiceStatus;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct SERVICE_STATUS
        {
            public SERVICE_TYPES dwServiceType;
            public SERVICE_STATE dwCurrentState;
            public uint dwControlsAccepted;
            public uint dwWin32ExitCode;
            public uint dwServiceSpecificExitCode;
            public uint dwCheckPoint;
            public uint dwWaitHint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            public ushort e_magic;    // Magic number
            public ushort e_cblp;     // Bytes on last page of file
            public ushort e_cp;       // Pages in file
            public ushort e_crlc;     // Relocations
            public ushort e_cparhdr;  // Size of header in paragraphs
            public ushort e_minalloc; // Minimum extra paragraphs needed
            public ushort e_maxalloc; // Maximum extra paragraphs needed
            public ushort e_ss;       // Initial (relative) SS value
            public ushort e_sp;       // Initial SP value
            public ushort e_csum;     // Checksum
            public ushort e_ip;       // Initial IP value
            public ushort e_cs;       // Initial (relative) CS value
            public ushort e_lfarlc;   // File address of relocation table
            public ushort e_ovno;     // Overlay number
            public uint e_res1;       // Reserved
            public uint e_res2;       // Reserved
            public ushort e_oemid;    // OEM identifier (for e_oeminfo)
            public ushort e_oeminfo;  // OEM information; e_oemid specific
            public uint e_res3;       // Reserved
            public uint e_res4;       // Reserved
            public uint e_res5;       // Reserved
            public uint e_res6;       // Reserved
            public uint e_res7;       // Reserved
            public int e_lfanew;      // File address of new exe header
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS_COMMON
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS32
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS64
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            public uint ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public ulong ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public ulong SizeOfStackReserve;
            public ulong SizeOfStackCommit;
            public ulong SizeOfHeapReserve;
            public ulong SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
        }

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
        }
    }
}
