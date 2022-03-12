// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>", Scope = "type", Target = "~T:Backend.WebApi.Tests.IoCFixtureCollection")]
[assembly: SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>", Scope = "type", Target = "~T:Backend.WebApi.Tests.ApiLocalDbCollection")]
[assembly: SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>", Scope = "type", Target = "~T:Backend.WebApi.Tests.ActionFilterFixtureCollection")]
[assembly: SuppressMessage("Design", "MA0042:Do not use blocking calls in an async method", Justification = "<Pending>", Scope = "member", Target = "~M:Backend.WebApi.Tests.App.Operations.UserInteractionCommands.UserInteractionUpdateCommandTests.Update_ExistingInteraction_Succeeds(System.Guid,System.DateTime,System.String,System.Boolean,Backend.WebApi.App.Operations.UserInteractionCommands.UserInteractionUpdateCommand)~System.Threading.Tasks.Task")]
