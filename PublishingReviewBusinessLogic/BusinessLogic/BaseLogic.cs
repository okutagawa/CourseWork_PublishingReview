using System;

namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public abstract class BaseLogic
    {
        protected void EnsureNotNull(object? obj, string paramName)
        {
            if (obj == null) throw new ArgumentNullException(paramName);
        }

        protected void EnsureIdValid(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive", nameof(id));
        }
    }
}
