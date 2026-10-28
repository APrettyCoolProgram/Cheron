// String utilities [151123]

using System;

namespace TheCards
{
    class DoString
    {
        /// <summary>Determines if a string meets various requirements.</summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="ignoreEmpties">Option to ignore empty strings [true/false].</param>
        /// <param name="ignoreComments">Option to ignore comment strings [true/false].</param>
        /// <param name="commentChar">The character that determines if a string is a comment ["#"].</param>
        /// <returns>True/false, depending on if the string meets the requirements.</returns>
        /// <remarks>
        /// Overview
        /// --------
        /// Verifies if the passed string meets various requirements, and returns true (it does) or false (it doesn't).
        /// Checks to make sure a string meets various requirements. Currently only checks to see if comment strings
        /// AND empty strings are ignored, not comment strings OR empty strings. ***NOTE*** This should eventually  be fixed.
        /// 
        /// Details
        /// -------
        /// [A] If we are ignoring comments AND the string starts with the comment character OR we are ignoring empty
        ///     lines AND the line is empty, then the string fails to meet the requirements, so return false. Otherwise
        ///     the string meets the requirements, so return true.
        /// </remarks>
        public static bool MeetRequirements(string stringToCheck, bool ignoreEmpties, bool ignoreComments, char commentChar)
        {
            if ((ignoreComments && stringToCheck.StartsWith(commentChar.ToString())) || (ignoreEmpties && (stringToCheck == String.Empty))) //[A]
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}