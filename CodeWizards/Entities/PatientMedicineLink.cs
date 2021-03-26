namespace CodeWizards.Entities
{
    public class PatientMedicineLink
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int MedicineId { get; set; }

        public Patient Patient { get; set; }
        public Medicine Medicine { get; set; }
    }
}
