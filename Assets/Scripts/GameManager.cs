using System;
using System.Collections;
using Character;
using Cinemachine;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float roundTime;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private UIRoundTimer roundTimerUI;
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private CityGridCreator cityGrid;
    [SerializeField] private CharacterMovement characterPrefab;
    [SerializeField] private Vector2Int characterStartCoordinates;
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private float rollPause;
    [SerializeField] private float rollTime;
    [SerializeField] private Button finalStartButton;

    private float _currentTime;
    private CharacterMovement _characterMovement;
    private CharacterAppearance _characterAppearance;
    private int _characterColorIndex;
    private int _characterShapeIndex;

    void Start()
    {
        //StartRound();
    }

    public void StartRollForPlayer(CharacterAppearance characterShowCase)
    {
        finalStartButton.interactable = false;
        StartCoroutine(RollForAppearance(characterShowCase));
    }

    private IEnumerator RollForAppearance(CharacterAppearance characterShowCase)
    {
        var colorLimit = characterShowCase.GetColorLength();
        var shapeLimit = characterShowCase.GetShapesLength();
        var appearanceWait = new WaitForSeconds(rollPause);

        _characterColorIndex = -1;
        _characterShapeIndex = -1;
        characterShowCase.Initialize();

        var timer = rollTime;
        while (timer > 0)
        {
            int newColorIndex;
            int newShapeIndex;
            do
            {
                newColorIndex = Random.Range(0, colorLimit);
                newShapeIndex = Random.Range(0, shapeLimit);
            } while (_characterColorIndex == newColorIndex && _characterShapeIndex == newShapeIndex);

            _characterColorIndex = newColorIndex;
            _characterShapeIndex = newShapeIndex;
            
            characterShowCase.SetAppearance(newShapeIndex, newColorIndex);
            timer -= rollPause;
            yield return appearanceWait;
        }

        finalStartButton.interactable = true;
    }

    public void StartRound()
    {
        cityGrid.CreateNewCityGrid();
        Time.timeScale = 1.0f;
        inputManager.enabled = true;

        if (_characterMovement != null)
        {
            Destroy(_characterMovement.gameObject);   
        }
        cityGrid.TryGetIntersectionPosition(characterStartCoordinates, out var characterStartPosition);
        _characterMovement = Instantiate(characterPrefab);
        _characterAppearance = _characterMovement.GetComponent<CharacterAppearance>();
        _characterAppearance.Initialize();
        _characterMovement.Initialize(characterStartPosition, characterStartCoordinates, cityGrid, scoringSystem);
        _characterAppearance.SetAppearance(_characterShapeIndex, _characterColorIndex);
        
        camera.Follow = _characterMovement.transform;
        
        StartCoroutine(RoundTimer());
    }
    
    public void RestartRound()
    {
        StartRound();
    }

    private IEnumerator RoundTimer()
    {
        _currentTime = roundTime;
        roundTimerUI.EnableTimer(_currentTime);
        while (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            roundTimerUI.UpdateTimerUI(_currentTime);
            yield return null;
        }
        
        // process End of turn
        inputManager.enabled = false;
        Time.timeScale = 0f;
        scoringSystem.ShowHighScoreList();
    }
}
