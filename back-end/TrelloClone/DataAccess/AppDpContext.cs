namespace TrelloClone.DataAccess;

public class AppDpContext : IdentityDbContext<AppUser>
{
    public AppDpContext(DbContextOptions<AppDpContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardMember> BoardMembers => Set<BoardMember>();
    public DbSet<BoardList> CardLists => Set<BoardList>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<CardMember> CardMembers => Set<CardMember>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<CardLabel> CardLabels => Set<CardLabel>();
    public DbSet<Checklist> Checklists => Set<Checklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<CardWatcher> CardWatchers => Set<CardWatcher>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Workspace>(e =>
        {
            e.HasOne(w => w.Owner)
                .WithMany(u => u.OwnedWorkspaces)
                .HasForeignKey(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(w => w.Visibility)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.HasIndex(w => w.OwnerId);
        });

        builder.Entity<WorkspaceMember>(e =>
        {
            e.HasOne(wm => wm.Workspace)
                .WithMany(w => w.Members)
                .HasForeignKey(wm => wm.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(wm => wm.User)
                .WithMany(u => u.WorkspaceMemberships)
                .HasForeignKey(wm => wm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(wm => wm.Role)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.HasIndex(wm => new { wm.WorkspaceId, wm.UserId }).IsUnique();
        });

        builder.Entity<Board>(e =>
        {
            e.HasOne(b => b.Workspace)
                .WithMany(w => w.Boards)
                .HasForeignKey(b => b.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(b => b.Visibility)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.HasQueryFilter(b => !b.IsArchived);
            e.HasIndex(b => b.WorkspaceId);
        });

        builder.Entity<BoardMember>(e =>
        {
            e.HasOne(bm => bm.Board)
                .WithMany(b => b.Members)
                .HasForeignKey(bm => bm.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(bm => bm.User)
                .WithMany(u => u.BoardMemberships)
                .HasForeignKey(bm => bm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(bm => bm.Role)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.HasIndex(bm => new { bm.BoardId, bm.UserId }).IsUnique();
        });

        builder.Entity<BoardList>(e =>
        {
            e.HasOne(cl => cl.Board)
                .WithMany(b => b.Lists)
                .HasForeignKey(cl => cl.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasQueryFilter(cl => !cl.IsArchived);
            e.HasIndex(cl => new { cl.BoardId, cl.Position });
        });

        builder.Entity<Card>(e =>
        {
            e.HasOne(c => c.List)
                .WithMany(l => l.Cards)
                .HasForeignKey(c => c.ListId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.Board)
                .WithMany()
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(c => !c.IsArchived);
            e.HasIndex(c => new { c.ListId, c.Position });
            e.HasIndex(c => c.BoardId);
            e.HasIndex(c => c.CreatedByUserId);
        });

        builder.Entity<CardMember>(e =>
        {
            e.HasOne(cm => cm.Card)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(cm => cm.User)
                .WithMany(u => u.CardAssignments)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(cm => cm.AssignedByUser)
                .WithMany()
                .HasForeignKey(cm => cm.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(cm => new { cm.CardId, cm.UserId }).IsUnique();
            e.HasIndex(cm => cm.UserId);
        });

        builder.Entity<Label>(e =>
        {
            e.HasOne(l => l.Board)
                .WithMany(b => b.Labels)
                .HasForeignKey(l => l.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(l => l.BoardId);
        });

        builder.Entity<CardLabel>(e =>
        {
            e.HasOne(cl => cl.Card)
                .WithMany(c => c.Labels)
                .HasForeignKey(cl => cl.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(cl => cl.Label)
                .WithMany(l => l.CardLabels)
                .HasForeignKey(cl => cl.LabelId)
                    .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(cl => new { cl.CardId, cl.LabelId }).IsUnique();
        });

        builder.Entity<Checklist>(e =>
        {
            e.HasOne(cl => cl.Card)
                .WithMany(c => c.Checklists)
                .HasForeignKey(cl => cl.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(cl => cl.CardId);
        });

        builder.Entity<ChecklistItem>(e =>
        {
            e.HasOne(ci => ci.Checklist)
                .WithMany(cl => cl.Items)
                .HasForeignKey(ci => ci.ChecklistId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ci => ci.AssignedUser)
                .WithMany()
                .HasForeignKey(ci => ci.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(ci => ci.ChecklistId);
        });

        builder.Entity<Comment>(e =>
        {
            e.HasOne(c => c.Card)
                .WithMany(card => card.Comments)
                .HasForeignKey(c => c.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(c => !c.IsDeleted);
            e.HasIndex(c => c.CardId);
        });

        builder.Entity<Notification>(e =>
        {
            e.HasOne(n => n.RecipientUser)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(n => n.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.HasIndex(n => new { n.RecipientUserId, n.IsRead, n.CreatedAt });
        });

        builder.Entity<ActivityLog>(e =>
        {
            e.HasOne(al => al.Board)
                .WithMany(b => b.ActivityLogs)
                .HasForeignKey(al => al.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(al => al.Card)
                .WithMany(c => c.ActivityLogs)
                .HasForeignKey(al => al.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(al => al.ActorUser)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(al => al.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(al => al.ActionType)
                .HasConversion<string>()
                .HasMaxLength(100);

            e.HasIndex(al => new { al.BoardId, al.CreatedAt });
            e.HasIndex(al => new { al.CardId, al.CreatedAt });
        });

        builder.Entity<Attachment>(e =>
        {
            e.HasOne(a => a.Card)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(a => a.UploadedByUser)
                .WithMany(u => u.Attachments)
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(a => a.CardId);
        });

        builder.Entity<Invitation>(e =>
        {
            e.HasOne(i => i.InviterUser)
                .WithMany(u => u.SentInvitations)
                .HasForeignKey(i => i.InviterUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(i => i.TargetType)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.Property(i => i.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.Property(i => i.Role)
                .HasConversion<string>()
                .HasMaxLength(50);

            e.HasIndex(i => i.Token).IsUnique();
            e.HasIndex(i => new { i.InviteeEmail, i.Status });
        });

        builder.Entity<CardWatcher>(e =>
        {
            e.HasOne(cw => cw.Card)
                .WithMany(c => c.Watchers)
                .HasForeignKey(cw => cw.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(cw => cw.User)
                .WithMany(u => u.WatchedCards)
                .HasForeignKey(cw => cw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(cw => new { cw.CardId, cw.UserId }).IsUnique();
            e.HasIndex(cw => cw.UserId);
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(rt => rt.Token);
            e.HasIndex(rt => rt.UserId);
        });
    }
}
