using MarketStore.Models;
using MarketStore.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketStore.constants
{
    public class AccountService
    {


        public bool IsValidUsername(string username)
        {
            Regex usernameRegex = new Regex(@"^(?!.*\.\.)(?!.*\.$)[^\W][\w.]{2,60}$", RegexOptions.IgnoreCase);
            return usernameRegex.IsMatch(username);
        }
        public bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }
        public bool IsValidPhone(string phone)
        {
            var isNumeric = int.TryParse("123", out _);

            if (!isNumeric) return false;

            Regex phoneRegex = new Regex(@"^\d{10}$");
            return phoneRegex.IsMatch(phone);
        }

        public bool ValidationRegister(RegisterUserViewModel input)
        {


            if (HasEmptyFields(input))
                return false;
            if (!IsValidUsername(input.Username))
                return false;
            if (!IsValidEmail(input.Email))
                return false;
            if (input.Password.Length < 6 || !input.Password.Equals(input.ConfirmPassword))
                return false;



            return true;
        }


        private bool HasEmptyFields(RegisterUserViewModel inputs)
        {
            return (
              String.IsNullOrEmpty(inputs.FirstName) || String.IsNullOrEmpty(inputs.LastName) ||
             String.IsNullOrEmpty(inputs.Username) || String.IsNullOrEmpty(inputs.Password) ||
             String.IsNullOrEmpty(inputs.ConfirmPassword) || String.IsNullOrEmpty(inputs.Email)
              );


        }
    }
}
