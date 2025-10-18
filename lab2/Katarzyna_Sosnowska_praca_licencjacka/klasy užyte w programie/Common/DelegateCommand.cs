using System;
using System.Windows.Input;

namespace Lintree.Common
{
    /// <summary>
    /// Klasa DelegateCommant, pozwala używać komend w programie.
    /// </summary>
    class DelegateCommand : ICommand
    {
        #region Prywatne pola
        private readonly Predicate<object> _canExecute;     // określa, czy można wykonać komendę
        private readonly Action<object> _execute;           // akcja wykonywana podczas działania komendy
        public event EventHandler CanExecuteChanged;        // zdarzenie aktualizaci wartości _canExecute
        #endregion

        #region Konstruktory
        /// <summary>
        /// Konstruktor klasy DelegateCommand.
        /// </summary>
        /// <param name="execute">Akcja wykonywana podczas działania komendy</param>
        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        /// <summary>
        /// Konstruktor klasy DelegateCommand.
        /// </summary>
        /// <param name="execute">Akcja wykonywana podczas działania komendy</param>
        /// <param name="canExecute">Metoda określania, czy można wykonać komendę</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region Publiczne metody
        /// <summary>
        /// Metoda stwierdzająca, czy można wykonać akcję związaną z działaniem komendy.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;

            return _canExecute(parameter);
        }

        /// <summary>
        /// Akcja wykonywana podczas działania komendy
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Zdarzenie zmiany parametru odpowiedzialnego za stwierdzanie możliwości wykonania komendy.
        /// </summary>
        public void RaiseCanExecutedChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
