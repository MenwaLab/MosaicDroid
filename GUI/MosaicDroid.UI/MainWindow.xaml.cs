
using MosaicDroid.Core;
using NetCoreAudio;
using System.IO;                   
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;




namespace MosaicDroid.UI
{
    public partial class MainWindow : Window
    {
        private int CanvasSize = 40;
        private const int MAX_CANVAS = 300;
        public string InstructionDocs { get; private set; }
        private Player _musicPlayer;
        private CancellationTokenSource _runCts;

        private ResourceManager _resmgr =
  new ResourceManager("MosaicDroid.UI.Resources.Strings",
                      typeof(MainWindow).Assembly);



        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        
            InitializeComponent();
            DataContext = this;
            LangCombo.SelectedIndex = (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "es") ? 1 : 0;
            //LangCombo.SelectionChanged += LangCombo_SelectionChanged;
            HookEvents();
            
            ReloadAllTexts();
            PixelGrid.SizeChanged += (s, e) => ResizeCanvas();
            ResizeCanvas();
            Editor.TextChanged += Editor_TextChanged;
            UpdateLineNumbers();

            StartMusic();
        }

        private void HookEvents()
        {
            PixelGrid.SizeChanged += (s, e) => ResizeCanvas();
            LangCombo.SelectionChanged += LangCombo_SelectionChanged;
        }

        private async void StartMusic()
        {
            var musicFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Osole_mio.mp3");
            if (File.Exists(musicFile))
            {
                BgMusic.Source = new Uri(musicFile, UriKind.Absolute);
                BgMusic.Play();
            }
            
        }
        
        private void BgMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            BgMusic.Position = TimeSpan.Zero;
            BgMusic.Play();
        }



        private void LangCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            var tag = (combo.SelectedItem as ComboBoxItem)?.Tag as string ?? "en";
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(tag);
            ReloadAllTexts();
        }

        private void ReloadAllTexts()
        {
            // buttons
            ResizeBtn.Content = _resmgr.GetString("Btn_Resize");
            LoadBtn.Content = _resmgr.GetString("Btn_Load");
            SaveBtn.Content = _resmgr.GetString("Btn_Save");
            RunBtn.Content = _resmgr.GetString("Btn_Run");
            StopBtn.Content = _resmgr.GetString("Btn_Stop");

            // labels
            // we bound SizeBox label via x:Name on a TextBlock in XAML
            ((TextBlock)LogicalTreeHelper.FindLogicalNode(this, "_sizeLabel"))
              .Text = _resmgr.GetString("Lbl_Size");
            ((TextBlock)LogicalTreeHelper.FindLogicalNode(this, "_langLabel"))
              .Text = _resmgr.GetString("Lbl_Language");

            // load docs text file (e.g. InstructionDocs_en.txt or .es)
            string docsFile = $"InstructionDocs_{Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName}.txt";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, docsFile);
            DocsBox.Text = File.Exists(path)
                ? File.ReadAllText(path)
                : _resmgr.GetString("Docs_NotFound");
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
                CanvasSize = Math.Min(sz, MAX_CANVAS); ;
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
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            _runCts?.Cancel();
        }

        private async void RunBtn_Click(object s, RoutedEventArgs e)
        {
            _runCts?.Cancel();
            _runCts = new CancellationTokenSource();
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
                MessageBox.Show(lexMsg, "Lexer Errors");
                //return;
            }

            
            if (parErr.Any())
            {
                var parMsg = string.Join("\n",
      parErr.Select(err2 =>
        $"[{err2.Location.Line},{err2.Location.Column}] {err2.Message}"
      )
    );
                MessageBox.Show(parMsg, "Parse Errors");
                //return;
            }

            
            if (semErr.Any())
            {
                var semMsg = string.Join("\n",
semErr.Select(err3 =>
$"[{err3.Location.Line},{err3.Location.Column}] {err3.Message}"
)
);
                MessageBox.Show(semMsg, "Semantic Errors");
                return;
            }



            // 5b) Interpret
            var runErr = new List<CompilingError>();
            var interp = new MatrixInterpreterVisitor(CanvasSize, runErr);


            try
            {
                //interp.VisitProgram(prog);
                await Task.Run(() => interp.VisitProgram(prog), _runCts.Token);
            }
            catch (PixelArtRuntimeException ex)
            {
                string message = string.Format(
                    _resmgr.GetString("Err_Runtime"),
                    ex.Message
                );
                MessageBox.Show(message, _resmgr.GetString("Err_Title"));
            }
            catch (OperationCanceledException)
            {
                // user hit Stop
            }
            


            // 5c) Paint the WPF grid
            for (int y = 0; y < CanvasSize; y++)
                for (int x = 0; x < CanvasSize; x++)
                {
                    var raw = interp.GetBrushCodeForUI(x, y);
                    SolidColorBrush brush;
                    if (string.IsNullOrEmpty(raw) || raw.Equals("Transparent", StringComparison.OrdinalIgnoreCase))
                    {
                        brush = Brushes.Transparent;
                    }
                    else
                    {
                        try
                        {
                            // use the WPF converter to turn the name or "#RRGGBB" into a Color
                            var col = (Color)ColorConverter.ConvertFromString(raw.Trim());
                            brush = new SolidColorBrush(col);
                        }
                        catch
                        {
                            brush = Brushes.Transparent;  // fallback on error
                        }
                    }
((Border)PixelGrid.Children[y * CanvasSize + x]).Background = brush;

                    // NOTE: expose a getter in your interpreter
                    /*var code = interp.GetBrushCodeForUI(x, y);
                    var brush = code switch
                    {
                        "bk" => Brushes.Black,
                        "bl" => Brushes.Blue,
                        "br" => Brushes.SaddleBrown,
                        "r " => Brushes.Red,
                        "g " => Brushes.Green,
                        "gr " => Brushes.Gray,
                        "y " => Brushes.Yellow,
                        "o " => Brushes.Orange,
                        "p " => Brushes.Purple,
                        "pi " => Brushes.Pink,
                        "w " => Brushes.White,
                        _ => Brushes.Transparent
                    };
                    
                    ((Border)PixelGrid.Children[y * CanvasSize + x]).Background = brush;*/
                }
        }
    }
}