using UnityEngine;

[CreateAssetMenu(fileName = "NewFeedItem", menuName = "Feed Item")]
public class FeedItem : ScriptableObject
{
    public Sprite image;
    public string username;
    public string text;
    public int likes;
    public string[] comments;
}
