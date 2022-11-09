/* COIS 2020H - Assignment 2
 * Original code provided by: Brian Patrick
 * Modifications by (alphabetical): Cole Miller, Jesse Laframboise, Matthew Hellard
 * 
 * How to run in VS:
 * create new Console App (.NET Frameword)
 * clear out code from new .cs file
 * copy and paste in this code into the new .cs file
 */
using System;
public interface IHashTable<TKey, TValue>
{
    void Insert(TKey key, TValue value); // Insert a <key,value> pair
    bool Remove(TKey key);               // Remove the value with key
    TValue Retrieve(TKey key);           // Return the value of a key
}

public class HashTable<TKey, TValue> : IHashTable<TKey, TValue>
{

    // <key,value> pair (item)
    private class Node
    {
        public TKey key;
        public TValue value;

        public Node next;

        public Node(TKey key, TValue value, Node next = null)
        {
            this.key = key;
            this.value = value;
            this.next = next;
        }
    }

    private Node[] HT; //actual hashtable, stored as array of nodes
    private Node[] header; //header node, seperate array is best minimal implementation (could also convert HT into List array but that wouldn't be minimal)
    private int numBuckets; //number of buckets
    private int numItems; //number of items

    public HashTable()
    {
        numBuckets = 1;
        HT = new Node[numBuckets];
        header = new Node[numBuckets]; //header array should always be same length as HT array
        MakeEmpty();
    }

    public void MakeEmpty()
    {
        for (int i = 0; i < numBuckets; i++)
        {
            header[i] = HT[i] = null; //set header and HT for each bucket to null
        }
        numItems = 0;
    }

    // NextPrime
    // Returns the next prime number > k
    private int NextPrime(int k)
    {
        int i;
        bool prime = false;

        // Begin at an odd number
        if (k == 1 || k == 2)
            return k + 1;

        if (k % 2 == 0) k++;

        while (!prime)
        {
            // Divide k by odd factors
            for (i = 3; i * i < k && k % i != 0; i += 2) ;

            if (k % i != 0)
                prime = true;
            else
                // Increase k to the next odd number
                k += 2;
        }
        return k;
    }

    // Rehash
    // Doubles the size of the hash table to the next highest prime number
    // Rehashes the items from the original hash table
    private void ReHash()
    {
        int oldNumBuckets = numBuckets; //old number of buckets
        Node[] oldHT = header; //old hashtable, uses header

        numBuckets = NextPrime(2 * numBuckets);
        // Create the new hash table array and initialize each bucket to empty
        HT = new Node[numBuckets];
        header = new Node[numBuckets]; //header must change with HT to stay in sync
        MakeEmpty();

        // Rehash items from the old to new hash table
        for (int i = 0; i < oldNumBuckets; i++)
        {
            Node p = oldHT[i];
            int k;
            while (p != null)
            {
                k = p.key.GetHashCode() % numBuckets;
                HT[k] = new Node(p.key, p.value, HT[k]);
                header[k] = HT[k];
                numItems++;
                p = p.next;
            }
        }
    }

    // Insert
    // Insert a <key,value> into the current hash table
    // If the key is already found, an exception is thrown
    public void Insert(TKey key, TValue value)
    {
        int i = key.GetHashCode() % numBuckets;
        Node p = header[i]; //uses header node

        while (p != null)
        {
            // Unsuccessful insert (key found already)
            if (p.key.Equals(key))
            {
                throw new InvalidOperationException("Duplicate key");
            }
            else
            {
                p = p.next;
            }
        }
        header[i] = new Node(key, value, header[i]); //uses header node
        numItems++;

        // Rehash if the average size of the buckets exceeds 5.0
        if ((double)numItems / numBuckets > 5.0)
        {
            ReHash();
        }
        Output(); //output always called after insertion to keep HT sorted
    }

    // Remove
    // Delete (if found) the <key,value> with the given key
    // Return true if successful, false otherwise
    public bool Remove(TKey key)
    {
        int i = key.GetHashCode() % numBuckets;
        Node p = header[i]; //uses header node

        if (p == null)
        {
            return false;
        }
        else
        {
            // Successful remove of the first item in a bucket
            if (p.key.Equals(key))
            {
                header[i] = header[i].next; //uses header node
                numItems--;
                Output(); //output always called after succesful remove to guarantee proper sorting
                return true;
            }
            else
            {
                while (p.next != null)
                {
                    // Successful remove (<key,value> found and deleted)
                    if (p.next.key.Equals(key))
                    {
                        p.next = p.next.next;
                        numItems--;
                        Output(); //output always called after successful remove to guarantee proper sorting
                        return true;
                    }
                    else
                    {
                        p = p.next;
                    }
                }
            }
            // Unsuccessful remove (key not found)
            return false;
        }
    }

