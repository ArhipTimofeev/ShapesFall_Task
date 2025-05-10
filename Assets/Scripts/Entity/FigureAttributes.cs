using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FigureAttributes 
{
    public enum ShapeType { Circle, Square, Triangle }
    public enum FrameColor { Red, Green, Blue }
    public enum AnimalType { Tiger, Elefant, Duck }

    [Serializable]
    public struct FigureConfig 
    {
        public ShapeType shape;
        public FrameColor color;
        public AnimalType animal;
    }

    public static FigureConfig GenerateRandomFigure()
    {
        return new FigureConfig
        {
            shape = (ShapeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ShapeType)).Length),
            color = (FrameColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(FrameColor)).Length),
            animal = (AnimalType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AnimalType)).Length)
        };
    }

    public static List<FigureConfig> GetAllPossibleConfigs()
    {
        return (from ShapeType shape in Enum.GetValues(typeof(ShapeType))
                from FrameColor color in Enum.GetValues(typeof(FrameColor))
                from AnimalType animal in Enum.GetValues(typeof(AnimalType))
                select new FigureConfig { shape = shape, color = color, animal = animal })
                .ToList();
    }
}