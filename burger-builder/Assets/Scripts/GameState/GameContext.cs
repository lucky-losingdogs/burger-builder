//<summary>
// has references to managers for perks to access to activate their effects
//</summary>
public class GameContext
{
    public TicketManager m_ticketManager { get; }
    public ComboManager m_comboManager { get; }

    //provides references of managers to the perk logic
    public GameContext(TicketManager ticketManager, ComboManager comboManager)
    {
        m_ticketManager = ticketManager;
        m_comboManager = comboManager;
    }

    //getters
    public TicketManager GetTicketManager() => m_ticketManager;
    public ComboManager GetComboManager() => m_comboManager;
}