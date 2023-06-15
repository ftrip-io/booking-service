using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.contracts.ReservationRequests
{
    public class PriceInfo
    {
        public decimal TotalPrice { get; set; }
        public List<string> Problems { get; set; }
        public int Days { get; set; }
        public Guid AccommodationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }

        public PriceInfo()
        {
            Problems = new List<string>();
        }
    }

}
