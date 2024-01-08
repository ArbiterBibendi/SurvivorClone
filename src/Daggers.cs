using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public class Daggers : Ability
{
    public Daggers()
    {
        Damage = 25;
        AttackTime = 0;
        Cooldown = 2000;
        UpgradeFunctions = new List<Action>()
        {
            () =>
            {
                Cooldown = 1500;
            },
            () =>
            {
                Cooldown = 1500;
            },
            () => 
            {
                Cooldown = 1500;
            },
            () => 
            {
                Cooldown = 1500;
            }
        };
    }
}
