using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoistureState
{
    DRY,
    DRYING,
    MOIST,
    WET,
}
public class Soil
{
    public enum RadiationState
    {
        NOT_RADITATED,
        RADIATED
    }
    private float moistureLevel;
    //private float radiationLevel;
    private float dryRate = 0.01f;
    public bool isWaterSource;


    public Soil()
    {
        moistureLevel = Random.Range(0, 1f);
    }
    /// <summary>
    /// Returns Moisture State based on moisture level
    /// </summary>
    /// <returns>MoistureState: moisture State</returns>
    public MoistureState GetMoisture()
    {
        if (moistureLevel < 0.25f)
            return MoistureState.DRY;
        if (moistureLevel < 0.4f)
            return MoistureState.DRYING;
        if (moistureLevel < 0.8f)
            return MoistureState.MOIST;
        return MoistureState.WET;
    }
    public float MoistureLevel
    { get
        {
            return moistureLevel;
        }
      set
        {
            moistureLevel = Mathf.Min(value,1);
        }
    }

    public void UpdateSoil()
    {
        if (isWaterSource) return;
        if(moistureLevel>0)
        {
            moistureLevel -= dryRate * Time.deltaTime;
        }
        else
        {
            moistureLevel = 0;
        }
    }
}
