using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.ViewModels;


namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public class EmployeeLogic : BaseLogic, IEmployeeLogic
    {
        private readonly ILogger<EmployeeLogic> _logger;
        private readonly IEmployeeStorage _storage;
        private readonly IPasswordHasher<EmployeeBindingModel> _passwordHasher;

        public EmployeeLogic(ILogger<EmployeeLogic> logger, IEmployeeStorage storage, IPasswordHasher<EmployeeBindingModel> passwordHasher)
        {
            _logger = logger;
            _storage = storage;
            _passwordHasher = passwordHasher;
        }

        public bool Create(EmployeeBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            ValidateEmployeeModel(model);

            var exists = _storage.GetFilteredList(new EmployeeSearchModel { Email = model.Email });
            if (exists != null && exists.Count > 0)
                throw new InvalidOperationException("Employee with this email already exists");

            // Хешируем пароль и очищаем plain-поле
            model.Password = model.Password ?? string.Empty;
            model.PasswordHash = _passwordHasher.HashPassword(model, model.Password);
            model.Password = string.Empty;

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(EmployeeBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var existing = _storage.GetElement(new EmployeeSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("Employee not found");

            if (!string.IsNullOrEmpty(model.Password))
            {
                model.PasswordHash = _passwordHasher.HashPassword(model, model.Password);
                model.Password = string.Empty;
            }

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(EmployeeBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        // Реализация интерфейсных методов — однозначные сигнатуры
        public EmployeeViewModel? ReadElement(EmployeeSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<EmployeeViewModel>? ReadList(EmployeeSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }

        private void ValidateEmployeeModel(EmployeeBindingModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName)) throw new ArgumentException("FullName is required");
            if (string.IsNullOrWhiteSpace(model.Email) || !Regex.IsMatch(model.Email, @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"))
                throw new ArgumentException("Invalid email", nameof(model.Email));
            if (string.IsNullOrEmpty(model.Password) || !Regex.IsMatch(model.Password, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$"))
                throw new ArgumentException("Password must be at least 8 chars, include upper/lower/digit/special", nameof(model.Password));
        }
    }
}
