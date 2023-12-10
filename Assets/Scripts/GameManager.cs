using System.Collections;
using Character;
using Cinemachine;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private float _currentTime;
    private CharacterMovement _characterMovement;
    private CharacterAppearance _characterAppearance;
    private int _characterColorIndex;
    private int _characterShapeIndex;

    private void Start()
    {
        Time.timeScale = 1f;
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
            yield return new WaitForSeconds(rollPause);
        }

        var lastColorIndex = PlayerPrefs.GetInt("lastColorIndex", -1);
        var lastShapeIndex = PlayerPrefs.GetInt("lastShapeIndex", -1);
        
        while(_characterColorIndex == lastColorIndex && _characterShapeIndex == lastShapeIndex)
        {
            _characterColorIndex = Random.Range(0, colorLimit);
            _characterShapeIndex = Random.Range(0, shapeLimit);
        }
        PlayerPrefs.SetInt("lastColorIndex", _characterColorIndex);
        PlayerPrefs.SetInt("lastShapeIndex", _characterShapeIndex);
        characterShowCase.SetAppearance(_characterShapeIndex, _characterColorIndex);
        
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
        var characterAttributes = _characterMovement.GetComponent<CharacterAttributes>();
        characterAttributes.SetAttributes((CharacterAttributes.CharShape)_characterShapeIndex, (CharacterAttributes.CharColor)_characterColorIndex);
        _characterMovement.Initialize(characterStartPosition, characterStartCoordinates, cityGrid, scoringSystem);
        
        cam.Follow = _characterMovement.transform;
        
        StartCoroutine(RoundTimer());
    }
    
    public void RestartRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        scoringSystem.GoToHighScores((CharacterAttributes.CharShape)_characterShapeIndex, (CharacterAttributes.CharColor)_characterColorIndex);
    }
}
