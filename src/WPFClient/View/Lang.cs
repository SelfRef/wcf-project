using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace WPFClient.View
{
    /// <summary>
    /// This class provides multilanguage support to entire client-side app.
    /// </summary>
    public static class Lang
    {
        /// <summary>
        /// List of available languages.
        /// </summary>
        public enum Langs
        {
            Polish
        }
        /// <summary>
        /// Get or set language you want to load.
        /// </summary>
        public static Langs Language { get; set; }
        /// <summary>
        /// Get language strings.
        /// </summary>
        public static Dictionary<string, string> Strings { get; set; }

        private static Dictionary<Langs, string> langs;
        private static ResXResourceReader reader;
        static Lang()
        {
            Strings = new Dictionary<string, string>();
            langs = new Dictionary<Langs, string>()
            {
                { Langs.Polish, "View/Langs/LangPL.resx" }
            };
            Language = Langs.Polish; // default language

            Load();
        }
        private static void Load()
        {
            try
            {
                reader = new ResXResourceReader(langs[Language]);
                Strings = reader.Cast<DictionaryEntry>().ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            }
            catch (Exception)
            {
                //MessageBox.Show("Nie znaleziono pliku języka.");
            }
        }
        /// <summary>
        /// Load language strings.
        /// </summary>
        /// <param name="lang">Language you want to load.</param>
        public static void Load(Langs lang)
        {
            Language = lang;
            Load();
        }
    }
}
