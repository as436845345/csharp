using System.ComponentModel.DataAnnotations;

namespace CSharp.SortingAlgorithm.RadixSort;

[Display]
public partial class RadixSort
{
    public static void Execute()
    {
        LSD_Basic([170, 45, 75, 90, 2, 802, 2, 66]);
        MSD_Basic([170, 45, 75, 25, 2, 24, 802, 66]);
    }

    struct Store
    {
        private int[] _array;
        private int _index;

        public readonly int Length => _index;
        public readonly int[] Array => _array;
        public readonly int this[int index] => _array[index];

        public void Add(int value)
        {
            if (_array == null)
                _array = new int[10];

            if (Length == _array.Length)
            {
                var array = _array;
                _array = new int[array.Length << 1];
                array.AsSpan().CopyTo(_array);
            }

            _array[_index++] = value;
        }

        public int[] Slice(int length)
        {
            return Slice(0, length);
        }

        public int[] Slice(int start, int length)
        {
            var array = new int[length];
            _array.AsSpan().Slice(start, length).CopyTo(array);
            return array;
        }

        public void Clear()
        {
            if (_index > 0)
            {
                _index = 0;
                _array.AsSpan().Clear();
            }
        }
    }

    /// <summary>
    /// 计算整数的位数（区间判断法，极致性能）
    /// </summary>
    /// <param name="number">要计算的整数</param>
    /// <returns>数字的位数</returns>
    public static int GetNumberDigitsByRange(int number)
    {
        number = Math.Abs(number);

        if (number < 10) return 1;
        if (number < 100) return 2;
        if (number < 1000) return 3;
        if (number < 10000) return 4;
        if (number < 100000) return 5;
        if (number < 1000000) return 6;
        if (number < 10000000) return 7;
        if (number < 100000000) return 8;
        if (number < 1000000000) return 9;
        return 10; // int最大值是2147483647，最多10位
    }
}
