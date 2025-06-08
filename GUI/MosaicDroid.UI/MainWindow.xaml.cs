using System;
using System.IO;
using IO = System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using MosaicDroid.Core;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MosaicDroid.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var gwFile = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Code.gw");
            if (!IO.File.Exists(gwFile))
            {
                Title = "⚠ Code.gw not found";
                return;
            }

            var code = File.ReadAllText(gwFile);
            var lexErrors = new List<CompilingError>();
            var tokens = Compiling.Lexical.GetTokens(code, lexErrors);
            Title = $"Tokens: {tokens?.Count()}  Errors: {lexErrors.Count}";
        }
    }
}