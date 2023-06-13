using AutoMapper;
using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewAccomodation
{
    public class ReviewAccomodationRequest : IRequest<AccomodationReview>
    {
        public Guid AccomodationId { get; set; }
        public Guid GuestId { get; set; }

        public int AccomodationGrade { get; set; }
        public int LocationGrade { get; set; }
        public int ValueForMoneyGrade { get; set; }

        public string RecensionText { get; set; }
    }

    public class ReviewAccomodationMappingConfiguration : Profile
    {
        public ReviewAccomodationMappingConfiguration()
        {
            CreateMap<ReviewAccomodationRequest, AccomodationReview>()
                .ForPath(r => r.Grades.Accomodation, m => m.MapFrom(s => s.AccomodationGrade))
                .ForPath(r => r.Grades.Location, m => m.MapFrom(s => s.LocationGrade))
                .ForPath(r => r.Grades.ValueForMoney, m => m.MapFrom(s => s.ValueForMoneyGrade))
                .ForPath(r => r.Recension.Text, m => m.MapFrom(s => s.RecensionText));
        }
    }
}