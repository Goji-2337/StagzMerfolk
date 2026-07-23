using RimWorld;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

public class MentalState_Charmed : MentalState
{
    public float charmChance;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look<float>(ref this.charmChance, "charmChance", 0.1f, false);
    }

    public MentalState_Charmed()
    {
        this.charmChance = 0.01f;
    }

    public override RandomSocialMode SocialModeMax()
    {
        return RandomSocialMode.Off;
    }

    public override void PostEnd()
    {
        base.PostEnd();

        pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
        if (pawn.RaceProps.Humanlike && Rand.Chance(charmChance))
        {
            SendAskToJoinLetter(this.pawn);
        }
    }

    public void SendAskToJoinLetter(Pawn p, string text = "CharmedJoins")
    {
        TaggedString label = ("StagzMerfolk_LetterLabel" + text).Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
        TaggedString taggedString = ("StagzMerfolk_Letter" + text).Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
        PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref label, ref taggedString, pawn);

        var letter = (ChoiceLetter_AcceptCharmedJoiner)LetterMaker.MakeLetter(label, taggedString, StagzDefOf.Stagz_AcceptCharmedJoiner, null, null);
        letter.asker = p;
        letter.lookTargets = new LookTargets(p);
        letter.requiresAliveAsker = true;
        letter.StartTimeout(60000);

        Find.LetterStack.ReceiveLetter(letter, null);
    }
}