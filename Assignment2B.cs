//Program:
//Author: Cole Miller
//Date:
//



using System.Collections;
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
        if (obj != null)
        {
            Console.WriteLine(obj.GetType);
        }


        return 1; //remove later
    }
}
class Huffman
{
    private Node HT; // Huffman tree to create codes and decode text
    private Dictionary<char, string> D = new Dictionary<char, string>(); // Dictionary to encode text
                                                                         // Constructor
    public Huffman(string S)
    {
        //Analyze string for letter frequencies
        int[] F = AnalyzeText(S);

        //Build huffman tree
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
    //Not part of the assignment
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

    //Print out every 
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
                //throw exception
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
        int offset = 26; //
        char character = ' ';   //Default
        foreach (int freq in F)
        {
            if (freq == 0)
            {
                //Skip
            }
            else
            {
                //Create a new leaf node
                //Console.WriteLine(freqIndex);
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
                //Console.WriteLine(character);

                Node node = new Node(character, freq, null, null);

                //Add node to priority queue
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

        this.HT = PQ.Dequeue();
    }

    // 12 marks
    // Create the code of 0s and 1s for each character by traversing the Huffman tree (invoked by Huffman)
    // Store the codes in Dictionary D using the char as the key
    private void CreateCodes()
    {
        string charToHuffmanCode = "";
        Preorder(this.HT, charToHuffmanCode);
    }

    //Recursiveley traverse a binary tree using Preorder traversal
    //If you go left add "0" to the string, right add a "1"
    private void Preorder(Node root, string charToHuffmanCode)
    {
        if (root != null)
        {
            if (root.Left == null)
            {
                this.D.Add(root.Character, charToHuffmanCode);

                //Remove the last char from the string
                charToHuffmanCode = charToHuffmanCode.Remove(charToHuffmanCode.Length - 1, 1);
            }

            Preorder(root.Left, charToHuffmanCode + "0");
            Preorder(root.Right, charToHuffmanCode + "1");
        }
    }

    // 8 marks
    // Encode the given text and return a string of 0s and 1s
    public string Encode(string S)
    {
        string encodedString = "";
        string charToEncodedString;

        //Encode string using dictionary
        try
        {
            //
            foreach (char character in S)
            {
                //Find code in dictionary
                charToEncodedString = this.D[character];

                //Assign code to encode string
                encodedString = encodedString + charToEncodedString;
            }
        }
        catch
        {
            return encodedString = "Exception: Please enter in a string with characters from the tree.";
        }

        return encodedString;
    }

    // 8 marks
    // Decode the given string of 0s and 1s and return the original text
    public string Decode(string S)
    {
        string decodedString = "";

        //Traverse the HuffmanTree to decode b/c you don't know when to stop


        return decodedString;
    }
}

//Main Program
class TestClass
{
    static void Main()
    {
        string testString = "Coollleeee";
        string testStringAll = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
        Huffman huffmanTest = new Huffman(testString);

        //Encode
        string testEncodeString = "Cole";
        string encodedTestString = huffmanTest.Encode(testEncodeString);
        Console.WriteLine(encodedTestString);

        //Decode
        string deencodedTestString = huffmanTest.Decode(testEncodeString);
        Console.WriteLine(deencodedTestString);
    }
}