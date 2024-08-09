using FluentValidation;
using TransactionManager.Dto;

namespace TransactionManager.Helpers.Validators;

using FluentValidation;
using System;

/// <summary>
/// Validator for <see cref="TransactionByDateDto"/> to ensure that date-related properties are valid.
/// </summary>
public class TransactionByDateDtoValidator : AbstractValidator<TransactionByDateDto>
{
    public TransactionByDateDtoValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Year must be greater or equal to 1")
            .LessThanOrEqualTo(9999)
            .WithMessage("Year must be less or equal to 9999");
        
        RuleFor(x => x.Month)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Month must be greater or equal to 1")
            .LessThanOrEqualTo(12)
            .WithMessage("Month must be less or equal to 12");
        
        RuleFor(x => x.Day)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Day must be greater or equal to 1")
            .LessThanOrEqualTo(31)
            .WithMessage("Day must be less or equal to 31");
        
        RuleFor(x => x)
            .Must(MonthBeNotEmptyIfDayIsNotNull)
            .WithMessage("Month can't be null if day has a value")
            .WithName("General")
            .Must(HaveDayInMonth)
            .WithName("General")
            .WithMessage("The specified day does not exist in the specified month of the specified year");
    }

    /// <summary>
    /// Validates that the Month property is not null if the Day property has a value.
    /// </summary>
    /// <param name="transactionByDateDto">The DTO being validated.</param>
    /// <returns><c>true</c> if Month is not null when Day is provided, otherwise <c>false</c>.</returns>
    private static bool MonthBeNotEmptyIfDayIsNotNull(TransactionByDateDto transactionByDateDto)
    {
        return transactionByDateDto.Day is null || transactionByDateDto.Month is not null;
    }

    /// <summary>
    /// Validates that the specified Day exists within the given Month and Year.
    /// </summary>
    /// <param name="transactionByDateDto">The DTO being validated.</param>
    /// <returns><c>true</c> if the Day is valid for the specified Month and Year, otherwise <c>false</c>.</returns>
    private static bool HaveDayInMonth(TransactionByDateDto transactionByDateDto)
    {
        if (transactionByDateDto is { Day: not null, Month: not null })
        {
            try
            {
                var daysInMonth = DateTime.DaysInMonth(transactionByDateDto.Year, transactionByDateDto.Month.Value);
                return transactionByDateDto.Day.Value <= daysInMonth;
            }
            catch
            {
                return false;
            }
        }
        return true;
    }
}
