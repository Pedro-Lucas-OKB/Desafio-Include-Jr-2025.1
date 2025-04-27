using Microsoft.AspNetCore.Mvc;
using ApiTarefas.Data;
using ApiTarefas.Models;

namespace ApiTarefas.Controllers;

[ApiController]
public class TarefaController : ControllerBase
{
    [HttpGet("/")]
    public List<Tarefa> Ler([FromServices] AppDbContext context)
    {
        return [.. context.Tarefas];
    }

    [HttpGet("/{id:int}")]
    public Tarefa LerViaId([FromRoute] int id, [FromServices] AppDbContext context)
    {
        return context.Tarefas.FirstOrDefault(x => x.Id == id);
    }

    [HttpPost("/")]
    public Tarefa Criar([FromBody]Tarefa tarefa, [FromServices] AppDbContext context)
    {
        tarefa.DataCriacao = DateTime.UtcNow;

        if (tarefa.Status)
        {
            tarefa.DataConclusao = DateTime.UtcNow;
        }
        
        context.Tarefas.Add(tarefa);
        context.SaveChanges();

        return tarefa;
    }

    [HttpPut("/{id:int}")]
    public Tarefa Atualizar([FromRoute] int id, [FromBody]Tarefa tarefa, [FromServices] AppDbContext context)
    {
        var model = context.Tarefas.FirstOrDefault(x => x.Id == id);
        
        if(model == null) return tarefa;

        model.Titulo = tarefa.Titulo;
        model.Descricao = tarefa.Descricao;
        model.Status = tarefa.Status;

        if (tarefa.Status)
        {
            model.DataConclusao = DateTime.UtcNow;
        }

        context.Tarefas.Update(model);
        context.SaveChanges();

        return model;
    }

    [HttpDelete("/{id:int}")]
    public Tarefa Excluir([FromRoute] int id, [FromServices] AppDbContext context)
    {
        var model = context.Tarefas.FirstOrDefault(x => x.Id == id);

        context.Tarefas.Remove(model);
        context.SaveChanges();

        return model;
    }

    [HttpPatch("/{id:int}/status")]
    public Tarefa AlterarStatus([FromRoute] int id, [FromQuery] bool novoStatus, [FromServices] AppDbContext context)
    {
        var model = context.Tarefas.FirstOrDefault(x => x.Id == id);
        
        if(model == null) return model;

        model.Status = novoStatus;
        
        if (novoStatus)
            model.DataConclusao = DateTime.UtcNow;
        
        context.Tarefas.Update(model);
        context.SaveChanges();

        return model;
    }
}
