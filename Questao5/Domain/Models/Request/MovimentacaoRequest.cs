using MediatR;
using Questao5.Domain.Models.Response;

namespace Questao5.Domain.Models.Request
{
    public class MovimentacaoRequest : IRequest<MovimentacaoResponse>
    {
        public string Id { get; set; } 
        public string IdContaCorrente { get; set; } 
        public double Valor { get; set; }
        public string TipoMovimento { get; set; } 
    }
}
