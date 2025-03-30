using System.ComponentModel.DataAnnotations;

namespace BoardManager.DTOs
{
    public class NewBoardRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public bool[][] InitialState { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var initialStateRows = InitialState.Length;
            for (int i = 0; i < initialStateRows; i++) { 
                var columns = InitialState[i].Length;
                if (columns != initialStateRows) {
                    yield return new ValidationResult(
                        $"Number of columns on the Board should be the same as the number of rows",
                        new[] { nameof(InitialState) });
                }
            }
        }
    }
}
