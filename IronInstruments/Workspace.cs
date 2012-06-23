using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using AvalonDock.Layout;

namespace IronInstruments
{
    class Workspace : ViewModelBase
    {
        protected Workspace()
        {
        }

        static Workspace _this = new Workspace();

        public static Workspace This
        {
            get { return _this; }
        }

        #region Files

        ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();
        ReadOnlyObservableCollection<FileViewModel> _readonyFiles = null;
        public ReadOnlyObservableCollection<FileViewModel> Files
        {
            get
            {
                if (_readonyFiles == null)
                {
                    _readonyFiles = new ReadOnlyObservableCollection<FileViewModel>(_files);
                }
                return _readonyFiles;
            }
        }

        #endregion

        public void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                var fileViewModel = Open(dlg.FileName);
                ActiveDocument = fileViewModel;
            }
        }

        public FileViewModel Open(string filepath)
        {
            var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath == filepath);
            if (fileViewModel != null)
                return fileViewModel;

            fileViewModel = new FileViewModel(filepath);
            _files.Add(fileViewModel);
            return fileViewModel;
        }

        public void OnNew(object sender, ExecutedRoutedEventArgs e)
        {
            _files.Add(new FileViewModel());
            ActiveDocument = _files.Last();
        }

        #region ActiveDocument

        private FileViewModel _activeDocument = null;
        public FileViewModel ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if (_activeDocument != value)
                {
                    _activeDocument = value;
                    RaisePropertyChanged("ActiveDocument");
                    if (ActiveDocumentChanged != null)
                        ActiveDocumentChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ActiveDocumentChanged;

        #endregion

        internal void Close(FileViewModel fileToClose)
        {
            if (fileToClose.IsModified)
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "AvalonDock Test App", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                    return;
                if (res == MessageBoxResult.Yes)
                {
                    Save(fileToClose);
                }
            }
            bool replaceActive = fileToClose == ActiveDocument;
            _files.Remove(fileToClose);
            if (replaceActive)
            {
                if (_files.Count > 0)
                {
                    ActiveDocument = _files.Last();
                }
                else {
                    ActiveDocument = null;
                }
            }
        }

        internal void Save(FileViewModel fileToSave, bool saveAsFlag = false)
        {
            if (fileToSave.FilePath == null || saveAsFlag)
            {
                var dlg = new SaveFileDialog();
                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    fileToSave.Save(dlg.FileName);
                }
            }
            else
            {
                fileToSave.Save();
            }
        }

        internal void Execute(FileViewModel fileToClose)
        {

        }
    }
}
