using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa TextureSample, reprezentuje pojedynczy plik tekstury.
    /// </summary>
    class TextureSample
    {
        #region Prywatne pola
        private string _textureName;    // Nazwa tekstury
        private string _texturePath;    // Ścieżka do pliku tekstury na dysku
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Nazwa tekstury.
        /// </summary>
        public string TextureName
        {
            get { return this._textureName; }
            set { }
        }

        /// <summary>
        /// Skrócona nazwa tekstury wyświetlana pod jej miniaturą w programie.
        /// </summary>
        public string TextureNameShort
        {
            get
            {
                if (this._textureName.Length > 7)
                {
                    return this._textureName.Substring(0, 4) + "...";
                }
                else return this._textureName;
            }
            set { }
        }

        /// <summary>
        /// Adres obrazu tekstury używany do pokazywania jej miniaturki w programie.
        /// </summary>
        public ImageSource TextureImage
        {
            get
            {
                var bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.CacheOption = BitmapCacheOption.OnLoad;
                bmi.UriSource = new Uri(this._texturePath);
                bmi.EndInit();
                return bmi;
            }
            set { }
        }

        /// <summary>
        /// Ścieżka do pliku tekstury na dysku.
        /// </summary>
        public string TexturePath
        {
            get { return this._texturePath; }
            set { }
        }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy TextureSample.
        /// </summary>
        /// <param name="textureName">Nazwa tekstury</param>
        /// <param name="currDir">Ścieżka do tekstury na dysku</param>
        public TextureSample(string textureName, string currDir)
        {
            // Inicjalizacja pól klasy :
            this._textureName = textureName;
            this._texturePath = currDir + "\\" + textureName;
        }
        #endregion
    }
}
