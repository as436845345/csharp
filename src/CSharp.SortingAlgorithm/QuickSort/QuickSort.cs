using System.ComponentModel.DataAnnotations;

namespace CSharp.SortingAlgorithm.QuickSort;

[Display]
public class QuickSort
{
    public static void Execute()
    {
        int[] array = [3, 1, 2, 5, 6, 8];
        Sort(array, 0, array.Length - 1);
    }

    public static void Sort(Span<int> span, int low, int high)
    {
        if (low >= high)
            return;

        var pivot = span[low];
        int i = low;
        int j = high;

        while (i < j)
        {
            while (i < j && span[j] >= pivot) j--;
            span[i] = span[j];

            while (i < j && span[i] < pivot) i++;
            span[j] = span[i];
        }

        span[i] = pivot;

        Sort(span, low, i - 1);
        Sort(span, i + 1, high);
    }
}
