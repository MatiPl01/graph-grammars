using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lintree.Common
{
    /// <summary>
    /// Klasa ObservableObject, pozwala na korzystanie z interfejsu INotifyPropertyChanged (po którym dziedziczy).
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        #region Publiczne pola INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;   // zdarzenie zmiany wartości właściwości
        #endregion

        #region Chronione metody
        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nazwa właściwości, której wartość uległa zmianie</param>
        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
