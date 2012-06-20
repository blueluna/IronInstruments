using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IronInstruments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = Workspace.This;
        }

        private void doExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void doExecuteFile(object sender, RoutedEventArgs e)
        {
            console.Pad.Console.RunStatements(Workspace.This.ActiveDocument.Document.Text);
        }
    }
}
