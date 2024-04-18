using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IContentsManager _manager;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentsManager manager, ILogger<ContentController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetManyContents()
        {
            try
            {
                var contents = await _manager.GetManyContents().ConfigureAwait(false);
                if (!contents.Any())
                {
                    _logger.LogInformation("Nenhum conteúdo encontrado.");
                    return NotFound();
                }

                return Ok(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter vários conteúdos: {ErrorMessage}", ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContent(Guid id)
        {
            try
            {
                var content = await _manager.GetContent(id).ConfigureAwait(false);
                if (content == null)
                {
                    _logger.LogInformation("Conteúdo não encontrado para o ID: {ContentId}", id);
                    return NotFound();
                }

                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conteúdo para o ID {ContentId}: {ErrorMessage}", id, ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] ContentInput content)
        {
            try
            {
                var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);
                _logger.LogInformation("Conteúdo criado com sucesso: {ContentId}", createdContent?.Id);
                return Ok(createdContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conteúdo: {ErrorMessage}", ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] ContentInput content)
        {
            try
            {
                var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);
                _logger.LogInformation("Conteúdo atualizado com sucesso: {ContentId}", id);
                return Ok(updatedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar conteúdo para o ID {ContentId}: {ErrorMessage}", id, ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(Guid id)
        {
            try
            {
                var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
                _logger.LogInformation("Conteúdo excluído com sucesso: {ContentId}", id);
                return Ok(deletedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir conteúdo para o ID {ContentId}: {ErrorMessage}", id, ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        [HttpPost("{id}/genre")]
        public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genres)
        {
            try
            {
                var content = await _manager.GetContent(id).ConfigureAwait(false);
                if (content == null)
                    return NotFound();

                foreach (var genre in genres)
                {
                    if (!content.GenreList.Contains(genre))
                    {
                        content.GenreList.Add(genre);
                    }
                }

                await _manager.UpdateContent(id, ConvertToContentDto(content)).ConfigureAwait(false);

                _logger.LogInformation("Gêneros adicionados ao conteúdo com sucesso: {ContentId}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar gêneros ao conteúdo para o ID {ContentId}: {ErrorMessage}", id, ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        [HttpDelete("{id}/genre")]
        public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genres)
        {
            try
            {
                var content = await _manager.GetContent(id).ConfigureAwait(false);
                if (content == null)
                    return NotFound();

                var contentGenres = content.GenreList.ToList();

                foreach (var genre in genres)
                {
                    contentGenres.Remove(genre);
                }

                var contentDto = ConvertToContentDto(content);
                await _manager.UpdateContent(id, contentDto).ConfigureAwait(false);

                _logger.LogInformation("Gêneros removidos do conteúdo com sucesso: {ContentId}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover gêneros do conteúdo para o ID {ContentId}: {ErrorMessage}", id, ex.Message);
                return Problem(detail: ex.Message);
            }
        }

        private ContentDto ConvertToContentDto(Content content)
        {
            return new ContentDto(
                content.Id,
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.GenreList.ToList(),
                content.StartTime,
                content.EndTime,
                content.GenreList
            );
        }
    }
}
