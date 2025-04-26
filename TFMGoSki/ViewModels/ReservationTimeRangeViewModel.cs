using System.ComponentModel.DataAnnotations;

namespace TFMGoSki.ViewModels
{
    public class ReservationTimeRangeViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "The start date is required")]
        [DataType(DataType.Date)]
        public DateOnly StartDateOnly { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "The end date is required")]
        [DataType(DataType.Date)]
        public DateOnly EndDateOnly { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "The start time is required")]
        [DataType(DataType.Time)]
        public TimeOnly StartTimeOnly { get; set; }

        [Display(Name = "End Time")]
        [Required(ErrorMessage = "The end time is required")]
        [DataType(DataType.Time)]
        public TimeOnly EndTimeOnly { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDateOnly <= DateOnly.FromDateTime(DateTime.Now))
            {
                yield return new ValidationResult(
                    "The start date cannot be earlier than the current date.",
                    new[] { nameof(StartDateOnly) });
            }
            if (StartTimeOnly <= TimeOnly.FromDateTime(DateTime.Now))
            {
                yield return new ValidationResult(
                    "The start time cannot be earlier than the current time.",
                    new[] { nameof(StartTimeOnly) });
            }

            if (StartDateOnly >= EndDateOnly)
            {
                yield return new ValidationResult(
                    "The start date must be before the end date.",
                    new[] { nameof(StartDateOnly), nameof(EndDateOnly) });
            }
            if (StartTimeOnly >= EndTimeOnly)
            {
                yield return new ValidationResult(
                    "The start time must be before the end time.",
                    new[] { nameof(StartTimeOnly), nameof(EndTimeOnly) });
            }

            if (StartDateOnly == DateOnly.MinValue)
            {
                yield return new ValidationResult(
                    "The start date cannot be null.",
                    new[] { nameof(StartDateOnly) });
            }
            if (EndDateOnly == DateOnly.MinValue)
            {
                yield return new ValidationResult(
                    "The end date cannot be null.",
                    new[] {  nameof(EndDateOnly) });
            }

            if (StartTimeOnly == TimeOnly.MinValue)
            {
                yield return new ValidationResult(
                    "The start time cannot be null.",
                    new[] { nameof(StartTimeOnly) });
            }
            if (EndTimeOnly == TimeOnly.MinValue)
            {
                yield return new ValidationResult(
                    "The end time cannot be null.",
                    new[] { nameof(EndTimeOnly) });
            }
        }
    }
}
