using System.Collections;
using Character;
using Cinemachine;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private float rollPause;
    [SerializeField] private float rollTime;
    [SerializeField] private Button finalStartButton;
    [SerializeField] private CharacterAppearance characterShowCase;
    [SerializeField] private GameObject youAreDialog;
    [SerializeField] private GameObject startDialog;

    private float _currentTime;
    private CharacterMovement _characterMovement;
    private int _characterColorIndex;
    private int _characterShapeIndex;
    private int _characterPatternIndex;
    private int _currentNumberOfRuns;
    private int _firstColorIndex;
    private int _firstShapeIndex;
    private int _lastColorIndex;
    private int _lastShapeIndex;

    private void Start()
    {
        Time.timeScale = 1f;
        _currentNumberOfRuns = 0;

        _firstColorIndex = -1;
        _firstShapeIndex = -1;
        _lastColorIndex = -1;
        _lastColorIndex = -1;
    }

    public void StartRollForPlayer()
    {
        youAreDialog.SetActive(true);
        finalStartButton.interactable = false;
        StartCoroutine(RollForAppearance(characterShowCase));
    }

    private IEnumerator RollForAppearance(CharacterAppearance characterShowCase)
    {
        var colorLimit = CharacterAttributes.GetColorsLength();
        var shapeLimit = CharacterAttributes.GetShapesLength();
        var patternLimit = CharacterAttributes.GetPatternsLength();

        _characterColorIndex = -1;
        _characterShapeIndex = -1;
        characterShowCase.Initialize();

        var timer = rollTime;
        while (timer > 0)
        {
            int newColorIndex;
            int newShapeIndex;
            int newPatternIndex;
            do
            {
                newColorIndex = Random.Range(0, colorLimit);
                newShapeIndex = Random.Range(0, shapeLimit);
                newPatternIndex = Random.Range(0, patternLimit);
            } while (_characterColorIndex == newColorIndex && _characterShapeIndex == newShapeIndex);

            _characterColorIndex = newColorIndex;
            _characterShapeIndex = newShapeIndex;
            _characterPatternIndex = newPatternIndex;
            
            characterShowCase.SetAppearance(newShapeIndex, newColorIndex, newPatternIndex);
            timer -= rollPause;
            yield return new WaitForSeconds(rollPause);
        }
        
        switch (_currentNumberOfRuns)
        {
            case 0: // should not be privileged
                while (_characterColorIndex + _characterShapeIndex < 3)
                {
                    _characterColorIndex = Random.Range(0, colorLimit);
                    _characterShapeIndex = Random.Range(0, shapeLimit);
                }
                break;
            case 1: // should be privileged
                while (_characterColorIndex + _characterShapeIndex > 1)
                {
                    _characterColorIndex = Random.Range(0, colorLimit);
                    _characterShapeIndex = Random.Range(0, shapeLimit);
                }
                break;
            case 2: // should not have the same attributes as in run 1 or 2
                while (_characterColorIndex == _firstColorIndex && _characterShapeIndex == _firstShapeIndex ||
                       _characterColorIndex == _lastColorIndex && _characterShapeIndex == _lastShapeIndex)
                {
                    _characterColorIndex = Random.Range(0, colorLimit);
                    _characterShapeIndex = Random.Range(0, shapeLimit);
                }
                break;
            default:
                while(_characterColorIndex == _lastColorIndex && _characterShapeIndex == _lastShapeIndex)
                {
                    _characterColorIndex = Random.Range(0, colorLimit);
                    _characterShapeIndex = Random.Range(0, shapeLimit);
                }
                break;
        }

        _lastColorIndex = _characterColorIndex;
        _lastShapeIndex = _characterShapeIndex;
        characterShowCase.SetAppearance(_characterShapeIndex, _characterColorIndex, _characterPatternIndex);
        
        finalStartButton.interactable = true;
    }

    public void StartRound()
    {
        cityGrid.CreateNewCityGrid(CharacterAttributes.SpawnRestrictions.none);
        
        cityGrid.TryGetIntersectionPosition(characterStartCoordinates, out var characterStartPosition);
        _characterMovement = Instantiate(characterPrefab);
        var characterAttributes = _characterMovement.GetComponent<CharacterAttributes>();
        characterAttributes.SetAttributes((CharacterAttributes.CharShape)_characterShapeIndex, (CharacterAttributes.CharColor)_characterColorIndex, (CharacterAttributes.CharPattern)_characterPatternIndex);
        _characterMovement.Initialize(characterStartPosition, characterStartCoordinates, cityGrid, scoringSystem);
        
        cam.Follow = _characterMovement.transform;
        
        StartCoroutine(RoundTimer());
    }
    
    public void RestartRound()
    {
        ResetGame();
        StartRollForPlayer();
    }

    public void GoBackToStartScreen()
    {
        ResetGame();
        startDialog.SetActive(true);
    }

    private void ResetGame()
    {
        Time.timeScale = 1.0f;
        inputManager.enabled = true;
        Destroy(_characterMovement.gameObject);
        scoringSystem.ResetScore();
        cityGrid.DeleteCurrentCityGrid();
    }

    private IEnumerator RoundTimer()
    {
        _currentTime = roundTime;
        roundTimerUI.EnableTimer(_currentTime);
        scoringSystem.SetTextActive(true);
        while (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            roundTimerUI.UpdateTimerUI(_currentTime);
            yield return null;
        }
        
        // process End of turn
        inputManager.enabled = false;
        Time.timeScale = 0f;
        roundTimerUI.HideTimer();
        scoringSystem.SetTextActive(false);
        scoringSystem.GoToHighScores((CharacterAttributes.CharShape)_characterShapeIndex, (CharacterAttributes.CharColor)_characterColorIndex);
        _currentNumberOfRuns++;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
