//Program:  Huffman Tree
//Author:   Cole Miller, Matthew Hellard, and Jesse Laframboise
//Date:     2022-11-06

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;

class Node : IComparable
{
    public char Character { get; set; }
    public int Frequency { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }

    public Node(char character, int frequency, Node left, Node right)
    {
        this.Character = character;
        this.Frequency = frequency;
        this.Left = left;
        this.Right = right;
    }

    // 3 marks
    //Compare the frequency of 2 nodes
    public int CompareTo(Object obj)
    {
        int value = 2;
        if (obj != null)
        {
            try
            {
                Node test = (Node)obj;

                if (this.Frequency < test.Frequency)
                {
                    value = -1;
                }
                else if (this.Frequency == test.Frequency)
                {
                    value = 0;
                }
                else if (this.Frequency > test.Frequency)
                {
                    value = 1;
                }
            }
            catch
            {
                //Exception: obj conversion error
            }
        }
        return value;
    }
}
class Huffman
{
    private Node HT; // Huffman tree to create codes and decode text
    private Dictionary<char, string> D = new Dictionary<char, string>();            // Dictionary to encode text
    private Dictionary<string, char> inverseD = new Dictionary<string, char>();     // Dictionary to decode text

    // Constructor
    public Huffman(string S)
    {
        int[] F = AnalyzeText(S);

        Build(F);

        //Testing
        PrintTree(HT, 0);

        CreateCodes();

        //Testing
        PrintDictCodes();
    }

    //Print out an array
    private void PrintArray(int[] array)
    {
        int count = 0;
        foreach (var item in array)
        {
            Console.WriteLine("Index: " + count + " " + item.ToString());
            count++;
        }
    }

    //Print Binary Tree
    //Taken from Binary Tree slide
    private void PrintTree(Node root, int indent)
    {
        if (root != null)
        {
            PrintTree(root.Right, indent + 5);
            Console.WriteLine(new String(' ', indent) + root.Frequency + ' ' + root.Character);
            PrintTree(root.Left, indent + 5);
        }
    }

    //Print out every key/value pair for Dictionary D
    private void PrintDictCodes()
    {
        foreach (KeyValuePair<char, string> entry in this.D)
        {
            Console.WriteLine("Char: " + entry.Key + " Code: " + entry.Value);
        }
        Console.WriteLine("End of Dict");
    }

    // 8 marks
    // Return the frequency of each character in the given text (invoked by Huffman)
    //https://www.geeksforgeeks.org/print-characters-frequencies-order-occurrence/  better than the way I was going to solve it
    private int[] AnalyzeText(string S)
    {
        Dictionary<char, int> charFrequencyDict = new Dictionary<char, int>();

        foreach (char c in S)
        {
            if (charFrequencyDict.ContainsKey(c))
            {
                charFrequencyDict[c]++;
            }
            else
            {
                charFrequencyDict[c] = 1;
            }
        }

        int[] freqArray = new int[53];  //53 A-Z,a-z, and space
        int freqIndex = 0;
        int offset = 26;

        //Map index to character ascii value
        foreach (KeyValuePair<char, int> entry in charFrequencyDict)
        {
            //Uppercase
            if (entry.Key >= 'A' & entry.Key <= 'Z')
            {
                freqIndex = entry.Key - 'A';
            }
            //Lowercase
            else if (entry.Key >= 'a' & entry.Key <= 'z')
            {
                freqIndex = (entry.Key - 'a') + offset;
            }
            //Space
            else if (entry.Key == ' ')
            {
                freqIndex = 52;  //Arbitrarily assigned to the last array index
            }
            else
            {
                //throw exception: character not recognized
            }
            freqArray[freqIndex] = entry.Value;
        }
        return freqArray;
    }

