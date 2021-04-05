using UnityEngine;

public class BarricadeTrap : MonoBehaviour
{
    [SerializeField]
    private int health;
    private GenericTrap trap;

    private void Awake()
    {
        trap = GetComponent<GenericTrap>();
    }


}
