
using System.Collections.Generic;

namespace Classes.Rating
{
    public class ReviewsCountAverage
    {
        public int NodeId { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public List<Review> Reviews { get; set; }
    }
}