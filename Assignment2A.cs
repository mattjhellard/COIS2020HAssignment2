/*
 * How to run in VS:
 * create new Console App (.NET Frameword)
 * clear out code from new .cs file
 * copy and paste in this code in this .cs file
 */
using System;
namespace OpenHashTableRevisited
{
public interface IHashTable<TKey,TValue>
{
    void Insert(TKey key, TValue value);
    bool Remove(TKey key);
    TValue Get(TKey key);
}

public class HashTable <TKey, TValue> : IHashTable<TKey,TValue>
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
            for(int i =0; i < numBuckets; i++)
            {
                HT[i] = null;
            }
            numItems = 0;
        }

        private int NextPrime(int current) //returns the next prime based on argument (can return argument if it's prime)
        {
            while (current<int.MaxValue) //loops until prime is found or int limit is reached
            {
                int i; //declared outside for post-loop use
                bool exit = false; //for loop exit condition
                for(i=2; i*i<=current && exit==false; i++) //checks all values of i until proven not prime
                {
                    if (current % i == 0) //if i leaves 0 remainder after dividing current (i cannot ==1 or ==current)
                    {
                        exit = true; //exits loop on next condition check
                    }
                }
                if (exit==false) //if for loop was exited and exit==false, then no number from 2 to sqrt(current) was found to divide with a remainder of 0, i.e, current is prime
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

            for(int i=0; i<oldNumBuckets; i++)
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
}
}

public class Point
{

}
