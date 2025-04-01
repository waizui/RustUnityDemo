using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class LibLoader
{

    const string DLL_NAME = "csbindgenlib";

    private static IntPtr libHandle = IntPtr.Zero;

    public static bool isLoaded
    {
        get
        {
            return libHandle != IntPtr.Zero;
        }
    }

    public static string libName
    {
        // getter
        get
        {
#if UNITY_EDITOR_WIN
            return  DLL_NAME+".dll";
#elif UNITY_EDITOR_OSX
           return DLL_NAME+ ".dylib";
#elif UNITY_EDITOR_LINUX
           return DLL_NAME + ".so";
#else
            throw new PlatformNotSupportedException("Platform not supported");
#endif
        }

    }

    public static string libPath
    {
        get
        {
            // https://docs.unity3d.com/Manual/StreamingAssets.html
            // put libs here preventing unity compile them automatically
            return Application.dataPath + "/StreamingAssets/" + libName;
        }
    }


    static LibLoader()
    {
        if (!isLoaded)
        {
            LoadLib();
        }
    }

    public static bool LoadLib()
    {
        var libPath = LibLoader.libPath;
        if (!System.IO.File.Exists(libPath))
        {
            Debug.LogError("lib not found: " + libPath);
            return false;
        }

        var handle = OpenLib(libPath);
        if (handle == IntPtr.Zero)
        {
            Debug.LogError("failed to load lib: " + libPath);
            return false;
        }
        libHandle = handle;
        return true;
    }

    public static void FreeLib()
    {
        if (!isLoaded)
        {
            return;
        }

        var libPath = LibLoader.libPath;
        var ok = !CloseLib(libHandle);
        if (!ok)
        {
            Debug.LogError("failed to free lib: " + libPath);
        }

        libHandle = IntPtr.Zero;
    }

    public static IntPtr GetFunctionPointer(string functionName)
    {
        if (!isLoaded)
        {
            return IntPtr.Zero;
        }

#if UNITY_EDITOR_WIN
        return GetProcAddress(libHandle, functionName);
#else
        return dlsym(libHandle, functionName);
#endif

    }

    private static IntPtr OpenLib(string lpFileName)
    {

#if UNITY_EDITOR_WIN
        return LoadLibrary(lpFileName);
#else
        return dlopen(lpFileName, RTLD_NOW);
#endif
    }

    private static bool CloseLib(IntPtr hModule)
    {
#if UNITY_EDITOR_WIN
        return FreeLibrary(hModule);
#else
        return dlclose(hModule) == 0;
#endif
    }

    #region platform specific functions
#if UNITY_EDITOR_WIN
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr LoadLibrary(string lpFileName);
    
    // https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-getprocaddress
    [DllImport("kernel32", SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr hModule);
#else
    [DllImport("libdl.dylib", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.dylib", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.dylib", CallingConvention = CallingConvention.Cdecl)]
    private static extern int dlclose(IntPtr handle);

    [DllImport("libdl.dylib", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr dlerror();

    private const int RTLD_NOW = 2;
#endif

    #endregion

}