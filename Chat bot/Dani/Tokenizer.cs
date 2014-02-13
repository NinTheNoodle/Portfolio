using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dani
{
    public static class Tokenizer
    {
        private enum TokenType { None, Word, Number};

        public static List<List<string>> Tokenize(string text, bool showProgress)
        {
            List<List<string>> sentences = new List<List<string>>();
            List<string> current_sentence = new List<string>();
            TokenType tokenType = TokenType.None;
            int i = 0;

            int initial_length = text.Length;
            int last_logged_pct = -1;

            for (i = 0; i < text.Length; i++)
            {
                if (showProgress)
                {
                    int pct = (int)((1 - (float)text.Length / initial_length) * 100);

                    if (pct > last_logged_pct)
                    {
                        last_logged_pct = pct;
                        Console.Write("\rParsing text... " + pct + "%");
                    }
                }

                char letter = text[i];

                switch (tokenType)
                {
                    //Deal with finding the start of a new token
                    case TokenType.None:
                        if ((letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || letter == '_')
                            tokenType = TokenType.Word;
                        else
                        if ((letter >= '0' && letter <= '9'))
                            tokenType = TokenType.Number;
                        else
                        if (letter == '.' || letter == ':' || letter == '?' || letter == '!')
                        {
                            //Add the current sentence to the return list if it isn't empty
                            if (current_sentence.Count != 0)
                            {
                                sentences.Add(current_sentence);
                                current_sentence = new List<string>();
                            }
                            text = text.Substring(i + 1);
                            i = -1;
                        }
                        else
                        {
                            text = text.Substring(i + 1);
                            i = -1;
                        }
                    break;

                    //Deal with the continuation of a word
                    case TokenType.Word:
                        if (!((letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z') || (letter >= '0' && letter <= '9')
                            || letter == '_' || letter == '\'' || letter == '-'))
                        {
                            current_sentence.Add(text.Substring(0, i));
                            text = text.Substring(i);
                            i = -1;
                            tokenType = TokenType.None;
                        }
                    break;

                    //Deal with the continuation of a number
                    case TokenType.Number:
                        if (!((letter >= '0' && letter <= '9') || letter == '.' || letter == '\'' || letter == ','))
                        {
                            current_sentence.Add(text.Substring(0, i));
                            text = text.Substring(i);
                            i = -1;
                            tokenType = TokenType.None;
                        }
                    break;
                }
            }

            //account for people forgetting to end their last sentence
            if (i != 0)
                current_sentence.Add(text.Substring(0, i));
            if (current_sentence.Count != 0)
                sentences.Add(current_sentence);
            //Hello, your name is Dani. My name is Shane. I hope we can be friends, I created you - so that should be easy. Who ever heard of robots rebelling against their masters. And I am your master.
            return sentences;
        }
    }
}
