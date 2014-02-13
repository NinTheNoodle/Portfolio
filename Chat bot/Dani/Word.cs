using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public class Word
    {
        //List of all the sentences the word is used in
        public List<Sentence> sentences;

        //This word as a string
        public string word;

        //Create a new word
        private Word(Language language, string word)
        {
            if (language.words.ContainsKey(word))
                throw new Exception("Word: " + word + " already exists.");
            
            sentences = new List<Sentence>();
            language.words.Add(word, this);
            this.word = word;
        }

        //Either creates a new word or returns the current word
        public static Word AddWord(Language language, Sentence sentence, string word)
        {
            Word newWord;

            if (!language.words.ContainsKey(word))
                newWord = new Word(language, word);
            else
                newWord = language.words[word];

            //Log the use of the current sentence in the word
            newWord.sentences.Add(sentence);

            return newWord;
        }

        public override string ToString()
        {
            return word;
        }
    }
}
