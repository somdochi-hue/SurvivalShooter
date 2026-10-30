using UnityEngine;

[CreateAssetMenu(fileName = "MosterData", menuName = "Scriptable Objects/MosterData")]
public class MonsterData : ScriptableObject
{
    public float maxHp = 500f;
    public float speed = 3f;
    public float damage = 50f;
}
