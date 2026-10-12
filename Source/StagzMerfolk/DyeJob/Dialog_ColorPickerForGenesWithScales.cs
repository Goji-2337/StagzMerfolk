using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

// heavily rewritten version of vanilla Dialog_ColorPickerBase
public class Dialog_ColorPickerForGenesWithScales : Window
{
	private const int ContextHash = 195906069;
	private static readonly List<string> focusableControlNames =
	[
		"title", "colorTextfields_0", "colorTextfields_1", "colorTextfields_2", "colorTextfields_3", "colorTextfields_4", "colorTextfields_5"
	];
	private const int ColorWheelSize = 128;
	private const int ColorTextfieldsWidth = 125;
	private const int ColorIconSize = 22;
	private const int ColorIconPadding = 2;
	private const int ColorIconSizeWithPadding = 2 * ColorIconPadding + ColorIconSize;
	private const int RotateButtonDimensions = 80;
	private static readonly int PaletteColumns = Gene_WithScaleColor.defaultColors.Count + 1;
    private static readonly int PaletteWidth = PaletteColumns * ColorIconSize + (PaletteColumns + 1) * ColorIconPadding;
    private readonly Color oldColor;
	private Color color;
	private bool hsvColorWheelDragging;
	private string[] textfieldBuffers = new string[6];
	private Color textfieldColorBuffer;
	private string previousFocusedControlName;
	private Rot4 portraitRotation = new (2);
	private static readonly Vector2 ButSize = new (150f, 38f);
	public override Vector2 InitialSize => new (600f, 610f);
	
	private readonly Pawn pawn;
	private readonly Thing parentOfComp;
	private bool accepted;

	public Dialog_ColorPickerForGenesWithScales(Pawn pawn, Thing parentOfComp = null)
    {
        forcePause = true;
        absorbInputAroundWindow = true;
        closeOnClickedOutside = true;
        closeOnAccept = false;
        
        this.pawn = pawn;
        this.parentOfComp = parentOfComp;
        color = pawn.GetMerrenScaleColorOrFailsafe();
        oldColor = color;
    }

