using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public class Sentence
    {
        public struct SimilarSentence
        {
            public Sentence sentence;
            public int similarity;
        }
        
        //Set of all the words in the sentence - order not preserved
        public Dictionary<Word, List<Word>> words;

        //List of all sentences that have at least one word in common, and their similarity rank
        public List<SimilarSentence> similarSentences;

        //List of all words that have started the current sentence
        public List<Word> startingWords;

        //List of all sentences are known to have followed this one
        public List<Sentence> followingSentences;

        //Random number generator
        Random random;

        //Create a new sentence
        private Sentence(Language language, List<string> words)
        {
            random = new Random();
            followingSentences = new List<Sentence>();
            startingWords = new List<Word>();
            this.words = new Dictionary<Word,List<Word>>();
            this.similarSentences = new List<SimilarSentence>();

            Word previousWord = null;

            foreach (string token in words)
            {
                //Log the usage in this sentence for each word, creating it if it doesn't exist
                Word newWord = Word.AddWord(language, this, token);

                //Add this word to the current sentence
                if (!this.words.ContainsKey(newWord))
                    this.words.Add(newWord, new List<Word>());

                if (previousWord != null)
                    this.words[previousWord].Add(newWord);

                previousWord = newWord;

                //Add each of the sentences that each word is used in to this sentence's similarity list and rank them based on similarity
                foreach (Sentence sentence in newWord.sentences)
                {
                    SimilarSentence mySimilarSentence = new SimilarSentence();

                    mySimilarSentence.sentence = sentence;
                    mySimilarSentence.similarity = GetSimilarityRank(sentence);

                    similarSentences.Add(mySimilarSentence);

                    //Deal with the other sentence
                    SimilarSentence theirSimilarSentence = new SimilarSentence();

                    theirSimilarSentence.sentence = this;
                    theirSimilarSentence.similarity = sentence.GetSimilarityRank(this);

                    sentence.similarSentences.Add(theirSimilarSentence);
                }
            }
        }

        //Return a similar sentence that uses the given word
        public Sentence GetRandomSimilarSentence(Word word)
        {
            List<SimilarSentence> sentences = new List<SimilarSentence>();

            foreach (SimilarSentence similarSentence in similarSentences)
            {
                if (similarSentence.sentence.words.ContainsKey(word))
                    sentences.Add(similarSentence);
            }

            if (sentences.Count == 0)
                return GetRandomSimilarSentence();

            return GetRandomSimilarSentence(sentences);
        }

        //Return a similar sentence - excluding the set of given sentences
        public Sentence GetRandomSimilarSentence(HashSet<Sentence> exclude)
        {
            List<SimilarSentence> sentences = new List<SimilarSentence>();

            foreach (SimilarSentence similarSentence in similarSentences)
            {
                if (!exclude.Contains(similarSentence.sentence))
                    sentences.Add(similarSentence);
            }

            if (sentences.Count == 0)
                return GetRandomSimilarSentence();

            return GetRandomSimilarSentence(sentences);
        }
        
        public Sentence GetRandomSimilarSentence()
        {
            return GetRandomSimilarSentence(similarSentences);
        }
        
        //Return a weighted random similar sentence
        private Sentence GetRandomSimilarSentence(List<SimilarSentence> sentences)
        {
            int totalSimilarity = 0, minSimilarity = int.MaxValue;

            foreach (SimilarSentence similarSentence in sentences)
            {
                totalSimilarity  += similarSentence.similarity;
                minSimilarity = Math.Min(minSimilarity, similarSentence.similarity);
            }

            int i = random.Next(totalSimilarity - minSimilarity * sentences.Count);

            foreach (SimilarSentence similarSentence in sentences)
            {
                
                i -= similarSentence.similarity - minSimilarity;

                if (i <= 0)
                    return similarSentence.sentence;
            }
            throw new Exception();
        }


        //Return a random following sentence
        public Sentence GetRandomFollowingSentence()
        {
            if (followingSentences.Count == 0)
                return null;
            return followingSentences[random.Next(followingSentences.Count)];
        }

        public Word GetRandomFollowingWord(Word word)
        {
            return GetRandomFollowingWord(word, new HashSet<Word>());
        }

        //Returns a randomly chosen word to follow the given word
        public Word GetRandomFollowingWord(Word word, HashSet<Word> excludeSet)
        {
            List<Word> validWords = new List<Word>();

            foreach (Word current_word in words[word])
            {
                if (!excludeSet.Contains(current_word))
                    validWords.Add(current_word);
            }

            if (validWords.Count == 0)
                return null;

            return validWords[random.Next(validWords.Count)];
        }

        //Returns a randomly chosen first word
        public Word GetRandomFirstWord()
        {
            int i = random.Next(startingWords.Count);

            foreach (Word current_word in startingWords)
            {
                i--;
                if (i <= 0)
                    return current_word;
            }
            return null;
        }

        //Get the key for the sentence in the 
        public static string GetKey(List<string> words)
        {
            List<string> wordsSorted = new List<string>(words);
            wordsSorted.Sort();

            string key = "";

            foreach (string word in wordsSorted)
                key += word + " ";

            return key;
        }

        //Add a sentence to the language
        public static Sentence AddSentence(Language language, List<string> words)
        {
            Sentence newSentence;

            string key = GetKey(words);

            //Add the sentence to the language if it doesn't exist, otherwise we'll work off that sentence
            if (!language.sentences.ContainsKey(key))
                language.sentences.Add(key, new Sentence(language, words));

            newSentence = language.sentences[key];

            //Add the word starting this sentence to the list of words that can start this sentence
            newSentence.startingWords.Add(language.words[words[0]]);
            
            return newSentence;
        }

        //Returns how similar this sentence is to the given sentence
        public int GetSimilarityRank(Sentence sentence)
        {
            int rank = 0;
            //Each word the languages have in common earns a point
            foreach (KeyValuePair<Word, List<Word>> word in words)
            {
                if (sentence.words.ContainsKey(word.Key))
                    rank++;
                else
                    rank--;
            }

            //Each word the sentences don't have in common costs a point
            foreach (KeyValuePair<Word, List<Word>> word in sentence.words)
            {
                if (!words.ContainsKey(word.Key))
                    rank--;
            }

            return rank;
        }
    }
}
