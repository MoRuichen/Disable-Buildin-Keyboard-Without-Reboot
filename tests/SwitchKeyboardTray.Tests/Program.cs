using System;

namespace SwitchKeyboardTray.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                Tests.RunAll();
                Console.WriteLine("All tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}
