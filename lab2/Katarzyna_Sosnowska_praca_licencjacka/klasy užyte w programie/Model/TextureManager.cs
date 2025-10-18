using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa eExtureManager, odpowiada za zarządzanie listą wgranych tekstur.
    /// </summary>
    class TextureManager
    {
        #region Prywatne pola
        private List<TextureSample> textList;   // Lista wgranych tekstur w katalogu programu
        private string texturesDir;             // Ścieżka do katalogu z teksturami
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Lista tekstur znalezionych w katalogu programu.
        /// </summary>
        public List<TextureSample> TextureList
        {
            get { return this.textList; }
            set { }
        }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy TextureManager.
        /// </summary>
        public TextureManager()
        {
            // Inicjalizacja pól klasy :
            this.LoadTexturesList();
        }
        #endregion

        #region Prywatne metody
        /// <summary>
        /// Tworzy listę plików w katalogu Textures programu.
        /// </summary>
        private void LoadTexturesList()
        {
            this.textList = new List<TextureSample>();
            this.texturesDir = Directory.GetCurrentDirectory() + "\\Textures";
            // sprawdza czy katalog Patterns istnieje - jeśli nie, tworzy nowy :
            if (!Directory.Exists(this.texturesDir))
            {
                Directory.CreateDirectory(this.texturesDir);
            }
            DirectoryInfo di = new DirectoryInfo(this.texturesDir);
            try
            {
                // wzór rozpoznający rozszerzenia: .jpeg, .bmp, .png, .ico, .tiff, .gif, .wdp
                string pattern = ".(jpg|bmp|gif|ico|png|wdp|tiff)";
                var files = di.GetFiles();
                // wyszukuje w katalogu pliki o rozszerzeniach zgodnych z wzorem
                foreach (var f in files)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(f.Name, pattern))
                    {
                        this.textList.Add(new TextureSample(f.Name, this.texturesDir));
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
        #endregion

        #region Publiczne metody
        /// <summary>
        /// Obsługuje opcję dodawania nowej tekstury do katalogu programu.
        /// </summary>
        public void AddTexture()
        {
            OpenFileDialog addDial = new OpenFileDialog();
            addDial.Filter = "bmp files (*.bmp)|*.bmp"
                           + "|png files (*.png)|*.png"
                           + "|jpeg files (*.jpeg)|*.jpeg"
                           + "|tiff files (*.tiff)|*.tiff"
                           + "|wdp files (*.wdp)|*.wdp"
                           + "|gif files (*.gif)|*.gif"
                           + "|ico files (*.ico)|*.ico"
                           + "|all files(*.*)|*.*";
            addDial.FilterIndex = 8;
            addDial.Multiselect = true;
            addDial.RestoreDirectory = true;
            Nullable<bool> result = addDial.ShowDialog();
            if (result == true)
            {
                try
                {
                    var fileArray = addDial.FileNames;
                    foreach (var f in fileArray)
                    {
                        var filename = Path.GetFileName(f);
                        File.Copy(f, Path.Combine(this.texturesDir, filename));
                    }
                    this.LoadTexturesList();
                }
                catch (Exception e) { MessageBox.Show(e.Message); }
            }
        }

        /// <summary>
        /// Usuwa wskazaną teksturęz katalogu programu.
        /// </summary>
        /// <param name="obj">Obiekt wskazanej reprezentacji tekstury</param>
        public void RemoveTexture(object obj)
        {
            var texture = (TextureSample)obj;
            var textRemovePath = Path.Combine(this.texturesDir, texture.TextureName);
            try
            {
                File.Delete(textRemovePath);
                this.LoadTexturesList();
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
        #endregion
    }
}
