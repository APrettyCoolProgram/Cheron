// Common utilities [151123]

using System.IO;

namespace TheCards
{
    class DoCommon
    {
        /// <summary>Get the bit level of the OS.</summary>
        /// <returns>The bit-level of the OS, 32 or 64.</returns>
        /// <remarks>If "Program Files (x86)" exists, return 64, otherwise return 32.</remarks>
        public static int GetOSBits()
        {
            if (Directory.Exists(@"C:\Program Files (x86)"))
            {
                return 64;
            }
            else
            {
                return 32;
            }
        }

        /// <summary>Get the root path for APCP applications.</summary>
        /// <returns>The root path for APCP applications.</returns>
        /// <remarks>Builds the root path to APCP folder, depending on the bit-level of the OS.</remarks>
        public static string GetPathAPCP()
        {
            if (GetOSBits() == 64)
            {
                return @"C:\Program Files\APCP\";
            }
            else
            {
                return @"C:\Program Files (x86)\APCP\";
            }
        }
    }
}