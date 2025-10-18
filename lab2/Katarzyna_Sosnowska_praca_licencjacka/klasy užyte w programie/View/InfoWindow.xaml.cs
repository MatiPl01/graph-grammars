using System.Windows;

namespace Lintree.View
{
    /// <summary>
    /// Klasa InfoWindow - definiuje okno informacji o aplikacji.
    /// </summary>
    public partial class InfoWindow : Window
    {
        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy InfoWindow.
        /// </summary>
        public InfoWindow()
        {
            // Inicjalizacja elementów okna :
            InitializeComponent();
        }
        #endregion

        #region Prywatne metody
        /// <summary>
        /// Zamyka okno informacji o programie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
