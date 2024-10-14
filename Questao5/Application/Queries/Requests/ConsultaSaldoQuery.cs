using MediatR;

namespace Questao5.Application.Queries.Requests
{
    public class ConsultaSaldoQuery : IRequest<ConsultaSaldoResult>
    {
        public string IdContaCorrente { get; set; }
    }

    public class ConsultaSaldoResult
    {
        public int Numero { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataHoraResposta { get; set; }
        public double SaldoAtual { get; set; }
    }
}
