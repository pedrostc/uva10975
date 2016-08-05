﻿using System;
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
        static void Main(string[] args)
        {
            InputReader inputReader = new InputReader();
            inputReader.ReadInput();

            foreach (char[,] searchMatrix in inputReader.SearchMatrix)
            {
                Trie trie = new Trie(searchMatrix);

                foreach (string word in inputReader.Dictionary)
                {
                    int wordMatches = trie.Match(word);

                    Console.WriteLine($"{word} - {wordMatches}");
                }
            }

            Console.ReadKey();
        }
    }

    class InputReader
    {
        public int T { get; private set; }
        public int D { get; private set; }
        public List<string> Dictionary { get; private set; }
        public int Q { get; set; }
        public List<int> M { get; set; }
        public List<int> N { get; set; }
        public List<char[,]> SearchMatrix { get; private set; }

        public InputReader()
        {
            Dictionary = new List<string>();
            SearchMatrix = new List<char[,]>();
            M = new List<int>();
            N = new List<int>();
        }

        public void ReadInput()
        {
            ReadTestCaseCount();
            ReadDictionaryLength();
            ReadDictionary();
            ReadSearchMatrixCount();
            ReadSearchMatrixes();
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
                D = value;
            else
                throw new ArgumentException("The input must be an integer: 1 <= D <= 15,000");
        }

        private void ReadDictionary()
        {
            for (int i = 0; i < D; i++)
            {
                string word = Console.ReadLine();
                if (word != null && word.Length > 1000)
                    throw new ArgumentException("Dictionary words must have less than 1000 letters");

                Dictionary.Add(word);
            }
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

        private void ReadSearchMatrixes()
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

                M.Add(int.Parse(inputs[0]));
                N.Add(int.Parse(inputs[1]));

                ReadSearchMatrix(i);
            }
        }

        private void ReadSearchMatrix(int matrixIndex)
        {
            char[,] matrix = new char[M[matrixIndex], N[matrixIndex]];

            for (int i = 0; i < M[matrixIndex]; i++)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) && input?.Length != N[matrixIndex])
                    throw new ArgumentException("The informed input is different from the informed value.");

                char[] inputArr = input?.ToCharArray();

                for (int ii = 0; ii < inputArr?.Length; ii++)
                {
                    matrix[i, ii] = inputArr[ii];
                }
            }

            SearchMatrix.Add(matrix);
        }
    }

    class TrieNode
    {
        public char Value { get; }
        public TrieNode Parent { get; private set; }
        public List<TrieNode> Children { get; }

        public TrieNode(char value, TrieNode parent)
        {
            Parent = parent;
            Value = value;

            Children = new List<TrieNode>();
        }

        public void AddChild(TrieNode child)
        {
            Children.Add(child);
        }

        public bool HasChild(char childValue)
        {
            return Children.Any(child => child.Value == childValue);
        }

        public List<TrieNode> GetChildren(char childValue)
        {
            return Children.Where(child => child.Value == childValue).ToList();
        }

        public int Match(string word)
        {
            return Match(word.ToCharArray());
        }

        public int Match(char[] word)
        {
            // Not a match
            if (Value != word[0])
                return 0;

            // Last char of the word, so it's a match
            if (word.Length == 1 && Value == word[0])
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
        private readonly int[][] _interactionOptions = new int[8][]
        {
            // Foward interation
            new[] {0, 1}, new[] {1, 0}, new[] {1, 1},
            // Backward interation 
            new[] {0, -1}, new[] {-1, 0}, new[] {-1, -1},
            // The other diagonals
            new[] {1, -1}, new[] {-1, 1}
        };

        public List<TrieNode> RootNodes { get; }

        public Trie()
        {
            RootNodes = new List<TrieNode>();
        }

        public Trie(char[,] matrix) : this()
        {
            PopulateFromMatrix(matrix);
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
                    RootNodes.Add(node);
                }
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

            return RootNodes.Where(rnode => rnode.Value == wChar).Sum(node => node.Match(word));
        }
    }
}
