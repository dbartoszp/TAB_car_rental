﻿using AutoMapper;
using Car_Rential.Authentication;
using Car_Rential.Authorization;
using Car_Rential.Entieties;
using Car_Rential.Exceptions;
using Car_Rential.Interfaces;
using Car_Rential.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Car_Rential.Services
{
    public class CustomersService : ICustomersService
    {
        private readonly RentialDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Customer> _passwordHasher;
        private readonly IAuthorizationService _authorizationHandler;
        private readonly ICustomerContextService _customerContextService;
        private readonly AuthenticationSettings _authenticationSettings;

        public CustomersService(
            RentialDbContext context,
            IMapper mapper,
            IPasswordHasher<Customer> passwordHasher,
            AuthenticationSettings authenticationSettings,
            IAuthorizationService authorizationHandler,
            ICustomerContextService customerContextService
        )
        {
            _dbContext = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _authorizationHandler = authorizationHandler;
            _customerContextService = customerContextService;
        }

        public void DeleteCustomer(int customerId)
        {
            var user = FindCustomer(customerId);

            _dbContext.Remove(user);
            _dbContext.SaveChanges();
        }

        public IEnumerable<ReturnCustomerDto> GetCustomers()
        {
            var customers = _dbContext.Custormers
                .Include(c => c.CustromerAddress)
                .Include(c => c.Reservations)
                .ToList();

            var result = _mapper.Map<List<ReturnCustomerDto>>(customers);

            return result;
        }

        public string LoginCustomer(LoginCustomerDto customerDto)
        {
            var user = _dbContext.Custormers.FirstOrDefault(c => c.Email == customerDto.Email);

            if (user == null)
            {
                throw new LoginFailException("Invalid username or password.");
            }

            var isPasswordCorrect = _passwordHasher.VerifyHashedPassword(
                user,
                user.HassedPassword,
                customerDto.Password
            );

            if (isPasswordCorrect == PasswordVerificationResult.Failed)
            {
                throw new LoginFailException("Invalid username or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey)
            );

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpiredDays);

            var token = new JwtSecurityToken(
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public int RegisterCustomer(InputCustomerDto customerDto)
        {
            var user = _mapper.Map<Customer>(customerDto);

            var userPassword = customerDto.Password;
            var userHassedPassword = _passwordHasher.HashPassword(user, userPassword);
            user.HassedPassword = userHassedPassword;

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            return user.Id;
        }

        public void UpdateCustomer(InputCustomerDto customerDto, int customerId)
        {
            var user = FindCustomer(customerId);

            var whoIsAsking = (int)(_customerContextService.GetUserId ?? 0);

            var authorizationResult = _authorizationHandler
                .AuthorizeAsync(
                    _customerContextService.GetCustomer,
                    user,
                    new OwnAccountActionRequirement(whoIsAsking)
                )
                .Result;

            if (!authorizationResult.Succeeded)
            {
                throw new AuthorizationFailException("You don't have access to this account");
            }

            if (customerDto.FirstName != null)
            {
                user.FirstName = customerDto.FirstName;
            }
            if (customerDto.LastName != null)
            {
                user.LastName = customerDto.LastName;
            }
            if (customerDto.Email != null)
            {
                user.Email = customerDto.Email;
            }
            if (customerDto.PhoneNumber != null)
            {
                user.PhoneNumber = customerDto.PhoneNumber;
            }
            if (customerDto.Pesel != null)
            {
                user.Pesel = customerDto.Pesel;
            }
            if (customerDto.Password != null)
            {
                var hassedPassword = _passwordHasher.HashPassword(user, customerDto.Password);
                user.HassedPassword = hassedPassword;
            }
            if (customerDto.Country != null)
            {
                user.CustromerAddress.Country = customerDto.Country;
            }
            if (customerDto.State != null)
            {
                user.CustromerAddress.State = customerDto.State;
            }
            if (customerDto.City != null)
            {
                user.CustromerAddress.City = customerDto.City;
            }
            if (customerDto.StreetName != null)
            {
                user.CustromerAddress.StreetName = customerDto.StreetName;
            }
            if (customerDto.BuildingNumber != null)
            {
                user.CustromerAddress.BuildingNumber = customerDto.BuildingNumber;
            }
            if (customerDto.ZipCode != null)
            {
                user.CustromerAddress.ZipCode = customerDto.ZipCode;
            }

            _dbContext.SaveChanges();
        }

        private Customer FindCustomer(int customerId)
        {
            var result = _dbContext.Custormers
                .Include(a => a.CustromerAddress)
                .FirstOrDefault(x => x.Id == customerId);

            if (result == null)
            {
                throw new CustomerNotFoundException("Account doesn't exist");
            }
            return result;
        }
    }
}
