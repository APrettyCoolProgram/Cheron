// Dictionary utilities [151123]

using System.Collections.Generic;
using System.IO;

namespace TheCards
{
    class DoDictionary
    {
        /// <summary>Combine multiple string dictionaries.</summary>
        /// <param name="dictionariesToCombine">A list of dictionaries to combine.</param>
        /// <returns>The combined dictionary.</returns>
        /// <remarks>
        /// Overview
        /// -------
        /// Combines multiple string dictionaries into one string dictionary, then returns that dictionary. The current
        /// version of this method (151123) only allows the combination of string dictionaries.
        /// 
        /// Local variables
        /// ---------------                                                    
        /// combinedDictionary: the dictionary that contains all of the other dictionaries
        /// 
        /// Details
        /// -------
        /// [A] Loop through the each dictionary of the list that was passed.
        /// [B] Loop through each of the items in the current dictionary, and add the key/value pair to the
        ///     combined dictionary that will be returned.
        /// </remarks>
        public static Dictionary<string, string> CombineStringDictionaries(List<Dictionary<string, string>> dictionariesToCombine)
        {
            var combinedDictionary = new Dictionary<string, string>();
            foreach (var individualDictionary in dictionariesToCombine)                                             //[A]
            {
                foreach (var item in individualDictionary)                                                          //[B]
                {
                    combinedDictionary.Add(item.Key, item.Value);
                }
            }
            return combinedDictionary;
        }
    }
}