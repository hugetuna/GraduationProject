using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/Vigour Heal")]
public class VigourHealEffect : ItemEffect
{
    public int healAmount;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        int before = target.vigour;
        target.vigour = Mathf.Min(target.vigour + healAmount, target.vigourMax);
        Debug.Log($"[��O��_] �q {before} �� {target.vigour}");
    }
}
