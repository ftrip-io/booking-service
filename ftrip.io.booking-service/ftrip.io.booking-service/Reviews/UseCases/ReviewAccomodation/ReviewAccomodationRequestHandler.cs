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
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewAccomodation
{
    public class ReviewAccomodationRequestHandler : IRequestHandler<ReviewAccomodationRequest, AccomodationReview>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ReviewAccomodationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationQueryHelper accommodationQueryHelper,
            IAccomodationReviewRepository accomodationReviewRepository,
            IReservationRepository reservationRepository,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            IMapper mapper,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accommodationQueryHelper = accommodationQueryHelper;
            _accomodationReviewRepository = accomodationReviewRepository;
            _reservationRepository = reservationRepository;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AccomodationReview> Handle(ReviewAccomodationRequest request, CancellationToken cancellationToken)
        {
            var review = _mapper.Map<AccomodationReview>(request);

            await Validate(review, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var createdReview = await Create(review, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishAccomodationReviewedEvent(review, cancellationToken);

            return createdReview;
        }

        private async Task Validate(AccomodationReview review, CancellationToken cancellationToken)
        {
            var guestId = review.GuestId;
            var accomodationId = review.AccomodationId;

            if (await _accomodationReviewRepository.IsReviewByGuestForAccomodation(guestId, accomodationId, cancellationToken))
            {
                _logger.Error("Accommodation already reviewed - GuestId[{GuestId}], AccommodationId[{AccommodationId}]", guestId, accomodationId);
                throw new BadLogicException(_stringManager.Format("Reviews_ForAccomodationAlreadyPlaced", guestId, accomodationId));
            }

            if (!await _reservationRepository.HasGuestReservedAccomodationInPast(guestId, accomodationId, cancellationToken))
            {
                _logger.Error("Accommodation cannot be reviewed because guest never reserved it - GuestId[{GuestId}], AccommodationId[{AccommodationId}]", guestId, accomodationId);
                throw new BadLogicException(_stringManager.Format("Reviews_GuestNeverReservedAccomodation", guestId, accomodationId));
            }
        }

        private async Task<AccomodationReview> Create(AccomodationReview review, CancellationToken cancellationToken)
        {
            var createdReview = await _accomodationReviewRepository.Create(review, cancellationToken);

            _logger.Information(
                "Accommodation Review created - ReviewId[{ReviewId}], GuestId[{GuestId}], AccommodationId[{AccommodationId}]",
                createdReview.Id, createdReview.GuestId, createdReview.AccomodationId
            );

            return createdReview;
        }

        private async Task PublishAccomodationReviewedEvent(AccomodationReview review, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationQueryHelper.ReadOrThrow(review.AccomodationId, cancellationToken);
            var accomodationReviewed = new AccomodationReviewedEvent()
            {
                AccomodationId = review.AccomodationId,
                HostId = accommodation.HostId,
                GuestId = review.GuestId,
                AverageGrade = review.Grades.Average
            };

            await _messagePublisher.Send<AccomodationReviewedEvent, string>(accomodationReviewed, cancellationToken);
        }
    }
}