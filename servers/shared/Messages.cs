using NServiceBus;
using Shared.Core;

namespace Shared.Messages;

public class NewStudentAuth : ICommand
{
    public Student Student { get; set; }
}
