using System;
using System.Collections.Generic;

namespace Template.Domain.Global
{
    public partial class Result
    {
        public bool IsSuccess { get; private set; }
        
        public HashSet<string> Errors { get; private set; } = new HashSet<string>();

        private Result(bool isSuccess, IEnumerable<string> errors = null)
        {
            IsSuccess = isSuccess;
            if (errors != null)
            {
                Errors = new HashSet<string>(errors);
            }
        }
        public Result()
        {
            IsSuccess = false;
        }
        // Success factory method
        public static Result Success()
        {
            return new Result(true);
        }

        // Failure factory method for single error
        public static Result Failure(string errorMessage)
        {
            return new Result(false, new[] { errorMessage });
        }

        // Failure factory method for multiple errors
        public static Result Failure(IEnumerable<string> errorMessages)
        {
            return new Result(false, errorMessages);
        }

        // Add a single error
        public void AddError(string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Errors.Add(errorMessage);
            }
        }

        // Add multiple errors
        public void AddErrors(IEnumerable<string> errorMessages)
        {
            foreach (var error in errorMessages)
            {
                AddError(error);
            }
        }

        public bool HasErrors => Errors.Any();

        public override string ToString()
        {
            return IsSuccess ? "Success" : $"Failure: {string.Join(", ", Errors)}";
        }
    }
}
