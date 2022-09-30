using UnityEngine;

public class ObstaclePower : MonoBehaviour
{
    public GameObject Obstacle;
    private float randomPower;
    private void Start()
    {
        randomPower = Random.Range(0, 40);
        if (randomPower <=10)
        {
            Obstacle.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else if (randomPower > 10 && randomPower <=20)
        {
            Obstacle.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        else if (randomPower > 20 && randomPower <= 30)
        {
            Obstacle.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if (randomPower > 30)
        {
            Obstacle.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}
