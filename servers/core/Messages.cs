using Core.Models;

namespace Core.Messages;

public class NewStudentAuthed
{
    public ActiveStudent Student { get; set; }
}
