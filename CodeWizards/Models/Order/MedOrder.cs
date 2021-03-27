namespace CodeWizards.Models.Order
{
    public class MedOrder
    {
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool Emergency { get; set; }
        public int[] Medicine { get; set; }
    }
}
