using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class LayoutDifficultyProvider
{
    private Dictionary<int, List<int>> _difficulties;
    private List<int> _difficultyListForHighPositiveNumber;
    private int _maxCustomYCoordinate = 51;

    public LayoutDifficultyProvider()
    {
        _difficulties = new()
        {
            {1, new(){0, 0, 0, 0, 0}}, // Layouts 1 - 5
            {6, new(){0, 0, 0, 0, 0}}, // Layouts 6 - 10
            {11, new(){0, 0, 0, 0, 0}}, // Layouts 11 - 15
            {16, new(){0, 0, 0, 0, 0}}, // Layouts 16 - 20
            {21, new(){0, 0, 0, 0, 0}}, // Layouts 21 - 25
            {26, new(){0, 0, 0, 0, 0}}, // Layouts 26 - 30
            {31, new(){0, 0, 0, 0, 0}}, // Layouts 31 - 35
            {36, new(){0, 0, 0, 0, 0}}, // Layouts 36 - 40
            {41, new(){0, 0, 0, 0, 0}}, // Layouts 41 - 45
            {46, new(){0, 0, 0, 0, 0}}, // Layouts 46 - 50
            {51, new(){0, 0, 0, 0, 0}} // Layouts 51 - 55 and all Layouts after that
        };

        _difficultyListForHighPositiveNumber = _difficulties[51].ToArray().ToList();
    }

    public int GetLayoutDifficulty(int yCoordinate)
    {
        if(yCoordinate <= 0)
        {
            return 0; // Todo: get random number of list
        }
        else if(yCoordinate > _maxCustomYCoordinate)
        {
            return _difficultyListForHighPositiveNumber[Random.Range(0, _difficultyListForHighPositiveNumber.Count)];
        }

        var layoutBlockNumber = Mathf.CeilToInt((float)yCoordinate / 3);
        var difficultyListId = Mathf.FloorToInt(((float)layoutBlockNumber - 1f) / 5) * 5 + 1;
        var difficultyList = _difficulties[difficultyListId];

        var difficulty = difficultyList[Random.Range(0, difficultyList.Count)];
        difficultyList.Remove(difficulty);
        
        return difficulty;
    }
}
