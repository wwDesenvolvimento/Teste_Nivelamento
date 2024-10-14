using MediatR;
using Questao5.Application.Exceptions;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Interfaces.Repository;


namespace Questao5.Application.Handlers
{

    public class ConsultaSaldoQueryHandler : IRequestHandler<ConsultaSaldoQuery, ConsultaSaldoResult>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ConsultaSaldoQueryHandler(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<ConsultaSaldoResult> Handle(ConsultaSaldoQuery request, CancellationToken cancellationToken)
        {
            
            var contaCorrente = await _contaCorrenteRepository.GetContaCorrenteByIdAsync(request.IdContaCorrente);

            if (contaCorrente == null)
            {
                throw new BadRequestException("Conta não encontrada.", "INVALID_ACCOUNT");
            }

            if (!contaCorrente.Ativo)
            {
                throw new BadRequestException("Conta inativa.", "INACTIVE_ACCOUNT");
            }

            var saldo = await _contaCorrenteRepository.GetSaldoAsync(request.IdContaCorrente);

            var resultado = new ConsultaSaldoResult
            {
                Numero = contaCorrente.Numero,
                NomeTitular = contaCorrente.Nome,
                SaldoAtual = saldo,
                DataHoraResposta = DateTime.UtcNow
            };

            return resultado;
        }
    }
}
