public sealed record ProgressCallbacks(
    Action<int,int> Init,
    Action Advance, 
    Action Finish);