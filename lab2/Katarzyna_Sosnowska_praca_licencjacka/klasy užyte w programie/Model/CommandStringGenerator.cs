using Lintree.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa CommandStringGenerator, odpowiada za konstrukcję listy poleceń do budowania modelu 3D. Dziedziczy po ObservableObject.
    /// </summary>
    class CommandStringGenerator : ObservableObject
    {
        #region Prywatne pola
        private string _axiom;          // Aksjomat (punkt startowy) wywodu dla danego L-systemu.
        private string _rules;          // Reguły produkcji dla danego L-systemu.
        private int _iterations;        // Liczba iteracji algorytmu tworzącego model 3D na bazie danego L-systemu.
        private string _commString;     // Lista poleceń tworzona na bazie parametrów danego L-systemu używana później do tworzenia modelu 3D.
        #endregion

        #region Publiczne właściwości
        /// <summary>
        /// Aksjomat (punkt startowy).
        /// </summary>
        public string Axiom
        {
            get { return this._axiom; }
            set
            {
                this._axiom = value;
                base.RaisePropertyChangedEvent("Axiom");
            }
        }

        /// <summary>
        /// Reguły produkcji.
        /// </summary>
        public string Rules
        {
            get { return this._rules; }
            set
            {
                this._rules = value;
                base.RaisePropertyChangedEvent("Rules");
            }
        }

        /// <summary>
        /// Liczba iteracji algorytmu.
        /// </summary>
        public string Iterations
        {
            get { return this._iterations.ToString(); }
            set
            {
                if (!int.TryParse(value, out this._iterations))
                {
                    this._iterations = 1;
                }
                if (this._iterations < 1)
                {
                    this._iterations = 1;
                }
                base.RaisePropertyChangedEvent("Iterations");
            }
        }

        /// <summary>
        /// Końcowy wywód dla danego L-systemu (lista poleceń dla metody tworzącej model 3D).
        /// </summary>
        public string CommandString
        {
            get { return this._commString.ToString(); }
            set
            {
                this._commString = value;
                base.RaisePropertyChangedEvent("CommandString");
            }
        }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor klasy CommandStringBuilder.
        /// </summary>
        public CommandStringGenerator()
        {
            // Inicjalizacja pól klasy domyślnymi wartościami :
            this._axiom = "F";
            this._rules = "F:F";
            this._iterations = 1;
        }
        #endregion

        #region Publiczne metody - generator końcowego wywodu dla danego L-systemu
        /// <summary>
        /// Metoda tworząca końcowy wywód dla danego L-systemu.
        /// </summary>
        public void CreateCommandString()
        {
            // inicjalizuje listę poleceń obecnym aksjomatem
            this._commString = new string(this._axiom.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());   // usuwa niepotrzebne białe znaki

            // pozyskuje liste reguł produkcji, które zostały poprawnie podane przez użytkownika :
            string[] separator = { "\r\n" };
            char[] techSym = {':', '='};
            string[] preRules = this._rules.Split(separator, StringSplitOptions.RemoveEmptyEntries);                // dzieli jednorodny tekst zbioru reguł na listę pojedynczych reguł
            List<string[]> rules = new List<string[]>();
            for (int i = 0; i < preRules.Length; i++)
            {
                preRules[i] = new string(preRules[i].ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());    // usuwa niepotrzebne białe znaki
                var tmp = preRules[i].Split(techSym);
                if (tmp.Length > 1 && tmp.Length < 3 && tmp[0] != "")                                               // sprawdza poprawność zapisu reguły
                {
                    rules.Add(preRules[i].Split(techSym));
                }
            }

            // algorytm generowania wywodu na bazie podanych informacji :
            StringBuilder temp;
            int commLen, j;
            bool hasRule;                                                           // flaga informująca, czy udało się dopadować jakąś regułę do badanego symbolu
            for (int i = 0; i < this._iterations; i++)                              // dla każdej iteracji wykonuje algorytm zamiany symboli
            {
                temp = new StringBuilder();
                commLen = this._commString.Length;
                j = 0;
                while(j < commLen)
                {
                    hasRule = false;
                    for (int k = 0; k < rules.Count; k++)                           // sprawdza, czy istnieje jakaś produkcja, którą można zastosować
                    {
                        if (j > commLen) break;

                        int numb = rules[k][0].Length;
                        if (j + numb <= commLen)
                        {
                            var substr = this._commString.Substring(j, numb);
                            if (substr == rules[k][0].ToString())
                            {
                                temp.Append(rules[k][1]);                           // jeśli została znaleziona produkcja pasująca do aktualnego podciągu znaków
                                j += rules[k][0].Length - 1;                        // to po jej zastosowaniu poszukiwania odpowiedniej reguły zostają przerwane
                                hasRule = true;                                     // udało się znaleźć jakąś regułę
                                break;
                            }
                        }
                    }
                    if (!hasRule) temp.Append(this._commString[j]);                           // jeśli nie udalo się dopasować żadnej reguły po prostu przepisuje badany symbol

                    j++;
                }
                this._commString = temp.ToString();
            }
        }
        #endregion
    }
}
