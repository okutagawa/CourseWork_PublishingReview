using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;

namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public class UserLogic : BaseLogic, IUserLogic
    {
        private readonly ILogger<UserLogic> _logger;
        private readonly IUserStorage _storage;
        private readonly IPasswordHasher<UserBindingModel> _passwordHasher;

        public UserLogic(ILogger<UserLogic> logger, IUserStorage storage, IPasswordHasher<UserBindingModel> passwordHasher)
        {
            _logger = logger;
            _storage = storage;
            _passwordHasher = passwordHasher;
        }

        public bool Create(UserBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            ValidateUserModel(model);

            // проверка уникальности по Email/Username
            var exists = _storage.GetFilteredList(new PublishingReviewContracts.SearchModels.UserSearchModel { Email = model.Email });
            if (exists != null && exists.Count > 0)
                throw new InvalidOperationException("User with this email already exists");

            model.PasswordHash = _passwordHasher.HashPassword(model, model.Password);
            model.Password = string.Empty;

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(UserBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var existing = _storage.GetElement(new PublishingReviewContracts.SearchModels.UserSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("User not found");

            if (!string.IsNullOrEmpty(model.Password))
            {
                model.PasswordHash = _passwordHasher.HashPassword(model, model.Password);
                model.Password = string.Empty;
            }

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(UserBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        public PublishingReviewContracts.ViewModels.UserViewModel? ReadElement(PublishingReviewContracts.SearchModels.UserSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<PublishingReviewContracts.ViewModels.UserViewModel>? ReadList(PublishingReviewContracts.SearchModels.UserSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }

        private void ValidateUserModel(UserBindingModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName)) throw new ArgumentException("FullName is required");
            if (string.IsNullOrWhiteSpace(model.Email) || !Regex.IsMatch(model.Email, @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"))
                throw new ArgumentException("Invalid email", nameof(model.Email));
            if (string.IsNullOrEmpty(model.Password) || !Regex.IsMatch(model.Password, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$"))
                throw new ArgumentException("Password must be at least 8 chars, include upper/lower/digit/special", nameof(model.Password));
        }
    }
}
