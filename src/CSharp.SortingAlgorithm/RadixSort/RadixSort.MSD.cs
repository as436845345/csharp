using System.Diagnostics;

namespace CSharp.SortingAlgorithm.RadixSort;

// 最高有效位（MSD）基数排序

public partial class RadixSort
{
    internal static int[] MSD_Basic(int[] input)
    {
        ArgumentNullException.ThrowIfNull(nameof(input));

        if (input.Length < 1)
            throw new InvalidOperationException(nameof(input));

        int maxNumberLength = 0;

        for (int i = 0; i < input.Length; i++)
        {
            var num = input[i];
            maxNumberLength = Math.Max(maxNumberLength, GetNumberDigitsByRange(num));
        }

        // 存储排序后的数据
        var output = new int[input.Length];

        Stack<(ArraySegment<int> Segment, int Offset, int CurrentDigit)> stack = new();
        stack.Push((new ArraySegment<int>(input), 0, maxNumberLength - 1));

        while (stack.Count > 0)
        {
            var stackValue = stack.Pop();

            var segment = stackValue.Segment;
            var currentDigit = stackValue.CurrentDigit;

            Debug.Assert(currentDigit > -2, "不可能进的位置：currentDigit < -1");

            if (segment.Count == 1)
            {
                output[stackValue.Offset] = segment[0];
                continue;
            }

            // 桶
            var buckets = new Store[10];

            for (int i = 0; i < segment.Count; i++)
            {
                var num = segment[i];
                var last = (num / (int)Math.Pow(10, currentDigit)) % 10;

                ref var bucket = ref buckets[last];
                bucket.Add(num);
            }

            int index = stackValue.Offset;

            for (int i = 0; i < buckets.Length; i++)
            {
                var bucket = buckets[i];
                if (bucket.Length == 0)
                    continue;

                stack.Push((new ArraySegment<int>(bucket.Slice(bucket.Length)), index, currentDigit - 1));

                index += bucket.Length;
            }
        }

        return output;
    }
}
