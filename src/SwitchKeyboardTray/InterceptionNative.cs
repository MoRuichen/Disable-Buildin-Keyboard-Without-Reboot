using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SwitchKeyboardTray
{
    internal static class InterceptionNative
    {
        public delegate int InterceptionPredicate(int device);

        [StructLayout(LayoutKind.Sequential)]
        public struct InterceptionKeyStroke
        {
            public ushort Code;
            public ushort State;
            public uint Information;
        }

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr interception_create_context();

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void interception_destroy_context(IntPtr context);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void interception_set_filter(IntPtr context, InterceptionPredicate predicate, ushort filter);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int interception_wait_with_timeout(IntPtr context, uint milliseconds);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int interception_receive(IntPtr context, int device, ref InterceptionKeyStroke stroke, uint strokeCount);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int interception_send(IntPtr context, int device, ref InterceptionKeyStroke stroke, uint strokeCount);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint interception_get_hardware_id(IntPtr context, int device, StringBuilder hardwareIdBuffer, uint bufferSize);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int interception_is_invalid(int device);

        [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int interception_is_keyboard(int device);

        public static readonly InterceptionPredicate KeyboardPredicate = IsKeyboardDevice;

        public static bool IsKeyboard(int device)
        {
            return interception_is_keyboard(device) != 0;
        }

        public static bool IsInvalid(int device)
        {
            return interception_is_invalid(device) != 0;
        }

        private static int IsKeyboardDevice(int device)
        {
            return IsKeyboard(device) ? 1 : 0;
        }
    }
}
