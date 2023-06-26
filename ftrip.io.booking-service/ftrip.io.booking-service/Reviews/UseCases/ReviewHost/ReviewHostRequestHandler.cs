using AutoMapper;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reservations;
using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewHost
{
    public class ReviewHostRequestHandler : IRequestHandler<ReviewHostRequest, HostReview>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostReviewRepository _hostReviewRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ReviewHostRequestHandler(
            IUnitOfWork unitOfWork,
            IHostReviewRepository hostReviewRepository,
            IAccommodationRepository accommodationRepository,
            IReservationRepository reservationRepository,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            IMapper mapper,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _hostReviewRepository = hostReviewRepository;
            _accommodationRepository = accommodationRepository;
            _reservationRepository = reservationRepository;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<HostReview> Handle(ReviewHostRequest request, CancellationToken cancellationToken)
        {
            var review = _mapper.Map<HostReview>(request);

            await Validate(review, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var createdReview = await Create(review, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishHostReviewedEvent(createdReview, cancellationToken);

            return createdReview;
        }

        private async Task Validate(HostReview review, CancellationToken cancellationToken)
        {
            var guestId = review.GuestId;
            var hostId = review.HostId;

            if (await _hostReviewRepository.IsHostReviewByGuest(guestId, hostId, cancellationToken))
            {
                _logger.Error("Host already reviewed - GuestId[{GuestId}], HostId[{HostId}]", guestId, hostId);
                throw new BadLogicException(_stringManager.Format("Reviews_ForHostAlreadyPlaced", guestId, hostId));
            }

            var possibleAccomodationIds = (await _accommodationRepository.ReadByHostId(hostId)).ToList().Select(a => a.AccommodationId);
            if (!await _reservationRepository.HasGuestReservedAnyOfAccomodationsInPast(guestId, possibleAccomodationIds, cancellationToken))
            {
                _logger.Error("Host can not be reviewed because the guest had never reserved any of his accommodations - GuestId[{GuestId}], HostId[{HostId}]", guestId, hostId);
                throw new BadLogicException(_stringManager.Format("Reviews_GuestNeverReservedAnyAccomodationOfHost", guestId, hostId));
            }
        }

        private async Task<HostReview> Create(HostReview review, CancellationToken cancellationToken)
        {
            var createdReview = await _hostReviewRepository.Create(review, cancellationToken);

            _logger.Information(
                "Host Review created - ReviewId[{ReviewId}], GuestId[{GuestId}], HostId[{HostId}]",
                createdReview.Id, createdReview.GuestId, createdReview.HostId
            );

            return createdReview;
        }

        private async Task PublishHostReviewedEvent(HostReview review, CancellationToken cancellationToken)
        {
            var hostReviewed = new HostReviewedEvent()
            {
                HostId = review.HostId,
                GuestId = review.GuestId,
                AverageGrade = review.Grades.Average
            };

            await _messagePublisher.Send<HostReviewedEvent, string>(hostReviewed, cancellationToken);
        }
    }
}