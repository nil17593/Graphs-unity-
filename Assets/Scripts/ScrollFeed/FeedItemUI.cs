using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedItemUI : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI commentsText;
    public TextMeshProUGUI likesText;
    public Button likeButton;
    public Button commentButton;
    public TMP_InputField commentField;
    public int likes;
    public int comments;

    public void Initialize(Sprite _feedSprite)
    {
        int i = Random.Range(0, 100);
        likes = i;
        int commentsRandomCount = Random.Range(0, 100);
        comments = commentsRandomCount;

        string name = "User_" + Random.Range(0, 9999);
        image.sprite = _feedSprite;
        usernameText.text = name;
        commentsText.text = commentsRandomCount.ToString()+ " comments";
        likesText.text = likes.ToString()+ " likes";
        likeButton.onClick.AddListener(() => HandleLikeButtonClick());
        commentButton.onClick.AddListener(() => HandleCommentButtonClick());
    }

    private void HandleLikeButtonClick()
    {
        likes++;
        likesText.text = likes.ToString();
    }

    private void HandleCommentButtonClick( )
    {
        // Handle opening the comment section or UI
        // You can also add logic to load and display comments here.
    }
}