    public override void PreClose()
    {
	    //if canceled
	    if (!accepted)
	    {
		    pawn.TrySetMerrenScaleColor(oldColor);
		    return;
	    }
	    //if devmode gizmo
	    if (parentOfComp is null)
	    {
		    pawn.TrySetMerrenScaleColor(color);
		    return;
	    }
	    //if accepted with new color
	    if (color != oldColor)
	    {
		    pawn.TrySetMerrenScaleColor(oldColor);
		    pawn.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.PendingColor = color;
		    if (parentOfComp.def == ThingDefOf.Dye && !parentOfComp.IsForbidden(pawn) && pawn.CanReserve(parentOfComp, 1, 1))
		    {
			    Job job = JobMaker.MakeJob(StagzDefOf.Stagz_ChangeScaleColorAtDye, parentOfComp);
			    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
		    } else 
		    {
			    Thing dye = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Dye), PathEndMode.ClosestTouch, TraverseParms.For(pawn), validator: x => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1));
			    if (dye is null) return;
			    Job job = JobMaker.MakeJob(StagzDefOf.Stagz_ChangeScaleColorAtComp, dye, parentOfComp);
			    job.count = 1;
			    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
		    }
	    }
    }

    //UI methods, ugh
	//pulled all copied UI-drawing methods into one since I am this close to having a mental break parsing it all
	public override void DoWindowContents(Rect inRect)
	{
		using (TextBlock.Default())
		{
			RectDivider layout = new(inRect, ContextHash);
			
			//HeaderRow
			using (new TextBlock(GameFont.Medium))
			{
				TaggedString taggedString = "ChooseAColor".Translate().CapitalizeFirst();
				RectDivider HeaderRowDivider = layout.NewRow(Text.CalcHeight(taggedString, layout.Rect.width));
				GUI.SetNextControlName(focusableControlNames[0]);
				Widgets.Label(HeaderRowDivider.Rect, taggedString);
			}
			
			//BottomButtons;
			RectDivider BottomButtonsDivider = layout.NewRow(ButSize.y, VerticalJustification.Bottom);
			if (Widgets.ButtonText(BottomButtonsDivider.NewCol(ButSize.x), "Close".Translate()))
			{
				Close();
			}
			if (Widgets.ButtonText(BottomButtonsDivider.NewCol(ButSize.x, HorizontalJustification.Right), "Accept".Translate()))
			{
				accepted = true;
				Close();
			}
			
			//I divide the window into right and left sides, and firstly draw the right side since it has all actual interface.
			//In the remaining space to the left, I draw the pawn preview later.
			RectDivider rightCol = layout.NewCol(PaletteWidth, HorizontalJustification.Right);

			var currentColor = color;
			//ColorPalette;
			using (new TextBlock(TextAnchor.MiddleLeft))
			{
				Color? geneticColor = pawn.genes?.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>()?.def.RenderNodeProperties.FirstOrFallback().color;
				if (geneticColor != null)
				{
					//the "Genetic Color" box and label at the top of palette
					RectDivider ColorBoxRowGenetic = rightCol.NewRow(ColorIconSizeWithPadding);
					RectDivider ColorBoxGenetic = ColorBoxRowGenetic.NewCol(ColorIconSizeWithPadding);
					Widgets.ColorBox(ColorBoxGenetic.Rect, ref color, (Color) geneticColor);
					Widgets.Label(ColorBoxRowGenetic.Rect, "StagzMerfolk_UI_GeneticColor".Translate().CapitalizeFirst());
				}
				//the "Old Color" box and label at the top of palette
				RectDivider ColorBoxRow = rightCol.NewRow(ColorIconSizeWithPadding);
				RectDivider ColorBox = ColorBoxRow.NewCol(ColorIconSizeWithPadding);
				Widgets.ColorBox(ColorBox.Rect, ref color, oldColor);
				Widgets.Label(ColorBoxRow.Rect, "OldColor".Translate().CapitalizeFirst());
				//the palette itself
				Widgets.ColorSelector(rightCol.Rect, ref color, Gene_WithScaleColor.defaultColors, out var paletteHeight);
				rightCol.NewRow(paletteHeight);
			}

			//just a bit of padding between palette and wheel. Arbitrary size
			rightCol.NewRow(ColorIconSizeWithPadding / 2f);
			
			//ColorWheel
			RectDivider ColorWheelDiv = rightCol.NewRow(ColorWheelSize);
			Widgets.HSVColorWheel(
				ColorWheelDiv.Rect.ContractedBy((ColorWheelDiv.Rect.width - ColorWheelSize) / 2f, (ColorWheelDiv.Rect.height - ColorWheelSize) / 2f), 
				ref color,
				ref hsvColorWheelDragging);
			
			//Padding
			rightCol.NewRow(ColorIconSizeWithPadding / 2f);
			
			//Value slider
			Rect aggregatorRect = rightCol.NewRow(ColorIconSizeWithPadding);
			Color.RGBToHSV(color, out var H, out var S, out var V);
			V = Widgets.HorizontalSlider(aggregatorRect, V, 0, 1);
			color = Color.HSVToRGB(H, S, V);
			
			//ColorTextfields
			RectAggregator aggregator = new (aggregatorRect, ContextHash);
			Widgets.ColorTextfields(ref aggregator, ref color, ref textfieldBuffers, ref textfieldColorBuffer, previousFocusedControlName, "colorTextfields");
			
			//Updating it here, before we draw the pawn
			if (currentColor != color) pawn.TrySetMerrenScaleColor(color);
			
			
			//Left panel - mainly pawn preview - thus uses "layout" as rect again
			//Rotate button on top of pawn
			RectDivider RotateButtonDivider = layout.NewRow(RotateButtonDimensions);
			RotateButtonDivider.NewRow(ColorIconSizeWithPadding / 2f);
			RotateButtonDivider = RotateButtonDivider.NewCol((RotateButtonDivider.Rect.width + RotateButtonDimensions) / 2, HorizontalJustification.Right);
			if (Widgets.ButtonImage(RotateButtonDivider.NewCol(RotateButtonDimensions), StagzAssets.DyeDialogRotatePawn))
			{
				portraitRotation.Rotate(RotationDirection.Counterclockwise);
			}

			//DrawPawn
			Rect position = layout.Rect;
			RenderTexture image = PortraitsCache.Get(
				pawn,
				new Vector2(position.width / 2, position.height / 2),
				portraitRotation,
				renderHeadgear: false,
				renderClothes: false);
			GUI.DrawTexture(position, image);
			
			TabControl();
			if (Event.current.type == EventType.Layout)
			{
				previousFocusedControlName = GUI.GetNameOfFocusedControl();
			}
		}
	}

	private static void TabControl()
	{
		if (Event.current.type != EventType.KeyDown || Event.current.keyCode != KeyCode.Tab)
			return;
		
		Event.current.Use();
		string text = GUI.GetNameOfFocusedControl();
		int index = text.NullOrEmpty() ? 0 : focusableControlNames.IndexOf(text);
		++index;
		index %= focusableControlNames.Count;
		
		GUI.FocusControl(focusableControlNames[index]);
	}
}
