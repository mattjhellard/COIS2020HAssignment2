/*
 * How to run in VS:
 * create new Console App (.NET Frameword)
 * clear out code from new .cs file
 * copy and paste in this code in this .cs file
 */
using System;
namespace OpenHashTableRevisited
{
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
        private int numBuckets;
        private int numItems;

        public HashTable()
        {
            numBuckets = 1;
            HT = new Node[numBuckets];
            MakeEmpty();
        }

        public void MakeEmpty()
        {
            for (int i = 0; i < numBuckets; i++)
            {
                HT[i] = null;
            }
            numItems = 0;
        }

        private int NextPrime(int current) //returns the next prime based on argument (can return argument if it's prime)
        {
            while (current < int.MaxValue) //loops until prime is found or int limit is reached
            {
                int i; //declared outside for post-loop use
                bool exit = false; //for loop exit condition
                for (i = 2; i * i <= current && exit == false; i++) //checks all values of i until proven not prime
                {
                    if (current % i == 0) //if i leaves 0 remainder after dividing current (i cannot ==1 or ==current)
                    {
                        exit = true; //exits loop on next condition check
                    }
                }
                if (exit == false) //if for loop was exited and exit==false, then no number from 2 to sqrt(current) was found to divide with a remainder of 0, i.e, current is prime
                {
                    return current;
                }
                current++; //if here, current number was not prime (see above comment), raise current for next loop
            }
            return -1; //failure code, due to behavior of parent class, initial argument and thus return should never be negative, only gets here if main loop exited without returning 
        }

        private void ReHash()
        {
            int oldNumBuckets = numBuckets;
            Node[] oldHT = HT;

            numBuckets = NextPrime(2 * numBuckets);
            HT = new Node[numBuckets];
            MakeEmpty();

            for (int i = 0; i < oldNumBuckets; i++)
            {
                Node p = oldHT[i];
                while (p != null)
                {
                    int k = p.key.GetHashCode() % numBuckets;
                    HT[k] = new Node(p.key, p.value, HT[k]);
                    numItems++;
                    p = p.next;
                }
            }
        }

        public void Insert(TKey key, TValue value)
        {
            int i = key.GetHashCode() % numBuckets;
            Node p = HT[i];

            while (p != null)
            {
                if (p.key.Equals(key))
                {
                    throw new InvalidOperationException("Duplicate key");
                }
                else
                {
                    p = p.next;
                }
            }

            HT[i] = new Node(key, value, HT[i]);
            numItems++;

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

        public void Print()
        {
            int i;
            Node p;

            for (i = 0; i < numBuckets; i++)
            {
                Console.Write(i.ToString().PadLeft(2) + ": ");

                p = HT[i];
                while (p != null)
                {
                    Console.Write("<" + p.key.ToString() + "," + p.value.ToString() + "> ");
                    p = p.next;
                }
                Console.WriteLine();
            }
        }
    }
}

public class Point
{

}
