using LibrarySystem.Common.DTOs.Library.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LibrarySystem.API.Helpers
{
    public static class ValidationHelper
    {
        public static ValidationResultDto ValidateDto(object dto)
        {
            var result = new ValidationResultDto();

            if (dto == null)
            {
                result.IsValid = false;
                result.Errors.Add("Request body is required.");
                return result;
            }

            var properties = dto.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(dto);
                var attributes = prop.GetCustomAttributes<ValidationAttribute>(true);

                foreach (var attribute in attributes)
                {
                    var context = new ValidationContext(dto)
                    {
                        MemberName = prop.Name
                    };

                    var validationResult = attribute.GetValidationResult(value, context);

                    if (validationResult != ValidationResult.Success)
                    {
                        result.IsValid = false;
                        result.Errors.Add($"{prop.Name}: {validationResult.ErrorMessage}");
                    }
                }
            }

            if (!result.Errors.Any())
                result.IsValid = true;

            return result;
        }
    }
}
