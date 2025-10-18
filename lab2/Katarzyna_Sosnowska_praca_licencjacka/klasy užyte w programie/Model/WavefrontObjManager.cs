using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa WavefrontObjManeger, umożliwia eksport i import modelu 3D do i z plików .obj oraz .mtl.
    /// </summary>
    class WavefrontObjManager
    {
        #region Prywatne pola
        private List<Point3D> _points;           // Lista wierzchołków geometrycznych
        private List<Point> _textureCoords;      // Lista punktów reprezentujących współrzędne tekstur
        private List<int> _triangleIndices;      // Lista indeksów do tworzenia trójkątnych poligonów
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy WavefrontObjManager.
        /// </summary>
        public WavefrontObjManager()
        {
            // Inicjalizacja pól klasy :
            this._points = new List<Point3D>();
            this._textureCoords = new List<Point>();
            this._triangleIndices = new List<int>();
        }
        #endregion

        #region Publiczne metody
        /// <summary>
        /// Dodaje podane punkty do listy wierzchołków geometrycznych.
        /// </summary>
        /// <param name="points">Lista punktów</param>
        public void AddPoints(params Point3D[] points)
        {
            foreach (Point3D p in points)
            {
                this._points.Add(p);
            }
        }

        /// <summary>
        /// Dodaje podane punkty do listy punktów reprezentujących współrzędne tekstur.
        /// </summary>
        /// <param name="coords">Lista punktów</param>
        public void AddTextureCoords(params Point[] coords)
        {
            foreach (Point p in coords)
            {
                this._textureCoords.Add(p);
            }
        }

        /// <summary>
        /// Dodaje podane indeksy do listy indeksów określających poligony.
        /// </summary>
        /// <param name="indices">Lista punktów</param>
        public void AddTriangleIndices(params int[] indices)
        {
            foreach (int p in indices)
            {
                this._triangleIndices.Add(p);
            }
        }

        /// <summary>
        /// Resetuje zawartość wszystkich list.
        /// </summary>
        public void ClearAllLists()
        {
            this._points.Clear();
            this._textureCoords.Clear();
            this._triangleIndices.Clear();
        }

        /// <summary>
        /// Eksportuje podany model 3D do pliku.
        /// </summary>
        /// <param name="model">Model 3D do wyeksportowania</param>
        public void SaveCurrent3DModel(TextureSample texture)
        {
            // otwiera okno zapisywania pliku :
            SaveFileDialog saveDial = new SaveFileDialog();
            saveDial.Filter = "obj files (*.obj)|*.obj|all files(*.*)|*.*";
            saveDial.FilterIndex = 0;
            var currDir = Directory.GetCurrentDirectory() + "\\Models";
            if (!Directory.Exists(currDir))
            {
                Directory.CreateDirectory(currDir);
            }
            saveDial.InitialDirectory = currDir;
            saveDial.RestoreDirectory = true;
            Nullable<bool> result = saveDial.ShowDialog();
            if (result == true)
            {
                string fname = saveDial.SafeFileName.Substring(0, saveDial.SafeFileName.Length - 4);

                // Generowanie pliku OBJ :
                using (var writer = new StreamWriter(File.Create(saveDial.FileName)))
                {
                    // zapisuje nagłówek pliku:
                    writer.WriteLine("# Lintree 3D Plant Generator v1.0 OBJ File:\n");
                    // zapisuje wiązanie z plikiem mtl:
                    writer.WriteLine("mtllib {0}.mtl", fname);
                    
                    int counter = 0;    // licznik poszczególnych elementów - ma charakter czysto informacyjny

                    fname = fname.Replace(" ", "_");
                    writer.WriteLine("\no {0}", fname);
                    // zapisuje położenie wierzchołków geometrycznych:
                    foreach (var p in this._points)
                    {
                        writer.WriteLine("v {0:0.000000} {1:0.000000} {2:0.000000}", p.X, p.Y, p.Z);
                        counter++;
                    }
                    writer.WriteLine("# {0} geometric vertices", counter);
                    counter = 0;

                    // zapisuje położenie wierzchołków tekstur:
                    foreach (var p in this._textureCoords)
                    {
                        writer.WriteLine("vt {0:0.000000} {1:0.000000}", p.X, p.Y);

                        counter++;
                    }
                    writer.WriteLine("# {0} texture vertices", counter);
                    counter = 0;

                    // zapisuje informację o używanym materiale
                    if (texture == null)
                    {
                        writer.WriteLine("usemtl default_grey");
                    }
                    else
                    {
                        writer.WriteLine("usemtl {0}_mat", fname);
                    }

                    // zapisuje domyślną wartość parametru smoothing group - off
                    writer.WriteLine("s off");

                    // zapisuje indeksy wierzchołków tworzących trójkątne poligony:
                    int index = 1;
                    int t_index = 1;
                    while (index + 2 <= this._triangleIndices.Count)
                    {
                        if (texture == null)
                        {
                            writer.WriteLine("f {0} {1} {2}", index++, index++, index++);
                        }
                        else
                        {
                            writer.WriteLine("f {0}/{1} {2}/{3} {4}/{5}", index++, t_index++, index++, t_index++, index++, t_index++);
                        }
                        counter++;
                    }
                    writer.WriteLine("# {0} faces", counter);
                }

                // Generowanie pliku MTL :
                var mtlFilePath = saveDial.FileName.Substring(0, saveDial.FileName.Length - 4) + ".mtl";
                using (var writer = new StreamWriter(File.Create(mtlFilePath)))
                {
                    // zapisuje nagłówek pliku:
                    writer.WriteLine("# Lintree 3D Plant Generator v1.0 MTL File:\n");
                    // zapisuje szczegóły materiału (domyślne wartości):
                    if (texture != null)    // użytkownik wybrał jakąś teksturę
                    {
                        writer.WriteLine("newmtl {0}_mat", fname);              // nazwa materiału
                        writer.WriteLine("Ns 0");                               // poziom połysku materiału
                        writer.WriteLine("Ka 0.0 0.0 0.0");                     // kolor ambient
                        writer.WriteLine("Ka 0.8 0.8 0.8");                     // kolor diffuse
                        writer.WriteLine("Ka 0.8 0.8 0.8");                     // kolor specular
                        writer.WriteLine("d 1");                                // przeźroczystość materiału
                        writer.WriteLine("illum 2");                            // model iluminacji (2 - obecność rozświetleń specular)
                        writer.WriteLine("map_Ka {0}", texture.TexturePath);    // źródło tekstury
                    }
                    else    // brak wybranej tekstury
                    {
                        writer.WriteLine("newmtl default_grey");    // nazwa materiału
                        writer.WriteLine("Ns 0");                   // poziom połysku materiału
                        writer.WriteLine("Ka 0.6 0.6 0.6");         // kolor ambient
                        writer.WriteLine("Ka 0.8 0.8 0.8");         // kolor diffuse
                        writer.WriteLine("Ka 0.8 0.8 0.8");         // kolor specular
                        writer.WriteLine("d 1");                    // przeźroczystość materiału
                        writer.WriteLine("illum 2");                // model iluminacji (2 - obecność rozświetleń specular)
                    }
                }

                // czyszczenie list
                this.ClearAllLists();
            }
        }

        /// <summary>
        /// Importuje wskazany obiekt do widoku 3D programu.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns>Zaimportowany model 3D</returns>
        public GeometryModel3D LoadCurrent3DModel(out Point3D minP, out Point3D maxP)
        {
            MeshGeometry3D mesh = null;         // model 3D
            DiffuseMaterial diffMat = null;     // materiał dla modelu 3D
            minP = new Point3D();               // zeruje punkt z minimalnymi współrzędnymi
            maxP = new Point3D();               // zeruje punkt z maksymalnymi współrzędnymi

            // otwiera okno otwierania pliku :
            OpenFileDialog loadDial = new OpenFileDialog();
            loadDial.Filter = "obj files (*.obj)|*.obj|all files(*.*)|*.*";
            loadDial.FilterIndex = 0;
            var currDir = Directory.GetCurrentDirectory() + "\\Models";
            if (!Directory.Exists(currDir))
            {
                Directory.CreateDirectory(currDir);
            }
            loadDial.InitialDirectory = currDir;
            loadDial.RestoreDirectory = true;
            Nullable<bool> result = loadDial.ShowDialog();
            if (result == true)
            {
                mesh = new MeshGeometry3D();

                string fname = loadDial.FileName;                                       // pełna nazwa pliku (z rozszerzeniem .obj)
                string mtl_fname = "";                                                  // nazwa pliku .mtl, która znajduje się w pliku .obj
                string fileDir = loadDial.FileName.Replace(loadDial.SafeFileName, "");  // lokalizacja plików .obj i .mtl
                string usedmtl = "";                                                    // nazwa używanego materiału

                // Odczytuje zawartość pliku OBJ :
                using (StreamReader reader = new StreamReader(fname))
                {
                    string line = "";
                    char[] separator = new char[] { ' ' };

                    try
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            var temp = line.Split(separator);
                            switch (temp[0])
                            {
                                // odczytuje nazwę dołączonego pliku .mtl
                                case "mtllib":
                                    mtl_fname = line.Substring(7);
                                    break;

                                // odczytuje współrzędne wierzchołka geometrycznego
                                case "v":
                                    Point3D p = new Point3D(
                                                    double.Parse(temp[1]),
                                                    double.Parse(temp[2]),
                                                    double.Parse(temp[3])
                                                    );
                                    mesh.Positions.Add(p);
                                    minP = this.GetMaxOrMinPoint(true, minP, p);
                                    maxP = this.GetMaxOrMinPoint(false, maxP, p);
                                    break;

                                // odczytuje współrzędne tekstury
                                case "vt":
                                    mesh.TextureCoordinates.Add(
                                        new Point(
                                            double.Parse(temp[1]),
                                            double.Parse(temp[2])
                                            ));
                                    break;

                                // odczytuje nazwę używanego materiału
                                case "usemtl":
                                    usedmtl = line.Substring(7);
                                    break;

                                // odczytuje indeksy określające poligon
                                case "f":
                                    char[] sep = new char[] { '/' };
                                    for (int i = 1; i < temp.Length; i++)
                                    {
                                        var trind = temp[i].Split(sep);
                                        mesh.TriangleIndices.Add(int.Parse(trind[0]) - 1);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error: " + e.Message);
                        return null;
                    }
                }

                // Odczytuje zawartość pliku MTL :
                if (mtl_fname != "" && usedmtl != "default_grey" && usedmtl != "")
                {
                    mtl_fname = fileDir + mtl_fname;

                    using (StreamReader reader = new StreamReader(mtl_fname))
                    {
                        string line = "";
                        char[] separator = new char[] { ' ' };
                        string last_mtl = "";                   // ostatnio odczytana nazwa materiału

                        try
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                var temp = line.Split(separator);
                                switch (temp[0])
                                {
                                    // odczytuje nazwę zdefiniowanego materiału
                                    case "newmtl":
                                        last_mtl = line.Substring(7);
                                        break;

                                    // odczytuje ścieżkę wykorzystanej tekstury (jeśli takowa istnieje)
                                    case "map_Ka":
                                        if (last_mtl == usedmtl)
                                        {
                                            string texturePath = line.Substring(7);
                                            BitmapImage bmi = new BitmapImage();
                                            bmi.BeginInit();
                                            bmi.CacheOption = BitmapCacheOption.OnLoad;
                                            bmi.UriSource = new Uri(texturePath);
                                            bmi.EndInit();
                                            ImageBrush imageBrush = new ImageBrush();
                                            imageBrush.ImageSource = bmi;
                                            diffMat = new DiffuseMaterial(imageBrush);
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        catch (Exception e) { MessageBox.Show("Error: " + e.Message); }
                    }
                }
            }
            else
            {
                return null;
            }

            if (diffMat == null)
            {
                diffMat = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
            }

            return new GeometryModel3D(mesh, diffMat);
        }
        #endregion

        #region Prywatne metody
        /// <summary>
        /// Tworzy punkt o minimalnych/maksymalnych wartościach współrzędnych na bazie podanej pary punktów.
        /// </summary>
        /// <param name="calcMin">true jeśli ma obliczyć minimum, false jeśli maksimum</param>
        /// <param name="p1">pierwszy punkt</param>
        /// <param name="p2">drugi punkt</param>
        /// <returns>Obliczony nowy punkt</returns>
        private Point3D GetMaxOrMinPoint(bool calcMin, Point3D p1, Point3D p2)
        {
            Point3D p = new Point3D();
            if (calcMin)
            {
                p.X = (p1.X < p2.X) ? p1.X : p2.X;
                p.Y = (p1.Y < p2.Y) ? p1.Y : p2.Y;
                p.Z = (p1.Z < p2.Z) ? p1.Z : p2.Z;
            }
            else
            {
                p.X = (p1.X > p2.X) ? p1.X : p2.X;
                p.Y = (p1.Y > p2.Y) ? p1.Y : p2.Y;
                p.Z = (p1.Z > p2.Z) ? p1.Z : p2.Z;
            }
            return p;
        }
        #endregion
    }
}
