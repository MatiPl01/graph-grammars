using Lintree.Common;
using Lintree.Model;
using Lintree.View;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Lintree.ViewModel
{
    /// <summary>
    /// Klasa HelpViewModel, zawiera ModelView dla okna Help. Dziedziczy po ViewModelBase.
    /// </summary>
    class HelpViewModel : ObservableObject
    {
        /// <summary>
        /// Poniższe właściwości tekstowe mają na celu jedynie zwrócić tekst o bardziej złożonym formatowaniu, który będzie wyświetlany w GUI.
        /// </summary>
        #region Właściwości tekstowe

        #region Temat pomocy: "Construction parameters"
        public string cp_Title = "Construction parameters";
        public string cp_text1 = "In order to create a plant-like 3D model you will need to set values of multiple parameters." 
                               + '\n'
                               + "Lintree 3D Plant Generator uses L-systems to generate virtual plants. Below there is an explanation of each of L-system parameters with sample values:";
        public string cp_text2 = "Rules for writing L-system rules:" + '\n' + "Every rule has the following form:     [left side] = [right side]" 
                               + '\n' + "(you can replace  =  with  :  )."
                               + '\n' + "For example, F=F+F means that each instance of F will be replaced with F+F"
                               + '\n' + "This application also supports context L-systems - you can also write rules like this: F+F=FF-FF."
                               + '\n' + "In the following table you will find all symbols for writing rules, which are recognised by program:";
        public string CPText1 { get { return this.cp_text1; } set { } }
        public string CPText2 { get { return this.cp_text2; } set { } }
        #endregion

        #region Temat pomocy: "Saving/Loading construction parameters"
        public string slcp_Title = "Saving/Loading construction parameters";
        public string slcp_text = "Lintree 3D Plant Generator supports saving and loading current values of construction settings to and from XML files"
                                + "\n\n"
                                + "You can store current parameters by using \'Save...\' button in the \"Construction settings\" panel, "
                                + "\'Save L-system...\' option from menu or by using the \'Shift+S\' shortcut."
                                + '\n' + "The set of current parameters values will be stored in a XML file."
                                + "\n\n"
                                + "In order to load stored values use \'Load...\' button in the \"Construction settings\" panel, "
                                + "\'Load L-system...\' option from menu or the \'Shift+O\' shortcut";
        public string SLCPText { get { return this.slcp_text; } set { } }
        #endregion

        #region Temat pomocy: "Managing textures"
        public string mt_Title = "Managing textures";
        public string mt_text = "You can apply a texture to 3D model. In order to do this, a texture file must exist inside the program directory. "
                              + "To load a texture file simply click \'Add...\' button in the bottom of \"Trunk texture\" panel."
                              + "\n\n"
                              + "NOTE: this program supports only following file extensions: .jpg, .bmp, .gif, .ico, .png, .wdp and .tiff."
                              + "\n\n"
                              + "If you want to remove a texture from program directory, select the chosen texture from the texture list and click \'Remove...\' button in the bottom of the the \"Trunk texture\" panel.";
        public string MTText { get { return this.mt_text; } set { } }
        #endregion

        #region Temat pomocy: "Generating a 3D model"
        public string gm_Title = "Generating a 3D model";
        public string gm_text = "The main goal of this application is to enable its users generating 3D models of plants, which can be used in other programs."
                              + "\n\n"
                              + "To create a new model you have to set construction parameters to a proper value and click \'Generate\' button in the bottom right corner of the app window."
                              + "\n\n"
                              + "Congratulations! - you have created a new model :)"
                              + "\n\n"
                              + "If you want to generate a model with texture simply select one from the texture list (\"Trunk texture\" panel) and then use the \'Generate\' button.";
        public string GMText { get { return this.gm_text; } set { } }
        #endregion

        #region Temat pomocy: "Exporting/Importing a 3D model"
        public string eigm_Title = "Exporting/Importing a 3D model";
        public string eigm_text = "Lintree 3D Plant Generator supports exporting and importing current 3D model to and from .obj and .mtl files."
                                + "\n\n"
                                + "You can export current 3D model by using \'Save model...\' button below the 3D view panel, "
                                + "\'Save model...\' option from menu or by using the \'Ctrl+S\' shortcut."
                                + '\n' + "There will be generated a pair of files with extensions .obj and .mtl, whose store the information about current 3D model."
                                + "\n\n"
                                + "In order to import saved model use \'Load model...\' button below the 3D view panel, "
                                + "\'Load model...\' option from menu or the \'Ctrl+O\' shortcut";
        public string EIGMText { get { return this.eigm_text; } set { } }
        #endregion

        #endregion

        #region Prywatne pola
        private List<HelpMenuOption> _options;      // Lista zawierająca dostępne tematy pomocy
        private List<ScrollViewer> _helpPanels;     // Lista paneli pomocy, po jednym na każdy temat
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Aktualna instancja klasy okna pomocy.
        /// </summary>
        public HelpWindow CurrHelpWindow { get; set; }

        /// <summary>
        /// Lista tematów pomocy.
        /// </summary>
        public List<HelpMenuOption> LBItems
        {
            get { return this._options; }
            set { }
        }

        /// <summary>
        /// Panel aktualnie wyświetlanego tematu pomocy.
        /// </summary>
        public ScrollViewer CurrentPanel { get; set; }

        /// <summary>
        /// Lista paneli z poszczególnymi tematami pomocy.
        /// </summary>
        public List<ScrollViewer> HelpPanels
        {
            get { return this._helpPanels; }
            set { this._helpPanels = value; }
        }
        #endregion

        #region Publiczne komendy
        /// <summary>
        /// Zamyka okno pomocy.
        /// </summary>
        public DelegateCommand CloseHelpWindow { get; set; }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy HelpViewModel.
        /// </summary>
        public HelpViewModel()
        {
            // Inicjalizacja pól klasy :
            this._options = new List<HelpMenuOption>();
            this._options.Add(new HelpMenuOption() { Issue = this.cp_Title });
            this._options.Add(new HelpMenuOption() { Issue = this.slcp_Title });
            this._options.Add(new HelpMenuOption() { Issue = this.mt_Title });
            this._options.Add(new HelpMenuOption() { Issue = this.gm_Title });
            this._options.Add(new HelpMenuOption() { Issue = this.eigm_Title });
            // Inicjalizacja komendy :
            this.CloseHelpWindow = new DelegateCommand(CloseHelp, CanCloseHelp);
        }

        #endregion

        #region Komendy
        /// <summary>
        /// Metoda sprawdzająca, czy komenda CloseHelpWindow może zostać wykonana.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true</returns>
        private bool CanCloseHelp(object obj) { return true; }

        /// <summary>
        /// Działania wywoływane podczas wykonywania komendy CloseHelpWindow.
        /// </summary>
        /// <param name="obj"></param>
        private void CloseHelp(object obj)
        {
            this.CurrHelpWindow.Close();
        }
        #endregion

        #region Publiczne metudy
        /// <summary>
        /// Zachowanie po zmianie zaznaczenia na liście tematów pomocy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void menuHelpList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int newPanelIndex = this.CurrHelpWindow.menuHelpList.SelectedIndex;
            ScrollViewer newPanel = this._helpPanels[newPanelIndex];
            if (newPanel == this.CurrentPanel) return;
            else
            {
                this.CurrentPanel.Visibility = System.Windows.Visibility.Collapsed;
                newPanel.Visibility = System.Windows.Visibility.Visible;
                this.CurrentPanel = newPanel;
            }
        }
        #endregion
    }
}
