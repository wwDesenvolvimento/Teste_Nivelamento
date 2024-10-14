using FluentValidation.Results;

namespace Questao5.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public List<ValidationFailure> Errors { get; set; }
        public string TipoErro { get; set; } = string.Empty;

        public BadRequestException(string message) : base(message)
        {
            Errors = new List<ValidationFailure>();
            TipoErro = "DEFAULT_ERROR";
        }
        public BadRequestException(string message, string tipoErro) : base(message)
        {
            Errors = new List<ValidationFailure>();
            TipoErro = tipoErro;
        }

        public BadRequestException(string message, List<ValidationFailure> validationsFailures) : base(message)
        {
            Errors = validationsFailures ?? new List<ValidationFailure>();
            TipoErro = "VALIDATION_ERROR";
        }

        public BadRequestException(string message, List<ValidationFailure> validationsFailures, string tipoErro) : base(message)
        {
            Errors = validationsFailures ?? new List<ValidationFailure>();
            TipoErro = tipoErro;
        }
    }
}
