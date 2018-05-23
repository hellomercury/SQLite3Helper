using System;
using Framework.Tools;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public static class RandomUtility
{
    public static int RandomRange(int InMin, int InMax)
    {
        return InMin == InMax ? InMin : Random.Range(InMin, InMax + 1);
    }

    public static float RandomRange(float InMin, float InMax)
    {
        return InMin == InMax ? InMin : Random.Range(InMin, InMax + 1);
    }

    public static int RandomWithWeight(int[] InValues, int[] InWeights)
    {
        Assert.IsNotNull(InValues);
        Assert.IsNotNull(InWeights);
        Assert.IsTrue(InValues.Length == InWeights.Length);
        int length = InWeights.Length;

        int sum = 0;
        for (int i = 0; i < length; ++i)
        {
            sum += InWeights[i];
        }
        int random = Random.Range(0, sum);
        sum = 0;
        for (int i = 0; i < length; ++i)
        {
            sum += InWeights[i];
            if (random < sum) return InValues[i];
        }

        return InValues[length - 1];
    }

    public static int RandomWithWeight(int[] InValues, float[] InWeights)
    {
        Assert.IsNotNull(InValues);
        Assert.IsNotNull(InWeights);
        Assert.IsTrue(InValues.Length == InWeights.Length);

        int length = InValues.Length;
        float sum = 0;
        for (int i = 0; i < length; ++i)
        {
            sum += InWeights[i];
        }
        float random = Random.Range(0, sum);
        sum = 0;
        for (int i = 0; i < length; ++i)
        {
            sum += InWeights[i];
            if (random < sum) return InValues[i];
        }

        return InValues[length - 1];
    }

    public static int[] RandomWithWeightCanNotRepeat(int[] InValues, float[] InWeights, int InCount)
    {
        Assert.IsNotNull(InValues);
        Assert.IsNotNull(InWeights);
        Assert.IsTrue(InValues.Length == InWeights.Length);

        int length = InValues.Length;
        if (InCount == InValues.Length) return InValues;
        else
        {
            int[] values = new int[length];
            Array.Copy(InValues, values, length);
            float[] weights = new float[length];
            Array.Copy(InWeights, weights, length);

            int[] result = new int[InCount];
            for (int i = 0; i < InCount; ++i)
            {
                float sum = 0;
                for (int j = 0; j < length; ++j)
                {
                    sum += InWeights[j];
                }
                float random = Random.Range(0, sum);
                sum = 0;
                for (int k = 0; k < length; ++k)
                {
                    sum += InWeights[k];
                    if (random < sum)
                    {
                        --length;
                        result[i] = InValues[k];
                        InWeights[k] = InWeights[length];
                        InValues[k] = InValues[length];
                        break;
                    }
                }
            }

            return result;
        }
    }

    public static int[] RandomWithWeightCanNotRepeatReturnIndex(int[] InValues, float[] InWeights, int InCount)
    {
        Assert.IsNotNull(InValues);
        Assert.IsNotNull(InWeights);
        Assert.IsTrue(InValues.Length == InWeights.Length);

        int[] result = new int[InCount];
        int length = InValues.Length;
        if (InCount == InValues.Length)
        {
            for (int i = 0; i < InCount; ++i)
            {
                result[i] = i;
            }
        }
        else
        {
            int[] values = new int[length];
            Array.Copy(InValues, values, length);
            float[] weights = new float[length];
            Array.Copy(InWeights, weights, length);

            for (int i = 0; i < InCount; ++i)
            {
                float sum = 0;
                for (int j = 0; j < length; ++j)
                {
                    sum += InWeights[j];
                }
                float random = Random.Range(0, sum);
                sum = 0;
                for (int k = 0; k < length; ++k)
                {
                    sum += InWeights[k];
                    if (random < sum)
                    {
                        --length;
                        result[i] = k;//InValues[k];
                        weights[k] = weights[length];
                        values[k] = values[length];
                        break;
                    }
                }
            }
        }

        return result;
    }

    public static int[] RandomIndex(int InMin, int InMax, int InCount)
    {
        int length = InMax - InMin;
        Assert.IsTrue(length >= InCount);

        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = InMin + i;
        }

        int[] result = new int[InCount];
        for (int i = 0; i < InCount; i++)
        {
            int index = Random.Range(0, length);
            result[i] = array[index];
            array[index] = array[length - 1];
            --length;
        }

        return SortUtility.BubbleSort(result);
    }
}
