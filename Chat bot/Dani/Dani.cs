using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public class Dani
    {
        //Dani's stats
        public double predictability;
        public int verbosity;
        public int habituality;

        //All the languages that Dani knows
        public List<Language> languages;

        //Dani's random generator
        Random random;

        //Base constructor for Dani
        private Dani()
        {
            languages = new List<Language>();
            random = new Random();

            //Dani is given random stats by default
            predictability = random.NextDouble();
            verbosity = random.Next(10, 100);
            habituality = random.Next(0, 10);
        }

        //Create a new instance of Dani, with a blank language of the given name
        public Dani(string languageName)
            : this()
        {
            languages.Add(new Language(languageName));
        }

        //Create a new instance of Dani, using the given language
        public Dani(Language language)
            : this()
        {
            languages.Add(language);
        }

        //Talk to Dani and get his response
        public string GetResponse(string text)
        {
            return languages[random.Next(languages.Count())].GetResponse(predictability, verbosity, habituality, text);
        }

        //Tell Dani something, without expecting a response, returns whether the given language exists
        public bool Tell(string language, string text)
        {
            foreach (Language lang in languages)
            {
                if (lang.name.ToLower() == language.ToLower())
                {
                    lang.AddText(text, true);
                    return true;
                }
            }
            return false;
        }
    }
}
