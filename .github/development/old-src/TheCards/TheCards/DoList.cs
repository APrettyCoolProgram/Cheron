// List utilities [151123]

using System.Collections.Generic;

namespace TheCards
{
    class DoList
    {
        /// <summary>Create a list of dictionary keys or values.</summary>
        /// <param name="aDictionary">The passed dictionary.</param>
        /// <param name="keyList">Determines if we are building a list of keys [true/false].</param>
        /// <returns>Return the list.</returns>
        /// <remarks>
        /// Overview
        /// --------
        /// Use this if you need to create a list of just the keys or just the values in a dictionary.
        /// 
        /// Local variables
        /// ---------------
        /// listOfValues: the list of keys or values
        /// 
        /// Details
        /// -------
        /// [A] Create working list, then loop through the items of the passed dictionary. If we are building a
        /// list of keys, add the key to the list, otherwise add the value to the list.
        /// </remarks>
        public static List<string> OfKeysOrValuesFromDictionary(Dictionary<string, string> aDictionary, bool keyList)
        {
            var listOfKeysOrValues = new List<string>(); //[A]
            foreach (var item in aDictionary)
            {
                if (keyList)
                {
                    listOfKeysOrValues.Add(item.Key);
                }
                else
                {
                    listOfKeysOrValues.Add(item.Value);
                }
            }
            return listOfKeysOrValues;
        }
    }
}