namespace TrelloClone.Models;

public enum Visibility
{
    Private,
    Public
}

public enum WorkspaceRole
{
    Owner,
    Admin,
    Member
}

public enum BoardRole
{
    Admin,
    Member,
    Viewer
}
public enum NotificationType
{
    CardAssigned,
    CommentAdded,
    CardDueSoon,
    CardMoved,
    MemberAdded,
    InvitationReceived,
    CardArchived,
    LabelAdded,
    AttachmentAdded
}

public enum InvitationTargetType
{
    Workspace,
    Board
}

public enum InvitationStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired,
    Cancelled
}

public enum ActionType
{
    Created,
    Updated,
    Deleted,
    Archived,
    Restored,
    Moved,
    Added,
    Removed,
    Assigned,
    Unassigned,
    Checked,
    Unchecked
}

public enum EntityType
{
    Workspace,
    Board,
    List,
    Card,
    Comment,
    Checklist,
    ChecklistItem,
    Label,
    Attachment,
    Invitation,
    WorkspaceMember,
    BoardMember
}