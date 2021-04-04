using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandWeapon : MonoBehaviour
{
    public GenericAnimator animator;

    //This can be changed to a more accurate location (e.g. the location of the wand)
    //Might need to change the direction if this is done so as to go to the center of the screen
    public Transform projectileSpawn;

    //Projectile prefab must have the projectile script, a collider and a rigid body.
    public GameObject projectilePrefab;

    public Transform cameraTransform;

    public LayerMask aimLayerMask;

    public int weaponDamage;
    public float fireRate;
    public float range;
    public float projectileSpeed;

    private float timeToFire;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            FireProjectile();
            animator.TriggerAttack(out _, out _);
        }
    }

    //Fire the projectile
    void FireProjectile()
    {
        //Pick a destination using the range of the weapon.
        //projectileDestination = projectileSpawn.transform.position + (projectileSpawn.transform.forward * range);

        //Instantiate the projectile.
        Vector3 spawnPosition = projectileSpawn.position;
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        //Set the damage of the projectile. Set the speed and direction too.
        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.SetDamage(weaponDamage);
        projectileScript.SetRange(range);

        // zeroPoint = where the crosshairs is aiming at.
        const float aimRaycastDistance = 100.0f;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 projectileForward;
        if (Physics.Raycast(new Ray(cameraTransform.position, cameraForward), out RaycastHit hit, aimRaycastDistance, aimLayerMask))
        {
            projectileForward = (hit.point - spawnPosition).normalized;
            if(Vector3.Dot(projectileForward, cameraForward) < 0)
            {
                // the projectile is pointing backwards!
                projectileForward = cameraForward;
            }
        }
        else
            projectileForward = cameraForward;

        projectileObject.GetComponent<Rigidbody>().velocity = projectileForward * projectileSpeed;
        projectileObject.transform.rotation = Quaternion.LookRotation(projectileForward);
    }
}
