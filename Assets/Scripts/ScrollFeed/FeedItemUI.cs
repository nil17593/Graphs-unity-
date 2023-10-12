using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Garuna.ScrollingFeed
{
    public class FeedItemUI : MonoBehaviour
    {
        [Header("Serialized fields")]
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI commentsText;
        [SerializeField] private TextMeshProUGUI likesText;
        [SerializeField] private Button likeButton;
        [SerializeField] private Button commentButton;
        [SerializeField] private TMP_InputField commentField;
        [SerializeField] private int likes;
        [SerializeField] private int comments;
        [SerializeField] private GameObject commentsPanel;

        #region private members
        private bool isLiked = false;
        #endregion

        public void Initialize(Sprite _feedSprite)
        {
            int i = Random.Range(0, 100);
            likes = i;
            int commentsRandomCount = Random.Range(0, 100);
            comments = commentsRandomCount;

            string name = "User_" + Random.Range(0, 9999);
            image.sprite = _feedSprite;
            usernameText.text = name;
            commentsText.text = commentsRandomCount.ToString() + " comments";
            likesText.text = likes.ToString() + " likes";
            likeButton.onClick.AddListener(() => HandleLikeButtonClick());
            //commentButton.onClick.AddListener(() => HandleCommentButtonClick());
        }

        public int GetComments()
        {
            return comments;
        }

        private void HandleLikeButtonClick()
        {
            if (!isLiked)
            {
                isLiked = true;
                likeButton.image.color = Color.green;
                likes++;
                likesText.text = likes.ToString() + " likes";
            }
            else
            {
                isLiked = false;
                likeButton.image.color = Color.white;
                likes--;
                likesText.text = likes.ToString() + " likes";
            }
        }

        //private void HandleCommentButtonClick( )
        //{
        //    commentsPanel.SetActive(true);
        //}
    }
}
