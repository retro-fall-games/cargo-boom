using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Sprites/Sprite Renderer Random Color")]
  public class SpriteRendererRandomColor : MonoBehaviour
  {
    [field: SerializeField] private Palette Palette { get; set; }

    private SpriteRenderer _sprite;

    private void Awake()
    {
      _sprite = GetComponent<SpriteRenderer>();
      _sprite.material.color = Palette.GetRandomColor();
    }
  }
}