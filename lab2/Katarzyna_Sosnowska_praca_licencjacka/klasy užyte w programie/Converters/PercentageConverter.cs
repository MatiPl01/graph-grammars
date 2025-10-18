using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Lintree.Converters
{
    /// <summary>
    /// Klasa PercentageConverter, udostępnia metodę konwertującą używaną do konwersji atrybutu 'Width' w GUI.
    /// Kod wzorowany na poście użytkownika Somedust opublikowanym 2 grudnia 2013r. na stronie www.stackoverflow.com,
    /// link: http://stackoverflow.com/questions/20326744/wpf-binding-width-to-parent-width0-3
    /// </summary>
    public class PercentageConverter : MarkupExtension, IValueConverter
    {
        #region Pola prywatne
        private static PercentageConverter _instance;
        #endregion

        #region Publiczne metody IValueConverter
        /// <summary>
        /// Konwertuje aktualną szerokość danego elementu GUI do nowej wartości będącej procentową częścią tego elementu.
        /// </summary>
        /// <param name="value">Wartość aktualnej szerokości wskazanej kontrolki GUI</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Procentowa wartość przez którą zostanie pomnożona wartość argumentu 'value'</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

        /// <summary>
        /// Nie zaimplementowany konwerter w drugą stronę.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Publiczne metody IServiceProvider
        /// <summary>
        /// Implementacja abstrakcyjnej metody odziedziczonej z IServiceProvider.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new PercentageConverter());
        }
        #endregion
    }
}
