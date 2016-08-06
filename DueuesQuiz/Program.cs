using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.XPath;
using DueuesQuiz;
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace DueuesQuiz
{
    public class Program
    {
        static void Main()
        {
            InputReader inputReader = new InputReader();
            inputReader.ReadInput();

            Console.WriteLine("");

            for (int i = 0; i < inputReader.T; i++)
            {
                Trie trie = new Trie();

                foreach (string word in inputReader.Dictionary[i])
                {
                    trie.PopulateFromArray(word.ToCharArray());
                }

                Console.WriteLine($"Test Case #{i + 1}");

                for (int iM = 0; iM < inputReader.SearchMatrix[i].Count; iM++)
                {
                    Dictionary<string, int> matchs = trie.Match(inputReader.SearchMatrix[i][iM]);

                    Console.WriteLine($"Query #{iM + 1}");

                    foreach (KeyValuePair<string, int> match in matchs)
                    {
                        Console.WriteLine($"{match.Key} {match.Value}");
                    }
                }

                Console.WriteLine("");
                
            }

            Console.ReadKey();
        }
    }

    class InputReader
    {
        public int T { get; private set; }
        public List<int> D { get; private set; }
        public List<HashSet<string>> Dictionary { get; }
        public int Q { get; set; }
        public List<List<int>> M { get; set; }
        public List<List<int>> N { get; set; }
        public List<List<char[,]>> SearchMatrix { get; }

        public InputReader()
        {
            Dictionary = new List<HashSet<string>>();
            SearchMatrix = new List<List<char[,]>>();
            D = new List<int>();
            M = new List<List<int>>();
            N = new List<List<int>>();
        }

        public void ReadInput()
        {
            ReadTestCaseCount();
            for (int i = 0; i < T; i++)
            {
                ReadDictionaryLength();
                ReadDictionary(i);
                ReadSearchMatrixCount();
                ReadSearchMatrixes(i);
            }
        }

        private void ReadTestCaseCount()
        {
            string input = Console.ReadLine();
            int value;
            if (int.TryParse(input, out value) && value >= 1 && value <= 100)
            {
                T = value;
            }
            else
                throw new ArgumentException("The input must be an integer: 1 <= T <= 10");
        }

        private void ReadDictionaryLength()
        {
            string input = Console.ReadLine();
            int value;
            if (int.TryParse(input, out value) && value >= 1 && value <= 15000)
                D.Add(value);
            else
                throw new ArgumentException("The input must be an integer: 1 <= D <= 15,000");
        }

        private void ReadDictionary(int testCase)
        {
            for (int i = 0; i < D[testCase]; i++)
            {
                string word = Console.ReadLine();
                if (word != null && word.Length > 1000)
                    throw new ArgumentException("Dictionary words must have less than 1000 letters");

                AddToDictionary(testCase, word);
            }
        }

        private void AddToDictionary(int testCase, string word)
        {
            if (Dictionary.Count < testCase + 1)
                Dictionary.Add(new HashSet<string>());

            Dictionary[testCase].Add(word);
        }

        private void ReadSearchMatrixCount()
        {
            string input = Console.ReadLine();
            int value;
            if (int.TryParse(input, out value) && value >= 1 && value <= 100)
                Q = value;
            else
                throw new ArgumentException("The input must be an integer: 1 <= Q <= 100");
        }

        private void ReadSearchMatrixes(int testCase)
        {
            for (int i = 0; i < Q; i++)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    throw new ArgumentNullException();

                string[] inputs = input.Split(' ');
                int test;

                if (inputs.Length != 2 || !inputs.All(inp => int.TryParse(inp, out test) && test >= 1 && test <= 100))
                    throw new ArgumentException(
                        "The matrix size must be composed of 2 integers separated by ' ': 1 <= M, N <= 100");

                if (M.Count < testCase + 1)
                    M.Add(new List<int>());
                M[testCase].Add(int.Parse(inputs[0]));

                if (N.Count < testCase + 1)
                    N.Add(new List<int>());
                N[testCase].Add(int.Parse(inputs[1]));

                ReadSearchMatrix(i, testCase);
            }
        }

        private void ReadSearchMatrix(int matrixIndex, int testCase)
        {
            char[,] matrix = new char[M[testCase][matrixIndex], N[testCase][matrixIndex]];

            for (int i = 0; i < M[testCase][matrixIndex]; i++)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) && input?.Length != N[testCase][matrixIndex])
                    throw new ArgumentException("The informed input is different from the informed value.");

                char[] inputArr = input?.ToCharArray();

                for (int ii = 0; ii < inputArr?.Length; ii++)
                {
                    matrix[i, ii] = inputArr[ii];
                }
            }

            if(SearchMatrix.Count < testCase + 1)
                SearchMatrix.Add(new List<char[,]>());

            SearchMatrix[testCase].Add(matrix);
        }
    }

    class TrieNode
    {
        public string Value { get; set; }
        public char Key { get; }
        public TrieNode Parent { get; private set; }
        public HashSet<TrieNode> Children { get; }

        public TrieNode()
        {
            Children = new HashSet<TrieNode>();
        }

        public TrieNode(char key, TrieNode parent = null, string value = "") : this()
        {
            Parent = parent;
            Key = key;
            Value = value;
        }

        public void AddChild(TrieNode child)
        {
            Children.Add(child);
        }

        public bool HasChild(char childValue)
        {
            return Children.Any(child => child.Key == childValue);
        }

        public TrieNode GetChild(char childValue)
        {
            return Children.FirstOrDefault(child => child.Key == childValue);
        }

        public List<TrieNode> GetChildren(char childValue)
        {
            return Children.Where(child => child.Key == childValue).ToList();
        }

        public int Match(string word)
        {
            return Match(word.ToCharArray());
        }

        public int Match(char[] word)
        {
            // Not a match
            if (Key != word[0])
                return 0;

            // Last char of the word, so it's a match
            if (word.Length == 1 && Key == word[0])
                return 1;

            if (word.Length > 1 && HasChild(word[1]))
            {
                char[] newWord = new string(word).Substring(1).ToCharArray();

                return GetChildren(word[1]).Sum(node => node.Match(newWord));
            }

            return 0;
        }
    }

    class Trie
    {
        private readonly int[][] _interactionOptions =
        {
            // Foward interation
            new[] {0, 1}, new[] {1, 0}, new[] {1, 1},
            // Backward interation 
            new[] {0, -1}, new[] {-1, 0}, new[] {-1, -1},
            // The other diagonals
            new[] {1, -1}, new[] {-1, 1}
        };

        public TrieNode RootNode { get; }

        public Trie()
        {
            RootNode = new TrieNode();
        }

        public Trie(char[,] matrix) : this()
        {
            PopulateFromMatrix(matrix);
        }

        public void PopulateFromArray(char[] array)
        {
            ReadNode(0, array, RootNode);
        }

        public void PopulateFromMatrix(char[,] matrix)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    TrieNode node = new TrieNode(matrix[i, j], null);
                    foreach (int[] iterator in _interactionOptions)
                    {
                        ReadNode(i + iterator[0], j + iterator[1], matrix, iterator, node);
                    }
                    RootNode.Children.Add(node);
                }
            }
        }

        public void ReadNode(int i, char[] array, TrieNode parentNode)
        {
            if (i >= 0 && i < array.Length)
            {
                TrieNode node = null;

                if (parentNode.Children.Any(nd => nd.Key == array[i]))
                    node = parentNode.Children.Single(nd => nd.Key == array[i]);
                else
                {
                    node = new TrieNode(array[i], parentNode);
                    parentNode.AddChild(node);
                }

                int navRow = i + 1;

                if (navRow == array.Length)
                    node.Value = new string(array);

                ReadNode(navRow, array, node);
            }
        }

        public void ReadNode(int i, int j, char[,] matrix, int[] iterator, TrieNode parentNode)
        {
            if (i >= 0 && j >= 0 && i < matrix.GetLength(0) && j < matrix.GetLength(1))
            {
                TrieNode node = new TrieNode(matrix[i, j], parentNode);

                parentNode.AddChild(node);

                int navRow = i + iterator[0];
                int navCol = j + iterator[1];

                ReadNode(navRow, navCol, matrix, iterator, node);
            }
        }

        public int Match(string word)
        {
            char wChar = word.ToCharArray()[0];

            return RootNode.Children.Where(rnode => rnode.Key == wChar).Sum(node => node.Match(word));
        }

        public int Match(char[] word)
        {
            char wChar = word[0];

            return RootNode.Children.Where(rnode => rnode.Key == wChar).Sum(node => node.Match(word));
        }

        public Dictionary<string, int> Match(char[,] searchMatrix)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            for (int i = 0; i < searchMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < searchMatrix.GetLength(1); j++)
                {
                    foreach (int[] iterator in _interactionOptions)
                    {
                        List<string> matchs = ReadAndMatch(i, j, iterator, new List<char>(), searchMatrix);

                        foreach (string match in matchs)
                        {
                            if (!string.IsNullOrEmpty(match))
                                if (result.ContainsKey(match))
                                    result[match]++;
                                else
                                    result.Add(match, 1);
                        }
                    }
                }
            }

            return result;
        }

        public List<string> ReadAndMatch(int i, int j, int[] iterator, List<char> currentPrefix, char[,] searchMatrix, TrieNode parentNode = null)
        {
            List<string> result = new List<string>();

            TrieNode parent = parentNode ?? RootNode;
            if (i >= 0 && j >= 0 && i < searchMatrix.GetLength(0) && j < searchMatrix.GetLength(1))
            {
                if (parent.HasChild(searchMatrix[i, j]))
                {
                    TrieNode node = parent.GetChild(searchMatrix[i, j]);
                    currentPrefix.Add(searchMatrix[i, j]);

                    if (!string.IsNullOrEmpty(node?.Value) && node.Value == new string(currentPrefix.ToArray()))
                        result.Add(node.Value);

                        result.AddRange(ReadAndMatch(i + iterator[0], j + iterator[1], iterator, currentPrefix, searchMatrix, node));
                }
            }
            return result;
        }
    }
}
