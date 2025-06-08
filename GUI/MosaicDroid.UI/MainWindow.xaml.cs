
using MosaicDroid.Core;
using System.IO;                   // for File and Path
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace MosaicDroid.UI
{
    public partial class MainWindow : Window
    {
        private int CanvasSize = 40;

        public string InstructionDocs { get; } = @"
Color(string): cambia pincel…
Size(int): …
…";  // gotta paste my doc here or find another way

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            PixelGrid.SizeChanged += (s, e) => ResizeCanvas();
            ResizeCanvas();
            UpdateLineNumbers();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
          => UpdateLineNumbers();

        private void UpdateLineNumbers()
        {
            int lineCount = Math.Max(0, Editor.LineCount); // Ensure non-negative count
            LineNums.ItemsSource = Enumerable.Range(1, lineCount)
                                             .Select(i => i.ToString());
        }

        private void ResizeBtn_Click(object s, RoutedEventArgs e)
        {
            if (int.TryParse(SizeBox.Text, out var sz) && sz > 0)
            {
                CanvasSize = sz;
                ResizeCanvas();
            }
        }

        private void ResizeCanvas()
        {
            double avail = PixelGrid.ActualWidth;
            // If it's not laid out yet, fall back to 15px:
            double cell = CanvasSize > 0 && avail > CanvasSize
                          ? Math.Floor(avail / CanvasSize)
                          : 15;
            PixelGrid.Rows = PixelGrid.Columns = CanvasSize;
            PixelGrid.Children.Clear();
            for (int i = 0; i < CanvasSize * CanvasSize; i++)
            {
                var rect = new Border
                {
                    Width = cell,
                    Height = cell,
                    Background = Brushes.White,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0.5)
                };
                PixelGrid.Children.Add(rect);
            }
        }

        private void LoadBtn_Click(object s, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Mosaic scripts|*.gw"
            };
            if (dlg.ShowDialog() == true)
                Editor.Text = File.ReadAllText(dlg.FileName);
        }

        private void SaveBtn_Click(object s, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Mosaic scripts|*.gw",
                FileName = "Code.gw"
            };
            if (dlg.ShowDialog() == true)
                File.WriteAllText(dlg.FileName, Editor.Text);
        }

        private void RunBtn_Click(object s, RoutedEventArgs e)
        {
            // persist to Code.gw so your CLI logic works unchanged
            File.WriteAllText("Code.gw", Editor.Text);

            // 5a) Lex / parse / semant
            var lexErr = new List<CompilingError>();
            var tokens = Compiling.Lexical.GetTokens(Editor.Text, lexErr);
            var parErr = new List<CompilingError>();
            var parser = new Parser(new TokenStream(tokens), parErr);
            var prog = parser.ParseProgram();
            var semErr = new List<CompilingError>();
            prog.CheckSemantic(new Context(), new Scope(), semErr);
            
            var allErr = lexErr.Cast<CompilingError>().Concat(parErr).Concat(semErr).ToList();

            /* if (allErr.Any()) {
    // show each with its location
    var msg = string.Join("\n",
      allErr.Select(err =>
        $"[{err.Location.Line},{err.Location.Column}] {err.Message}"
      )
    );
    MessageBox.Show(msg, "Errors");
    return;
  }*/
            if (lexErr.Any())
            {
                var lexMsg = string.Join("\n",
      lexErr.Select(err1 =>
        $"[{err1.Location.Line},{err1.Location.Column}] {err1.Message}"
      )
    );
                MessageBox.Show(lexMsg, "Errors");
                //return;
            }

            
            if (parErr.Any())
            {
                var parMsg = string.Join("\n",
      parErr.Select(err2 =>
        $"[{err2.Location.Line},{err2.Location.Column}] {err2.Message}"
      )
    );
                MessageBox.Show(parMsg, "Errors");
                //return;
            }

            
            if (semErr.Any())
            {
                var semMsg = string.Join("\n",
semErr.Select(err3 =>
$"[{err3.Location.Line},{err3.Location.Column}] {err3.Message}"
)
);
                MessageBox.Show(semMsg, "Errors");
                return;
            }

            // 5b) Interpret
            var runErr = new List<CompilingError>();
            var interp = new MatrixInterpreterVisitor(CanvasSize, runErr);
            try
            {
                interp.VisitProgram(prog);
            }
            catch (PixelArtRuntimeException ex)
            {
                MessageBox.Show("Runtime error: " + ex.Message);
            }

            // 5c) Paint the WPF grid
            for (int y = 0; y < CanvasSize; y++)
                for (int x = 0; x < CanvasSize; x++)
                {
                    // NOTE: expose a getter in your interpreter
                    var code = interp.GetBrushCodeForUI(x, y);
                    var brush = code switch
                    {
                        "bk" => Brushes.Black,
                        "bl" => Brushes.Blue,
                        "r " => Brushes.Red,
                        "g " => Brushes.Green,
                        "y " => Brushes.Yellow,
                        "o " => Brushes.Orange,
                        "p " => Brushes.Purple,
                        "w " => Brushes.White,
                        _ => Brushes.Transparent
                    };
                    ((Border)PixelGrid.Children[y * CanvasSize + x]).Background = brush;
                }
        }
    }
}