using Shared.Models;

namespace Shared.Messages;

public class NewStudentAuthed
{
    public ActiveStudent Student { get; set; }
}
