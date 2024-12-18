using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinAnimListener : MonoBehaviour
{
    private void GoblinAttack()
    {
        this.transform.parent.GetComponent<Monster>().BasicAttack();
    }
    private void GoblinDeath()
    {
        this.transform.parent.GetComponent<Monster>().ReturnToPool();
    }
}
