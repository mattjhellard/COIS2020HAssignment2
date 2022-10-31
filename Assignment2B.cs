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
        this.Character = Character;
        this.Frequency = frequency;
        this.Left = left;
        this.Right = right;
    }

    // 3 marks
    public int CompareTo(Object obj)
    {
        //Compare the frequency of 2 nodes
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
        Console.WriteLine(F[0]);
        Console.WriteLine(F[1]);
        Console.WriteLine(F[2]);
        Console.WriteLine(F[3]);

        //build huffman tree
    }

    // 8 marks
    // Return the frequency of each character in the given text (invoked by Huffman)
    //https://www.geeksforgeeks.org/print-characters-frequencies-order-occurrence/  better than the way I was going to solve it
    private int[] AnalyzeText(string S)
    {
        int[] freqArray = new int[S.Length];
        int freqIndex = 0;
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
        foreach (KeyValuePair<char, int> entry in charFrequencyDict)
        {
            freqArray[freqIndex] = entry.Value;
            freqIndex++;
        }

        return freqArray;
    }

    // 16 marks
    // Build a Huffman tree based on the character frequencies greater than 0 (invoked by Huffman)
    //private void Build(int[] F) {       PriorityQueue<Node> PQ; …}

    // 12 marks
    // Create the code of 0s and 1s for each character by traversing the Huffman tree (invoked by Huffman)
    // Store the codes in Dictionary D using the char as the key
    //private void CreateCodes() { … }

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
        string testString = "cooleeelooo";
        Huffman huffmanTest = new Huffman(testString);

    }
}