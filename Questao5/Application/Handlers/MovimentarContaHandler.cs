using MediatR;
using Questao5.Application.Exceptions;
using Questao5.Domain.Interfaces.Repository;
using Questao5.Domain.Interfaces.Services;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;

namespace Questao5.Application.Handlers
{
    public class MovimentarContaHandler : IRequestHandler<MovimentacaoRequest, MovimentacaoResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IContaCorrenteService _contaCorrenteService;

        public MovimentarContaHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IContaCorrenteService contaCorrenteService)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _contaCorrenteService = contaCorrenteService;
        }

        public async Task<MovimentacaoResponse> Handle(MovimentacaoRequest request, CancellationToken cancellationToken)
        {
            // Validações
            var conta = await _contaCorrenteRepository.GetContaCorrenteByIdAsync(request.IdContaCorrente);

            if (conta == null || !conta.Ativo)
                throw new BadRequestException("Conta não encontrada ou inativa", "INVALID_ACCOUNT");

            if (request.Valor <= 0)
                throw new BadRequestException("Valor deve ser positivo", "INVALID_VALUE");

            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                throw new BadRequestException("Tipo de movimento inválido", "INVALID_TYPE");

            // Verificar Idempotência
            if (await _contaCorrenteRepository.ExisteIdempotenciaAsync(request.Id))
                throw new BadRequestException("Movimentação já realizada anteriormente", "DUPLICATE_REQUEST");

            // Chamar o serviço para processar a movimentação
            return await _contaCorrenteService.MovimentarConta(request);
        }
    }

}
