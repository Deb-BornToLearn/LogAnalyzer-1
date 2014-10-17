using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAnalyzer.Model
{
    public class RegexHandlerModel
    {

        private Regex _rgx;

        public RegexHandlerModel()
        {
            DataDictionary = new Dictionary<int, Dictionary<string, ObservableCollection<string>>>();
            RegexCollection = new ObservableCollection<string>();
            FullTextCollection = new ObservableCollection<string>();
            RegexGroupsCollection = new ObservableCollection<string>();
        }

        public Dictionary<int, Dictionary<string, ObservableCollection<string>>> DataDictionary { get; set; }
        public ObservableCollection<string> RegexCollection { get; set; }
        public ObservableCollection<string> RegexGroupsCollection { get; set; }
        public ObservableCollection<string> FullTextCollection { get; set; }


        /// <summary>
        ///     <para>Clears the data.</para>
        /// </summary>
        private void ClearData()
        {
            if (FullTextCollection != null) FullTextCollection.Clear();
            if (DataDictionary != null) DataDictionary.Clear();
            if (RegexGroupsCollection != null) RegexGroupsCollection.Clear();
        }

        /// <summary>
        ///     <para>Main trigger. populate the collections with the regex matches</para>
        /// </summary>
        /// <param name="regexString">The regex string.</param>
        /// <param name="fileLocation">The file location.</param>
        /// <returns>
        ///     <c>0</c> if there were matches; otherwise, <c>>0</c>.
        /// </returns>
        public async Task<int> LoadData(string regexString, string fileLocation)
        {
            // remove all the data from the previous search
            ClearData();
            //call for LoadDataTask, so ui won't freeze.
            Task<int> loadData = Task.Factory.StartNew(() => LoadDataTask(regexString, fileLocation));
            return await loadData;
        }


        /// <summary>
        ///     <para>Actual Main trigger. populate the collections with the regex matches</para>
        /// </summary>
        /// <param name="regexString">The regex string.</param>
        /// <param name="fileLocation">The file location.</param>
        /// <returns></returns>
        private int LoadDataTask(string regexString, string fileLocation)
        {
            try
            {
                _rgx = new Regex(regexString, RegexOptions.ExplicitCapture);
            }
            catch (ArgumentException)
            {
                //invalid regex 
                return 2;
            }
            // Add group names from regex to RegexGroupsCollection
            foreach (string name in _rgx.GetGroupNames())
            {
                if (name.Equals("0")) // 0 will be All
                {
                    RegexGroupsCollection.Add("All");
                }
                else
                {
                    RegexGroupsCollection.Add(name);
                }
            }
            // Read the file
            //string line;
            using (var reader = new StreamReader(fileLocation))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    FullTextCollection.Add(line); // add the line to the list
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
            if (DataDictionary.Count == 0)
            {
                ClearData();
                return 1;
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