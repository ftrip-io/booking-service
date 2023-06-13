using AutoMapper;
using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewHost
{
    public class ReviewHostRequest : IRequest<HostReview>
    {
        public Guid GuestId { get; set; }
        public Guid HostId { get; set; }

        public int CommunicationGrade { get; set; }
        public int OverallGrade { get; set; }

        public string RecensionText { get; set; }
    }

    public class ReviewHostMappingConfiguration : Profile
    {
        public ReviewHostMappingConfiguration()
        {
            CreateMap<ReviewHostRequest, HostReview>()
                .ForPath(r => r.Grades.Communication, m => m.MapFrom(s => s.CommunicationGrade))
                .ForPath(r => r.Grades.Overall, m => m.MapFrom(s => s.OverallGrade))
                .ForPath(r => r.Recension.Text, m => m.MapFrom(s => s.RecensionText));
        }
    }
}