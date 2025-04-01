using System;
using System.Runtime.InteropServices;

namespace CsBindgen
{
    internal static unsafe partial class NativeMethods
    {
        // can be moved to a separate file for succinctness
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int AddDelegate(int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void ArrayDelegate(float* x, int y, float z);

        private static AddDelegate my_add_delegate;

        private static ArrayDelegate process_float_arr_delegate;

        internal static int my_add(int x, int y)
        {
            if (my_add_delegate == null)
            {
                var handle = LibLoader.GetFunctionPointer("my_add");
                my_add_delegate = Marshal.GetDelegateForFunctionPointer<AddDelegate>(handle);
            }
            return my_add_delegate(x, y);
        }

        internal static void process_float_arr(float* floats_ptr, int count, float time)
        {
            if (process_float_arr_delegate == null)
            {
                var handle = LibLoader.GetFunctionPointer("process_float_arr");
                process_float_arr_delegate = Marshal.GetDelegateForFunctionPointer<ArrayDelegate>(handle);
            }
            process_float_arr_delegate(floats_ptr, count, time);
        }
        /*
        const string __DllName = "csbindgenlib";

        [DllImport(__DllName, EntryPoint = "my_add", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int my_add(int x, int y);

        [DllImport(__DllName, EntryPoint = "process_float_arr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void process_float_arr(float* floats_ptr, int count, float time);
        */

    }



}
