using Microsoft.AspNetCore.Mvc;
using ApiTarefas.Data;
using ApiTarefas.Models;

namespace ApiTarefas.Controllers;

[ApiController]
//Controller com as principais funções da API
public class TarefaController : ControllerBase
{
    [HttpGet("/")]
    public ActionResult<List<Tarefa>> Ler([FromServices] AppDbContext context)
    {
        List<Tarefa> tarefas = [.. context.Tarefas];
        
        // Retorna um 200 OK com a lista de tarefas
        return Ok(tarefas);
    }

    [HttpGet("/{id:int}")]
    public ActionResult<Tarefa> LerViaId([FromRoute] int id, [FromServices] AppDbContext context)
    {
        var tarefa = context.Tarefas.FirstOrDefault(x => x.Id == id);

        if (tarefa == null)
            return NotFound(new { message = $"Tarefa com id {id} não encontrada."}); // Retorna 404 indicando que a tarefa não foi encontrada

        return Ok(tarefa); // Retorna 200 OK com a tarfa encontrada
    }

    [HttpPost("/")]
    public ActionResult<Tarefa> Criar([FromBody]Tarefa tarefa, [FromServices] AppDbContext context)
    {
        if (tarefa == null)
            return BadRequest(new { message = "Os dados da tarefa estão inválidos." }); // Retorna 400 BadRequest indicando um erro no json de criação da tarefa

        tarefa.DataCriacao = DateTime.UtcNow; // Definindo data de criação

        if (tarefa.Status)
        {
            tarefa.DataConclusao = DateTime.UtcNow; // Definindo data de conclusão caso a tarefa seja criada como concluída
        }
        
        context.Tarefas.Add(tarefa);
        context.SaveChanges();

        // Retorna um 201 Created e a tarefa com base no método 'LerViaId'
        return CreatedAtAction(nameof(LerViaId), new { id = tarefa.Id }, tarefa);
    }

    [HttpPut("/{id:int}")]
    public ActionResult<Tarefa> Atualizar([FromRoute] int id, [FromBody]Tarefa tarefa, [FromServices] AppDbContext context)
    {
        var model = context.Tarefas.FirstOrDefault(x => x.Id == id);
        
        if (model == null)
            return NotFound(new { message = $"Tarefa com id {id} não encontrada para a atualização."});

        // Atualizando valores da tarefa
        model.Titulo = tarefa.Titulo;
        model.Descricao = tarefa.Descricao;
        model.Status = tarefa.Status;

        if (tarefa.Status)
        {
            model.DataConclusao = DateTime.UtcNow;
        }

        context.Tarefas.Update(model);
        context.SaveChanges();

        return Ok(model); // Retorna 200 OK indicando que a tarefa foi atualizada
    }

    [HttpDelete("/{id:int}")]
    public ActionResult<Tarefa> Excluir([FromRoute] int id, [FromServices] AppDbContext context)
    {
        var model = context.Tarefas.FirstOrDefault(x => x.Id == id);
        
        if (model == null)
            return NotFound(new { message = $"Tarefa com id {id} não encontrada para a exclusão."});
        
        context.Tarefas.Remove(model);
        context.SaveChanges();

        return NoContent(); // retorna 204 No CONTENT indicando a exclusão da tarefa
    }

    [HttpPatch("/{id:int}/status")]
    public ActionResult<Tarefa> AlterarStatus([FromRoute] int id, [FromQuery] bool novoStatus, [FromServices] AppDbContext context)
    {
        var model = context.Tarefas.FirstOrDefault(x => x.Id == id);
        
        if (model == null)
            return NotFound(new { message = $"Tarefa com id {id} não encontrada para a mudança de status."});

        model.Status = novoStatus;
        
        if (novoStatus)
            model.DataConclusao = DateTime.UtcNow; // Atualizando a data de conclusão caso o novo status seja true
        
        context.Tarefas.Update(model);
        context.SaveChanges();

        return Ok(model); // Retorna 200 OK indicando que o status da tarefa foi atualizado com sucesso
    }
}
