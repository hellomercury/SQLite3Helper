public static class SortUtility
{
    public static int[] BubbleSort(int[] InSourceArray)
    {
        int length = InSourceArray.Length;
        for (int i = 0; i < length; i++)
        {
            for (int j = i; j < length; j++)
            {
                if (InSourceArray[i] > InSourceArray[j])
                {
                    InSourceArray[i] = InSourceArray[i] + InSourceArray[j];
                    InSourceArray[j] = InSourceArray[i] - InSourceArray[j];
                    InSourceArray[i] = InSourceArray[i] - InSourceArray[j];
                }
            }
        }

        return InSourceArray;
    }
}
