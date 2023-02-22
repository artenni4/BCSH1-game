using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Constants.Enums
{
    internal enum AnimationState
    {
        None = 0,

        Idle,
        IdleLeft,
        IdleRight,
        IdleUp,
        IdleDown,

        Walk,
        WalkLeft,
        WalkRight,
        WalkUp,
        WalkDown,

        Attack,
        AttackLeft,
        AttackRight,
        AttackUp,
        AttackDown,

        Death,
        DeathLeft,
        DeathRight,
        DeathUp,
        DeathDown,

        Hurt,
        HurtLeft,
        HurtRight,
        HurtTop,
        HurtDown
    }
}
