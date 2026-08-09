using UnityEngine;

namespace Logic
{
    //abstract parent class
    public abstract class PerkLogic
    {
        public abstract void Effect(GameContext context);
    }

    //clears all the tickets currently in the queue
    public class ClearAllTickets : PerkLogic
    {
        public override void Effect(GameContext context)
        {
            Debug.Log("ClearAllTickets perk effect");
            context.m_ticketManager.ClearTicketQueue();
        }
    }
    
    public class ClearAllOfType : PerkLogic
    {
        public override void Effect(GameContext context)
        {
            Debug.Log("ClearAllOfType perk effect");
        }
    }
}