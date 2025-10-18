using Lintree.Model;
using Lintree.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Lintree.View
{
    /// <summary>
    /// Klasa HelpWindow - definiuje okno pomocy.
    /// </summary>
    public partial class HelpWindow : Window
    {
        #region Prywatne pola
        private HelpViewModel helpVM;   // obiekt klasy HelpViewModel
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy HelpWindow.
        /// </summary>
        public HelpWindow()
        {
            // Inicjalizacja elementów okna :
            InitializeComponent();

            // Podobnie jak w oknie MainWindow, w widoku XAML obiekt klasy HelpViewModel został ustawiony deklaratywnie jako źródło danych okna
            // Poniżej pobierana jest referencja do tego obiektu
            this.helpVM = (HelpViewModel)this.DataContext;

            // przekazuje informacje o obecnej instancji okna pomocy (potrzebne do komendy zamykającej okno)
            this.helpVM.CurrHelpWindow = this;

            // dodaje elementy do listy tematów pomocy
            foreach (HelpMenuOption hmo in this.helpVM.LBItems)
            {
                this.menuHelpList.Items.Add(hmo);
            }
            this.menuHelpList.SelectedIndex = 0;

            // zapisuje dane nt. obecnie zaznaczonego tematu pomocy
            this.helpVM.CurrentPanel = this.constructionSettings_HelpPanel;
            this.helpVM.CurrentPanel.Visibility = System.Windows.Visibility.Visible;
            this.menuHelpList.SelectionChanged += this.helpVM.menuHelpList_SelectionChanged;

            // dodaje panele pomocy do listy
            this.helpVM.HelpPanels = new List<ScrollViewer>();
            this.helpVM.HelpPanels.Add(this.constructionSettings_HelpPanel);
            this.helpVM.HelpPanels.Add(this.sl_constructionSettings_HelpPanel);
            this.helpVM.HelpPanels.Add(this.textureManaging_HelpPanel);
            this.helpVM.HelpPanels.Add(this.generatingModel_HelpPanel);
            this.helpVM.HelpPanels.Add(this.ei_generatingModel_HelpPanel);
        }
        #endregion
    }
}
