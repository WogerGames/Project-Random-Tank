using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class EventsHolder
{

    public class PlayerSpawned : UnityEvent<Player> { }

    public static PlayerSpawned playerSpawnedMine = new PlayerSpawned();

    //-----------------------------------------------------------------------

    public class EnemyDestroyed : UnityEvent<Transform, int> { }

    public static EnemyDestroyed enemyDestroyed = new EnemyDestroyed();

    //-----------------------------------------------------------------------

    public class PlayerSpawnedAny : UnityEvent<Player> { }

    public static PlayerSpawnedAny playerSpawnedAny = new PlayerSpawnedAny();

    //-----------------------------------------------------------------------

    public class LeftJoystickMoved : UnityEvent<Vector2> { }

    public static LeftJoystickMoved leftJoystickMoved = new LeftJoystickMoved();

    //-----------------------------------------------------------------------

    //public class WeaponPicked : UnityEvent<Player, Weapon> { }

    //public static WeaponPicked weaponPicked = new WeaponPicked();

    ////-----------------------------------------------------------------------

    //public class WeaponThrowed : UnityEvent<Player, Weapon> { }

    //public static WeaponThrowed weaponThrowed = new WeaponThrowed();

    ////-----------------------------------------------------------------------

    //public class WeaponTriggered : UnityEvent<Player, Weapon> { }

    //public static WeaponTriggered weaponTriggered = new WeaponTriggered();

    ////-----------------------------------------------------------------------

    //public class WeaponTriggerExit : UnityEvent<Player, Weapon> { }

    //public static WeaponTriggerExit weaponTriggerExit = new WeaponTriggerExit();

    //-----------------------------------------------------------------------

}
