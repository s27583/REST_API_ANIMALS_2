using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace REST_API_ANIMALS_2
{
    public static class Validators
    {
        public static void RegisterValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddAnimalRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateAnimalRequestValidator>();
        }

    }
}