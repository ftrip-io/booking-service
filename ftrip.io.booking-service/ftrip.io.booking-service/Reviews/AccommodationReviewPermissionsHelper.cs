﻿using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    public interface IAccommodationReviewPermissionsHelper
    {
        void CanBeWrittenByCurrentGuest(Guid guestId);

        Task IsWrittenByCurrentGuest(Guid reviewId, CancellationToken cancellationToken);
    }

    public class AccommodationReviewPermissionsHelper : IAccommodationReviewPermissionsHelper
    {
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;
        private readonly CurrentUserContext _currentUserContext;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public AccommodationReviewPermissionsHelper(
            IAccomodationReviewRepository accomodationReviewRepository,
            CurrentUserContext currentUserContext,
            IStringManager stringManager,
            ILogger logger)
        {
            _accomodationReviewRepository = accomodationReviewRepository;
            _currentUserContext = currentUserContext;
            _stringManager = stringManager;
            _logger = logger;
        }

        public void CanBeWrittenByCurrentGuest(Guid guestId)
        {
            var doingForHimself = guestId.ToString() == _currentUserContext.Id;
            if (!doingForHimself)
            {
                _logger.Error("Error while trying to execute action for other guest - GuestId[{GuestId}], ExecutingAsId[{ExecutingAsId}]", _currentUserContext.Id, guestId);
                throw new ForbiddenException(_stringManager.Format("Reviews_CannotExecuteForThatGuest", guestId));
            }
        }

        public async Task IsWrittenByCurrentGuest(Guid reviewId, CancellationToken cancellationToken)
        {
            var review = await _accomodationReviewRepository.Read(reviewId, cancellationToken);
            var isReviewWritter = review.GuestId.ToString() == _currentUserContext.Id;
            if (!isReviewWritter)
            {
                _logger.Error(
                    "Error while trying to execute action on someone else's review - ReviewId[{ReviewId}], WritterId[{WritterId}], GuestId[{GuestId}]",
                    reviewId, review.GuestId, _currentUserContext.Id
                );
                throw new ForbiddenException(_stringManager.Format("Reviews_CannotExecuteForThatReview", reviewId));
            }
        }
    }
}