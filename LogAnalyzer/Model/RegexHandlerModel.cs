using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Model
{
    public class RegexHandlerModel
    {
        private Regex _rgx;

        public RegexHandlerModel()
        {
            DataDictionary = new Dictionary<int, Dictionary<string, ObservableCollection<string>>>();
            RegexExpressionCollection = new ObservableCollection<string>();
            FullFileTextCollection = new ObservableCollection<string>();
            RegexExpressionGroupsCollection = new ObservableCollection<string>();
        }

        public string RegexExpressionGroups { get; set; }
        public Dictionary<int, Dictionary<string, ObservableCollection<string>>> DataDictionary { get; set; }
        public ObservableCollection<string> RegexExpressionCollection { get; set; }
        public ObservableCollection<string> RegexExpressionGroupsCollection { get; set; }
        public ObservableCollection<string> FullFileTextCollection { get; set; }

        /// <summary>
        ///     <para>Clears the data.</para>
        /// </summary>
        private void ClearData()
        {
            if (FullFileTextCollection != null) FullFileTextCollection.Clear();
            if (DataDictionary != null) DataDictionary.Clear();
            if (RegexExpressionGroupsCollection != null) RegexExpressionGroupsCollection.Clear();
        }

        /// <summary>
        ///     <para>Main trigger. populate the collections with the regex matches</para>
        /// </summary>
        /// <param name="regexString">The regex string.</param>
        /// <param name="fileLocation">The file location.</param>
        /// <returns></returns>
        public int LoadData(string regexString, string fileLocation)
        {
            ClearData(); // remove all the data from the previous search
            _rgx = new Regex(regexString, RegexOptions.ExplicitCapture);
            // Add group names from regex to RegexExpressionGroupsCollection
            foreach (string name in _rgx.GetGroupNames())
            {
                if (name.Equals("0")) // 0 will be All
                {
                    RegexExpressionGroupsCollection.Add("All");
                }
                else
                {
                    RegexExpressionGroupsCollection.Add(name);
                }
            }
            // Read the file
            using (var reader = new StreamReader(fileLocation))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    FullFileTextCollection.Add(line); // add the line to the list
                    foreach (Match match in _rgx.Matches(line))
                    {
                        foreach (int group in _rgx.GetGroupNumbers())
                        {
                            /* for each group in every match, 
                             * append text to the dictionary with the group number as a key and text as the value */
                            if (group == 0)
                            {
                                ; //if group = 0 than it's the full text, and we already have it.
                            }
                            AddToDict(group,
                                match.Groups[group].ToString(), match.ToString());
                        }
                    }
                }
            }
            // if the DataDictionary is empty, we have no matches
            if (DataDictionary.Count == 0)
            {
                ClearData();
                return -1;
            }
            // all ok
            return 0;
        }

        /// <summary>
        ///     <para>Adds to dictionary.</para>
        /// </summary>
        /// <param name="index">The match group index</param>
        /// <param name="K">The match name in that match group</param>
        /// <param name="V">The value of the match</param>
        private void AddToDict(int index, string K, string V)
        {
            if (!DataDictionary.ContainsKey(index))
            {
                /* if DataDictionary doesn't hold the key - match group index
                 * we should create the key and initialize the inner dictionary */
                DataDictionary.Add(index, new Dictionary<string, ObservableCollection<string>>());
            }
            if (!DataDictionary[index].ContainsKey(K))
            {
                /* if DataDictionary[match group index] doesn't hold the key - match name 
                 * we should create the key and initialize the inner collection */
                var value = new ObservableCollection<string> {V};
                DataDictionary[index].Add(K, value);
            }
            else
            {
                //DataDictionary holds the keys
                DataDictionary[index][K].Add(V);
            }
        }
    }
}