using System;
/// <summary>
/// A context struct that holds all the data needed for the tutorial
/// </summary>
/// <remarks>
///This could be done better with dependency injection, but at this point it's not worth it since my DI system only works on MonoBehaviors and is primarily scoped for scene injection. I don't like this pattern either, but ideally, the tutorial should be scoped small enough for it not to really matter. If this somehow gets really bad, then please refactor with a proper dependency container 
/// </remarks>
[Serializable] 
public struct TutorialContext
{
    public InterfaceReference<IPlayerController>[] PlayerControllers;
    
}