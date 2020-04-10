namespace Ji.Droid
{
    interface IChatObject
    {
        string Text { get; set; }
        int ObjId { get; set; }
        int TypeId { get; set; }
        Period period { get; set; }

    }
}