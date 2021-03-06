﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor
{
    public static void Attack(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        string action_id = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Action action = Engine.CombatManager.ActionTable[action_id];
        Timeline t = ActionPatterns.Start(action, false);
        ActionPatterns.UseWeapon(t, caster, targetTile);

        targetUnit = ActionPatterns.TargetUnit(targetTile);
        if (targetUnit)
        {
            bool critical = Action.GetCritical();
            bool hitSuccess = Action.HitSuccess(caster, targetUnit, 100);
            int damage = Action.Mod2(caster.PA, critical, Element.None, caster, targetUnit, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (hitSuccess)
            {
                ActionPatterns.HitDamage(t, caster, targetUnit, damage);
            }
            else
            {
                ActionPatterns.HitAvoid(t, caster, targetUnit);
            }
        }

        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }
}
