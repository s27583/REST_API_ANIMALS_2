using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace REST_API_ANIMALS_2
{
    public class UpdateAnimalRequestValidator : AbstractValidator<UpdateAnimalRequest>
    {
        public UpdateAnimalRequestValidator()
        {
            RuleFor(e => e.Name).MaximumLength(50).NotNull();
            RuleFor(e => e.Description).MaximumLength(50).NotNull();
            RuleFor(e => e.Category).Length(20).NotNull();
            RuleFor(e => e.Area).MaximumLength(50).NotNull();
        }
    }
}
