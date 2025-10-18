using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa CameraController, udostępnia kamerę używaną w widoku 3D.
    /// </summary>
    class CameraController
    {
        #region Prywatne pola
        private PerspectiveCamera _camera;                       // Obiekt perspektywicznej kamery
        private Point3D _position = new Point3D(0, 0, 50);       // Domyślna (startowa) pozycja kamery
        private Vector3D _lookDirVec = new Vector3D(0, 0, -50);  // Wektor LookDirection
        private Vector3D _upDirVec = new Vector3D(0, 1, 0);      // Wektor UpDirection
        private double _fov = 70.0;                              // Wartość FieldOfView
        private double _npd = 0.125;                             // Wartość NearPlaneDistance
        private Transform3DGroup _transformGroup;                // Kolekcja transformacji głównego modelu 3D
        private bool _isMLBdown;                                 // Flaga wskazująca, czy LPM jest wciśnięty
        private Point _mousePos;                                 // Pozycja kursora myszy na ekranie
        private double _smallDouble = 0.000001;                  // Dolna granica wartości zmiany położenia dla której obliczana jest transformacja modelu 3D
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Kamera perspektywiczna.
        /// </summary>
        public PerspectiveCamera PerspCamera
        {
            get { return this._camera; }
            set { }
        }

        /// <summary>
        /// Kolekcja transformacji modelu 3D
        /// </summary>
        public Transform3DGroup TransformGroup
        {
            get { return this._transformGroup; }
            set { }
        }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy CameraController.
        /// </summary>
        public CameraController()
        {
            // Inicjalizacja pól klasy :
            this._camera = new PerspectiveCamera();
            this._camera.Position = this._position;
            this._camera.LookDirection = this._lookDirVec;
            this._camera.UpDirection = this._upDirVec;
            this._camera.FieldOfView = this._fov;
            this._camera.NearPlaneDistance = this._npd;
            this._transformGroup = new Transform3DGroup();
            this._isMLBdown = false;
            this._mousePos = new Point();
        }
        #endregion

        /* =======================================================================================
         * Kod w poniższym regionie został napisany w oparciu o pracę użytkownika o nicku arussell
         * opublikowaną 21 grudnia 2014 r. pod licencją CPOL na stronie www.codeproject.com,
         * link: http://www.codeproject.com/Articles/855693/D-L-System-with-Csharp-and-WPF
         * ======================================================================================= */
        #region Publiczne metody symulujące pracę kamery
        /// <summary>
        /// Umożliwia przybliżanie/oddalanie widoku modelu 3D.
        /// </summary>
        public void MouseWheelBeh(MouseWheelEventArgs e)
        {
            double speed = 100.0;
            double pos = this._camera.Position.Z - e.Delta / speed;

            if (pos < 5.0) pos = 5.0;
            if (pos > 1000.0) pos = 1000.0;

            this._camera.Position = new Point3D(this._camera.Position.X,
                                                this._camera.Position.Y,
                                                pos);
        }

        /// <summary>
        /// Resetuje pozycję kamery do ustawień domyślnych.
        /// </summary>
        public void MouseRightButtonUpBeh()
        {
            this._camera.Position = this._position;
            _transformGroup.Children.Clear();
        }

        /// <summary>
        /// Włącza możliwość obracania widoku modelu 3D.
        /// </summary>
        /// <param name="panel3D"></param>
        /// <param name="e"></param>
        public void MouseLeftButtonDownBeh(Grid panel3D, MouseButtonEventArgs e)
        {
            this._isMLBdown = true;
            this._mousePos = e.GetPosition(panel3D);
        }

        /// <summary>
        /// Wyłącza możliwość obracania widoku modelu 3D.
        /// </summary>
        public void MouseLeftButtonUpBeh()
        {
            this._isMLBdown = false;
        }

        /// <summary>
        /// Obraca widok modelu 3D przy poruszeniu myszą (LPM musi być wciśnięty).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseMoveBeh(Grid panel3D, MouseEventArgs e)
        {
            if (this._isMLBdown)
            {
                Point currPos = new Point(this._mousePos.X - e.GetPosition(panel3D).X,
                                          this._mousePos.Y - e.GetPosition(panel3D).Y);
                RotateTransform3D rotation;

                if (Math.Abs(currPos.X) > this._smallDouble) // obrót wokół osi Z
                {
                    rotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), currPos.X));
                    _transformGroup.Children.Add(rotation);
                }

                if (Math.Abs(currPos.Y) > this._smallDouble) // obrót wokół osi X
                {
                    rotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), currPos.Y));
                    _transformGroup.Children.Add(rotation);
                }

                this._mousePos = e.GetPosition(panel3D);
            }
        }
        #endregion
    }
}
