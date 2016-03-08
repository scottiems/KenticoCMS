
using System;
using System.Collections.Generic;
using CMS.TranslationServices.TranslationsComDocumentService;

namespace Classes.Rating
{
    public class Review
    {
        public int ID { get; set; }
        public string Comment { get; set; }
        public int Age { get; set; }
        public string Grade { get; set; }
        public string RoleName { get; set; }
        public decimal Rating { get; set; }
        public List<int> SpecialPopulationIDs { get; set; }
        public int NodeID { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }


    }
}