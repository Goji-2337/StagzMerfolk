using RimWorld;
using Verse;

namespace StagzMerfolk;

public class IncidentWorker_VirtuosoSummoned : IncidentWorker_ArielSummoned
{
    public override string letterlabeljoins =>  "StagzMerfolk_LetterLabelVirtuosoJoins";
    public override string letterjoins => "StagzMerfolk_LetterVirtuosoJoins";

    protected override ChoiceLetter_AcceptCharmedJoiner MakeAcceptLetter(TaggedString label, TaggedString taggedString)
    {
        return (ChoiceLetter_AcceptCharmedJoiner)LetterMaker.MakeLetter(label, taggedString, StagzDefOf.Stagz_AcceptCharmedJoiner, null, null);
    }

    protected override void ControllerPawnEffects(IncidentParms parms, Map map, Pawn pawn)
    {
    }
}