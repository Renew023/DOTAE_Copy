using System;

[Serializable]
public class GameSaveData
{
    public TimeSaveData timeData;
    public AffectionSaveData affectionData;
    public VillageRelationSaveData villageRelationData;
    public WeatherSaveData weatherData;
}