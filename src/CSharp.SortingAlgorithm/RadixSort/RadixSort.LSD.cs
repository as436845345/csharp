namespace CSharp.SortingAlgorithm.RadixSort;

// 最低有效位（LSD）基数排序

public partial class RadixSort
{
    internal static int[] LSD_Basic(int[] input)
    {
        ArgumentNullException.ThrowIfNull(nameof(input));

        if (input.Length < 1)
            throw new InvalidOperationException(nameof(input));

        int maxNumberLength = 0;

        // 存储排序后的数据
        var output = new int[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            var num = input[i];
            output[i] = num;
            maxNumberLength = Math.Max(maxNumberLength, GetNumberDigitsByRange(num));
        }

        // 桶
        var buckets = new Store[10];

        int len = maxNumberLength;
        do
        {
            for (int i = 0; i < output.Length; i++)
            {
                var num = output[i];
                var last = (num / (int)Math.Pow(10, len - maxNumberLength)) % 10;

                ref var bucket = ref buckets[last];
                bucket.Add(num);
            }

            // 更新 input
            for (int i = 0, j = 0; i < buckets.Length; i++)
            {
                ref var bucket = ref buckets[i];

                for (int p = 0; p < bucket.Length; p++)
                {
                    output[j++] = bucket[p];
                }

                bucket.Clear();
            }
        }
        while (--maxNumberLength > 0);

        return output;
    }
}
