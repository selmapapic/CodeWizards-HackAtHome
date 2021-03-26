namespace CodeWizards.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public bool Served { get; set; }
        public bool Emergency { get; set; }
    }
}
