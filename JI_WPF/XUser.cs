
namespace JI_WPF
{
    public class XUserMy
    {
        public string Name { get; set; }
        public string OU { get; set; }
        public string Description { get; set; }
        public string Email
        {
            get {
                return "333333";
            }
            set { }
        }
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
