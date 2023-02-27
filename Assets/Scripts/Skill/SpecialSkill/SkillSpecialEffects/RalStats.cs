using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialEffect", menuName = "SpecialEffect/Create new StatsChangeSpecialEffect")]

public class RalStats : SpecialEffect
{
    [SerializeField] GameObject statsChangeObjPrefab;
    GameObject statsChangeObj;
    StatsChange statsChange;
    public override void Init(GameObject target)
    {
        statsChangeObj = Instantiate(statsChangeObjPrefab);
        statsChange = statsChangeObj.GetComponent<StatsChange>();
        statsChange.AttachEffects(target.GetComponent<PersonnageScript>());
    }
}
