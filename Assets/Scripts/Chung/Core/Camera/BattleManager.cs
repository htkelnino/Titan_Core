using Assets.Scripts.Dat;
using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] characterPrefabs;

    [SerializeField] private Transform spawnP1;
    [SerializeField] private Transform spawnP2;
    private GameObject player1;
    private GameObject player2;

    private void Start()
    {
        SpawnPlayers();
        
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (player1 != null && player2 != null)
        {
            cameraFollow.SetTargets(player1.transform, player2.transform);
        }
        
    }

    private void SpawnPlayers()
    {
        GameObject prefab1 = characterPrefabs[(int)CharacterSelection.SelectedCharacter1];
        GameObject prefab2 = characterPrefabs[(int)CharacterSelection.SelectedCharacter2];

        player1 = Instantiate(
            prefab1,
            spawnP1.position,
            spawnP1.rotation
        );

        player2 = Instantiate(
            prefab2,
            spawnP2.position,
            spawnP2.rotation
        );
    }
}
