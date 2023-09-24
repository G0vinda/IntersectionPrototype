using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHighScoreEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite markedSprite;

        private Image _image;

        public void Initialize(int score, bool marked = false)
        {
            _image = GetComponent<Image>();
            scoreText.text = score.ToString();

            _image.sprite = marked ? markedSprite : defaultSprite;
        }
    }
}
