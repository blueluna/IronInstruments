using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;

namespace IronInstruments
{
    class FileViewModel : PaneViewModel
    {
        public FileViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }

        public FileViewModel()
        {
            Title = FileName;
        }

        #region FilePath
        private string _filePath = null;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    RaisePropertyChanged("FilePath");
                    FileName = System.IO.Path.GetFileName(FilePath);
                }
            }
        }
        #endregion

        #region FileName

        private string _fileName = "Untitled";
        public string FileName
        {
            get 
            {
                return _fileName;
            }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    RaisePropertyChanged("FileName");
                    updateTitle();
                }
            }
        }

        #endregion

        public override Uri IconSource
        {
            get
            {
                return new Uri("/IronInstruments;Images/document.png", UriKind.RelativeOrAbsolute);
            }
        }

        #region Document
        private TextDocument _document = null;
        public TextDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;

                    _document.BeginUpdate();
                    if (File.Exists(_filePath))
                    {
                        _document.Text = File.ReadAllText(_filePath);
                        ContentId = _filePath;
                    }
                    _document.EndUpdate();
                    _document.UndoStack.ClearAll();
                    _document.UndoStack.MarkAsOriginalFile();

                    _document.UpdateFinished += onUpdateFinished;

                    RaisePropertyChanged("Document");
                }
            }
        }
        #endregion

        private void onUpdateFinished(object sender, EventArgs e)
        {
            RaisePropertyChanged("IsModified");
            updateTitle();
        }

        #region IsModified
        public bool IsModified
        {
            get {
                if (_document != null)
                {
                    return !_document.UndoStack.IsOriginalFile;
                }
                else {
                    return false;
                }
            }
        }
        #endregion

        private void updateTitle()
        {
            Title = _fileName + (IsModified ? "*" : "");
        }

        #region SaveCommand
        RelayCommand _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand((p) => OnSave(p), (p) => CanSave(p));
                }

                return _saveCommand;
            }
        }

        private bool CanSave(object parameter)
        {
            return IsModified && FilePath != null;
        }

        private void OnSave(object parameter)
        {
            Workspace.This.Save(this, false);
        }

        #endregion

        #region SaveAsCommand
        RelayCommand _saveAsCommand = null;
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand((p) => OnSaveAs(p), (p) => CanSaveAs(p));
                }

                return _saveAsCommand;
            }
        }

        private bool CanSaveAs(object parameter)
        {
            return IsModified;
        }

        private void OnSaveAs(object parameter)
        {
            Workspace.This.Save(this, true);
        }

        #endregion

        #region CloseCommand
        RelayCommand _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand((p) => OnClose(), (p) => CanClose());
                }

                return _closeCommand;
            }
        }

        private bool CanClose()
        {
            return true;
        }

        private void OnClose()
        {
            Workspace.This.Close(this);
        }
        #endregion

        #region ExecuteCommand
        RelayCommand _executeCommand = null;
        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new RelayCommand((p) => OnClose(), (p) => CanClose());
                }

                return _executeCommand;
            }
        }

        private bool CanExecute()
        {
            return true;
        }

        private void OnExecute()
        {
            Workspace.This.Execute(this);
        }
        #endregion

        public void Save(string filepath)
        {
            File.WriteAllText(filepath, _document.Text);
            _document.UndoStack.MarkAsOriginalFile();
            if (filepath != _filePath)
            {
                FilePath = filepath;
            }
            RaisePropertyChanged("IsModified");
            updateTitle();
        }

        public void Save()
        {
            Save(_filePath);
        }
    }
}
