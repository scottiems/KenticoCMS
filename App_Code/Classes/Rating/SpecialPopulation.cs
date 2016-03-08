using System;

namespace Classes.Rating
{
    public class SpecialPopulation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}