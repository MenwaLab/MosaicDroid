
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

        private static readonly ResourceManager _resmgr = new ResourceManager("MosaicDroid.Core.Resources.Strings", typeof(MatrixInterpreterVisitor).Assembly);

        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        
            InitializeComponent();
            DataContext = this;

            LangCombo.SelectedIndex = (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "es") ? 1 : 0;
            HookEvents();
            
            ReloadAllTexts();
            ResizeCanvas();
            Editor.TextChanged += Editor_TextChanged; // se activa cuando el usuario escribe,pega o borra texto en el editor
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
                BgMusic.Source = new Uri(musicFile, UriKind.Absolute); // une el path a un obj q mediaElement de WPF puede consumir
                BgMusic.Play();
            }
            
        }
        
        private void BgMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            BgMusic.Position = TimeSpan.Zero; // para q la cancion se reproduzca indefinidamente
            BgMusic.Play();
        }

        private void LangCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender; //lee el selectedItem
            var tag = (combo.SelectedItem as ComboBoxItem)?.Tag as string ?? "en";
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(tag);
            ReloadAllTexts(); // para q se actualicen los botones de acuerdo al lenguaje
        }

        private void ReloadAllTexts()
        {
            // botones
            ResizeBtn.Content = _resmgr.GetString("Btn_Resize");
            LoadBtn.Content = _resmgr.GetString("Btn_Load");
            SaveBtn.Content = _resmgr.GetString("Btn_Save");
            RunBtn.Content = _resmgr.GetString("Btn_Run");
            MuteBtn.Content = _resmgr.GetString("Btn_Mute");


            ((TextBlock)LogicalTreeHelper.FindLogicalNode(this, "_sizeLabel"))
              .Text = _resmgr.GetString("Lbl_Size");
            ((TextBlock)LogicalTreeHelper.FindLogicalNode(this, "_langLabel"))
              .Text = _resmgr.GetString("Lbl_Language");

            // Carga los docs 
            string docsFile = $"InstructionDocs_{Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName}.txt";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, docsFile);
            DocsBox.Text = File.Exists(path) ? File.ReadAllText(path) : _resmgr.GetString("Docs_NotFound");
        }

         private void Editor_TextChanged(object sender, TextChangedEventArgs e) => UpdateLineNumbers();

        private void UpdateLineNumbers()
        {
            int lineCount = Math.Max(0, Editor.LineCount); 
            LineNums.ItemsSource = Enumerable.Range(1, lineCount) // muestra un numero por linea en el editor
                                             .Select(i => i.ToString());
        }

        private void ResizeBtn_Click(object s, RoutedEventArgs e)
        {
            if (int.TryParse(SizeBox.Text, out var size) && size > 0)
            {
                CanvasSize = Math.Min(size, MAX_CANVAS); ;
                ResizeCanvas();
            }
        }

        private void ResizeCanvas()
        {
            double width = PixelGrid.ActualWidth;

            // 15px para que el canvas se vea cuadriculado aunque el tamaño sea pequeño 
            double cell = CanvasSize > 0 && width > CanvasSize ? Math.Floor(width / CanvasSize): 15;

            PixelGrid.Rows = PixelGrid.Columns = CanvasSize;
            PixelGrid.Children.Clear();

            for (int i = 0; i < CanvasSize * CanvasSize; i++)
            {
                var border = new Border
                {
                    Width = cell,
                    Height = cell,
                    Background = Brushes.White,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0.5)
                };
                PixelGrid.Children.Add(border);
            }
        }

        private void LoadBtn_Click(object s, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog // abre los archivos de windows
            {
                Filter = "Mosaic scripts|*.pw" //solo permite .pw
            };
            if (dialog.ShowDialog() == true) // retorna true si el usuario selecciono Open
                Editor.Text = File.ReadAllText(dialog.FileName); // lee el contenido y lo pone en el editor
        }

        private void SaveBtn_Click(object s, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Mosaic scripts|*.pw",
                FileName = "Code.pw" // nombre por defecto
            };
            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, Editor.Text);
        }
        private void MuteBtn_Click(object sender, RoutedEventArgs e)
        {
            BgMusic.IsMuted = !BgMusic.IsMuted;
            // Actualiza la etiqueta:
            MuteBtn.Content = BgMusic.IsMuted
                ? _resmgr.GetString("Btn_Unmute")
                : _resmgr.GetString("Btn_Mute");
        }

        private void PaintCanvas(MatrixInterpreterVisitor interp)
        {
            // Pinta el canvas de WPF
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
                            // Usa el convertidor de WPF para transformar el nombre del Color o el #RRGGBB a un color
                            var col = (Color)ColorConverter.ConvertFromString(raw.Trim());
                            brush = new SolidColorBrush(col);
                        }
                        catch
                        {
                            brush = Brushes.Transparent;  // fallback si hay un error
                        }
                    }
                    ((Border)PixelGrid.Children[y * CanvasSize + x]).Background = brush;
                }
        }

        private async void RunBtn_Click(object s, RoutedEventArgs e)
        {
            _runCts?.Cancel();
            _runCts = new CancellationTokenSource();
            File.WriteAllText("Code.pw", Editor.Text);

            // Lexea? / Parsea /  Chequea semánticamente
            var lexErr = new List<CompilingError>();
            var tokens = Compiling.Lexical.GetTokens(Editor.Text, lexErr);

            var parErr = new List<CompilingError>();
            var parser = new Parser(new TokenStream(tokens), parErr);
            var prog = parser.ParseProgram();

            var semErr = new List<CompilingError>();
            prog.CheckSemantic(new Context(), new Scope(), semErr);

            
            if (lexErr.Any() )
            {
                var lexMsg = string.Join("\n",lexErr.Select(err1 => $"[{err1.Location.Line},{err1.Location.Column}] {err1.Message}" ));
                MessageBox.Show(lexMsg, _resmgr.GetString("Lex_Err"));
            }

             if (parErr.Any())
            {
                var parMsg = string.Join("\n", parErr.Select(err2 => $"[{err2.Location.Line},{err2.Location.Column}] {err2.Message}"));
                MessageBox.Show(parMsg, _resmgr.GetString("Parser_Error"));
            }

             if (semErr.Any())
            {
                var semMsg = string.Join("\n", semErr.Select(err3 => $"[{err3.Location.Line},{err3.Location.Column}] {err3.Message}"));
                MessageBox.Show(semMsg, _resmgr.GetString("Sem_Err"));
            }
            if (lexErr.Any() || parErr.Any() || semErr.Any())
                return;

            // Interpreta
            var runErr = new List<CompilingError>();
            var interp = new MatrixInterpreterVisitor(CanvasSize, runErr);

            try
            {
                await Task.Run(() => interp.VisitProgram(prog), _runCts.Token);
            }
            catch (PixelArtRuntimeException ex)
            {
                PaintCanvas(interp); // siempre pinta el canvas lo que haya antes del error:
                string message = string.Format(_resmgr.GetString("Err_Runtime"),ex.Message);
                MessageBox.Show(message, _resmgr.GetString("Err_Title"));
                return;
            }
            PaintCanvas(interp); // si no hay excepcion pinta el canvas sin problema
        }
    }
}