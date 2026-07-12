using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Services.IServices;

namespace TrelloClone.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/admin/workspaces")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin")]
public class WorkspacesManagementController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspacesManagementController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var workspaces = await _workspaceService.GetAllWorkspacesAsync();
        return Ok(workspaces);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _workspaceService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
