using Microsoft.AspNetCore.Mvc;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;

namespace Questao5.Domain.Interfaces.Services
{
    public interface IContaCorrenteService
    {
        Task<MovimentacaoResponse> MovimentarConta(MovimentacaoRequest request);
        Task<SaldoResponse> ConsultarSaldo(string idContaCorrente);
    }
}
