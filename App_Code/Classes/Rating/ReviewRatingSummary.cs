using System.Collections.Generic;

namespace Classes.Rating
{
    public class ReviewRatingSummary
    {
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public IDictionary<decimal, int> RatingSummary { get; set; }
    }
}
