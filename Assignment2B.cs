//Program:
//Author: Cole Miller
//Date:
//

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
    private Dictionary<char, string> D; // Dictionary to encode text
                                        // Constructor
    public Huffman(string S)
    {
        //Analyze string for letter frequencies
        int[] F = AnalyzeText(S);

        //TEsting
        //Console.WriteLine(F[0]);
        //Console.WriteLine(F[1]);
        //Console.WriteLine(F[2]);
        //Console.WriteLine(F[3]);


        //build huffman tree
        Build(F);


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

        //Move all freq. values to the int[] array
        int[] freqArray = new int[charFrequencyDict.Count];
        int freqIndex = 0;

        foreach (KeyValuePair<char, int> entry in charFrequencyDict)
        {
            freqArray[freqIndex] = entry.Value;
            freqIndex++;
        }

        return freqArray;
    }

    // 16 marks
    // Build a Huffman tree based on the character frequencies greater than 0 (invoked by Huffman)
    private void Build(int[] F)
    {
        PriorityQueue<Node, int> PQ = new PriorityQueue<Node, int>();

        //Create leaf nodes for all unique characters in the string
        int indexFreq = 0;
        foreach (int item in F)
        {
            //Get list of all unique chars in the string and match with frequencies



            //Replace 'c' with character for the freq.


            //Create new node
            Node node = new Node('c', F[indexFreq], null, null);

            //Add node to priority queue
            PQ.Enqueue(node, node.Frequency);
            indexFreq++;
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

        //Testing
        PrintTree(PQ.Peek(), 0);
    }

    //Print Binary Tree
    //Not part of the assignment
    //Taken from Binary Tree slide
    private void PrintTree(Node root, int indent)
    {
        if (root != null)
        {
            PrintTree(root.Right, indent + 5);
            Console.WriteLine(new String(' ', indent) + root.Frequency);
            PrintTree(root.Left, indent + 5);
        }
    }

    // 12 marks
    // Create the code of 0s and 1s for each character by traversing the Huffman tree (invoked by Huffman)
    // Store the codes in Dictionary D using the char as the key
    private void CreateCodes()
    {
        //Use preorder traversal

        //Store the char and binary code in dictionary D
    }

    // 8 marks
    // Encode the given text and return a string of 0s and 1s
    //public string Encode(string S) { … }

    // 8 marks
    // Decode the given string of 0s and 1s and return the original text
    //public string Decode(string S) { … }
}

//Main Program
class TestClass
{
    static void Main()
    {
        string testString = "coollleee";
        Huffman huffmanTest = new Huffman(testString);

    }
}