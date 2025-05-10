using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FigureAssetDatabase", menuName = "Game/Figure Asset Database")]
public class FigureAssetDatabase : ScriptableObject
{
    [SerializeField] private List<ShapePrefabPair> shapePrefabs;
    [SerializeField] private List<AnimalSpritePair> animalSprites;
    [SerializeField] private List<FrameColorPair> frameColors;

    private Dictionary<FigureAttributes.ShapeType, Sprite> shapeCache;
    private Dictionary<FigureAttributes.AnimalType, Sprite> animalCache;
    private Dictionary<FigureAttributes.FrameColor, Color> colorCache;

    private void OnEnable()
    {
        shapeCache = new Dictionary<FigureAttributes.ShapeType, Sprite>();
        animalCache = new Dictionary<FigureAttributes.AnimalType, Sprite>();
        colorCache = new Dictionary<FigureAttributes.FrameColor, Color>();

        foreach (var pair in shapePrefabs)
            shapeCache[pair.Shape] = pair.Sprite;

        foreach (var pair in animalSprites)
            animalCache[pair.Animal] = pair.Sprite;

        foreach (var pair in frameColors)
            colorCache[pair.FrameColor] = pair.DisplayColor;
    }

    public Sprite GetShapePrefab(FigureAttributes.ShapeType shape)
    {
        return shapeCache.TryGetValue(shape, out var sprite) ? sprite : null;
    }

    public Sprite GetAnimalSprite(FigureAttributes.AnimalType animal)
    {
        return animalCache.TryGetValue(animal, out var sprite) ? sprite : null;
    }

    public Color GetFrameColor(FigureAttributes.FrameColor color)
    {
        return colorCache.TryGetValue(color, out var displayColor) ? displayColor : Color.white;
    }
}

[System.Serializable]
public class ShapePrefabPair
{
    [SerializeField] private FigureAttributes.ShapeType shape;
    [SerializeField] private Sprite sprite;

    public FigureAttributes.ShapeType Shape => shape;
    public Sprite Sprite => sprite;
}

[System.Serializable]
public class AnimalSpritePair
{
    [SerializeField] private FigureAttributes.AnimalType animal;
    [SerializeField] private Sprite sprite;

    public FigureAttributes.AnimalType Animal => animal;
    public Sprite Sprite => sprite;
}

[System.Serializable]
public class FrameColorPair
{
    [SerializeField] private FigureAttributes.FrameColor frameColor;
    [SerializeField] private Color displayColor;

    public FigureAttributes.FrameColor FrameColor => frameColor;
    public Color DisplayColor => displayColor;
}