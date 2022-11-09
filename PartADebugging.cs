/*
 * How to run in VS:
 * create new Console App (.NET Frameword)
 * clear out code from new .cs file
 * copy and paste in this code in this .cs file
 */
using System;
public interface IHashTable<TKey, TValue>
{
    void Insert(TKey key, TValue value);
    bool Remove(TKey key);
    TValue Retrieve(TKey key);
}

public class HashTable<TKey, TValue> : IHashTable<TKey, TValue>
{
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

    private Node[] HT;
    private Node[] header; //header node, seperate array is best minimal implementation (could also convert HT into List array but that wouldn't be minimal)
    private int numBuckets;
    private int numItems;

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
            header[i] = HT[i] = null;
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
        Print();
        int oldNumBuckets = numBuckets; //old number of buckets
        Node[] oldHT = header; //old hashtable

        numBuckets = NextPrime(2 * numBuckets);
        HT = new Node[numBuckets];
        header = new Node[numBuckets];
        MakeEmpty();

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
        //Console.WriteLine(i);
        Node p = header[i];

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
        header[i] = new Node(key, value, header[i]);
        numItems++;

        //Console.WriteLine(key);
        // Rehash if the average size of the buckets exceeds 5.0
        if ((double)numItems / numBuckets > 5.0)
        {
            ReHash();
        }

        //Output();
    }

    // Remove
    // Delete (if found) the <key,value> with the given key
    // Return true if successful, false otherwise
    public bool Remove(TKey key)
    {
        int i = key.GetHashCode() % numBuckets;
        Node p = header[i];

        if (p == null)
        {
            return false;
        }
        else
        {
            // Successful remove of the first item in a bucket
            if (p.key.Equals(key))
            {
                header[i] = header[i].next;
                numItems--;
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
        Node p = header[i];

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

            p = header[i];
            while (p != null)
            {
                Console.Write("<" + p.key.ToString() + "," + p.value.ToString() + "> ");
                p = p.next;
            }
            Console.WriteLine();
        }
    }

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
                { //if first value in unsorted matches max key
                    unsorted = unsorted.next; //trim first value
                }
                else
                {
                    while (p != null && p.next != null) //go through unsorted,
                    {
                        if (p.next.key.Equals(Max.key)) //once max found,
                        {
                            p.next = p.next.next; //trim it
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
        Point point = (Point)obj;
        if (point.x == x && point.y == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString()
    {
        return ("[" + x + " " + y + "]");
        //return Convert.ToString(Math.Abs(x * y));
    }

    public int CompareTo(object obj)
    {
        Point point = (Point)obj;
        if (point.x - x == 0)
        {
            return (point.y - y);
        }
        else
        {
            return (point.x - x);
        }
    }
}

public class Demo
{
    public static void Main()
    {
        int size = 3;
        Point[,] tester = new Point[size, size];
        HashTable<Point, int> table = new HashTable<Point, int>();
        int i = 0;
        for (int ix = 0; ix < size; ix++)
        {
            for (int iy = 0; iy < size; iy++)
            {
                i++;
                tester[ix, iy] = new Point(ix, iy);
                table.Insert(tester[ix, iy], i);
            }
        }
        table.Print();
        Console.WriteLine("finished");
        Console.ReadLine();
    }
}
