using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AirKuma {
  [Serializable]
  public class CircularBuffer<T> : IEnumerable<T> {

    private T[] buf;
    private int readIndex;
    private int writeIndex;
    public bool allowsOverwriting;

    private int SingleWrap(int index) {
      return index & (Capacity - 1);
    }
    private int DoubleWrap(int index) {
      return index & (2 * Capacity - 1);
    }
    private int Invert(int index) {
      return index ^ Capacity;
    }
    private int Increase(int index) {
      return DoubleWrap(index + 1);
    }

    public CircularBuffer(int capacity, bool allowsOverwriting = true) {
      buf = new T[capacity.GetCeilOfPowerOf2()]; // it must be power of 2 for working correctly
      readIndex = 0;
      writeIndex = 0;
      this.allowsOverwriting = allowsOverwriting;
    }

    public bool Empty => readIndex == writeIndex;
    public bool Full => readIndex == Invert(writeIndex);


    public int Capacity => buf.Length;
    public int Count => Empty ? 0 :
        (SingleWrap(writeIndex) > SingleWrap(readIndex)
        ? writeIndex - readIndex
        : Invert(SingleWrap(writeIndex)) - SingleWrap(readIndex));

    public void Write(T item) {
      if (Full) {
        if (allowsOverwriting) {
          readIndex = Increase(readIndex);
        } else {
          throw new Exception("can not write item because circular buffer is full");
        }
      }
      buf[SingleWrap(writeIndex)] = item;
      writeIndex = Increase(writeIndex);
    }
    public T Read() {
      if (Empty) {
        throw new Exception("can not read item because circular buffer is empty");
      }
      T item = buf[SingleWrap(readIndex)];
      readIndex = Increase(readIndex);
      return item;
    }

    public IEnumerable<T> PeekEachWrittenItems() {
      for (int ri = readIndex; DoubleWrap(ri) != DoubleWrap(writeIndex); ++ri) {
        yield return buf[SingleWrap(ri)];
      }
    }
    public IEnumerator<T> GetEnumerator() {
      return PeekEachWrittenItems().GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return PeekEachWrittenItems().GetEnumerator();
    }

    public IEnumerable<T> PeekEachWrittenItemsReversely() {
      for (int wi = writeIndex; DoubleWrap(wi) != DoubleWrap(readIndex); --wi) {
        yield return buf[SingleWrap(wi - 1)];
      }
    }

    public override string ToString() {
      return this.ToStringForEach();
    }

    private static void Test() {
      var buf = new CircularBuffer<int>(4);
      buf.Write(0);
      buf.Write(1);
      buf.Write(2);
      buf.Write(3);
      int i;
      Debug.Log(i = buf.Read());
      Debug.Log(i = buf.Read());
      Debug.Log(i = buf.Read());
      Debug.Log(i = buf.Read());
      Debug.Log(buf.Count);
    }
  }
}