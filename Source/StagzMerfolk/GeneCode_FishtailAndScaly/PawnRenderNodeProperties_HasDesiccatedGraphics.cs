using JetBrains.Annotations;
using Verse;

namespace StagzMerfolk;

[PublicAPI]
public class PawnRenderNodeProperties_HasDesiccatedGraphics : PawnRenderNodeProperties
{
    //TODO: ideally this should be null-checked from everywhere it's called but I'm tired and mod doesn't need it unless it's been messed with
    public string desiccatedTexPath;
}