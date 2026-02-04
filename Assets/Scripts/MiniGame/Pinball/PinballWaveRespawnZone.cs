using UnityEngine;

public class PinballWaveRespawnZone : MonoBehaviour
{
    [SerializeField] private PinballGameManager _game;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_game == null) return;
        if (!other.CompareTag("Player")) return; //공 태그(Player)만 허용

        _game.TrySpawnNextWaveFromZone(); //대기중이면 다음 웨이브 소환
    }
}
