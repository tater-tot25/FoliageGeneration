using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateFoliage : MonoBehaviour
{
    public int pub_size = 5;
    public float[] pub_rarity; // must be in order from least to greatest, clamped betweened 0-10
    public int pub_statLen; // the length of the rarity array
    public int[] outputList;



    //Initialize the grid filled with zeroes
    private int[][] generateGrid(int size)
    {
        int[][] grid = new int[size][];
        for (int i = 0; i < size; i++)
        {
            grid[i] = new int[size];
        }
        for (int i = 0; i < size; i++)
        {
            for (int n = 0; n < size; n++)
            {
                grid[i][n] = 0;
            }
        }
        return grid;
    }

    //Randomly try to fill about 1/3 of the graph with the 
    //lowest rarity item (1)
    public int[][] fillInitialGrid(int[][] grid, int size)
    {
        System.Random random = new System.Random();
        int[][] returnVal = grid;
        for (int i = 0; i < size; i++)
        {
            for (int n = 0; n < size; n++)
            {
                int pick = random.Next(3);
                if (pick == 0)
                {
                    returnVal[i][n] = 1;
                }
            }
        }
        return returnVal;
    }

    //get the eight surrounding cell values, if the cell is out of the scope of
    //the array, set its value to negative 1;
    public float[] getEightClosest(int[][] grid, int x, int y, int size, float[] stats)
    {
        float[] returnVal = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        int xStart = x - 1;
        int yStart = y - 1;
        int xEnd = x + 1;
        int yEnd = y + 1;
        if (x == 0)
        {
            xStart = x;
        }
        else if (x >= size - 1)
        {
            xEnd = x;
        }
        if (y == 0)
        {
            yStart = y;
        }
        else if (y >= size - 1)
        {
            yEnd = y;
        }
        int indexReturnVal = 0;
        for (int i = yStart; i < yEnd; i++)
        {
            for (int n = xStart; n < xEnd; n++)
            {
                int temp = grid[i][n];
                float rarity = stats[grid[i][n]];
                returnVal[indexReturnVal] = rarity;
                indexReturnVal++;
            }
        }
        //Console.WriteLine(returnVal);
        return returnVal;
    }

    public float getRarityIndex(float[] surroundingVals, int statLen)
    {
        float total = 0;
        for (int i = 0; i < 9; i++)
        {
            if (surroundingVals[i] == -1)
            {
                System.Random random = new System.Random();
                //the divisor is some magic number I had to guess, dunno why it likes 0
                surroundingVals[i] = 0;
            }
            total += surroundingVals[i];
        }
        //Console.WriteLine(total);
        return total / 9;
    }

    //generates an array representing the order that we seed the values
    public int[][] createSeedOrder(int size)
    {
        int[][] returnVal = new int[size * size][];
        int count = 0;
        // creates a 2D array with tuples indicating the coord to operate on
        for (int i = 0; i < size; i++)
        {
            for (int n = 0; n < size; n++)
            {
                int[] tuple = { i, n };
                returnVal[count] = tuple;
                count++;
            }
        }
        // shuffles the order of the tuples in the 2D array
        // use fisherYates which works by randomly swapping
        // elements for duration of the length of the array
        count = size * size;
        while (count > 1)
        {
            System.Random rand = new System.Random();
            int index = rand.Next(count--);
            int[] temp = returnVal[count];
            returnVal[count] = returnVal[index];
            returnVal[index] = temp;
        }
        return returnVal;
    }

    //Takes the rarityIndex and finds the intended int in stats
    int distributeRarity(float rarityIndex, float[] stats, int statLen)
    {
        double temp = (((System.Math.Log(rarityIndex)) * (-1)) + 1) * 4.2f;
        float newRarityIndex = (float)temp;
        if (newRarityIndex > 10)
        {
            newRarityIndex = 9.9f;
        }
        //get closest val in stats
        int idx = System.Array.BinarySearch(stats, newRarityIndex);
        if (idx < 0)
        {
            idx = (idx * (-1)) - 1;
        }
        //determine if this is a closer value than the 
        //previous index
        if ((idx > 0))
        {
            if (idx >= statLen)
            {
                idx = statLen - 1;
            }
            //Debug.Log(idx);
            if ((System.Math.Abs(rarityIndex - stats[idx - 1]))
                           < System.Math.Abs(rarityIndex - stats[idx]))
            {
                idx = idx - 1;
            }
        }
        return (idx);
    }

    //refill grid with randomly generated things
    public int[][] propagateGrid(int[][] grid, int size, float[] stats, int statLen)
    {
        int[][] returnVal = grid;
        int[][] order = this.createSeedOrder(size);
        //this.printOrder(order, size * size);
        int count = 0;
        for (int i = 0; i < size; i++)
        {
            for (int n = 0; n < size; n++)
            {
                int tempX = order[count][1];
                int tempY = order[count][0];
                float[] neighbors = getEightClosest(returnVal, tempX, tempY, size, stats);
                float rarity = getRarityIndex(neighbors, statLen);
                int propNum = distributeRarity(rarity, stats, statLen);
                returnVal[i][n] = propNum;
                count++;
            }
        }
        return returnVal;
    }

    //Kickstarts the generation of the foliage diversity
    public void startGeneration()
    {
        int size = pub_size;
        float[] rarity = pub_rarity;
        int statLen = pub_statLen;
        int[][] grid = generateGrid(size);
        grid = fillInitialGrid(grid, size);
        //running propagate more balances out the values,
        //two is a good middle ground for getting good distribution
        //of lower numbers with the occasional high  number
        grid = propagateGrid(grid, size, rarity, statLen);
        grid = propagateGrid(grid, size, rarity, statLen);
        int[] temp = new int[size * size];
        int count = 0;
        for (int i = 0; i < size; i++)
        {
            for (int n = 0; n < size; n++)
            {
                temp[count] = grid[i][n];
                count++;
            }
        }
        outputList = temp;
        //printGrid(grid, size);
    }
}
