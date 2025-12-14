using UnityEngine;

public class IslandPetController : MonoBehaviour
{
    [SerializeField] private bool _isMyPet;
    private IslandManager _islandManager;

    private void Start()
    {
        _islandManager = FindObjectOfType<IslandManager>();
    }
    private void FeedIslandPet()
    {

    }

}
