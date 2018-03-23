//#define AlphabetEnabled

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    class Program
    {
        private const string c_pathToFiles = @"D:\USERS\Rodion\Рабочий стол\Лекции\6-й семестр\Комп'ютерні системи\ComputerSystem\Lab1\";

        struct AlphabetResearch
        {
            public Dictionary<char, double> charFrequency;
            public double entropy;
            public double amountOfInformation;
        }

        static void Main(string[] args)
        {
            TextResearch("Мені_тринадцятий_минало", GetFileContent("Мені_тринадцятий_минало.txt"));
            TextResearch("Казка_про_рєпку,_або_Хулі_не_ясно", GetFileContent("Казка_про_рєпку,_або_Хулі_не_ясно.txt"));
            TextResearch("Специфікація_інтерфейсу_PCI", GetFileContent("Специфікація_інтерфейсу_PCI.txt"));

            Console.WriteLine("\nBASE64 Research:\n");

            TextResearch("Мені_тринадцятий_минало",
                Base64.Encode(GetByteFileContent("Мені_тринадцятий_минало.txt")));
            TextResearch("Мені_тринадцятий_минало Archive",
                Base64.Encode(GetByteFileContent("Мені_тринадцятий_минало.txt.bz2")));
            TextResearch("Казка_про_рєпку,_або_Хулі_не_ясно",
                Base64.Encode(GetByteFileContent("Казка_про_рєпку,_або_Хулі_не_ясно.txt")));
            TextResearch("Казка_про_рєпку,_або_Хулі_не_ясно Archive",
                Base64.Encode(GetByteFileContent("Казка_про_рєпку,_або_Хулі_не_ясно.txt.bz2")));
            TextResearch("Специфікація_інтерфейсу_PCI",
                Base64.Encode(GetByteFileContent("Специфікація_інтерфейсу_PCI.txt")));
            TextResearch("Специфікація_інтерфейсу_PCI Archive",
                Base64.Encode(GetByteFileContent("Специфікація_інтерфейсу_PCI.txt.bz2")));

            Console.ReadKey();
        }

        static void TextResearch(string fileName, string content)
        {
            AlphabetResearch alphabetResearch = new AlphabetResearch();
            HashSet<char> alphabet;


            alphabet = new HashSet<char>(content);
            alphabetResearch.charFrequency = new Dictionary<char, double>();

            foreach (char ch in alphabet)
                alphabetResearch.charFrequency[ch] = CalculateFrequency(content, ch);

            alphabetResearch.entropy = CalculateEntropy(alphabetResearch.charFrequency);
            alphabetResearch.amountOfInformation = CalculateAmountOfInformation(content, alphabetResearch.entropy);

            Console.WriteLine(fileName + "\n");
            ShowResearch(alphabetResearch);
        }

        static string GetFileContent(string fileName)
        {
            string content;

            using (StreamReader sr = new StreamReader(c_pathToFiles + fileName, Encoding.UTF8))
                content = sr.ReadToEnd().ToLower().Replace("\r\n", "\n");

            return content;
        }

        static byte[] GetByteFileContent(string fileName)
        {
            return File.ReadAllBytes(c_pathToFiles + fileName);
        }

        static double CalculateFrequency(string text, char ch)
        {
            int amountOfCharRepetition = text.Count(letter => letter == ch);

            return (double)amountOfCharRepetition / text.Length;
        }

        static double CalculateEntropy(Dictionary<char, double> charFrequency)
        {
            double entropy = 0;

            foreach (var alphabetChar in charFrequency)
                entropy -= alphabetChar.Value * Math.Log(alphabetChar.Value, 2);

            return entropy;
        }

        static double CalculateAmountOfInformation(string text, double entropy)
        {
            return entropy * text.Length / 8;
        }

        static void ShowResearch(AlphabetResearch research)
        {

#if AlphabetEnabled
            Console.WriteLine("Char\t-\tFrequency");

            foreach (var alphabetChar in research.charFrequency)
                Console.WriteLine("{0}\t-\t{1}",
                    alphabetChar.Key == '\n' ? "\\n" : alphabetChar.Key.ToString(),
                    alphabetChar.Value);
            Console.WriteLine();
#endif

            Console.WriteLine("Entropy: {0};\nAmount of information: {1}\n", research.entropy, research.amountOfInformation);
        }
    }

    static class Base64
    {
        private static char[] m_alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1',
            '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };

        public static string Encode(byte[] byteContent)
        {
            string encodedContent = "";

            for (int i = 0; byteContent.Length - i > 2; i += 3)
            {
                encodedContent += m_alphabet[byteContent[i] >> 2];
                encodedContent += m_alphabet[
                    ((byteContent[i] & 0b00000011) << 4) |
                    ((byteContent[i + 1] & 0b11110000) >> 4)];
                encodedContent += m_alphabet[
                    ((byteContent[i + 1] & 0b00001111) << 2) |
                    ((byteContent[i + 2] & 0b11000000) >> 6)];
                encodedContent += m_alphabet[byteContent[i + 2] & 0b00111111];
            }

            if (byteContent.Length % 3 == 1)
            {
                byte lastByte = byteContent.Last();
                encodedContent += m_alphabet[lastByte >> 2];
                encodedContent += m_alphabet[(lastByte & 0b00000011) << 4];
                encodedContent += "==";
            }
            else if (byteContent.Length % 3 == 2)
            {
                encodedContent += m_alphabet[byteContent[byteContent.Length - 2] >> 2];
                encodedContent += m_alphabet[
                    ((byteContent[byteContent.Length - 2] & 0b00000011) << 4) |
                    ((byteContent[byteContent.Length - 1] & 0b11110000) >> 4)];
                encodedContent += m_alphabet[(byteContent[byteContent.Length - 1] & 0b00001111) << 2];
                encodedContent += "=";
            }

            return encodedContent;
        }
    }
}