#nullable disable
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTienda.Utilidades
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly object _targetValue;

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            // 1) Buscamos la propiedad dependiente
            var prop = context.ObjectType.GetProperty(_dependentProperty);
            if (prop == null)
                return new ValidationResult($"No se encontró la propiedad '{_dependentProperty}'.");

            // 2) Obtenemos su valor
            var dependentValue = prop.GetValue(context.ObjectInstance);

            // 3) Determinamos si debemos validar:
            //    - Si _targetValue es null ⇒ validamos cuando dependentValue != null/empty
            //    - Si _targetValue no es null ⇒ validamos cuando Equals(dependentValue, _targetValue)
            bool shouldValidate;
            if (_targetValue == null)
            {
                shouldValidate = dependentValue != null 
                                 && !string.IsNullOrWhiteSpace(dependentValue.ToString());
            }
            else
            {
                shouldValidate = object.Equals(dependentValue, _targetValue);
            }

            // 4) Si toca validar, comprobamos que 'value' no sea nulo o vacío
            if (shouldValidate)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var msg = ErrorMessage ?? $"{context.DisplayName} es obligatorio.";
                    return new ValidationResult(msg);
                }
            }

            // 5) Todo OK
            return ValidationResult.Success;
        }
    }
}
#nullable restore
