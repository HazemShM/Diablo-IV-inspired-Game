using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] GameObject barbarianPrefab;
    [SerializeField] GameObject sorcererPrefab;
    [SerializeField] GameObject roguePrefab;
    [SerializeField] Transform spawnPoint;

    void Start()
    {
        // Spawn the selected character
        switch (GameManager.SelectedCharacter)
        {
            case "Barbarian":
                GameObject player = Instantiate(barbarianPrefab, spawnPoint.position, spawnPoint.rotation);
                GameManager.NotifyPlayerInstantiated(player);
                break;
            case "Sorcerer":
                player = Instantiate(sorcererPrefab, spawnPoint.position, spawnPoint.rotation);
                GameManager.NotifyPlayerInstantiated(player);
                break;
            case "Rogue":
                player = Instantiate(roguePrefab, spawnPoint.position, spawnPoint.rotation);
                GameManager.NotifyPlayerInstantiated(player);
                break;
            default:
                Debug.LogError("No character selected!");
                break;
        }
    }
}
