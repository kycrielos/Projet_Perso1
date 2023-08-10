using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialEffect", menuName = "SpecialEffect/Create new GlyphSpecialEffect")]

public class CreateGlyph : SpecialEffect
{
    [SerializeField] GameObject glyphObjPrefab;
    GameObject glyphObj;

    Node glyphNode;
    public override void Init(GameObject target)
    {
        glyphNode = GridManager.Instance.NodeFromWorldPoint(target.transform.position);
        glyphObj = Instantiate(glyphObjPrefab, glyphNode.worldPosition, Quaternion.identity);
        glyphNode.glyhpScript = glyphObj.GetComponent<GlyphBase>();
        glyphObj.GetComponent<GlyphBase>().caster = CombatManager.Instance.ActualPlayerScript;
        CombatManager.Instance.ActualPlayerScript.possessedGlyph.Add(glyphObj.GetComponent<GlyphBase>());
    }
}
