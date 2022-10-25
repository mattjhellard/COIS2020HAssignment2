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

        private int NextPrime(int current)
        {
            while (current<int.MaxValue)
            {
                int i;
                for(i=2; i*i<current; i++)
                {
                    if (current % i == 0)
                    {
                        i = current;
                    }
                }
                if (i < current)
                {
                    return current;
                }
                current++;
            }
            return -1;
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
