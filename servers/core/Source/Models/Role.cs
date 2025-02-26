namespace Core.Models;

// [Owned]
// public class AccessMode
// {
//     public required int Value { get; set; }
//     public required string Name { get; set; }
// }

// [Owned]
// public class Permission
// {
//     public required string Resource { get; set; }
//     public required string Description { get; set; }
//     public required ICollection<AccessMode> AccessModes { get; set; }
// }

// [Owned]
// public class RouteMetadata
// {
//     public required string HttpMethod { get; set; }
//     public required string Description { get; set; }
//     public required string Path { get; set; }
// }

// public class Permission
// {
//     public int Id { get; set; }
//     public required string Value { get; set; }
//     public required RouteMetadata Metadata { get; set; }

// public required int RoleId { get; set; }
// public Role? Role { get; set; }
// }

// [Owned]
// public class ExtraPermission
// {
//     public int PermissionId { get; set; }
//     public Permission? Permission { get; set; }

//     public Guid AdminId { get; set; }
//     public Admin? Admin { get; set; }
// }

// public class Role
// {
//     public int Id { get; set; }
//     public required string Name { get; set; }

//     public ICollection<Permission> Permissions { get; set; } = [];

//     public Guid AdminId { get; set; }
//     public Admin? Admin { get; set; }
// }