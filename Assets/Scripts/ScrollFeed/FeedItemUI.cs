using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedItemUI : MonoBehaviour
{
    public Sprite image;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI text;
    public TextMeshProUGUI likesText;
    public Button likeButton;
    public Button commentButton;
    public TMP_InputField commentField;


    public void Initialize(FeedItem feedItem)
    {
        image = feedItem.image;
        usernameText.text = feedItem.username;
        text.text = feedItem.text;
        likesText.text = feedItem.likes.ToString();
        GetComponent<Image>().sprite = image;
        // Attach event listeners to likeButton and commentButton
        likeButton.onClick.AddListener(() => HandleLikeButtonClick(feedItem));
        commentButton.onClick.AddListener(() => HandleCommentButtonClick(feedItem));
    }

    private void HandleLikeButtonClick(FeedItem feedItem)
    {
        // Handle liking logic
        feedItem.likes++;
        likesText.text = feedItem.likes.ToString();
    }

    private void HandleCommentButtonClick(FeedItem feedItem)
    {
        // Handle opening the comment section or UI
        // You can also add logic to load and display comments here.
    }
}