    // Retrieve
    // Returns (if found) the value of the given key
    // If the key is not found, an exception is thrown
    public TValue Retrieve(TKey key)
    {
        int i = key.GetHashCode() % numBuckets;
        Node p = header[i]; //uses header node

        while (p != null)
        {
            // Successful retrieval (value found and returned)
            if (p.key.Equals(key))
            {
                return p.value;
            }
            else
            {
                p = p.next;
            }

        }
        throw new InvalidOperationException("Key not found");
    }

    // Print
    // Prints the hash table entries, one line per bucket
    public void Print()
    {
        int i;
        Node p;

        for (i = 0; i < numBuckets; i++)
        {
            Console.Write(i.ToString().PadLeft(2) + ": ");

            p = header[i]; //uses header node
            while (p != null)
            {
                Console.Write("<" + p.key.ToString() + "," + p.value.ToString() + "> ");
                p = p.next;
            }
            Console.WriteLine();
        }
    }

    // Output
    // Sorts contents of each bucket based on TKey (hash code), hash code used as it's the only comparable value (int) a key should inherently have. 
    public void Output()
    {
        for (int i = 0; i < numBuckets; i++) //for each bucket
        {
            Node unsorted = header[i]; //current bucket
            Node sorted = null; //should be new bucket once sort is finished
            while (unsorted != null) //unsorted should empty as each value is sorted one at a time
            {
                Node Max = new Node(unsorted.key, unsorted.value); //initial max value is front of node
                Node p = unsorted; //traversal node
                while (p.next != null) //traversing entire node chain
                {
                    if (p.next.key.GetHashCode() >= Max.key.GetHashCode()) //if the next node's key has a higher (or equal) hash code to the current
                    {
                        Max = new Node(p.next.key, p.next.value); //set max to the node value that was determined to be bigger based on the key
                    }
                    p = p.next; //go to the next node
                } //once exited every node in unsorted should have been tested and Max should currently be the key and value with the biggest hashcode
                sorted = new Node(Max.key, Max.value, sorted); //add the biggest node still in unsorted to sorted
                //removing current max node from unsorted
                p = unsorted;
                if (unsorted.key.Equals(Max.key))
                { //if first value in unsorted matches max key,
                    unsorted = unsorted.next; //trim first value.
                }
                else
                {
                    bool found = false; //control variable to exit asap, could theoretically also prevent excessive deletions but since keys must be unique it should never come up.
                    while (found == false && p != null && p.next != null) //go through unsorted, 
                    {
                        if (p.next.key.Equals(Max.key)) //once max found,
                        {
                            p.next = p.next.next; //trim it
                            found = true;
                        }
                        p = p.next;
                    }
                }
            }
            header[i] = sorted; //sorted should now contain, in order, every node from unsorted, so set the bucket to sorted 
        }
    }
}


public class Point : IComparable
{
    private int x;
    private int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override int GetHashCode()
    {
        return Math.Abs(x * y); //hashcode is kept simple to make testing easier to understand
    }

