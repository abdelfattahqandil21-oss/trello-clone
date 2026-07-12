using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.DTOs;

namespace TrelloClone.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/admin/statistics")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin")]
public class StatisticsController : ControllerBase
{
    private readonly AppDpContext _context;

    public StatisticsController(AppDpContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalWorkspaces = await _context.Workspaces.CountAsync();
        var totalBoards = await _context.Boards.CountAsync();
        var totalCards = await _context.Cards.CountAsync();

        return Ok(new StatisticsResponse(totalUsers, totalBoards, totalWorkspaces, totalCards));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsersStats()
    {
        var total = await _context.Users.CountAsync();
        var verified = await _context.Users.CountAsync(u => u.IsEmailVerified);
        var active = await _context.Users.CountAsync(u => u.IsActive);

        return Ok(new { total, verified, active, unverified = total - verified, inactive = total - active });
    }

    [HttpGet("boards")]
    public async Task<IActionResult> GetBoardsStats()
    {
        var total = await _context.Boards.CountAsync();
        var archived = await _context.Boards.CountAsync(b => b.IsArchived);

        return Ok(new { total, active = total - archived, archived });
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 20)
    {
        var activity = await _context.ActivityLogs
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(a => new ActivityLogResponse(
                a.Id, a.BoardId, a.CardId, a.ActorUserId,
                a.ActorUser.DisplayName ?? a.ActorUserId,
                a.ActionType.ToString(), a.EntityType.ToString(),
                a.EntityId, a.MetadataJson, a.CreatedAt
            ))
            .ToListAsync();

        return Ok(activity);
    }
}
