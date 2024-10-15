using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int level;
    public int health;
    public int attack;

    public int XP;
    public int gold;

    public new string name;

    private string[] firstNames = 
    {
        "Azmuth",
        "Arnold",
        "Ignis",
        "ArgonRog",
        "Alever",
        "Brick",
        "Frag",
        "Triston",
        "Almor",
        "Crithtik",
        "Mordur",
        "Orith",
        "Death",
        "Petricia",
        "Grim",
        "Alixer",
        "Arion"

    };

    private string[] lastNames = 
    {
        "BrickBreaker",
        "Thorston",
        "Pheonix",
        "MurderMan",
        "Broughston",
        "Roughson",
        "Reaper",
        "Clabberstonk",
        "Peters",
        "Effegy",
        "RockStopper",
        "Algemesh",
        "Larsnach"
    };

/// <summary>
/// resets all values according to the given enemy level
/// </summary>
/// <param name="level"></param>
    public void makeNewEnemy(int level)
    {
        this.level = level;

        //health starts at 10 at level 1 increases by 5 each level
        health = 5 + level*5;

        //attack starts at 3 increases by 1 each level
        attack = 2 + level * 1;

        //Xp starts at 7, increases by 4 each level
        XP = 3 + level * 4;

        //gold is random but influenced by level to give better rewards
        gold = Random.Range(0, 10) + level*2;

        name = GenerateName();
    }

    public string GenerateName()
    {
        string firstname = firstNames[Random.Range(0, firstNames.Length)];
        string lastname = lastNames[Random.Range(0, lastNames.Length)];
        return firstname + " " + lastname;
    }

}
