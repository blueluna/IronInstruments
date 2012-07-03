using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;

namespace IronInstruments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Load our custom highlighting definition:
            IHighlightingDefinition pythonHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("IronInstruments.Resources.Python.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    pythonHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new string[] { ".py" }, pythonHighlighting);

            InitializeComponent();

            this.DataContext = Workspace.This;
        }

        private void doExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Workspace.This.OnNew(sender, e);
        }

        private void OpenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Workspace.This.OnOpen(sender, e);
        }

        private void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Workspace.This.ActiveDocument != null;
        }

        private void CloseExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Workspace.This.ActiveDocument.OnClose(sender);
        }

        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Workspace.This.ActiveDocument == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = Workspace.This.ActiveDocument.CanSave();
        }

        private void SaveExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Workspace.This.ActiveDocument.OnSave();
        }

        private void SaveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Workspace.This.ActiveDocument == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = Workspace.This.ActiveDocument.CanSaveAs();
        }

        private void SaveAsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Workspace.This.ActiveDocument.OnSaveAs();
        }

        private void ExecuteScriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Workspace.This.ActiveDocument != null;
        }

        private void ExecuteScriptExecute(object sender, RoutedEventArgs e)
        {
            if (Workspace.This.ActiveDocument != null)
            {
                console.Pad.Console.RunStatements(Workspace.This.ActiveDocument.Document.Text);
            }
        }
    }
}
