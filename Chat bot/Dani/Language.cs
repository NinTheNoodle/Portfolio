using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public class Language
    {
        //Dictionary of all the words in the language
        public Dictionary<string, Word> words;

        //Dictionary of all the sentences that have been added to the language
        public Dictionary<string, Sentence> sentences;

        //The name of the language
        public string name;

        //Random number generator
        Random random;

        //Create a new blank language with the given name
        public Language(string name)
        {
            this.name = name;
            random = new Random();
            words = new Dictionary<string, Word>();
            sentences = new Dictionary<string, Sentence>();
        }

        //Splits the given text and adds the resulting sentences to the language then returns the number of sentences added
        public int AddText(string text, bool showProgress)
        {
            //Ignore case
            text = text.ToLower();
            Sentence followingSentence = null, newSentence;

            if (showProgress)
                Console.Write("\nParsing text...");

            List<List<string>> tokens = Tokenizer.Tokenize(text, showProgress);

            int sentence_count = tokens.Count, i = 0;

            //Add each sentence in turn
            foreach (List<string> sentence in tokens)
            {
                if (showProgress && sentence_count >= 100)
                {
                    if (i % (sentence_count / 100) == 0)
                        Console.Write("\rAdding sentences... " + (int)((((float)i) / sentence_count) * 100) + "%");

                    i++;
                }

                newSentence = Sentence.AddSentence(this, sentence);

                if (followingSentence != null)
                    followingSentence.followingSentences.Add(newSentence);

                followingSentence = newSentence;
            }

            if (showProgress)
                Console.Write("\rText processed!                    \n\n");

            return tokens.Count();
        }

        //Generate a response based on the given text using this language
        public string GetResponse(double predictability, int responseLength, int repeatability, string text)
        {
            //Ignore case
            text = text.ToLower();

            string response = "";
            Sentence sentence, followingSentence;
            List<List<string>> tokens = Tokenizer.Tokenize(text, false);

            //Add the text to the language first, so that comparrisons can be done on it
            AddText(text, false);
            
            //Get the sentence object for the last added sentence in order to start the sentence generation
            string key = Sentence.GetKey(tokens[random.Next(tokens.Count)]);
            sentence = sentences[key];

            Word word = null, followingWord = null;

            HashSet<Word> usedWords;
            HashSet<Sentence> usedSentences = new HashSet<Sentence>();

            //The quality is considered to have lowered whenever a word is inserted or an attempt is made to repeat a word or sentence
            int responseQuality = responseLength;

            //While the response is still high enough quality - churn out sentences
            while (responseQuality > 0)
            {
                //Find a similar sentence and then a response if it exists
                sentence = sentence.GetRandomSimilarSentence(usedSentences);
                followingSentence = sentence.GetRandomFollowingSentence();

                if (followingSentence != null)
                    sentence = followingSentence;

                if (usedSentences.Contains(sentence) && responseQuality > 0)
                {
                    responseQuality /= 2;
                }

                if (responseQuality <= 0)
                    break;

                //The set of all words used in this sentence - words may not repeat
                usedWords = new HashSet<Word>();
                int reusageExceptions = repeatability;

                bool isFirstWord = true;
                //Start with one of the words known to be able to start this sentence
                word = sentence.GetRandomFirstWord();
                //Populate the sentence
                while (true)
                {
                    usedSentences.Add(sentence);

                    //Abort the sentence if no word was found
                    if (word == null)
                        break;

                    responseQuality--;

                    if (responseQuality <= 0)
                        break;

                    usedWords.Add(word);

                    //Capitalise first word
                    if (isFirstWord)
                        response += word.word.Substring(0, 1).ToUpper() + word.word.Substring(1) + " ";
                    else
                        response += word + " ";

                    //All other words in the sentence obviously aren't the first
                    isFirstWord = false;

                    if (!sentence.words.ContainsKey(word))
                        sentence = sentence.GetRandomSimilarSentence(word);
                    //Select a random word form this sentence known to follow the current word
                    followingWord = sentence.GetRandomFollowingWord(word, usedWords);

                    //If there is no following word or the predictability test fails - change sentence and try again
                    if (followingWord == null || random.NextDouble() > predictability)
                    {
                        sentence = sentence.GetRandomSimilarSentence(word);

                        followingWord = sentence.GetRandomFollowingWord(word, usedWords);
                        if (followingWord == null && reusageExceptions > 0)
                        {
                            reusageExceptions--;
                            followingWord = sentence.GetRandomFollowingWord(word);
                        }
                    }

                    word = followingWord;
                }

                response = response.TrimEnd() + ". ";
            }
            
            return response;
        }
    }
}
