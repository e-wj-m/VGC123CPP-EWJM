using UnityEngine;

public class KillBoxScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject CheckPoint1;
    public GameObject CheckPoint2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (gameObject.name == "DeathZone1")
            {
                Player.transform.position = CheckPoint1.transform.position;
            }
            else if (gameObject.name == "DeathZone2")
            {
                Player.transform.position = CheckPoint2.transform.position;
            }
        }
    }
}