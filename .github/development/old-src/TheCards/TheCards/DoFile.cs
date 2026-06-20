// File utilities [151123]

using System;
using System.Collections.Generic;
using System.IO;

namespace TheCards
{
    class DoFile
    {
        /// <summary>Delete a file.</summary>
        /// <param name="filename">Filename to delete [full path].</param>
        /// <remarks>
        /// Checks to make sure that the passed filename exists, and deletes the file if it does.
        /// </remarks>
        public static void Delete(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        /// <summary>Append a string to a file.</summary>
        /// <param name="filename">Filename to append to [full path].</param>
        /// <param name="stringToAppend">Text to append.</param>
        /// <remarks>
        /// Append the passed string to a file, then add a the correct newline character depending on the environment.
        /// </remarks>
        public static void AppendString(string filename, string stringToAppend)
        {
            File.AppendAllText(filename, stringToAppend + Environment.NewLine);
        }

        /// <summary>Count the number of lines in a file.</summary>
        /// <param name="filename">Filename to count [full path].</param>
        /// <param name="ignoreEmpties">Ignore empty lines [true/false].</param>
        /// <param name="ignoreComment">Ignore comments [true/false].</param>
        /// <param name="commentChar">The comment character ["#"].</param>
        /// <returns>The number of lines in a file.</returns>
        /// <remarks>
        /// Overview
        /// --------
        /// Verify that the current fileline meets the requirements, and count it if it does.
        /// 
        /// Local variables
        /// ---------------
        /// numLines: number of lines, starts at 0
        /// fileline: the fileline to read
        /// 
        /// Details
        /// -------
        /// [A] Read through the lines of a file, and if it meets the requirements increment the line counter.
        /// </remarks>
        public static int GetNumberOfLines(string filename, bool ignoreEmpties, bool ignoreComments, char commentChar)
        {
            int numLines = 0;
            string fileline;
            StreamReader fileToRead = new StreamReader(filename);
            while ((fileline = fileToRead.ReadLine()) != null)                                                      //[A]
            {
                if (DoString.MeetRequirements(fileline, ignoreEmpties, ignoreComments, commentChar))
                {
                    numLines++;
                }
            }
            fileToRead.Close();
            return numLines;
        }

        /// <summary>Put filelines into an array.</summary>
        /// <param name="filename">Filename to count [full path].</param>
        /// <param name="ignoreEmpties">Ignore empty lines [true/false].</param>
        /// <param name="ignoreComment">Ignore comments [true/false].</param>
        /// <param name="commentChar">The comment character ["#"].</param>
        /// <returns>A array containing the filelines.</returns>
        /// <remarks>
        /// Overview
        /// --------
        /// Verify the current fileline meets the requirements, and add it to the array if it does.
        /// 
        /// Local variables
        /// ---------------
        /// element: the element, starts at 0
        /// fileline: the line of the file
        /// arrayOfLines: the array of lines
        /// 
        /// Details
        /// -------
        /// [A] The array size will be the number of lines in the file we are working with.
        /// [B] Loop through the file and add the line to the array if it meets the requirements. Manually
        ///     increment the element.
        /// </remarks>
        public static String[] ToArray(string filename, bool ignoreEmpties, bool ignoreComments, char commentChar)
        {
            int element = 0;
            string fileline;
            string[] arrayOfLines = new string[(GetNumberOfLines(filename, true, true, '#'))];                      //[A]
            StreamReader fileToRead = new StreamReader(filename);
            while ((fileline = fileToRead.ReadLine()) != null)                                                      //[B]
            {
                if (DoString.MeetRequirements(fileline, ignoreEmpties, ignoreComments, commentChar))
                {
                    arrayOfLines[element] = fileline;
                    element++;
                }
            }
            fileToRead.Close();
            return arrayOfLines;
        }

        /// <summary>Put filelines into a list</summary>
        /// <param name="filename">Filename to count [full path].</param>
        /// <param name="ignoreEmpties">Ignore empty lines [true/false].</param>
        /// <param name="ignoreComment">Ignore comments [true/false].</param>
        /// <param name="commentChar">The comment character ["#"].</param>
        /// <returns>A list containing the filelines.</returns>
        /// <remarks>
        /// Overview
        /// --------
        /// Verify the current fileline meets the requirements, and if it does add it to the list.
        /// 
        /// Local variables
        /// ---------------
        /// listOfLines: the list of lines
        /// fileline: the line of the file
        /// 
        /// Details
        /// -------
        /// [A] Check to see if the file meets the requirements, and add to the list if it does.
        /// 
        /// </remarks>
        public static List<string> ToList(string filename, bool ignoreEmpties, bool ignoreComments, char commentChar)
        {
            List<string> listOfLines = new List<string>();
            string fileline;
            StreamReader fileToRead = new StreamReader(filename);
            while ((fileline = fileToRead.ReadLine()) != null)                                                      //[A]
            {
                if (DoString.MeetRequirements(fileline, ignoreEmpties, ignoreComments, commentChar))
                {
                    listOfLines.Add(fileline);
                }
            }
            fileToRead.Close();
            return listOfLines;
        }

        /// <summary>Create a dictionary from key/value pairs in a file.</summary>
        /// <param name="filename">Filename to count [full path].</param>
        /// <param name="delimitKeyVal">Delimit character single line pairs, empty for multi-line pairs ["=", ""]</param>
        /// <param name="ignoreEmpties">Ignore empty lines [true/false].</param>
        /// <param name="ignoreComments">Ignore comments [true/false].</param>
        /// <param name="commentChar">The comment character ["#"].</param>
        /// <param name="delimitList">Delimit character for strings that will be split into a list [".</param>
        /// <returns>A dictionary containing key/value pairs from the file.</returns>
        /// <remarks>
        /// Variables
        /// ---------
        /// isKey: switch for key/value pairs on subsequent lines, starts as true since keys always come first
        /// fileline: name of the file we are working with
        /// key: value of the key, starts blank
        /// keyValuePair: array to hold the key/value pairs
        /// dictionaryOfLines: The dictionary that will hold the data
        /// 
        /// Overview
        /// --------
        /// Verify that each line meets the requirements to be added to the dictionary, and if it does then determine
        /// the key/value pair and add it. This requires the file to store the key/value pairs either:
        ///   1) on a single line, seperated by a delimiter character (i.e. key=value)
        ///   2) on subsequent lines (i.e. key
        ///                                value
        /// A key/value if "NOT_USED" indicates that while the key/value pair should be added to the dictionary, it is
        /// most likely a placeholder key/value pair that will not be used. Since there may be multiple "NOT_USED" keys
        /// in a file, each has it's location in the file added to the key as a postfix (i.e. "NOT_USED4", "NOT_USED23").
        /// This functionaluty is primarily used in the APCP Configurator application.
        /// 
        /// Details
        /// -------
        /// [A] If a delimiter character was passed (i.e. it's not empty), the key value will be on the same line
        ///     seperated by the delimiter character, so split it at the delimiter. Otherwise the key/value pair will
        ///     be on multiple lines.
        /// [B] If the key is "NOT_USED", this is most likely a placeholder variable (see comments above). Otherwise,
        ///     add the key/value pair to the dictionary.
        /// [C] If the line is a key, put the line in a placeholder value. If the line isn't a key, put the key/value
        ///     pair into the dictionary. Either way, reverse the isKey flag.
        /// </remarks>
        public static Dictionary<string, string> ToDictionary(string filename, char delimitKeyVal, bool ignoreEmpties, bool ignoreComments, char commentChar)
        {
            bool isKey = true;
            string fileline;
            string key = "";
            string[] keyValuePair;
            var dictionaryOfLines = new Dictionary<string, string>();
            var fileToRead = new StreamReader(filename);
            while ((fileline = fileToRead.ReadLine()) != null)
            {
                if (DoString.MeetRequirements(fileline, ignoreEmpties, ignoreComments, commentChar))
                {
                    if (delimitKeyVal != ' ')                                                                       //[A]
                    {
                        keyValuePair = fileline.Split(delimitKeyVal);
                        if (keyValuePair[0] == "NOT_USED")                                                          //[B]
                        {
                            dictionaryOfLines.Add("NOT_USED" + dictionaryOfLines.Count, keyValuePair[1]);
                        }
                        else
                        {
                            dictionaryOfLines.Add(keyValuePair[0], keyValuePair[1]);
                        }
                    }
                    else
                    {
                        if (isKey)                                                                                  //[C]
                        {
                            key = fileline;
                        }
                        else
                        {
                            dictionaryOfLines.Add(key, fileline);
                        }
                        isKey = !isKey;                
                    }
                }
            }
            fileToRead.Close();
            return dictionaryOfLines;
        }
    }
}