    public override bool Equals(object obj)
    {
        Point point = (Point)obj; //cast not checked as exception will be thrown automatically and it isn't this method's job to handle it. (because it can't request a correction)

        if (point.x == x && point.y == y) //if both data members match
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString() //here to make printing simpler and prettier
    {
        return ("[" + x + " " + y + "]");
    }

    public int CompareTo(object obj)
    {
        Point point = (Point)obj; //cast not checked as exception will be thrown automatically and it isn't this method's job to handle it. (because it can't request a correction)
        if (point.x == x) //if x is equal,
        {
            return (y - point.y); //return y difference, subtracts CompareTo Point from caller Point because we want + if caller Point is bigger
        }
        else //if x is not equal,
        {
            return (x - point.x); //return x difference, subtracts CompareTo Point from caller Point because we want + if caller Point is bigger
        }
    }
}

public class Demo //for demonstrating and testing
{
    public static void Main()
    {
        bool run = true;
        HashTable<Point, int> table = new HashTable<Point, int>(); //only ints used for value in demo, specific type used doesn't really matter
        Console.WriteLine("Open Hashtable Revisited\nNote: for this demo, all hashtables store ints, and all keys are Points");
        while (run == true) //repeat until quit (or crash (not likely))
        {
            Console.Write(
            "\nMenu:" +
            "\nEmpty HT               (c):" +
            "\nInsert point into HT   (i):" +
            "\nRemove point from HT   (d):" +
            "\nRetrieve point from HT (r):" +
            "\nPrint entire HT        (p):" +
            "\nMake, print dev HT     (a):" +
            "\nQuit                   (q):" +
            "\nInput :");

            bool valid = false; //validation
            char input = 'a'; //has to be set to something or VS complains, value will be overwritten before actually being used anyway 
            while (valid == false)
            { //main menu validation
                try
                {
                    input = Char.ToLower(Convert.ToChar(Console.ReadLine())); //cap set so user need not worry
                    valid = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Input error, please try again");
                }
            }

            switch (input) //operation based on menu input
            {
                case 'c': //reset ht contents, doesn't reset bucket count
                    table.MakeEmpty();
                    break;

                case 'i': //insert value
                    int x = 0;
                    int y = 0;
                    int value = 0;
                    valid = false;
                    while (valid == false) //x validation
                    {
                        try
                        {
                            Console.Write("\nInput key x (int):");
                            x = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    valid = false;
                    while (valid == false) //y validation
                    {
                        try
                        {
                            Console.Write("\nInput key y (int):");
                            y = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    valid = false;
                    while (valid == false) //value validation
                    {
                        try
                        {
                            Console.Write("\nInput value (int):");
                            value = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    Point key = new Point(x, y);
                    try
                    {
                        table.Insert(key, value); //actual insertion
                    }
                    catch (InvalidOperationException) //if this exception is thrown, the key already existed
                    {
                        Console.WriteLine("Key already exists in hashtable"); 
                    }
                    break;

                case 'd': //remove value from hashtable
                    x = 0;
                    y = 0;
                    valid = false;
                    while (valid == false) //x validation
                    {
                        try
                        {
                            Console.Write("\nInput key x (int):");
                            x = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    valid = false;
                    while (valid == false) //y validation
                    {
                        try
                        {
                            Console.Write("\nInput key y (int):");
                            y = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    key = new Point(x, y);
                    valid = table.Remove(key); //valid acts as bool checker for if remove succeeded
                    if (valid)
                    {
                        Console.WriteLine("Successfully removed");
                    }
                    else
                    {
                        Console.WriteLine("Could not remove key");
                    }
                    break;

                case 'r': //retreive value
                    x = 0;
                    y = 0;
                    valid = false;
                    while (valid == false) //x validation
                    {
                        try
                        {
                            Console.Write("\nInput key x (int):");
                            x = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    valid = false;
                    while (valid == false) //y validation
                    {
                        try
                        {
                            Console.Write("\nInput key y (int):");
                            y = Convert.ToInt32(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Input error, please try again");
                        }
                    }
                    key = new Point(x, y);
                    try
                    {
                        value = table.Retrieve(key);
                        Console.WriteLine("Retrieved value :" + value);
                    }
                    catch (InvalidOperationException) //if this exception is thrown, the key wasn't found
                    {
                        Console.WriteLine("Key not found");
                    }
                    ;
                    break;

                case 'p': //print entire hashtable
                    table.Print();
                    Console.WriteLine("hit enter to return"); //makes reading the actual print easier (because menu doesn't immediately pop back up)
                    Console.ReadLine();
                    break;

                case 'a': //generates default hashtable used when developing modification, we didn't have the heart to delete it
                    table.MakeEmpty();
                    int size = 10;
                    Point[,] tester = new Point[size, size]; //2d key array
                    int i = 0; //values to insert, indexed so insertion order can be seen in print
                    for (int ix = 0; ix < size; ix++) //x coords
                    { //rows
                        for (int iy = 0; iy < size; iy++) //y coords
                        { //columns
                            i++;
                            tester[ix, iy] = new Point(ix, iy); //set key to coords
                            table.Insert(tester[ix, iy], i); //insert i as value using current key
                        }
                    }
                    table.Print(); //print it
                    Console.WriteLine("hit enter to return"); //makes reading the actual print easier (because menu doesn't immediately pop back up)
                    Console.ReadLine();
                    break;

                case 'q': //quit
                    run = false; //main loop control variable
                    break;
                default: //if menu input was a char but didn't match up with any valid menu operations
                    Console.WriteLine("Unrecognised input, please try again");
                    break;
            }
        }
    }
}
