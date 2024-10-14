using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Exceptions;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Models.Request;
using Questao5.Domain.Models.Response;

namespace Questao5.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("movimentacao")]
        public async Task<IActionResult> MovimentarContaCorrente([FromBody] MovimentacaoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                MovimentacaoResponse response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, tipo = ex.TipoErro });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro inesperado.", details = ex.Message });
            }
        }

        [HttpGet("saldo/{idContaCorrente}")]
        public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
        {
            try
            {
                var query = new ConsultaSaldoQuery { IdContaCorrente = idContaCorrente };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { mensagem = ex.Message, tipo = ex.TipoErro });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }

}
