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

        public bool CanSave()
        {
            return IsModified;
        }

        public void OnSave()
        {
            Workspace.This.Save(this, false);
        }

        #endregion

        #region SaveAsCommand

        public bool CanSaveAs()
        {
            return IsModified;
        }

        public void OnSaveAs()
        {
            Workspace.This.Save(this, true);
        }

        #endregion

        #region CloseCommand

        public void OnClose(object sender)
        {
            Workspace.This.Close(this);
        }

        private ACommand _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null) {
                    _closeCommand = new ACommand(this.OnClose);
                }
                return _closeCommand;
            }
        }

        #endregion

        #region ExecuteCommand

        private bool CanExecute()
        {
            return true;
        }

        private void OnExecute()
        {
            Workspace.This.Execute(this);
        }

        #endregion

        #region Save

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

        #endregion
    }
}
