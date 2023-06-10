﻿using Car_Rential.Entieties;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Car_Rential.Model.Validators
{
    public class UpdateCustomerValidator : AbstractValidator<InputCustomerDto>
    {
        public UpdateCustomerValidator(RentalDbContext dbContext)
        {
            RuleFor(c => c.FirstName)
                .Matches("^[a-zA-Z]+$")
                .WithMessage("FirstName must not contain white spaces")
                .MaximumLength(30);

            RuleFor(c => c.LastName)
                .Matches("^[a-zA-Z]+$")
                .WithMessage("LastName must not contain white spaces")
                .MaximumLength(30);

            RuleFor(c => c.PhoneNumber)
                .Matches(@"^[\s-]?\d{3}[\s-]?\d{3}[\s-]?\d{3}$")
                .Custom(
                    (value, contex) =>
                    {
                        var result = dbContext.Custormers.Any(c => c.PhoneNumber == value);
                        if (result)
                        {
                            contex.AddFailure("PhoneNumber", "PhoneNumber must be uniqe");
                        }
                    }
                );

            RuleFor(c => c.Email)
                .Matches(@"^[^\s]+$")
                .WithMessage("Email must not contain white spaces")
                .EmailAddress()
                .Custom(
                    (value, contex) =>
                    {
                        var result = dbContext.Custormers.Any(c => c.Email == value);
                        if (result)
                        {
                            contex.AddFailure("Email", "Email must be uniqe");
                        }
                    }
                );

            RuleFor(c => c.Pesel)
                .Matches(@"^(\d{11})$")
                .WithMessage("Pesel must be 11 digits")
                .Custom(
                    (value, contex) =>
                    {
                        var result = dbContext.Custormers.Any(c => c.Pesel == value);
                        if (result)
                        {
                            contex.AddFailure("Pesel", "Pesel must be uniqe");
                        }
                    }
                );

            RuleFor(c => c.Password)
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                .WithMessage(
                    "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character [@$!%*?&], and be at least 8 characters long."
                );

            RuleFor(c => c.ConfirmPassword).Equal(p => p.Password);

            RuleFor(a => a.Country).MaximumLength(255);
            RuleFor(a => a.City).MaximumLength(255);
            RuleFor(a => a.StreetName).MaximumLength(255);
            RuleFor(a => a.BuildingNumber)
                .Matches(@"^[0-9]+[a-zA-Z]?$")
                .WithMessage("Building number can only contain digits and one optional letter");

            RuleFor(a => a.ZipCode)
                .Matches(@"^(\d{5})$")
                .WithMessage("ZipCode if format XXXXX where X is a digit");

            RuleFor(a => a.State).MaximumLength(255);
        }
    }
}
