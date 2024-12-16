using UnityEngine;

public static class GameManager
{
    public static string SelectedCharacter; // Stores the selected character name
    public static System.Action<GameObject> OnPlayerInstantiated;

    public static bool increaseLevels = false;
    public static void NotifyPlayerInstantiated(GameObject player)
    {
        OnPlayerInstantiated?.Invoke(player);
    }
}