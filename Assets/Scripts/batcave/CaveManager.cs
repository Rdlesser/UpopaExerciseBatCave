using System.Collections;
using System.Collections.Generic;
using BatCave;
using UnityEngine;

public class CaveManager : MonoSingleton<CaveManager>
{
    /// <summary>
    /// A manager for all the cave screens
    /// </summary>

    [SerializeField] private RepeatingBackground[] caveScreens;

    private int caveScreenIndex;

    protected override void Init()
    {
        Bat.OnBatPassedCave += RepositionBackground;
        GameManager.OnGameReset += CaveReset;
    }

    /// <summary>
    /// Check if the bat has passed the current cave screen
    /// </summary>
    /// <param name="positionX"> The x position of the bat</param>
    /// <returns></returns>
    public bool DidBatPassCave(float positionX)
    {
        RepeatingBackground currentBackground = caveScreens[caveScreenIndex];
        if (positionX > currentBackground.transform.position.x + currentBackground.CaveLength)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Reposition the cave to off-camera in front of the bat
    /// </summary>
    private void RepositionBackground()
    {
        caveScreens[caveScreenIndex].RepositionBackground();
        caveScreenIndex = (caveScreenIndex + 1) % caveScreens.Length;

    }

    private void CaveReset()
    {
        // Reset the index
        caveScreenIndex = 0;
    }
}
