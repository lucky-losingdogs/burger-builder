using System.Collections;
using UnityEngine;

namespace Logic
{
    //abstract parent class
    public abstract class PerkLogic
    {
        public abstract void Effect(GameContext context, float duration, float value);
    }

    //clears all the tickets currently in the queue
    public class ClearAllTickets : PerkLogic
    {
        public override void Effect(GameContext context, float duration, float value)
        {
            Debug.Log("ClearAllTickets perk effect");
            context.m_ticketManager.ClearTicketQueue();
        }
    }
    
    //clears all tickets of the same type as the previously cleared ticket
    //(the ticket that was cleared to increase the combo and trigger the perk)
    public class ClearAllOfType : PerkLogic
    {
        public override void Effect(GameContext context, float duration, float value)
        {
            Debug.Log("ClearAllOfType perk effect");
            TicketManager ticketManager = context.m_ticketManager;

            //make sure the previous ticket isn't null/not set yet
            TicketData prevTicket = ticketManager.GetPreviousCurrentTicket();
            if (prevTicket == null)
                prevTicket = ticketManager.GetCurrentTicket();
            ticketManager.ClearByTicketType(prevTicket);
        }
    }

    //decrease the amount that the combo drains when not maintained
    public class DecreaseComboDecrement : PerkLogic
    {
        public override void Effect(GameContext context, float duration, float value)
        {
            Debug.Log("DecreaseComboDecrement perk effect");
            context.m_comboManager.DecreaseDecrement(duration, value);
        }
    }
    
    //increase the time limit between clearing tickets before the combo starts to drain
    public class IncreaseComboLimit : PerkLogic
    {
        public override void Effect(GameContext context, float duration, float value)
        {
            Debug.Log("IncreaseComboLimit perk effect");
            context.m_comboManager.IncreaseComboTimeLimit(duration, value);
        }
    }
    
    public class ComboMultiplier : PerkLogic
    {
        public override void Effect(GameContext context, float duration, float value)
        {
            Debug.Log("ComboMultiplier perk effect");
            context.m_comboManager.IncreaseMultiplier(duration, value);
        }
    }
    
    public class LowerItemSpawnLimit : PerkLogic
    {
        public override void Effect(GameContext context, float duration, float value)
        {
            
        }
    }
}