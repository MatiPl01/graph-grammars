using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Lintree.Model
{
    /// <summary>
    /// Klasa XMLGenerator, zawierająca statyczne metody zapisywania/odczytywania plików XML z konfiguracją L-systemu.
    /// </summary>
    class XMLGenerator
    {
        /// <summary>
        /// Zapisuje parametry obecnego l-systemu i konstrukcji modelu do pliku xml.
        /// </summary>
        /// <param name="csGen">Referencja do obiektu klasy CommandStringGenerator</param>
        /// <param name="mGen">Referencja do obiektu klasy ModelGenerator</param>
        public static void SaveXMLFileContent(ref CommandStringGenerator csGen, ref ModelGenerator mGen)
        {
            // otwiera okno zapisywania pliku :
            SaveFileDialog saveDial = new SaveFileDialog();
            saveDial.Filter = "xml files (*.xml)|*.xml|text files (*.txt)|*.txt|all files(*.*)|*.*";
            saveDial.FilterIndex = 0;
            var currDir = Directory.GetCurrentDirectory() + "\\Patterns";
            // sprawdza czy katalog Patterns istnieje - jeśli nie, tworzy nowy :
            if (!Directory.Exists(currDir))
            {
                Directory.CreateDirectory(currDir);
            }
            saveDial.InitialDirectory = currDir;
            saveDial.RestoreDirectory = true;
            Nullable<bool> result = saveDial.ShowDialog();
            if (result == true)
            {
                // tworzy listy reguł produkcji :
                string[] separator = { "\r\n" };
                string[] preRules = csGen.Rules.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                List<XElement> postRules = new List<XElement>();
                foreach (string r in preRules)
                {
                    postRules.Add(new XElement("rule", new string(r.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray())));
                }
                // tworzy zawartość XML :
                XElement content = new XElement("pattern",
                    new XElement("l-system",
                        new XElement("axiom", csGen.Axiom),
                        new XElement("rules", from el in postRules select el),
                        new XElement("iterations", csGen.Iterations)
                    ),
                    new XElement("model-param",
                        new XElement("initAngle", mGen.InitAngle),
                        new XElement("rotAngle", mGen.RotAngle),
                        new XElement("devAngle", mGen.DevAngle),
                        new XElement("nrSides", mGen.SidesNumber),
                        new XElement("segmentLength", mGen.SegmLength),
                        new XElement("cutLengthFactor", mGen.CutLenFactor),
                        new XElement("segmentThickness", mGen.SegmThickness),
                        new XElement("cutThicknessFactor", mGen.CutThiFactor)
                    )
                );
                content.Save(saveDial.FileName);
            }
        }

        /// <summary>
        /// Wczytuje parametry l-systemu i konstrukcji modelu z pliku XML.
        /// </summary>
        /// <param name="csGen">Referencja do obiektu klasy CommandStringGenerator</param>
        /// <param name="mGen">Referencja do obiektu klasy ModelGenerator</param>
        public static void LoadXMLFileContent(ref CommandStringGenerator csGen, ref ModelGenerator mGen)
        {
            // otwiera okno otwierania pliku :
            OpenFileDialog loadDial = new OpenFileDialog();
            loadDial.Filter = "xml files (*.xml)|*.xml|text files (*.txt)|*.txt|all files(*.*)|*.*";
            loadDial.FilterIndex = 0;
            var currDir = Directory.GetCurrentDirectory() + "\\Patterns";
            // sprawdza czy katalog Patterns istnieje - jeśli nie, tworzy nowy :
            if (!Directory.Exists(currDir))
            {
                Directory.CreateDirectory(currDir);
            }
            loadDial.InitialDirectory = currDir;
            loadDial.RestoreDirectory = true;
            Nullable<bool> result = loadDial.ShowDialog();
            if (result == true)
            {
                try
                {
                    // odczytuje zawartość wskazanego pliku XML :
                    XElement content = XElement.Load(loadDial.FileName);
                    var nodes = content.Elements();
                    foreach (var n in nodes)
                    {
                        if (n.Name == "l-system")
                        {
                            var lsystem = n.Elements();
                            foreach (var l in lsystem)
                            {
                                switch (l.Name.ToString())
                                {
                                    case "axiom":
                                        csGen.Axiom = l.Value;
                                        break;

                                    case "rules":
                                        string temp = "";
                                        var rulesList = l.Elements();
                                        foreach (var r in rulesList)
                                        {
                                            temp += r.Value + "\r\n";
                                        }
                                        csGen.Rules = temp;
                                        break;

                                    case "iterations":
                                        csGen.Iterations = l.Value;
                                        break;

                                    default:
                                        MessageBox.Show("There is an error in this file!");
                                        return;
                                }
                            }
                        }
                        else if (n.Name == "model-param")
                        {
                            var modelParam = n.Elements();
                            foreach (var m in modelParam)
                            {
                                switch(m.Name.ToString())
                                {
                                    case "initAngle":
                                        mGen.InitAngle = m.Value;
                                        break;

                                    case "rotAngle":
                                        mGen.RotAngle = m.Value;
                                        break;

                                    case "devAngle":
                                        mGen.DevAngle = m.Value;
                                        break;

                                    case "nrSides":
                                        mGen.SidesNumber = m.Value;
                                        break;

                                    case "segmentLength":
                                        mGen.SegmLength = m.Value;
                                        break;

                                    case "cutLengthFactor":
                                        mGen.CutLenFactor = m.Value;
                                        break;

                                    case "segmentThickness":
                                        mGen.SegmThickness = m.Value;
                                        break;

                                    case "cutThicknessFactor":
                                        mGen.CutThiFactor = m.Value;
                                        break;

                                    default:
                                        MessageBox.Show("There is an error in this file!");
                                        return;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("There is an error in this file!");
                            return;
                        }
                    }
                }
                catch (Exception e) { MessageBox.Show("Error: " + e.Message); }
            }
        }
    }
}
