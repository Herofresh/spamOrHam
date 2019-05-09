using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SpamORHamV2
{
    class Document
    {
        public bool spam = false;
        public string msg;
        public ConcurrentDictionary<keys, bool> vector = new ConcurrentDictionary<keys, bool>(6,6);

        List<string> keyWords = new List<string>() { "offer", "xxx", "sex", "dear", "award", "only", "prize", "award", "free", "won", "ringtone", "txt", "urgent", "give away"};

        public enum keys
        {
            forbiddenWord, maxWordLength, repeatingSpecialChar, emailOrLink, capitalLetters, call, dollar
        }

        public Document(string rawText)
        {
            var splitt = rawText.Split(';');
            if(splitt[0].Equals("spam"))
            {
                spam = true;
            }
            msg = splitt[1];
            EvaluateText(msg);
        }

        private void EvaluateText(string s)
        {
            Thread[] allThreads = new Thread[] {
            new Thread(checkForbiddenWords),
            new Thread(checkMaxWordLength),
            new Thread(checkRepeatingSpecialChars),
            new Thread(checkForEmailOrLink),
            new Thread(checkRateOfCapitalLettersGreaterHalf),
            new Thread(checkForCallNumber),
            new Thread(checkForDollar),
            };
            foreach (var t in allThreads)
            {
                t.Start(s);
            }
            foreach (var t in allThreads)
            {
                t.Join();
            }
        }

        private void checkForbiddenWords(object s)
        {
            string[] msg = Convert.ToString(s).Split(' ');
            bool flag = false;
            msg = msg.Select(x => x.ToLowerInvariant()).ToArray();
            foreach (var word in keyWords)
            {
                if(msg.Contains(word))
                {
                    flag = true;
                    break;
                }
            }
            vector.TryAdd(keys.forbiddenWord, flag);
        }

        private void checkMaxWordLength(object s)
        {
            string[] msg = Convert.ToString(s).Split(' ');
            int max = 0;
            bool flag = false;
            foreach(var word in msg)
            {
                if(word.Length > max)
                {
                    max = word.Length;
                }
            }
            if(max > 75)
            {
                flag = true;
            }
            vector.TryAdd(keys.maxWordLength, flag);
        }

        private void checkRepeatingSpecialChars(object s)
        {
            string msg = Convert.ToString(s);
            bool flag = false;
            for (int i = 0; i <msg.Length -2; i++)
            {
                if(msg[i] == msg[i+1] && Char.IsLetterOrDigit(msg[i]))
                {
                    flag = true;
                }
            }
            vector.TryAdd(keys.repeatingSpecialChar, flag);
        }

        private void checkForEmailOrLink(object s)
        {
            string msg = Convert.ToString(s);
            bool flag = false;
            var emailRegex = new Regex(@"^[a - zA - Z0 - 9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
            var linkRegex = new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$");
            if (emailRegex.IsMatch(msg))
                flag = true;
            if (linkRegex.IsMatch(msg))
                flag = true;
            vector.TryAdd(keys.emailOrLink, flag);
        }

        private void checkRateOfCapitalLettersGreaterHalf(object s)
        {
            string msg = Convert.ToString(s);
            bool flag = false;
            double count = 0;
            foreach(var c in msg)
            {
                if (Char.GetUnicodeCategory(c).Equals(UnicodeCategory.UppercaseLetter))
                    count++;
            }
            if (count / msg.Length >= 0.5)
                flag = true;
            vector.TryAdd(keys.capitalLetters, flag);
        }

        private void checkForCallNumber(object s)
        {
            string msg = Convert.ToString(s);
            bool flag = false;
            var callRegex = new Regex(@"call \d");
            if (callRegex.IsMatch(msg))
                flag = true;
            vector.TryAdd(keys.call, flag);
        }

        private void checkForDollar(object s)
        {
            string msg = Convert.ToString(s);
            bool flag = false;
            if (msg.Contains("$"))
                flag = true;
            vector.TryAdd(keys.dollar, flag);
        }
    }
}
