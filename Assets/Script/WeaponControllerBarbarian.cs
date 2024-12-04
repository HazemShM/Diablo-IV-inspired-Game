using UnityEngine;

public class WeaponControllerBarbarian : MonoBehaviour
{
    public BarbarianAnimation barbarian;
    public GameObject hitParticle;

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Enemy" && !barbarian.canAttack){
            Debug.Log(other.name);
            GameObject particleInstance = Instantiate(hitParticle, new Vector3(other.transform.position.x,
            transform.position.y, other.transform.position.z),
            other.transform.rotation);
            Destroy(particleInstance, 2.0f);
        }
    }

}