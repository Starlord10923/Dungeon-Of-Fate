using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour
{
    public Transform owner;
    public Unit unit;
    public List<Transform> enemiesHit;
    void Start()
    {
        GetOwner();
    }

    public void GetOwner(){
        Transform parent = transform;
        while(!(parent.CompareTag("Player") || parent.CompareTag("Enemy"))){
            parent = parent.parent;
        }
        if(parent.CompareTag("Player") || parent.CompareTag("Enemy")){
            owner = parent;
            unit = owner.GetComponent<Unit>();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if(otherUnit!=null){
            if(unit.unitFaction!=otherUnit.unitFaction && !enemiesHit.Contains(other.transform)){
                otherUnit.GetHit(owner,unit.attackPoints);
                StartCoroutine(EnemiesHitCooldown(other.transform));
            }
        }
    }
    IEnumerator EnemiesHitCooldown(Transform enemy){
        float cooldownTime = 0.6f;
        enemiesHit.Add(enemy);
        yield return new WaitForSeconds(cooldownTime);
        enemiesHit.Remove(enemy);
    }
}
