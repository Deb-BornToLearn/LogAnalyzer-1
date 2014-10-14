using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using LogAnalyzer.Helpers;

namespace LogAnalyzer.Model
{
    public class RegexHandlerModel : NotifyPropertyBase
    {
        private string _fileLocation;
        private ObservableCollection<string> _fullFileTextCollection;
        private string _regexExpressionGroups;
        private Regex _rgx;
        private ObservableCollection<string> _regexExpressionCollection;
        private ObservableCollection<string> _regexExpressionGroupsCollection;
        private Dictionary<int, Dictionary<string, ObservableCollection<string>>> _dataDictionary;


        public string FileLocation
        {
            get
            {
                return _fileLocation;
            }
            set
            {
                _fileLocation = value;
                NotifyPropertyChanged("FileLocation");
            }
        }

        public string RegexExpressionGroups
        {
            get
            {
                return _regexExpressionGroups;
            }
            set
            {
                _regexExpressionGroups = value;
                NotifyPropertyChanged("RegexExpressionGroups");
            }
        }
        public Dictionary<int, Dictionary<string, ObservableCollection<string>>> DataDictionary
        {
            get
            {
                return _dataDictionary;
            }
            set
            {
                _dataDictionary = value;
                NotifyPropertyChanged("DataDictionary");
            }
        }

        public ObservableCollection<string> RegexExpressionCollection
        {
            get
            {
                return _regexExpressionCollection;
            }
            set
            {
                _regexExpressionCollection = value;
                NotifyPropertyChanged("RegexExpressionCollection");
            }
        }

        public ObservableCollection<string> RegexExpressionGroupsCollection
        {
            get
            {
                return _regexExpressionGroupsCollection;
            }
            set
            {
                _regexExpressionGroupsCollection = value;
                NotifyPropertyChanged("RegexExpressionGroupsCollection");
            }
        }

        public ObservableCollection<string> FullFileTextCollection
        {
            get
            {
                return _fullFileTextCollection;
            }
            set
            {
                _fullFileTextCollection = value;
                NotifyPropertyChanged("FullFileTextCollection");
            }
        }

        public int LoadData(string RegexString)
        {
            if (FullFileTextCollection != null) FullFileTextCollection.Clear();
            if (DataDictionary != null) DataDictionary.Clear();
            if (RegexExpressionGroupsCollection != null) RegexExpressionGroupsCollection.Clear();
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
