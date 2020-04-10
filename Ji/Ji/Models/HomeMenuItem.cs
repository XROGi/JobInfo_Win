namespace Ji.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        Contacts,
        ConnectServer,
        MessageList,
        ParkingPass,
        UpdateServerPage
            , SetupPage
            , DebugPage
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
