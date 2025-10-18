using Lintree.ViewModel;
using System.Windows;

namespace Lintree
{
    /// <summary>
    /// Klasa MainWindow - definiuje główne okno aplikacji.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Prywatne pola
        private MainViewModel mainViewModel;    // Obiekt klasy MainViewModel
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy MainWindow.
        /// </summary>
        public MainWindow()
        {
            // Inicjalizacja elementów okna :
            InitializeComponent();

            // W widoku XAML obiekt klasy MainViewModel został ustawiony deklaratywnie jako źródło danych okna
            // Poniżej pobierana jest referencja do tego obiektu
            this.mainViewModel = (MainViewModel)this.DataContext;

            // Dodaje do kontrolki GUI widok 3D z modelu danych :
            this.viewportGrid.Children.Add(this.mainViewModel.MainViewport);

            // Dodaje obsługę myszy :
            this.viewportGrid.MouseWheel += this.mainViewModel.MouseWheelBeh;
            this.viewportGrid.MouseRightButtonUp += this.mainViewModel.MouseRightButtonUpBeh;
            this.viewportGrid.MouseLeftButtonDown += this.mainViewModel.MouseLeftButtonDownBeh;
            this.viewportGrid.MouseLeftButtonUp += this.mainViewModel.MouseLeftButtonUpBeh;
            this.viewportGrid.MouseMove += this.mainViewModel.MouseMoveBeh;
        }
        #endregion
    }
}
