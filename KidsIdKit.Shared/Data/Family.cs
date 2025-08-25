
namespace KidsIdKit.Data
{
    public class Family
    {
        public Family()
        {
            Children = new List<Child>();
        }

        public List<Child> Children { get; set; }
    }
}
