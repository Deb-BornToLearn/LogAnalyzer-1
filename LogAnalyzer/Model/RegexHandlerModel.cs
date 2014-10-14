using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Model
{
    public class RegexHandlerModel
    {
        private Regex _rgx;

        public string RegexExpressionGroups { get; set; }
        public Dictionary<int, Dictionary<string, ObservableCollection<string>>> DataDictionary { get; set; }
        public ObservableCollection<string> RegexExpressionCollection { get; set; }
        public ObservableCollection<string> RegexExpressionGroupsCollection { get; set; }
        public ObservableCollection<string> FullFileTextCollection { get; set; }

        private void clearData()
        {
            if (FullFileTextCollection != null) FullFileTextCollection.Clear();
            if (DataDictionary != null) DataDictionary.Clear();
            if (RegexExpressionGroupsCollection != null) RegexExpressionGroupsCollection.Clear();
        }

        public int LoadData(string RegexString, string FileLocation)
        {
            clearData();
            _rgx = new Regex(RegexString, RegexOptions.ExplicitCapture);
            // Add group names from regex to groupComboBox
            string[] names = _rgx.GetGroupNames();
            foreach (var name in names)
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
            using (var reader = new StreamReader(FileLocation))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    FullFileTextCollection.Add(line);
                    foreach (Match match in _rgx.Matches(line))
                    // put only the matched text in the list, other text is irrelevant
                    {
                       // processeddLog.Add(match.ToString());
                        foreach (var group in _rgx.GetGroupNumbers())
                        {
                            if (group == 0) {;}
                            addToDict(group,
        match.Groups[group].ToString(), match.ToString());
                        }
                    }
                }
                if (DataDictionary.Count == 0)
                {
                    clearData();
                    return -1;
                }
            }
            return 0;
        }

        public void addToDict(int index, string K, string V)
        {
            if (!DataDictionary.ContainsKey(index)) // if data dict doesn't hold the key
            {
                DataDictionary.Add(index, new Dictionary<string, ObservableCollection<string>>()); //create the key and the dict
            }
            if (!DataDictionary[index].ContainsKey(K)) // if data doesn't hold the groupMatch key
            {
                var value = new ObservableCollection<string> {V}; //create the list
                DataDictionary[index].Add(K, value); // add key and value to data
            }
            //if data holds the key
          
            else
            {
                DataDictionary[index][K].Add(V);
            }
        }

        public RegexHandlerModel()
        {
            DataDictionary = new Dictionary<int, Dictionary<string, ObservableCollection<string>>>();
            RegexExpressionCollection = new ObservableCollection<string>();
            FullFileTextCollection = new ObservableCollection<string>();
            RegexExpressionGroupsCollection = new ObservableCollection<string>();
        }


    }
}
