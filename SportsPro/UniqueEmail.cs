using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SportsPro.Models;

namespace SportsPro
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        private SportsProContext context { get; set; }

       
        public UniqueEmailAttribute(SportsProContext ctx)
        {
            context = ctx;
        }
        protected override ValidationResult IsValid(object value, ValidationContext ctx)
            {

            if (value is string)
            {
                string emailToCheck = value.ToString();
                List<string> allEmails = context.Customers.Select(c => c.Email).ToList();

                if (allEmails.FirstOrDefault(e => e == emailToCheck) is null)
                {
                    return ValidationResult.Success;
                }
            }
               
                string msg = base.ErrorMessage ?? $"{ctx.DisplayName} must be a unique email."; 
                return new ValidationResult(msg);
            }
        

    }
}
