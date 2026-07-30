using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace StagzMerfolk;

/*
 Ariel and Virtuoso would be identical incidents if not for these differences:
     - trigger condition, handled in trigger code
     - arrival cell, handled in trigger code
     - mute hediff for Ariel
     - controller pawn in Ariel version is automatically tended using Ariel's skill
     - loiter spot
     - Letter label and text, handled in incident def
     - different consequences for rejecting the letter, handled in letter class
*/
[UsedImplicitly]
public class IncidentWorker_ArielSummoned : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (parms.spawnCenter == IntVec3.Invalid)
        {
            parms.spawnCenter = parms.controllerPawn.Position;
            Log.Warning(GetType() + ": water cell for merren joiner was invalid, using controller pawn's position");
        }
        
        Map map = (Map)parms.target;
        
        //generate pawn with temp faction - stole from Sanguophage temp faction code
        List<FactionRelation> list = [];
        foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
        {
            if (!faction.def.PermanentlyHostileTo(def.pawnKind.defaultFactionDef))
            {
                list.Add(new FactionRelation(faction, FactionRelationKind.Neutral));
            }
        }
        Faction tempFaction = FactionGenerator.NewGeneratedFactionWithRelations(def.pawnKind.defaultFactionDef, list, hidden: true);
        tempFaction.temporary = true;
        //yes, I pre-convert them to player ideo
        tempFaction.ideos.SetPrimary(Faction.OfPlayer.ideos.PrimaryIdeo);
        Find.FactionManager.Add(tempFaction);
        Pawn merrenJoiner = PawnGenerator.GeneratePawn(def.pawnKind, tempFaction);
        
        //spawn pawn
        GenSpawn.Spawn(merrenJoiner, parms.spawnCenter, map);
        StagzDefOf.Stagz_MerrenJoinerEmergeFromWater.Spawn(merrenJoiner, map);
        
        //Sets Nautian style to joiner's items if Nautian mod is present
        if (StagzDefOf.GM_Ocean != null)
        {
            foreach (var item in merrenJoiner.EquippedWornOrInventoryThings)
            {
                if (item.def.CanBeStyled())
                {
                    var styleDef = StagzDefOf.GM_Ocean.GetStyleForThingDef(item.def);
                    if (styleDef != null) item.StyleDef = styleDef;
                }
            }
        }
        
        //just gives a simple job to visit colony atm
        AssignLord(ref parms, merrenJoiner, map);

        //give pawn hediff when needed
        if (def.pawnHediff != null)
        {
            merrenJoiner.health.AddHediff(def.pawnHediff);
        }

        //tend pawn that was downed to call this incident
        ControllerPawnEffects(parms, map, merrenJoiner);
        
       
        //Can't just SendIncidentLetter because we use custom letter class, sigh
        TaggedString label = def.letterLabel.Formatted(merrenJoiner.Named("PAWN"), parms.controllerPawn.Named("SUBJECT"));
        TaggedString taggedString = def.letterText.Formatted(merrenJoiner.Named("PAWN"), parms.controllerPawn.Named("SUBJECT"));
        PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref taggedString, ref label, merrenJoiner);
        var letter = (ChoiceLetter_AcceptCharmedJoiner) LetterMaker.MakeLetter(label, taggedString, def.letterDef, quest: null);
        letter.asker = merrenJoiner;
        letter.lookTargets = new LookTargets(merrenJoiner);
        letter.requiresAliveAsker = true;
        letter.StartTimeout(GenDate.TicksPerDay);
        Find.LetterStack.ReceiveLetter(letter);

        return true;
    }

    protected virtual void AssignLord(ref IncidentParms parms, Pawn merrenJoiner, Map map)
    {
        var lordJobVisitColony = new LordJob_VisitColony(parms.faction, parms.controllerPawn.Position, GenDate.TicksPerDay);
        parms.lord = LordMaker.MakeNewLord(parms.faction, lordJobVisitColony, map, [merrenJoiner]);
    }

    protected virtual void ControllerPawnEffects(IncidentParms parms, Map map, Pawn pawn)
    {
        if (parms.controllerPawn != null)
        {
            foreach (var _ in parms.controllerPawn.health.hediffSet.GetHediffsTendable())
            {
                Medicine medicine = (Medicine)GenSpawn.Spawn(ThingDefOf.MedicineHerbal, parms.spawnCenter, map);
                TendUtility.DoTend(pawn, parms.controllerPawn, medicine);
            }
        }
    }
}