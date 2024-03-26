using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LayoutDifficultyProvider
{
    private Dictionary<int, List<int>> _difficulties;
    private List<int> _difficultyForNegativeNumber;
    private List<int> _difficultyForHighPositiveNumber;
    private int _minCustomYCoordinate = 1;
    private int _maxCustomYCoordinate = 51;

    public LayoutDifficultyProvider()
    {
        _difficulties = new()
        {
            {1, new(){0, 0, 0, 0, 0}},
            {6, new(){0, 0, 0, 0, 0}},
            {11, new(){0, 0, 0, 0, 0}},
            {16, new(){0, 0, 0, 0, 0}},
            {21, new(){0, 0, 0, 0, 0}},
            {26, new(){0, 0, 0, 0, 0}},
            {31, new(){0, 0, 0, 0, 0}},
            {36, new(){0, 0, 0, 0, 0}},
            {41, new(){0, 0, 0, 0, 0}},
            {46, new(){0, 0, 0, 0, 0}},
            {51, new(){0, 0, 0, 0, 0}}
        };

        _difficultyForNegativeNumber = new List<int> {0, 0, 0, 0, 0};
        _difficultyForHighPositiveNumber = new List<int> {0, 0, 0, 0, 0};
    }

    public int GetLayoutDifficulty(int yCoordinate)
    {
        if(yCoordinate <= 0)
        {
            return 0; // Todo: get random number of list
        }
        else if(yCoordinate > _maxCustomYCoordinate)
        {
            return 0; // Todo: get random number of list
        }

        var layoutBlockNumber = Mathf.CeilToInt((float)yCoordinate / 3);
        var difficultyListId = Mathf.FloorToInt(((float)layoutBlockNumber - 1f) / 5) * 5 + 1;
        var difficultyList = _difficulties[difficultyListId];

        var difficulty = difficultyList[Random.Range(0, difficultyList.Count)];
        difficultyList.Remove(difficulty);
        
        return difficulty;
    }
}
