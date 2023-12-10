using Character;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UIHighScoreEntry : MonoBehaviour
    {
        [SerializeField] private GameObject markedBackground;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image characterIconImage;
        [SerializeField] private TextMeshProUGUI rankText;
        

        [Header("Character Icon Sprites")] 
        [SerializeField] private Sprite square;
        [SerializeField] private Sprite triangle;
        [SerializeField] private Sprite circle;

        [Header("Character Colors")] 
        [SerializeField] private Color blue;
        [SerializeField] private Color red;
        [SerializeField] private Color yellow;

        public void Initialize(ScoringSystem.HighScoreEntryData data, int rank, bool marked = false)
        {
            scoreText.text = data.highScore.ToString();
            rankText.text = rank + ".";
            playerNameText.text = data.playerName;

            var sprite = data.playerShape switch
            {
                (int)CharacterAttributes.CharShape.Cube => square,
                (int)CharacterAttributes.CharShape.Pyramid => triangle,
                (int)CharacterAttributes.CharShape.Sphere => circle,
                _ => square
            };
            characterIconImage.sprite = sprite;

            var color = data.playerColor switch
            {
                (int)CharacterAttributes.CharColor.Blue => blue,
                (int)CharacterAttributes.CharColor.Red => red,
                (int)CharacterAttributes.CharColor.Yellow => yellow,
                _=> Color.black
            };
            characterIconImage.color = color;
            
            markedBackground.SetActive(marked);
        }
    }
}
