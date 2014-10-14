using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LogAnalyzer.Helpers;
using LogAnalyzer.Model;

namespace LogAnalyzer.ViewModels
{
    public class ViewModel : NotifyPropertyBase
    {
        private RegexHandlerModel _regexHandler;
        private string _regexExpressionString;
        private int _comboBoxSelectedIndex;
        private string _listBoxSelectedValue;
        private string _textBoxText;
        private ObservableCollection<string> _textBoxTextCollection;
        private ObservableCollection<string> _regexExpressionListBoxCollection;
        private ICommand _openFileCommand;
        private ICommand _goCommand;

        private void InitData()
        {
            if (TextBoxTextCollection != null) TextBoxTextCollection.Clear();
            if (RegexExpressionListBoxCollection != null) RegexExpressionListBoxCollection.Clear();
            int res = RegexHandler.LoadData(RegexExpressionString);
            if (res == 0)
            {
                TextBoxTextCollection = RegexHandler.FullFileTextCollection;

            }
            else 
            {
                TextBoxTextCollection = new ObservableCollection<string>();
                TextBoxTextCollection.Add("Sorry, nothing matched");
            }
        }

        private void ComboBox_SelectionChanged(int SelectionIndex)
        {
            RegexExpressionListBoxCollection.Clear();
            if (TextBoxTextCollection == null ||
                !TextBoxTextCollection.Equals(RegexHandler.FullFileTextCollection))
            {
                TextBoxTextCollection = RegexHandler.FullFileTextCollection;
            }
            if (SelectionIndex == 0) return;
            try
            {
                foreach (var key in RegexHandler.DataDictionary[SelectionIndex].Keys)
                {
                    RegexExpressionListBoxCollection.Add(key);
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (KeyNotFoundException)
            {
            }
        }

        private void ListBox_SelectionChanged(string SelectionValue)
        {
            if (string.IsNullOrEmpty(SelectionValue)) return;
            ObservableCollection<string> values;
            if (RegexHandler.DataDictionary[ComboBoxSelectedIndex].TryGetValue(SelectionValue, out values))
            {
                TextBoxTextCollection = values;
            }

        }

        public string TextBoxText
        {
            get
            {
                return _textBoxText;
            }
            set
            {
                _textBoxText = value;
                NotifyPropertyChanged("TextBoxText");
            }
        }

        public string RegexExpressionString
        {
            get
            {
                return _regexExpressionString;
            }
            set
            {
                _regexExpressionString = value;
                NotifyPropertyChanged("RegexExpressionString");
            }
        }

        private void OpenFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                RegexHandler.FileLocation = dlg.FileName;
            }
        }

        private void GoExecute()
        {
            if (!RegexHandler.RegexExpressionCollection.Contains(RegexExpressionString))
            {
                RegexHandler.RegexExpressionCollection.Add(RegexExpressionString);
            }
            InitData();

        }

        private bool IsGoClickable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RegexHandler.FileLocation) || string.IsNullOrWhiteSpace(RegexExpressionString))
                {
                    return false;
                }
                return true;
            }
        }

        public int ComboBoxSelectedIndex
        {
            get
            {
                return _comboBoxSelectedIndex;
            }
            set
            {
                _comboBoxSelectedIndex = value;
                ComboBox_SelectionChanged(value);
                NotifyPropertyChanged("ComboBoxSelectedItem");
            }
        }

        public string ListBoxSelectedValue
        {
            get
            {
                return _listBoxSelectedValue;
            }
            set
            {
                _listBoxSelectedValue = value;
                ListBox_SelectionChanged(value);
                NotifyPropertyChanged("ListBoxSelectedValue");
            }
        }

        public ObservableCollection<string> TextBoxTextCollection
        {
            get
            {
                return _textBoxTextCollection;
            }
            set
            {
                _textBoxTextCollection = value;
                NotifyPropertyChanged("TextBoxTextCollection");
            }
        }

        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                {
                    _openFileCommand = new RelayCommand(param => this.OpenFile(),
                        null);
                }
                return _openFileCommand;
            }
        }

        public ICommand GoCommand
        {
            get
            {
                if (_goCommand == null)
                {
                    _goCommand = new RelayCommand(param => this.GoExecute(),
                        param => IsGoClickable);
                }
                return _goCommand;
            }
        }

        public ObservableCollection<string> RegexExpressionListBoxCollection
        {
            get
            {
                return _regexExpressionListBoxCollection;
            }
            set
            {
                _regexExpressionListBoxCollection = value;
                NotifyPropertyChanged("RegexExpressionListBoxCollection");
            }
        }

        public RegexHandlerModel RegexHandler
        {
            get { return _regexHandler; }
            set { _regexHandler = value; }
        }

        public ViewModel()
        {
            RegexHandler = new RegexHandlerModel();
            RegexExpressionListBoxCollection = new ObservableCollection<string>();
            RegexHandler.RegexExpressionCollection.Add(@"<(?<user>\w+)>(?<text>.+)");
            RegexHandler.RegexExpressionCollection.Add(@"(?<IP>^((\d{1,3}\.){3}\d{1,3})).*");
        }
   }
}
