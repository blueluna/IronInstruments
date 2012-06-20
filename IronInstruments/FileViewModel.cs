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
                    RaisePropertyChanged("FileName");
                    RaisePropertyChanged("Title");
                }
            }
        }
        #endregion


        public string FileName
        {
            get 
            {
                string filename;
                if (FilePath == null)
                {
                    filename = "Untitled" + (IsModified ? "*" : "");
                }
                else
                {
                    filename = System.IO.Path.GetFileName(FilePath) + (IsModified ? "*" : "");
                }
                RaisePropertyChanged("Title");
                return filename;
            }
        }

        public override Uri IconSource
        {
            get
            {
                return new Uri("/IronInstruments;component/Images/document.png", UriKind.RelativeOrAbsolute);
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

                    _document.Changed += onTextChanged;

                    RaisePropertyChanged("Document");
                }
            }
        }
        #endregion

        private void onTextChanged(object sender, EventArgs e)
        {
            bool canUndo = _document.UndoStack.CanUndo;
            IsModified = canUndo;
        }

        #region IsModified
        private bool _isModified = false;
        public bool IsModified
        {
            get {
                return _isModified;
            }
            set {
                if (_isModified != value) {
                    _isModified = value;
                    Title = FileName;
                }
            }
        }
        #endregion

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
            return IsModified;
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

    }
}
