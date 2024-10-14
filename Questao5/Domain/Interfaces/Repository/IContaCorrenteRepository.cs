using Questao5.Domain.Entities;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;

namespace Questao5.Domain.Interfaces.Repository
{
    public interface IContaCorrenteRepository
    {
        Task<bool> ExisteIdempotenciaAsync(string chaveIdempotencia);
        Task SalvarIdempotenciaAsync(string chaveIdempotencia, MovimentacaoRequest request, MovimentacaoResponse response);
        Task InserirMovimentoAsync(MovimentacaoRequest request, string idMovimento);
        Task<ContaCorrente> GetContaCorrenteByIdAsync(string idContaCorrente);
        Task<double> GetSaldoAsync(string idContaCorrente);
    }
}
