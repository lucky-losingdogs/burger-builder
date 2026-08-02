using UnityEngine;

namespace Logic
{
    //abstract parent class
    public abstract class PerkLogic
    {
        public abstract void Effect();
    }

    //clears all the tickets currently in the queue
    public class ClearAllTickets : PerkLogic
    {
        public override void Effect()
        {
            Debug.Log("perk effect");
        }
    }
}