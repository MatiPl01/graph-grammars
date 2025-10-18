using Lintree.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa ModelGenerator, odpowiada za rysowanie modelu 3D na bazie podanej listy poleceń. Dziedziczy po ObservableObject.
    /// </summary>
    class ModelGenerator : ObservableObject
    {
        #region Prywatne pola klasy
        private string _commandStr;                 // Lista poleceń do rysowania
        private double _initAngle;                  // Początkowy kąt obrotu kursora rysującego
        private double _rotAngle;                   // Kąt obrotu kursora rysującego
        private double _devAngle;                   // Odchylenie od wartości bazowej kąta obrotu kursora rysującego
        private int _sidesNumber;                   // Liczba bocznych ścian w jednym segmencie
        private double _segmentLength;              // Początkowa długość jednego segmentu
        private double _cutLenFactor;               // Określa stosunek długości kolejnych segmentów do segmentów z poprzedniej iteracji
        private double _segmentThickness;           // Początkowa grubość jednego segmentu
        private double _cutThiFactor;               // Określa stosunek grubości kolejnych segmentów do segmentów z poprzedniej iteracji
        private Model3DGroup _curr3Dmodel;          // Kontener na elementy składowe modelu 3D
        private GeometryModel3D _loaded3Dmodel;     // Model 3D wczytany z pliku
        private Random rand;                        // Obiekt klasy Random, służy do obliczania losowych wartości kąta obrotu
        private TextureSample _selectedTexture;     // Wybrana przez użytkownika tekstura
        private WavefrontObjManager _wfom;          // Obiekt klasy WaveFrontObjManager
        private Point3D _minimumP;                  // Punkt zawierający minimalne wartości współrzędnych x, y, z w obecnym modelu
        private Point3D _maximumP;                  // Punkt zawierający maksymalne wartości współrzędnych x, y, z w obecnym modelu
        private Transform3DGroup _transformGroup;   // Grupa transformacji zawierająca początkowe przesunięcie modelu
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Aktualnie generowany model 3D.
        /// </summary>
        public Model3DGroup Curr3DModel
        {
            get { return this._curr3Dmodel; }
            set
            {
                this._curr3Dmodel = value;
                base.RaisePropertyChangedEvent("Curr3DModel");
            }
        }

        /// <summary>
        /// Początkowy kąt obrotu kursora rysującego model.
        /// </summary>
        public string InitAngle
        {
            get { return this._initAngle.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._initAngle))
                {
                    this._initAngle = 0;
                }
                base.RaisePropertyChangedEvent("InitAngle");
            }
        }

        /// <summary>
        /// Kąt obrotu kursora rysującego model.
        /// </summary>
        public string RotAngle
        {
            get { return this._rotAngle.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._rotAngle))
                {
                    this._rotAngle = 60;
                }
                base.RaisePropertyChangedEvent("RotAngle");
            }
        }

        /// <summary>
        /// Odchylenie kąta obrotu kursora rysującego model.
        /// </summary>
        public string DevAngle
        {
            get { return this._devAngle.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._devAngle))
                {
                    this._devAngle = 5;
                }
                if (this._devAngle < 0)
                {
                    this._devAngle = 0;
                }
                base.RaisePropertyChangedEvent("DevAngle");
            }
        }

        /// <summary>
        /// Liczba bocznych ścian w pojedynczym segmencie.
        /// </summary>
        public string SidesNumber
        {
            get { return this._sidesNumber.ToString(); }
            set
            {
                if (!int.TryParse(value, out this._sidesNumber))
                {
                    this._segmentLength = 8;
                }
                else
                {
                    this._sidesNumber = Math.Abs(this._sidesNumber);
                    if (this._sidesNumber < 3) this._sidesNumber = 3;
                    else if (this._sidesNumber > 64) this._segmentLength = 64;
                }
                base.RaisePropertyChangedEvent("SidesNumber");
            }
        }

        /// <summary>
        /// Początkowa długość pojedynczego segmentu.
        /// </summary>
        public string SegmLength
        {
            get { return this._segmentLength.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._segmentLength))
                {
                    this._segmentLength = 1;
                }
                else
                {
                    this._segmentLength = Math.Abs(this._segmentLength);
                    if (this._segmentLength < 1) this._segmentLength = 1;
                    else if (this._segmentLength > 50) this._segmentLength = 50;
                }
                base.RaisePropertyChangedEvent("SegmLength");
            }
        }

        /// <summary>
        /// Stosunek długości segmentów potomnych do segmentów-przodków.
        /// </summary>
        public string CutLenFactor
        {
            get { return this._cutLenFactor.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._cutLenFactor))
                {
                    this._cutLenFactor = 1;
                }
                else
                {
                    if (this._cutLenFactor < 0.5) this._cutLenFactor = 0.5;
                    else if (this._cutLenFactor > 3) this._cutLenFactor = 3;
                }
                base.RaisePropertyChangedEvent("CutLenFactor");
            }
        }

        /// <summary>
        /// Początkowa grubość pojedynczego segmentu.
        /// </summary>
        public string SegmThickness
        {
            get { return this._segmentThickness.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._segmentThickness))
                {
                    this._segmentThickness = 1;
                }
                else
                {
                    this._segmentThickness = Math.Abs(_segmentThickness);
                    if (this._segmentThickness < 1) this._segmentThickness = 1;
                    else if (this._segmentThickness > 20) this._segmentThickness = 20;
                }
                base.RaisePropertyChangedEvent("SegmThickness");
            }
        }

        /// <summary>
        /// Stosunek grubości segmentów potomnych do segmentów-przodków.
        /// </summary>
        public string CutThiFactor
        {
            get { return this._cutThiFactor.ToString(); }
            set
            {
                if (!double.TryParse(value, out this._cutThiFactor))
                {
                    this._cutThiFactor = 1;
                }
                else
                {
                    if (_cutThiFactor < 0.5) this._cutThiFactor = 0.5;
                    else if (_cutThiFactor > 3) this._cutThiFactor = 3;
                }
                base.RaisePropertyChangedEvent("CutThiFactor");
            }
        }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy ModelGenerator.
        /// </summary>
        /// <param name="_commandStr">Wywód L-systemu, zawiera listę poleceń dla kursora rysującego model</param>
        public ModelGenerator()
        {
            // Inicjalizacja pól klasy :
            this._initAngle = 0;
            this._rotAngle = 60;
            this._devAngle = 5;
            this._sidesNumber = 8;
            this._segmentLength = 5;
            this._segmentThickness = 1;
            this._cutLenFactor = 0.9;
            this._cutThiFactor = 0.9;
            this._curr3Dmodel = new Model3DGroup();
            this._loaded3Dmodel = null;
            this.rand = new Random();
            this._wfom = new WavefrontObjManager();
            this._minimumP = new Point3D();
            this._maximumP = new Point3D();
            this._transformGroup = new Transform3DGroup();
        }
        #endregion

        #region Metody publiczne
        /// <summary>
        /// Tworzy nowy model 3D rośliny na bazie pobranych z GUI parametrów.
        /// Metoda stworzona w oparciu o pracę użytkownika arussell opublikowaną 21 grudnia 2014 r. pod licencją CPOL na stronie www.codeproject.com.
        /// Link: http://www.codeproject.com/Articles/855693/D-L-System-with-Csharp-and-WPF
        /// </summary>
        /// <param name="commandStr">Lista poleceń rysowania</param>
        /// <param name="selectedTexture">Wybrana tekstura</param>
        public void GenerateModel(string commandStr, object selectedTexture)
        {
            this._commandStr = commandStr;
            this._selectedTexture = (TextureSample)selectedTexture;         // wskzana przez użytkownika tekstura
            Point3D currPos = new Point3D(0, 0, 0);                         // obecna pozycja rysującego kursora
            Vector3D vPitch = new Vector3D(0, 0, 1);                        // obrót wokół osi Z
            Quaternion quatRot = Quaternion.Identity;
            Quaternion quatUp = new Quaternion(new Vector3D(0, 0, 1), 90);

            // obraca kwaterniony o początkowy kąt
            quatRot *= new Quaternion(vPitch, _initAngle);
            quatUp *= new Quaternion(vPitch, _initAngle);

            // zeruje listę składowych modelu 3D i grupę transformacji
            this._curr3Dmodel.Children.Clear();
            this._transformGroup.Children.Clear();
            this._maximumP = new Point3D();
            this._minimumP = new Point3D();
            this._wfom.ClearAllLists();

            // rysuje nowy model rośliny
            this.DrawOneBranch(0, currPos, quatRot, quatUp, this._segmentLength, this._segmentThickness);

            // przesuwa narysowany model tak, aby jego centrum znalazło się w środku układu współrzędnych
            Vector3D vec = (this._maximumP - this._minimumP) / -2;
            TranslateTransform3D translation = new TranslateTransform3D(vec);
            this._transformGroup.Children.Add(translation);
            this._curr3Dmodel.Transform = this._transformGroup;
        }
        
        /// <summary>
        /// Czyści widok 3D.
        /// </summary>
        public void ResetCurrentModel()
        {
            this._curr3Dmodel.Children.Clear();
            this._transformGroup.Children.Clear();
            this._maximumP = new Point3D();
            this._minimumP = new Point3D();
        }

        /// <summary>
        /// Zapisuje aktualny model 3D do pliku.
        /// </summary>
        public void SaveCurrent3DModel()
        {
            this._wfom.SaveCurrent3DModel(this._selectedTexture);
        }

        /// <summary>
        /// Wczytuje model 3D z pliku.
        /// </summary>
        /// <returns>true jeśli udało się wczytać model, false wpp.</returns>
        public bool LoadCurrent3DModel()
        {
            this._loaded3Dmodel = this._wfom.LoadCurrent3DModel(out this._minimumP, out this._maximumP);
            return (this._loaded3Dmodel != null) ? true : false;
        }

        /// <summary>
        /// Zamienia model obecnie wyświetlany w widoku 3D na wczytany z pliku.
        /// </summary>
        public void ChangeModelInViewport3D()
        {
            // zeruje listę składowych modelu 3D i grupę transformacji
            this._curr3Dmodel.Children.Clear();
            this._transformGroup.Children.Clear();
            this._maximumP = new Point3D();
            this._minimumP = new Point3D();
            this._wfom.ClearAllLists();

            // aktualizuje model wyświetlany w widoku 3D
            this._curr3Dmodel.Children.Add(this._loaded3Dmodel);

            // przesuwa wczytany model tak, aby jego centrum znalazło się w środku układu współrzędnych
            Vector3D vec = (this._maximumP - this._minimumP) / -2;
            TranslateTransform3D translation = new TranslateTransform3D(vec);
            this._transformGroup.Children.Add(translation);
            this._curr3Dmodel.Transform = this._transformGroup;
        }
        #endregion

        #region Metody prywatne
        /// <summary>
        /// Zwraca losową liczbę typu double z podanego przedziału.
        /// </summary>
        /// <param name="min">Początek przedziału</param>
        /// <param name="max">Koniec przedziału.</param>
        /// <returns></returns>
        private double GetRandomDouble(double min, double max)
        {
            return this.rand.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Tworzy punkt o minimalnych/maksymalnych wartościach współrzędnych na bazie podanej pary punktów.
        /// </summary>
        /// <param name="calcMin">true jeśli ma obliczyć minimum, false jeśli maksimum</param>
        /// <param name="p1">Pierwszy punkt</param>
        /// <param name="p2">Drugi punkt</param>
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

        /// <summary>
        /// Oblicza wektor przesunięcia kursora rysującego.
        /// Metoda stworzona w oparciu o pracę użytkownika arussell opublikowaną 21 grudnia 2014 r. pod licencją CPOL na stronie www.codeproject.com.
        /// Link: http://www.codeproject.com/Articles/855693/D-L-System-with-Csharp-and-WPF
        /// </summary>
        /// <param name="quat">Kwaternion z akrualną transformacją</param>
        /// <param name="dd">Długość wektora</param>
        /// <param name="vec">Wektor, który zostanie przekształcony</param>
        /// <returns>Obliczony nowy wektor</returns>
        private Vector3D CalculateNextPointVector(Quaternion quat, double dd, Vector3D vec)
        {
            Matrix3D m = Matrix3D.Identity;
            m.Rotate(quat);
            vec = m.Transform(vec * dd);
            return vec;
        }

        /// <summary>
        /// Dodaje nowy trójkątny poligon do modelu 3D.
        /// Metoda stworzona w oparciu o pracę użytkownika arussell opublikowaną 21 grudnia 2014 r. pod licencją CPOL na stronie www.codeproject.com.
        /// Link: http://www.codeproject.com/Articles/855693/D-L-System-with-Csharp-and-WPF
        /// </summary>
        /// <param name="mesh">Model 3D, do którego zostanie dodany poligon</param>
        /// <param name="p1">Współrzędna poligonu</param>
        /// <param name="p2">Współrzędna poligonu</param>
        /// <param name="p3">Współrzędna poligonu</param>
        private void AddTriangle(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3)
        {
            // dodaje wierzchołki geometryczne do modelu
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            this._wfom.AddPoints(p1, p2, p3);
            // dodaje indeksy wierzchołków geometrycznych opisujących nowy trójkątny poligon
            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);
            this._wfom.AddTriangleIndices(index, index + 1, index + 2);
        }

        /// <summary>
        /// Dodaje nowy trójkątny poligon do modelu 3D z nałożoną teksturą.
        /// Metoda stworzona w oparciu o pracę użytkownika arussell opublikowaną 21 grudnia 2014 r. pod licencją CPOL na stronie www.codeproject.com.
        /// Link: http://www.codeproject.com/Articles/855693/D-L-System-with-Csharp-and-WPF
        /// </summary>
        /// <param name="mesh">Model 3D, do którego zostanie dodany poligon</param>
        /// <param name="p1">Współrzędna poligonu</param>
        /// <param name="p2">Współrzędna poligonu</param>
        /// <param name="p3">Współrzędna poligonu</param>
        /// <param name="t1">Współrzędna tekstury</param>
        /// <param name="t2">Współrzędna tekstury</param>
        /// <param name="t3">Współrzędna tekstury</param>
        private void AddTriangle(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3, Point t1, Point t2, Point t3 )
        {
            // dodaje wierzchołki geometryczne do modelu
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            this._wfom.AddPoints(p1, p2, p3);
            // dodaje współrzędne tekstury do modelu
            mesh.TextureCoordinates.Add(t1);
            mesh.TextureCoordinates.Add(t2);
            mesh.TextureCoordinates.Add(t3);
            this._wfom.AddTextureCoords(t1, t2, t3);
            // dodaje indeksy wierzchołków geometrycznych opisujących nowy trójkątny poligon
            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);
            this._wfom.AddTriangleIndices(index, index + 1, index + 2);
        }

        /// <summary>
        /// Tworzy segment cylindrycznego kształtu.
        /// Metoda stworzona w oparciu o pracę Rod'a Stephens'a opublikowaną 13 kwietnia 2015 r. na stronie www.csharphelper.com.
        /// Link: http://csharphelper.com/blog/2015/04/draw-cylinders-using-wpf-and-c/
        /// </summary>
        /// <param name="doDrawing">Określa, czy rysować ściany segmentu czy też nie</param>
        /// <param name="begin_point">Środek początkowego konća segmentu</param>
        /// <param name="begins">Lista zawierająca punkty leżące na okręgu początkowego końca segmentu</param>
        /// <param name="end_point">Środek nowego konća segmentu</param>
        /// <param name="ends">Lista na punkty leżące na okręgu nowego końca segmentu</param>
        /// <param name="axis">Wektor kierunku rysowania segmentu</param>
        /// <param name="length">Długość segmentu</param>
        /// <param name="radius">Szerokość początkowego końca segmentu</param>
        /// <returns>Nowo utworzony segment 3D</returns>
        private GeometryModel3D CreateCylindricalSegment(bool doDrawing, Point3D begin_point, List<Point3D> begins, Point3D end_point, out List<Point3D> ends, Vector3D axis, double length, double radius)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            // tworzy dwa wektory prostopadłe do wektora axis
            Vector3D v1 = (axis.Z < -0.01) || (axis.Z > 0.01) ? new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y) : new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // kąt początkowy i kąt pomiędzy poszczególnymi punktami tworzącymi segment
            double theta = 0;
            double dtheta = 2 * Math.PI / this._sidesNumber;

            // jeśli podano za mało wierzchołków początkowych, tworzy ich nową listę
            if (begins.Count == 0)
            {
                // skaluje długość wektorów do promienia okręgu
                v1 *= radius / v1.Length;
                v2 *= radius / v2.Length;

                for (int i = 0; i < this._sidesNumber; i++)
                {
                    Point3D p1 = begin_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;     // tworzy punkt na okręgu wyznaczonym przez środek i promień
                    begins.Add(p1);                                                             // dodaje do listy punktów pierwszego końca segmentu
                    theta += dtheta;                                                            // zwiększa kąt wskazujący położenie następnego punktu na okręgu

                    this._minimumP = this.GetMaxOrMinPoint(true, this._minimumP, p1);           // aktualizuje wartości współrzędnych minimalnego punktu kontrolnego
                    this._maximumP = this.GetMaxOrMinPoint(false, this._maximumP, p1);          // aktualizuje wartości współrzędnych maksymalnego punktu kontrolnego
                }
            }

            // tworzy listę wierzchołków drugiego (nowego) końca segmentu:
            var new_radius = radius * this._cutThiFactor;                                       // skaluje promień okręgu
            v1 *= new_radius / v1.Length;
            v2 *= new_radius / v2.Length;
            ends = new List<Point3D>();
            for (int i = 0; i < this._sidesNumber; i++)
            {
                Point3D p1 = end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;           // tworzy punkt na okręgu wyznaczonym przez środek i promień
                ends.Add(p1);                                                                   // dodaje do listy punktów pierwszego końca segmentu
                theta += dtheta;                                                                // zwiększa kąt wskazujący położenie następnego punktu na okręgu

                this._minimumP = this.GetMaxOrMinPoint(true, this._minimumP, p1);               // aktualizuje wartości współrzędnych minimalnego punktu kontrolnego
                this._maximumP = this.GetMaxOrMinPoint(false, this._maximumP, p1);              // aktualizuje wartości współrzędnych maksymalnego punktu kontrolnego
            }


            if (doDrawing)
            {
                // tworzy boczne ściany segmentu wraz z odpowiednim materiałem:
                int sides = 0, x = 0, y = 1;
                DiffuseMaterial diffMat;
                if (this._selectedTexture == null)
                {
                    // tworzy materiał używając koloru szarego
                    diffMat = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
                    while (sides < this._sidesNumber + this._sidesNumber)
                    {
                        this.AddTriangle(mesh, begins[x % this._sidesNumber], ends[y % this._sidesNumber], begins[(x + 1) % this._sidesNumber]);
                        this.AddTriangle(mesh, begins[(x + 1) % this._sidesNumber], ends[y % this._sidesNumber], ends[(y + 1) % this._sidesNumber]);
                        x++; y++; sides++;
                    }
                }
                else
                {
                    // tworzy materiał z wybranej tekstury
                    ImageBrush texture = new ImageBrush();
                    texture.ImageSource = this._selectedTexture.TextureImage;
                    diffMat = new DiffuseMaterial(texture);
                    // tworzy punkty współrzędnych tekstur
                    Point t1 = new Point(0, 0);
                    Point t2 = new Point(1, 0);
                    Point t3 = new Point(1, 1);
                    Point t4 = new Point(0, 1);
                    while (sides < this._sidesNumber + this._sidesNumber)
                    {
                        this.AddTriangle(mesh, begins[x % this._sidesNumber], ends[y % this._sidesNumber], begins[(x + 1) % this._sidesNumber], t3, t1, t4);
                        this.AddTriangle(mesh, begins[(x + 1) % this._sidesNumber], ends[y % this._sidesNumber], ends[(y + 1) % this._sidesNumber], t3, t2, t1);
                        x++; y++; sides++;
                    }
                }
                return new GeometryModel3D(mesh, diffMat);
            }
            else return null;
        }

        /// <summary>
        /// Tworzy zakończenie krańcowego segmentu.
        /// </summary>
        /// <param name="ends">Lista punktów okręgu końca krańcowego segmentu</param>
        /// <param name="end_point">Szczyt zakończenia dla krańcowego segmentu</param>
        /// <returns>Nowo utworzony segment 3D</returns>
        private GeometryModel3D CreateSegmentCap(Point3D begin_point, List<Point3D> begins, Point3D end_point, Vector3D axis, double radius)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            // jeśli podano za mało wierzchołków początkowych, tworzy ich nową listę
            if (begins.Count == 0)
            {
                // tworzy dwa wektory prostopadłe do wektora axis
                Vector3D v1 = (axis.Z < -0.01) || (axis.Z > 0.01) ? new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y) : new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
                Vector3D v2 = Vector3D.CrossProduct(v1, axis);

                // kąt początkowy i kąt pomiędzy poszczególnymi punktami tworzącymi segment
                double theta = 0;
                double dtheta = 2 * Math.PI / this._sidesNumber;

                // skaluje długość wektorów do promienia okręgu
                v1 *= radius / v1.Length;
                v2 *= radius / v2.Length;

                for (int j = 0; j < this._sidesNumber; j++)
                {
                    Point3D p1 = begin_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;     // tworzy punkt na okręgu wyznaczonym przez środek i promień
                    begins.Add(p1);                                                             // dodaje do listy punktów pierwszego końca segmentu
                    theta += dtheta;                                                            // zwiększa kąt wskazujący położenie następnego punktu na okręgu

                    this._minimumP = this.GetMaxOrMinPoint(true, this._minimumP, p1);           // aktualizuje wartości współrzędnych minimalnego punktu kontrolnego
                    this._maximumP = this.GetMaxOrMinPoint(false, this._maximumP, p1);          // aktualizuje wartości współrzędnych maksymalnego punktu kontrolnego
                }
            }


            int sides = 0, i = 0;
            DiffuseMaterial diffMat;

            if (this._selectedTexture == null)
            {
                // tworzym materiał używając koloru szarego
                diffMat = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
                // tworzy ściany segmentu
                while (sides < this._sidesNumber)
                {
                    this.AddTriangle(mesh, begins[(i + 1) % this._sidesNumber], begins[i % this._sidesNumber], end_point);
                    i++; sides++;
                }
            }
            else
            {
                // tworzym materiał z tekstury
                ImageBrush texture = new ImageBrush();
                texture.ImageSource = this._selectedTexture.TextureImage;
                diffMat = new DiffuseMaterial(texture);
                // tworzym punkty współrzędnych tekstur
                Point t1 = new Point(0.5, 0);
                Point t2 = new Point(0, 1);
                Point t3 = new Point(1, 1);
                // tworzy ściany segmentu
                while (sides < this._sidesNumber + this._sidesNumber)
                {
                    this.AddTriangle(mesh, begins[(i + 1) % this._sidesNumber], begins[i % this._sidesNumber], end_point, t3, t2, t1);
                    i++; sides++;
                }
            }

            this._minimumP = this.GetMaxOrMinPoint(true, this._minimumP, end_point);        // aktualizuje wartości współrzędnych minimalnego punktu kontrolnego
            this._maximumP = this.GetMaxOrMinPoint(false, this._maximumP, end_point);       // aktualizuje wartości współrzędnych maksymalnego punktu kontrolnego

            return new GeometryModel3D(mesh, diffMat);
        }

        /// <summary>
        /// Tworzy nową odnogę trójwymiarowej rośliny.
        /// Metoda stworzona w oparciu o pracę użytkownika arussell opublikowaną 21 grudnia 2014 r. pod licencją CPOL na stronie www.codeproject.com.
        /// Link: http://www.codeproject.com/Articles/855693/D-L-System-with-Csharp-and-WPF
        /// </summary>
        /// <param name="currIndex">Indeks wskazujący znak w liscie poleceń</param>
        /// <param name="currPoz">Obecna pozycja kursora rysującego</param>
        /// <param name="quatRot">Kwaternion z aktualną transformacją</param>
        /// <param name="quatUp">Kwaternion z aktualną transformacją</param>
        /// <param name="segmLen">Dlugość pojedynczego segmentu</param>
        /// <param name="segmThi">Szerokość pojedynczego segmentu</param>
        /// <returns>Indeks aktualnie analizowanego symbolu na liście poleceń</returns>
        private int DrawOneBranch(int currIndex, Point3D currPoz, Quaternion quatRot, Quaternion quatUp, double segmLen, double segmThi)
        {
            Point3D nextPoz = new Point3D();                    // następna pozycja rysującego kursora
            Vector3D vMove = new Vector3D();                    // wektor przemieszczenia
            List<Point3D> beginPoints = new List<Point3D>();    // lista punktów okręgu początkowego końca segmentu
            List<Point3D> endPoints;                            // lista punktów okręgu nowego końca segmentu
            double segmLength = segmLen;                        // początkowa długość pojedynczego segmentu
            double segmThickness = segmThi;                     // początkowa szerokość pojedynczego segmentu
            // wektory obrotu:
            Vector3D vRoll = new Vector3D(1, 0, 0);             // obrót wokół osi X
            Vector3D vYaw = new Vector3D(0, 1, 0);              // obrót wokół osi Y
            Vector3D vPitch = new Vector3D(0, 0, 1);            // obrót wokół osi Z
            Vector3D vForward = vYaw;                           // kierunek 'przed siebie'

            int i;                                              // pozycja badanego symbolu na liście poleceń
            
            for (i = currIndex; i < this._commandStr.Length; i++) // bada każdy znak z podanego zakresu symboli z listy poleceń
            {
                switch (this._commandStr[i])
                {
                    case 'F':   // rysuj naprzód
                        vMove = this.CalculateNextPointVector(quatRot, this._segmentLength, vForward);
                        nextPoz = currPoz + vMove;
                        this._curr3Dmodel.Children.Add(this.CreateCylindricalSegment(true, currPoz, beginPoints, nextPoz, out endPoints, vMove, segmLength, segmThickness));
                        currPoz = nextPoz;
                        beginPoints = endPoints;
                        segmLength *= this._cutLenFactor;
                        segmThickness *= this._cutThiFactor;
                        break;

                    case 'f':   // idź naprzód (bez rysowania)
                        vMove = this.CalculateNextPointVector(quatRot, this._segmentLength, vForward);
                        nextPoz = currPoz + vMove;
                        this.CreateCylindricalSegment(false, currPoz, beginPoints, nextPoz, out endPoints, vMove, segmLength, segmThickness);
                        currPoz = nextPoz;
                        beginPoints = endPoints;
                        segmLength *= this._cutLenFactor;
                        segmThickness *= this._cutThiFactor;
                        break;

                    case 'B':   // rysuj do tyłu
                        vMove = this.CalculateNextPointVector(quatRot, this._segmentLength, vForward);
                        nextPoz = currPoz - vMove;
                        this._curr3Dmodel.Children.Add(this.CreateCylindricalSegment(true, currPoz, beginPoints, nextPoz, out endPoints, vMove, segmLength, segmThickness));
                        currPoz = nextPoz;
                        beginPoints = endPoints;
                        segmLength *= this._cutLenFactor;
                        segmThickness *= this._cutThiFactor;
                        break;

                    case 'b':   // idź do tyłu (bez rysowania)
                        vMove = this.CalculateNextPointVector(quatRot, this._segmentLength, vForward);
                        nextPoz = currPoz - vMove;
                        this.CreateCylindricalSegment(false, currPoz, beginPoints, nextPoz, out endPoints, vMove, segmLength, segmThickness);
                        currPoz = nextPoz;
                        beginPoints = endPoints;
                        segmLength *= this._cutLenFactor;
                        segmThickness *= this._cutThiFactor;
                        break;

                    case '+':   // obrót X+ (Pitch +)
                        double ra1 = this.GetRandomDouble(this._rotAngle - this._devAngle, this._rotAngle + this._devAngle);
                        quatRot *= new Quaternion(vPitch, ra1);
                        quatUp *= new Quaternion(vPitch, ra1);
                        break;

                    case '-':   // obrót X- (Pitch -)
                        double ra2 = this.GetRandomDouble(this._rotAngle - this._devAngle, this._rotAngle + this._devAngle);
                        quatRot *= new Quaternion(vPitch, -ra2);
                        quatUp *= new Quaternion(vPitch, -ra2);
                        break;

                    case '}':   // obrót Y+ (Yaw +)
                        double ra3 = this.GetRandomDouble(this._rotAngle - this._devAngle, this._rotAngle + this._devAngle);
                        quatRot *= new Quaternion(vYaw, ra3);
                        quatUp *= new Quaternion(vRoll, ra3);
                        break;

                    case '{':   // obrót Y- (Yaw -)
                        double ra4 = this.GetRandomDouble(this._rotAngle - this._devAngle, this._rotAngle + this._devAngle);
                        quatRot *= new Quaternion(vYaw, -ra4);
                        quatUp *= new Quaternion(vRoll, -ra4);
                        break;

                    case '>':   // obrót Z- (Roll +)
                        double ra5 = this.GetRandomDouble(this._rotAngle - this._devAngle, this._rotAngle + this._devAngle);
                        quatRot *= new Quaternion(vRoll, ra5);
                        quatUp *= new Quaternion(vYaw, ra5);
                        break;

                    case '<':   // obrót Z- (Roll -)
                        double ra6 = this.GetRandomDouble(this._rotAngle - this._devAngle, this._rotAngle + this._devAngle);
                        quatRot *= new Quaternion(vRoll, -ra6);
                        quatUp *= new Quaternion(vYaw, -ra6);
                        break;

                    case '[':   // zapamiętuje obecną pozycję kursora rysującego
                        i = this.DrawOneBranch(i + 1, currPoz, quatRot, quatUp, segmLength, segmThickness);
                        i--;
                        break;

                    case ']':   // wczytuje zapamiętaną pozycję kursora rysującego
                        // rysuje zakończenie gałęzi
                        vMove = this.CalculateNextPointVector(quatRot, this._segmentLength, vForward);
                        nextPoz = currPoz + vMove;
                        this._curr3Dmodel.Children.Add(this.CreateSegmentCap(currPoz, beginPoints, nextPoz, vMove, segmThickness));
                        return i + 1;

                    default:    // ignoruje nierozpoznane znaki
                        break;
                }
            }

            // rysuje zakończenie ostatniej gałęzi
            vMove = this.CalculateNextPointVector(quatRot, this._segmentLength, vForward);
            nextPoz = currPoz + vMove;
            this._curr3Dmodel.Children.Add(this.CreateSegmentCap(currPoz, beginPoints, nextPoz, vMove, segmThickness));

            return i + 1;
        }
        #endregion
    }
}
