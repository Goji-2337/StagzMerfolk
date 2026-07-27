using RimWorld;
using Verse;

namespace StagzMerfolk;

public class ChoiceLetter_AcceptAriel : ChoiceLetter_AcceptCharmedJoiner
{
    protected override DiaOption RejectOption => new ("RejectLetter".Translate())
        {
            action = delegate
            {
                //clears frozen water but this is vanilla "bug"
                GenExplosion.DoExplosion(center: asker.Position, map: asker.Map, radius: 4.9f, damType: DamageDefOf.Extinguish, instigator: null, explosionSound: SoundDefOf.Explosion_FirefoamPopper, postExplosionSpawnThingDef: ThingDefOf.Filth_FireFoam, postExplosionSpawnChance: 1);
                
                asker.Kill(null);
                CompRottable comp;
                if (asker.ParentHolder is Corpse c && (comp = c.GetComp<CompRottable>()) != null)
                {
                    comp.RotProgress = comp.PropsRot.TicksToDessicated;
                }
                Find.LetterStack.RemoveLetter(this);
            },
            resolveTree = true
        };
}