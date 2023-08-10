using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialEffect", menuName = "SpecialEffect/Create new BleedingSpecialEffect")]
public class BleedingEffect : SpecialEffect
{
    [SerializeField] GameObject bleedingBuffObjPrefab;
    GameObject bleedingBuffObj;
    BleedingBuff bleedingBuffChange;
    public override void Init(GameObject target)
    {
        bleedingBuffObj = Instantiate(bleedingBuffObjPrefab);
        bleedingBuffChange = bleedingBuffObj.GetComponent<BleedingBuff>();
        bleedingBuffChange.AttachEffects(target.GetComponent<PersonnageScript>());
        bleedingBuffChange.casterScript = CombatManager.Instance.ActualPlayerScript;
        bleedingBuffChange.casterAtk = CombatManager.Instance.ActualPlayerScript.actualAtk;
        bleedingBuffChange.casterSpeAtk = CombatManager.Instance.ActualPlayerScript.actualSpeAtk;
    }
}
