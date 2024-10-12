using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public Enemy enemyPrefab; // Prefab for enemy
    public Vector3 startingPosition; // Starting position of the enemy
    public float normalStepTime = 0.025f; // Time between each step
    public float slowStepTime = 0.06f; // Time between each step when the enemy is slowed down
    public bool isLandEnemy; // Flag to check if the enemy is a land enemy
    public bool isDestroyer; // Flag to check if the enemy is a destroyer
    public int damage; // Flag to check if the enemy can damage the land
}