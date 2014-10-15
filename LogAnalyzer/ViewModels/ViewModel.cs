using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LogAnalyzer.Helpers;
using LogAnalyzer.Model;
using Microsoft.Win32;

namespace LogAnalyzer.ViewModels
{
    public class ViewModel : NotifyPropertyBase
    {
        private int _comboBoxSelectedIndex;
        private ICommand _goCommand;
        private ICommand _closeCommand;
        private ICommand _openFileCommand;
        private string _listBoxSelectedValue;
        private string _regexExpressionString;
        private ObservableCollection<string> _regexExpressionListBoxCollection;
        private ObservableCollection<string> _textBoxTextCollection;


        public ViewModel()
        {
            RegexHandler = new RegexHandlerModel();
            RegexExpressionListBoxCollection = new ObservableCollection<string>();
            //for demo
            RegexExpressionCollection.Add(@"<(?<user>\w+)>(?<text>.+)");
            RegexExpressionCollection.Add(@"(?<IP>^((\d{1,3}\.){3}\d{1,3})).*");
        }

        private string FileLocation { get; set; }
        private RegexHandlerModel RegexHandler { get; set; }

        /// <summary>
        ///     <para>Holds the regex expression that the user has searched.</para>
        /// </summary>
        /// <value>
        ///     The regex expression that the user has searched.
        /// </value>
        public ObservableCollection<string> RegexExpressionCollection
        {
            get { return RegexHandler.RegexExpressionCollection; }
            set
            {
                RegexHandler.RegexExpressionCollection = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     <para>Holds the regex match groups.</para>
        /// </summary>
        /// <value>
        ///     The regex match groups.
        /// </value>
        public ObservableCollection<string> RegexExpressionGroupsCollection
        {
            get { return RegexHandler.RegexExpressionGroupsCollection; }
            set
            {
                RegexHandler.RegexExpressionGroupsCollection = value;
                NotifyPropertyChanged();
            }
        }


        /// <summary>
        ///     <para>Gets or sets the current regex expression string.</para>
        /// </summary>
        /// <value>
        ///     The current regex expression string.
        /// </value>
        public string RegexExpressionString
        {
            get { return _regexExpressionString; }
            set
            {
                _regexExpressionString = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     <para>Checks if Go button is clickable by making sure that:</para>
        ///     <para>1. the user has selected a file.</para>
        ///     <para>2. the user has entered a regex to search.</para>
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is go clickable; otherwise, <c>false</c>.
        /// </value>
        private bool IsGoClickable
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FileLocation) && !string.IsNullOrWhiteSpace(RegexExpressionString);
            }
        }

        /// <summary>
        ///     <para>Gets or sets the index of the ComboBox.</para>
        ///     <para>indicates the selected match group</para>
        /// </summary>
        /// <value>
        ///     The index of the ComboBox.
        /// </value>
        public int ComboBoxSelectedIndex
        {
            get { return _comboBoxSelectedIndex; }
            set
            {
                _comboBoxSelectedIndex = value;
                ComboBox_SelectionChanged(value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     <para>Gets or sets the ListBox selected value.</para>
        ///     <para>indicates the selected match from the match group</para>
        /// </summary>
        /// <value>
        ///     The ListBox selected value.
        /// </value>
        public string ListBoxSelectedValue
        {
            get { return _listBoxSelectedValue; }
            set
            {
                _listBoxSelectedValue = value;
                ListBox_SelectionChanged(value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     <para>Holds the textbox text</para>
        ///     <para>Should be a reference to other collection.</para>
        /// </summary>
        /// <value>
        ///     The text box text collection.
        /// </value>
        public ObservableCollection<string> TextBoxTextCollection
        {
            get { return _textBoxTextCollection; }
            set
            {
                _textBoxTextCollection = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     <para>Triggers the OpenFile method</para>
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(param => Application.Current.MainWindow.Close(),
                        null);
                }
                return _closeCommand;
            }
        }

        /// <summary>
        ///     <para>Triggers the OpenFile method</para>
        /// </summary>
        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                {
                    _openFileCommand = new RelayCommand(param => OpenFile(),
                        null);
                }
                return _openFileCommand;
            }
        }

        /// <summary>
        ///     <para>Triggers the GoExecute method</para>
        /// </summary>
        public ICommand GoCommand
        {
            get
            {
                if (_goCommand == null)
                {
                    _goCommand = new RelayCommand(param => GoExecute(),
                        param => IsGoClickable);
                }
                return _goCommand;
            }
        }

        /// <summary>
        ///     <para>Holds the matches of the selected match group</para>
        /// </summary>
        /// <value>
        ///     matches of the selected match group
        /// </value>
        public ObservableCollection<string> RegexExpressionListBoxCollection
        {
            get { return _regexExpressionListBoxCollection; }
            set
            {
                _regexExpressionListBoxCollection = value;
                NotifyPropertyChanged();
            }
        }


        /// <summary>
        ///     <para>Initializes the data</para>
        /// </summary>
        private void InitData()
        {
            /* call for RegexHander.LoadData with the regex we want to search and the file */
            int res = RegexHandler.LoadData(RegexExpressionString, FileLocation);
            // if the returned result is "0" than all ok and we should update the TextBox text.
            if (res == 0)
            {
                TextBoxTextCollection = RegexHandler.FullFileTextCollection;
                ComboBoxSelectedIndex = 0;
            }
                // otherwise there were no matches
            else
            {
                TextBoxTextCollection = new ObservableCollection<string> {"Sorry, nothing matched"};
            }
        }

        /// <summary>
        ///     <para>Updates the ListBox values according to the match group that was selected</para>
        /// </summary>
        /// <param name="selectionIndex">Index of the ComboBox selection.</param>
        private void ComboBox_SelectionChanged(int selectionIndex)
        {
            RegexExpressionListBoxCollection.Clear(); // clear data
            // updates TextBoxTextCollection if it`s empty or it's not equals to RegexHandler.FullFileTextCollection 
            if (TextBoxTextCollection == null ||
                !TextBoxTextCollection.Equals(RegexHandler.FullFileTextCollection))
            {
                TextBoxTextCollection = RegexHandler.FullFileTextCollection;
            }
            // if the selectionIndex is 0 than we should stop here.
            if (selectionIndex == 0 || selectionIndex == - 1) return;
            // if it's not, we should update RegexExpressionListBoxCollection with the matches of the selected match group
            foreach (var key in RegexHandler.DataDictionary[selectionIndex].Keys)
            {
                RegexExpressionListBoxCollection.Add(key);
            }
        }

        /// <summary>
        ///     <para>Updates TextBoxTextCollection according to the match that was selected</para>
        /// </summary>
        /// <param name="selectionValue">The selection value.</param>
        private void ListBox_SelectionChanged(string selectionValue)
        {
            if (string.IsNullOrEmpty(selectionValue)) return;
            ObservableCollection<string> values;
            if (RegexHandler.DataDictionary[ComboBoxSelectedIndex].TryGetValue(selectionValue, out values))
            {
                TextBoxTextCollection = values;
            }
        }

        /// <summary>
        ///     Opens the file.
        /// </summary>
        private void OpenFile()
        {
            var dlg = new OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                FileLocation = dlg.FileName;
            }
        }

        /// <summary>
        ///     <para>calls to InitData</para>
        /// </summary>
        private void GoExecute()
        {
            // if the user has searched with a new expression, add it to the list.
            if (!RegexExpressionCollection.Contains(RegexExpressionString))
            {
                RegexExpressionCollection.Add(RegexExpressionString);
            }
            InitData();
        }
    }
}