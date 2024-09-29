using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string itemDescription;
    public string itemSpriteBase64;

    public ItemData(string name, string description, Sprite sprite)
    {
        itemName = name;
        itemDescription = description;
        itemSpriteBase64 = SpriteToBase64(sprite);
    }

    // Sprite를 Base64 문자열로 변환하는 메서드
    public string SpriteToBase64(Sprite sprite)
    {
        if (sprite == null) return null;

        Texture2D texture = sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        return System.Convert.ToBase64String(bytes);
    }

    // Base64 문자열을 Sprite로 변환하는 메서드
    public Sprite Base64ToSprite()
    {
        if (string.IsNullOrEmpty(itemSpriteBase64)) return null;

        byte[] bytes = System.Convert.FromBase64String(itemSpriteBase64);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
