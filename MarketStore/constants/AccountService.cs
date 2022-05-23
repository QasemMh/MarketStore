using MarketStore.Models;
using MarketStore.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketStore.constants
{
    public class AccountService
    {

        public bool ValidationRegister(RegisterUserViewModel input)
        {
            Regex usernameRegex = new Regex(@"^(?!.*\.\.)(?!.*\.$)[^\W][\w.]{2,60}$", RegexOptions.IgnoreCase);


            if (HasEmptyFields(input))
                return false;
            if (!usernameRegex.IsMatch(input.Username))
                return false;
            if (!new EmailAddressAttribute().IsValid(input.Email))
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
