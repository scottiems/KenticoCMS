using System.Collections.Generic;

namespace Enrich
{
    public sealed class EnrichRequest
    {
        public Student Student { get; set; }
        public string ItemDefID { get; set; }
        public string ProgramVariantID { get; set; }
        public string ReasonId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string OutcomeID { get; set; }
        public List<CustomFormSection> CustomFormSections { get; set; }
        public string AuditLogMessage { get; set; }
    }
}