    // 16 marks
    // Build a Huffman tree based on the character frequencies greater than 0 (invoked by Huffman)
    private void Build(int[] F)
    {
        PriorityQueue<Node, int> PQ = new PriorityQueue<Node, int>();

        //Create leaf nodes for all unique characters in the string
        int freqIndex = 0;
        int offset = 26;        //Used when switching from capital to lowercase letters
        char character = ' ';   //Arbitrary default value
        foreach (int freq in F)
        {
            if (freq == 0)
            {
                //Skip
            }
            else
            {
                //Create a new leaf node
                if (freqIndex >= 0 & freqIndex <= 25)
                {
                    character = (char)(freqIndex + 65);
                }
                //Lowercase
                else if (freqIndex >= 25 & freqIndex <= 51)
                {
                    character = (char)(freqIndex + 97 - offset);
                }
                //Space
                else if (freqIndex == 52)
                {
                    character = ' ';  //Arbitrarily assigned to the last array index
                }

                Node node = new Node(character, freq, null, null);

                //Add node to the priority queue
                PQ.Enqueue(node, node.Frequency);
            }
            freqIndex++;
        }

        //While the Huffman tree is not finished building
        while (PQ.Count > 1)
        {
            //Get the two lowest priority nodes
            Node minNodeOne = PQ.Dequeue();
            Node minNodeTwo = PQ.Dequeue();

            int frequencySum = minNodeOne.Frequency + minNodeTwo.Frequency;

            //Make a new node based on the info from the two nodes
            Node internalNode = new Node('/', (frequencySum), minNodeOne, minNodeTwo);

            PQ.Enqueue(internalNode, internalNode.Frequency);
        }
        //PQ.Dequeue() will be the root node of the finished Huffman Tree
        this.HT = PQ.Dequeue();
    }

    // 12 marks
    // Create the code of 0s and 1s for each character by traversing the Huffman tree (invoked by Huffman)
    // Store the codes in Dictionary D using the char as the key
    private void CreateCodes()
    {
        string charToHuffmanCode = "";
        CreateHuffmanCodeDictionaries(this.HT, charToHuffmanCode);
    }

    //Recursiveley traverse a binary tree using Preorder traversal and build two dictionaries for encoding/decoding
    private void CreateHuffmanCodeDictionaries(Node root, string charToHuffmanCode)
    {
        if (root != null)
        {
            if (root.Left == null)
            {
                //Add dictionary code for encoding strings
                this.D.Add(root.Character, charToHuffmanCode);

                //Add inverse dictionary code for decoding strings
                //https://stackoverflow.com/questions/2444033/get-dictionary-key-by-value
                this.inverseD.Add(charToHuffmanCode, root.Character);

                //Remove the last char from the string
                charToHuffmanCode = charToHuffmanCode.Remove(charToHuffmanCode.Length - 1, 1);
            }
            CreateHuffmanCodeDictionaries(root.Left, charToHuffmanCode + "0");
            CreateHuffmanCodeDictionaries(root.Right, charToHuffmanCode + "1");
        }
    }

    // 8 marks
    // Encode the given text and return a string of 0s and 1s
    public string Encode(string S)
    {
        string encodedString = "";
        string foundCharacter;

        //Encode string using dictionary
        try
        {
            foreach (char character in S)
            {
                //Find code in dictionary
                foundCharacter = this.D[character];

                encodedString = encodedString + foundCharacter;
            }
        }
        catch
        {
            //Exception: Please enter in a string with characters from the tree.
        }
        return encodedString;
    }

    // 8 marks
    // Decode the given string of 0s and 1s and return the original text
    public string Decode(string S)
    {
        string decodedString = "";
        string tempCode = "";
        char tempChar = ' ';
        Node root = this.HT;

        foreach (char character in S)
        {
            //Left
            if (character == '0')
            {
                tempCode = tempCode + character;
                root = root.Left;
            }
            //Right, character == '1'
            else
            {
                tempCode = tempCode + character;
                root = root.Right;
            }

            //If the root is at a leaf node
            if (root.Left == null)
            {
                //Get decoded value from dictionary
                tempChar = this.inverseD[tempCode];

                decodedString = decodedString + tempChar;

                //Reset
                tempCode = "";
                root = this.HT;
            }
        }
        return decodedString;
    }
}

//Main Program
class TestClass
{
    static void Main()
    {
        //Will throw exceptions if characters outside of the 53 approved ones are used
        string[] testStrings = { "Cole", "Cole Miller", "Coollleeee", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ", "ABCVOK HDEsDA WefghjopDD SDdwtygj hyuvwxyz", "Programming is cool" };

        foreach (string item in testStrings)
        {
            Huffman huffmanTest = new Huffman(item);

            //Encode
            string encodedTestString = huffmanTest.Encode(item);
            Console.WriteLine(encodedTestString);

            //Decode
            string decodedTestString = huffmanTest.Decode(encodedTestString);
            Console.WriteLine(decodedTestString);
        }
    }
}