using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandWeapon : MonoBehaviour
{
    //This can be changed to a more accurate location (e.g. the location of the wand)
    //Might need to change the direction if this is done so as to go to the center of the screen
    public GameObject target;

    //Projectile prefab must have the projectile script, a collider and a rigid body.
    public GameObject projectilePrefab;

    public int weaponDamage;
    public float fireRate;
    public float range;
    public float projectileSpeed;

    private Vector3 projectileDestination;
    private float timeToFire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            FireProjectile();
        }
    }

    //Fire the projectile
    void FireProjectile()
    {
        //Pick a destination using the range of the weapon.
        projectileDestination = target.transform.position + (target.transform.forward * range);

        //Instantiate the projectile.
        GameObject projectileObject = Instantiate(projectilePrefab, target.transform.position, Quaternion.identity);

        //Set the damage of the projectile. Set the speed and direction too.
        Projectile projecltileScipt = projectileObject.GetComponent<Projectile>();
        projecltileScipt.SetDamage(weaponDamage);
        projecltileScipt.SetRange(range);

        projectileObject.GetComponent<Rigidbody>().velocity = (projectileDestination - target.transform.position).normalized * projectileSpeed;
    }
}
