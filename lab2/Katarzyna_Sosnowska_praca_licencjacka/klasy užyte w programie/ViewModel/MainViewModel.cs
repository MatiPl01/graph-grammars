using Lintree.Common;
using Lintree.Model;
using Lintree.View;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lintree.ViewModel
{
    /// <summary>
    /// Klasa MainViewModel, reprezentuje główny ViewModel programu. Dziedziczy po ViewModelBase.
    /// </summary>
    class MainViewModel : ViewModelBase
    {
        #region Prywatne pola
        private Viewport3D _viewport;                   // Kontrolka Viewport3D (wyświetlanie sceny 3D)
        private CameraController _camera;               // Kamera w widoku 3D
        private ModelVisual3D _baseModel;               // Bazowy model 3D który będzie rysowany w widoku 3D
        private CommandStringGenerator _csGen;          // Obiekt klasy CommandStringGenerator, zapewnia dostęp do metod tworzących wywód danego L-systemu
        private ModelGenerator _mGen;                   // obiekt klasy ModelGenerator, zapewnia dostęp do metod tworzących model 3D na bazie wywodu danego L-systemu
        private TextureManager _textMan;                // obiekt klasy TextureManager, zarządza teksturami używanymi w programie
        private List<ModelVisual3D> _lightViewports;    // Lista elementów ModelVisual3D, które przechowują światła widoku 3D
        private List<Vector3D> _lightDirections;        // Lista wektorów kierunku padania światła
        #endregion
        
        #region Publiczne komendy (właściwości)
        /// <summary>
        /// Komenda wywołująca tworzenie nowego modelu rośliny na bazie dostarczonych informacji.
        /// </summary>
        public DelegateCommand GenerateNewModel { get; set; }

        /// <summary>
        /// Zapisuje obene parametry l-systemu oraz modelu drzewa.
        /// </summary>
        public DelegateCommand SaveCurrentPattern { get; set; }

        /// <summary>
        /// Wczytuje zapisane parametry l-systemu oraz modelu drzewa.
        /// </summary>
        public DelegateCommand LoadSavedPattern { get; set; }

        /// <summary>
        /// Dodaje nową teksturę do programu.
        /// </summary>
        public DelegateCommand AddNewTexture { get; set; }

        /// <summary>
        /// Usuwa wskazaną teksturę z katalogu programu.
        /// </summary>
        public DelegateCommand RemoveChosenTexture { get; set; }

        /// <summary>
        /// Eksportuje obecny model 3D do pliku.
        /// </summary>
        public DelegateCommand SaveCurrentModel { get; set; }

        /// <summary>
        /// Importuje wskazany model 3D z pliku.
        /// </summary>
        public DelegateCommand LoadSavedModel { get; set; }

        /// <summary>
        /// Zamyka program.
        /// </summary>
        public DelegateCommand QuitApp { get; set; }

        /// <summary>
        /// Zeruje zawartość widoku 3D oraz listę parametrów.
        /// </summary>
        public DelegateCommand SetNewModelEnv { get; set; }

        /// <summary>
        /// Otwiera okno informacji o programie.
        /// </summary>
        public DelegateCommand OpenInfoWindow { get; set; }

        /// <summary>
        /// Otwiera okno pomocy.
        /// </summary>
        public DelegateCommand OpenHelpWindow { get; set; }
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Kontrolka Viewport3D - wyświetlanie sceny 3D.
        /// </summary>
        public Viewport3D MainViewport
        {
            get { return this._viewport; }
            set
            {
                this._viewport = value;
                base.RaisePropertyChangedEvent("MainViewport");
            }
        }

        /// <summary>
        /// Wybrana tekstura z listy dostępnych plików tekstur.
        /// </summary>
        public TextureSample SelectedTexture { get; set; }

        #region --> Właściwości csGen
        /// <summary>
        /// Aksjomat - punkt startowy dla wywodu danego L-systemu.
        /// </summary>
        public string Axiom
        {
            get { return this._csGen.Axiom; }
            set
            {
                if (value != this._csGen.Axiom)
                {
                    this._csGen.Axiom = value;
                }
            }
        }

        /// <summary>
        /// Lista reguł produkcji danego L-systemu.
        /// </summary>
        public string Rules
        {
            get { return this._csGen.Rules; }
            set
            {
                if (value != this._csGen.Rules)
                {
                    this._csGen.Rules = value;
                }
            }
        }

        /// <summary>
        /// Liczba iteracji dla reguły przepisywania w danym L-systemie.
        /// </summary>
        public string Iterations
        {
            get { return this._csGen.Iterations; }
            set
            {
                if (value != this._csGen.Iterations)
                {
                    this._csGen.Iterations = value;
                }
            }
        }
        #endregion

        #region --> Właściwości mGen
        /// <summary>
        /// Początkowy kąt obrotu kursora rysującego model.
        /// </summary>
        public string InitAngle
        {
            get { return this._mGen.InitAngle; }
            set
            {
                if (value != this._mGen.InitAngle)
                {
                    this._mGen.InitAngle = value;
                }
            }
        }

        /// <summary>
        /// Kąt obrotu kursora rysującego model.
        /// </summary>
        public string RotAngle
        {
            get { return this._mGen.RotAngle; }
            set
            {
                if (value != this._mGen.RotAngle)
                {
                    this._mGen.RotAngle = value;
                }
            }
        }

        /// <summary>
        /// Odchylenie kąta obrotu kursora rysującego model.
        /// </summary>
        public string DevAngle
        {
            get { return this._mGen.DevAngle; }
            set
            {
                if (value != this._mGen.DevAngle)
                {
                    this._mGen.DevAngle = value;
                }
            }
        }

        /// <summary>
        /// Liczba bocznych ścian w pojedynczym segmencie.
        /// </summary>
        public string SidesNumber
        {
            get { return this._mGen.SidesNumber; }
            set
            {
                if (value != this._mGen.SidesNumber)
                {
                    this._mGen.SidesNumber = value;
                }
            }
        }

        /// <summary>
        /// Początkowa długość pojedynczego segmentu.
        /// </summary>
        public string SegmLength
        {
            get { return this._mGen.SegmLength; }
            set
            {
                if (value != this._mGen.SegmLength)
                {
                    this._mGen.SegmLength = value;
                }
            }
        }

        /// <summary>
        /// Stosunek długości segmentów potomnych do segmentów-przodków.
        /// </summary>
        public string CutLenFactor
        {
            get { return this._mGen.CutLenFactor; }
            set
            {
                if (value != this._mGen.CutLenFactor)
                {
                    this._mGen.CutLenFactor = value;
                }
            }
        }

        /// <summary>
        /// Początkowa grubość pojedynczego segmentu.
        /// </summary>
        public string SegmThickness
        {
            get { return this._mGen.SegmThickness; }
            set
            {
                if (value != this._mGen.SegmThickness)
                {
                    this._mGen.SegmThickness = value;
                }
            }
        }

        /// <summary>
        /// Stosunek grubości segmentów potomnych do segmentów-przodków.
        /// </summary>
        public string CutThiFactor
        {
            get { return this._mGen.CutThiFactor; }
            set
            {
                if (value != this._mGen.CutThiFactor)
                {
                    this._mGen.CutThiFactor = value;
                }
            }
        }
        #endregion

        #region --> Właściwości textMan
        /// <summary>
        /// Lista tekstur znalezionych w katalogu programu.
        /// </summary>
        public List<TextureSample> TextureList
        {
            get { return this._textMan.TextureList; }
            set { }
        }
        #endregion
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            // Tworzenie nowego obiektu klasy CommandStringGenerator :
            this._csGen = new CommandStringGenerator();
            // Tworzenie nowego obiektu klasy ModelGenerator :
            this._mGen = new ModelGenerator();
            // Tworzenie nowego obiektu klasy TextureManager :
            this._textMan = new TextureManager();
            
            // Inicjalizacja komend :
            this.GenerateNewModel = new DelegateCommand(GenerateModel, CanGenerateModel);
            this.SaveCurrentModel = new DelegateCommand(SaveCurrModel, CanSaveCurrModel);
            this.LoadSavedModel = new DelegateCommand(LoadModelFromFile, CanLoadModelFromFile);
            this.SetNewModelEnv = new DelegateCommand(SetNewModel, CanSetNewModel);
            this.SaveCurrentPattern = new DelegateCommand(SaveCurrPattern, CanSaveCurrPattern);
            this.LoadSavedPattern = new DelegateCommand(LoadSavPattern, CanLoadSavPattern);
            this.AddNewTexture = new DelegateCommand(AddTexture, CanAddTexture);
            this.RemoveChosenTexture = new DelegateCommand(RemoveTexture, CanRemoveTexture);
            this.QuitApp = new DelegateCommand(QuitTheApp, CanQuitTheApp);
            this.OpenInfoWindow = new DelegateCommand(ShowInfoWindow, CanShowInfoWindow);
            this.OpenHelpWindow = new DelegateCommand(ShowHelpWindow, CanShowHelpWindow);

            #region Inicjalizowanie elementów kontrolki Viewport3D
            // Tworzenie sceny 3D :
            this._viewport = new Viewport3D();
            // -- kamera:
            this._camera = new CameraController();
            this._viewport.Camera = this._camera.PerspCamera;
            // -- światło:
            this._lightDirections = new List<Vector3D>(){ new Vector3D(-12, -10, -15),
                                                          new Vector3D(0, 10, -10),
                                                          new Vector3D(0, 0, 15)
                                                        };
            this._lightViewports = new List<ModelVisual3D>(8);
            foreach(var direction in this._lightDirections)
            {
                DirectionalLight light = new DirectionalLight();
                light.Color = Colors.White;
                light.Direction = direction;
                ModelVisual3D lightViewport = new ModelVisual3D();
                lightViewport.Content = light;
                this._lightViewports.Add(lightViewport);
                this._viewport.Children.Add(lightViewport);
            }
            // -- model 3D:
            this._baseModel = new ModelVisual3D();
            this._baseModel.Content = this._mGen.Curr3DModel;
            this._baseModel.Transform = this._camera.TransformGroup;
            this._viewport.Children.Add(this._baseModel);
            #endregion
        }
        #endregion

        #region Komendy
        #region Komendy związane z modelem 3D
        /// <summary>
        /// Metoda sprawdzająca, czy komenda CreateCommandString może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true jeśli komenda możę zostać wykonana, false wpp.</returns>
        private bool CanGenerateModel(object obj)
        {
            if (this.Axiom == "") return false;

            int iterations;
            bool result1 = int.TryParse(this.Iterations, out iterations);

            if (result1 && iterations > 0) return true;
            else return false;
        }

        /// <summary>
        /// Działania wywoływane przy wykonywaniu komendy CreateCommandString.
        /// </summary>
        /// <param name="obj"></param>
        private void GenerateModel(object obj)
        {
            if (obj == null) return;
            
            // uzyskuje dostęp do kontrolki Grid przechowującej widok 3D
            Grid viewportContainer = (Grid)obj;
            viewportContainer.Children.Clear();

            // zeruje model wyświetlany we ViewPorcie
            this._baseModel.Transform = null;
            this._baseModel.Content = null;
            this._viewport.Children.Clear();

            // tworzy listę poleceń
            this._csGen.CreateCommandString();

            // tworzy nowy model 3D
            this._mGen.GenerateModel(this._csGen.CommandString, this.SelectedTexture);

            // aktualizuje model wyświetlany we ViewPorcie
            this._baseModel.Transform = this._camera.TransformGroup;
            this._baseModel.Content = this._mGen.Curr3DModel;

            // dodaje z powrotem światła i model 3D do ViewPortu
            foreach (var lvp in this._lightViewports)
            {
                this._viewport.Children.Add(lvp);
            }
            this._viewport.Children.Add(this._baseModel);

            // dodaje z powrotem widok 3D oraz obsługe gestów myszy do kontrolki Grid
            // bez aktualizacji potomków kontrolki Grid nie byłoby widać nowego modelu 3D
            viewportContainer.Children.Add(this._viewport);
            viewportContainer.MouseWheel += this.MouseWheelBeh;
            viewportContainer.MouseRightButtonUp += this.MouseRightButtonUpBeh;
            viewportContainer.MouseLeftButtonDown += this.MouseLeftButtonDownBeh;
            viewportContainer.MouseLeftButtonUp += this.MouseLeftButtonUpBeh;
            viewportContainer.MouseMove += this.MouseMoveBeh;
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda SaveCurrentModel może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true, gdy istnieje Curr3DModel; wpp false</returns>
        private bool CanSaveCurrModel(object obj) { return (this._mGen.Curr3DModel != null) ? true : false; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy SaveCurrentModel.
        /// </summary>
        /// <param name="obj"></param>
        private void SaveCurrModel(object obj)
        {
            this._mGen.SaveCurrent3DModel();
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda LoadCurrentModel może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanLoadModelFromFile(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy LoadCurrentModel.
        /// </summary>
        /// <param name="obj"></param>
        private void LoadModelFromFile(object obj)
        {
            if (obj == null) return;

            if (this._mGen.LoadCurrent3DModel())
            {
                // uzyskuje dostęp do kontrolki Grid przechowującej widok 3D
                Grid viewportContainer = (Grid)obj;
                viewportContainer.Children.Clear();

                // zeruje model wyświetlany we ViewPorcie
                this._baseModel.Transform = null;
                this._baseModel.Content = null;
                this._viewport.Children.Clear();

                // aktualizuje model w widoku 3D
                this._mGen.ChangeModelInViewport3D();

                // aktualizuje model wyświetlany we ViewPorcie
                this._baseModel.Transform = this._camera.TransformGroup;
                this._baseModel.Content = this._mGen.Curr3DModel;

                // dodaje z powrotem światła i model 3D do ViewPortu
                foreach (var lvp in this._lightViewports)
                {
                    this._viewport.Children.Add(lvp);
                }
                this._viewport.Children.Add(this._baseModel);

                // dodaje z powrotem widok 3D oraz obsługę gestów myszy do kontrolki Grid
                // bez aktualizacji potomków kontrolki Grid nie byłoby widać nowego modelu 3D
                viewportContainer.Children.Add(this._viewport);
                viewportContainer.MouseWheel += this.MouseWheelBeh;
                viewportContainer.MouseRightButtonUp += this.MouseRightButtonUpBeh;
                viewportContainer.MouseLeftButtonDown += this.MouseLeftButtonDownBeh;
                viewportContainer.MouseLeftButtonUp += this.MouseLeftButtonUpBeh;
                viewportContainer.MouseMove += this.MouseMoveBeh;
            }
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda SetNewModelEnv może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanSetNewModel(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy SetNewModelEnv.
        /// </summary>
        /// <param name="obj"></param>
        private void SetNewModel(object obj)
        {
            this._mGen.ResetCurrentModel();
            this.SelectedTexture = null;
            base.RaisePropertyChangedEvent("SelectedTexture");
        }
        #endregion

        #region Komenty związane z parametrami konstrukcji
        /// <summary>
        /// Metoda sprawdzająca, czy komenda SaveCurrentPattern może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true jeśli komenda możę zostać wykonana, false wpp.</returns>
        private bool CanSaveCurrPattern(object obj)
        {
            return (this.Axiom == "") ? false : true;
        }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy SaveCurrentPattern.
        /// </summary>
        /// <param name="obj"></param>
        private void SaveCurrPattern(object obj)
        {
            XMLGenerator.SaveXMLFileContent(ref this._csGen, ref this._mGen);
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda LoadSavedPattern może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true jeśli komenda możę zostać wykonana, false wpp.</returns>
        private bool CanLoadSavPattern(object obj)
        {
            return (this.Axiom == "") ? false : true;
        }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy LoadSavedPattern.
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSavPattern(object obj)
        {
            // wczytuje dane z pliku XML
            XMLGenerator.LoadXMLFileContent(ref this._csGen, ref this._mGen);
            // aktualizuje GUI
            base.RaisePropertyChangedEvent("Axiom");
            base.RaisePropertyChangedEvent("Rules");
            base.RaisePropertyChangedEvent("Iterations");
            base.RaisePropertyChangedEvent("InitAngle");
            base.RaisePropertyChangedEvent("RotAngle");
            base.RaisePropertyChangedEvent("DevAngle");
            base.RaisePropertyChangedEvent("SidesNumber");
            base.RaisePropertyChangedEvent("SegmLength");
            base.RaisePropertyChangedEvent("CutLenFactor");
            base.RaisePropertyChangedEvent("SegmThickness");
            base.RaisePropertyChangedEvent("CutThiFactor");
        }
        #endregion

        #region Komendy związane z teksturami
        /// <summary>
        /// Metoda sprawdzająca, czy komenda AddNewTexture może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanAddTexture(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy AddNewTexture.
        /// </summary>
        /// <param name="obj"></param>
        private void AddTexture(object obj)
        {
            // dodajey nową teksturę do katalogu programu
            this._textMan.AddTexture();
            // aktualizuje GUI
            base.RaisePropertyChangedEvent("TextureList");
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda RemoveChosenTexture może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanRemoveTexture(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy RemoveChosenTexture.
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveTexture(object obj)
        {
            // usuwa wskazaną teksturę z katalogu programu
            this._textMan.RemoveTexture(obj);
            // aktualizuje GUI
            base.RaisePropertyChangedEvent("TextureList");
        }
        #endregion

        #region Komendy związane z ogólnymi funkcjonalnościami programu
        /// <summary>
        /// Metoda sprawdzająca, czy komenda QuitApp może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanQuitTheApp(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy QuitApp.
        /// </summary>
        /// <param name="obj"></param>
        private void QuitTheApp(object obj)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda OpenInfoWindow może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanShowInfoWindow(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy OpenInfoWindow.
        /// </summary>
        /// <param name="obj"></param>
        private void ShowInfoWindow(object obj)
        {
            InfoWindow infoW = new InfoWindow();
            infoW.Show();
        }

        /// <summary>
        /// Metoda sprawdzająca, czy komenda OpenHelpWindow może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanShowHelpWindow(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy OpenHelpWindow.
        /// </summary>
        /// <param name="obj"></param>
        private void ShowHelpWindow(object obj)
        {
            HelpWindow helpW = new HelpWindow();
            helpW.Show();
        }
        #endregion
        #endregion
        
        #region Zachowanie myszy wewnątrz kontrolki Viewport3D
        /// <summary>
        /// Zachowanie kółka myszy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseWheelBeh(object sender, MouseWheelEventArgs e)
        {
            this._camera.MouseWheelBeh(e);
        }

        /// <summary>
        /// Zachowanie PPM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseRightButtonUpBeh(object sender, MouseButtonEventArgs e)
        {
            this._camera.MouseRightButtonUpBeh();
        }

        /// <summary>
        /// Zachowanie LPM (przycisk wciskany).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftButtonDownBeh(object sender, MouseButtonEventArgs e)
        {
            this._camera.MouseLeftButtonDownBeh((Grid)sender, e);
        }

        /// <summary>
        /// Zachowanie LPM (przycisk zwalniany).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftButtonUpBeh(object sender, MouseButtonEventArgs e)
        {
            this._camera.MouseLeftButtonUpBeh();
        }

        /// <summary>
        /// Zachowanie podczas ruchu myszy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseMoveBeh(object sender, MouseEventArgs e)
        {
            this._camera.MouseMoveBeh((Grid)sender, e);
        }
        #endregion
    }
}
