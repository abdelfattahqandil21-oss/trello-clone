using Microsoft.AspNetCore.Identity;

namespace TrelloClone.Models
{
    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public ICollection<Workspace> OwnedWorkspaces { get; set; } = new List<Workspace>();
        public ICollection<WorkspaceMember> WorkspaceMemberships { get; set; } = new List<WorkspaceMember>();
        public ICollection<BoardMember> BoardMemberships { get; set; } = new List<BoardMember>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Invitation> SentInvitations { get; set; } = new List<Invitation>();
        public ICollection<CardMember> CardAssignments { get; set; } = new List<CardMember>();
        public ICollection<CardWatcher> WatchedCards { get; set; } = new List<CardWatcher>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
