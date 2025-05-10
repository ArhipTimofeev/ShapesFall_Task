using UnityEngine;
using UnityEngine.EventSystems;

public class Figure : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer animalRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameManager gameManager;

    private FigureAttributes.ShapeType _shape;
    private FigureAttributes.AnimalType _animal;
    private FigureAttributes.FrameColor _color;

    public FigureAttributes.ShapeType Shape => _shape;
    public FigureAttributes.AnimalType Animal => _animal;
    public FigureAttributes.FrameColor Color => _color;

    public void Initialize(FigureAttributes.FigureConfig config, FigureAssetDatabase assetDatabase, GameManager manager)
    {
        _shape = config.shape;
        _animal = config.animal;
        _color = config.color;
        gameManager = manager;

        spriteRenderer.sprite = assetDatabase.GetShapePrefab(_shape);
        if (animalRenderer != null)
        {
            animalRenderer.sprite = assetDatabase.GetAnimalSprite(_animal);
        }
        frameRenderer.color = assetDatabase.GetFrameColor(_color);
    }

    public void DisablePhysics()
    {
        if (rb != null) DestroyImmediate(rb);
        if (TryGetComponent<CircleCollider2D>(out var collider)) DestroyImmediate(collider);
    }

    public bool IsMatching(Figure other)
    {
        return _shape == other._shape && _color == other._color && _animal == other._animal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager != null)
        {
            gameManager.HandleFigureClick(gameObject);
        }
        else if (gameManager == null)
        {
            Debug.LogError("GameManager is not assigned to Figure!");
        }

        Debug.Log("Figure was clicked");
    }
    
    public string GetKey()
    {
        return _shape.ToString() + "_" + _color.ToString() + "_" + _animal.ToString();
    }
}