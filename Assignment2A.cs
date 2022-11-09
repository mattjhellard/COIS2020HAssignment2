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
            HT[i] = null;
            header[i] = HT[i];
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

    private void ReHash()
    {
        int oldNumBuckets = numBuckets;
        Node[] oldHT = HT;

        numBuckets = NextPrime(2 * numBuckets);
        HT = new Node[numBuckets];
        header = new Node[numBuckets];
        MakeEmpty();

        for (int i = 0; i < oldNumBuckets; i++)
        {
            Node p = oldHT[i];
            while (p != null)
            {
                int k = p.key.GetHashCode() % numBuckets;
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
        Node p = HT[i];

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
        if (HT[i] == null || key.GetHashCode() <= HT[i].key.GetHashCode()) //cases for inserting new pair in front (bucket is empty or key is less than/equal to front of existing bucket)
        {
            HT[i] = new Node(key, value, HT[i]); //bucket redefined as new pair with old bucket as the next(s)
            numItems++;
        }
        else //if new pair isn't going to go in front of bucket
        {
            p = HT[i]; //p equals front of bucket again
            while (p.next != null && key.GetHashCode() > p.next.key.GetHashCode()) //while next isn't null and key code is greater than next key code
            {
                p = p.next; //go to next node
            } //exits when next value is null or key code is not greater than it
            p.next = new Node(key, value, p.next); //next node equals insertion node with its next node as the old next node
            numItems++;
        }

        // Rehash if the average size of the buckets exceeds 5.0
        if ((double)numItems / numBuckets > 5.0)
        {
            ReHash();
        }
    }

    // Remove
    // Delete (if found) the <key,value> with the given key
    // Return true if successful, false otherwise
    public bool Remove(TKey key)
    {
        int i = key.GetHashCode() % numBuckets;
        Node p = HT[i];

        if (p == null)
        {
            return false;
        }
        else
        {
            // Successful remove of the first item in a bucket
            if (p.key.Equals(key))
            {
                HT[i] = HT[i].next;
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
        Node p = HT[i];

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

    public void Output()
    {
        for (int i = 0; i < numBuckets; i++)
        {
            Console.Write(i + ":");
            Node p = HT[i];
            while (p != null)
            {
                Console.Write("<" + p.key.GetHashCode() + "," + p.value + "> ");
                p = p.next;
            }
            Console.WriteLine();
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
        return Math.Abs(x * x + y * y); //hashcode is kept simple to make testing easier to understand
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
        int size = 10;
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
        table.Output();
        Console.ReadLine();
    }
}
