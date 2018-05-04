using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hangman
{
    class Program
    {
        static void Main(string[] args)
        {
            string strWord;
            bool blnWordCorrect = false;
            string[] strDictionary = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "brit-a-z.txt");
            string[] strMatchingWords;
            int[] intLetters = new int[30];
            int[] intCorrectLetters = new int[30];
            int[] intCurrentWord = new int[30];
            int maxValue;
            int maxValueIndex;
            int maxValueIndexFormatted;
            int veryHighPriority;
            int medPriority;
            int lowPriority;
            bool blnCorrectLetter;
            bool blnCorrectWord = false;
            int intTries = 0;
            int intMistakes = 0;
            List<string> approvedWords = new List<string>();
            List<string> lowPriorityApprovedWords = new List<string>();
            List<string> medPriorityApprovedWords = new List<string>();
            List<string> highPriorityApprovedWords = new List<string>();
            List<string> veryHighPriorityApprovedWords = new List<string>();
            List<char> completedChars = new List<char>();
            char[] wordGuess;
            char[] currentWord;
            int intRelevancy;

            do
            {
                Console.WriteLine("Write the word for the program to guess.");

                strWord = Console.ReadLine().ToLower();

                if (!String.IsNullOrEmpty(strWord) && Array.Exists(strDictionary, s => s.Equals(strWord)))
                {
                    blnWordCorrect = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid word.");
                }
            }
            while (!blnWordCorrect);

            wordGuess = new char[strWord.Length];
            currentWord = new char[strWord.Length];
            veryHighPriority = strWord.Length - 1;
            medPriority = (strWord.Length / 3) * 2;
            lowPriority = strWord.Length / 3;
            strMatchingWords = Array.FindAll(strDictionary, s => s.Length == strWord.Length);

            foreach (string word in strMatchingWords)
            {
                foreach (char c in word)
                {
                    int i = ((int)c % 32);

                    intLetters[i]++;
                }
            }

            do
            {
                veryHighPriorityApprovedWords = new List<string>();
                highPriorityApprovedWords = new List<string>();
                approvedWords = new List<string>();

                maxValue = intLetters.Max();
                maxValueIndex = intLetters.ToList().IndexOf(maxValue);
                maxValueIndexFormatted = maxValueIndex + 96;

                Console.WriteLine("The letter I guess is " + (char)maxValueIndexFormatted);

                blnCorrectLetter = false;

                int currentStep = 0;

                foreach (char c in strWord)
                {
                    if (c == (char)maxValueIndexFormatted)
                    {
                        Console.Write(c);
                        blnCorrectLetter = true;
                        wordGuess[currentStep] = c;
                        completedChars.Add(c);
                    }
                    else if (c == wordGuess[currentStep])
                    {
                        Console.Write(c);
                    }
                    else
                    {
                        Console.Write("_");
                    }

                    currentStep++;
                }

                Console.WriteLine();

                if (!blnCorrectLetter)
                {
                    intMistakes++;

                    intLetters[maxValueIndex] = 0;
                }
                else
                {
                    foreach (string word in strMatchingWords)
                    {
                        intRelevancy = 0;
                        currentWord = new char[strWord.Length];
                        currentStep = 0;

                        foreach (char c in word)
                        {
                            currentWord[currentStep] = c;

                            currentStep++;
                        }

                        currentStep = 0;

                        foreach (char c in currentWord)
                        {
                            if (c == wordGuess[currentStep])
                            {
                                intRelevancy++;
                            }

                            currentStep++;
                        }

                        if (intRelevancy >= veryHighPriority)
                        {
                            veryHighPriorityApprovedWords.Add(word);
                        }
                        else if (intRelevancy > medPriority && intRelevancy < veryHighPriority)
                        {
                            highPriorityApprovedWords.Add(word);
                        }
                        else if (intRelevancy > lowPriority && intRelevancy <= medPriority)
                        {
                            medPriorityApprovedWords.Add(word);
                        }
                        else if (intRelevancy > 1 && intRelevancy <= lowPriority)
                        {
                            lowPriorityApprovedWords.Add(word);
                        }
                        else if (intRelevancy == 1)
                        {
                            approvedWords.Add(word);
                        }
                    }

                    intLetters = new int[30];

                    if (veryHighPriorityApprovedWords.Count > 0)
                    {
                        ProcessList(veryHighPriorityApprovedWords, ref intLetters);
                    }
                    else if (highPriorityApprovedWords.Count > 0)
                    {
                        ProcessList(highPriorityApprovedWords, ref intLetters);
                    }
                    else if (medPriorityApprovedWords.Count > 0)
                    {
                        ProcessList(medPriorityApprovedWords, ref intLetters);
                    }
                    else if (lowPriorityApprovedWords.Count > 0)
                    {
                        ProcessList(lowPriorityApprovedWords, ref intLetters);
                    }
                    else
                    {
                        ProcessList(approvedWords, ref intLetters);
                    }

                    foreach (char c in completedChars)
                    {
                        int i = (int)c % 32;

                        intLetters[i] = 0;
                    }
                }

                intTries++;

                string finalWord = new string(wordGuess);

                if (strWord == finalWord) {
                    blnCorrectWord = true;
                }
                
            } while (!blnCorrectWord);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("It took {0} tries to guess your word!", intTries);

            if (intMistakes > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("However, I made {0} mistake(s)", intMistakes);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else {
                Console.WriteLine("And, I made no mistakes!");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Read();
        }

        static void ProcessList(List<string> wordList, ref int[] intLetters)
        {
            foreach (string word in wordList)
            {
                foreach (char c in word)
                {
                    int i = ((int)c % 32);

                    intLetters[i]++;
                }
            }
        }
    }
}
