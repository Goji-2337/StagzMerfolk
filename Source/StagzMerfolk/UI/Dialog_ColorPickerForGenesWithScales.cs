using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

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
	private static readonly int PaletteColumns = Stagz_GeneWithScaleColor.defaultColors.Count + 1;
    private static readonly int PaletteWidth = PaletteColumns * ColorIconSize + (PaletteColumns + 1) * ColorIconPadding;
    private readonly Color oldColor;
	private Color color;
	private bool hsvColorWheelDragging;
	private string[] textfieldBuffers = new string[6];
	private Color textfieldColorBuffer;
	private string previousFocusedControlName;
	private static readonly Vector2 ButSize = new (150f, 38f);
	public override Vector2 InitialSize => new (600f, 610f);
	Rot4 portraitRotation = new (2);
	
	private readonly Pawn pawn;
    
    public Dialog_ColorPickerForGenesWithScales(Pawn pawn)
    {
        forcePause = true;
        absorbInputAroundWindow = true;
        closeOnClickedOutside = true;
        closeOnAccept = false;
        
        this.pawn = pawn;
        color = pawn.GetMerrenScaleColorOrFailsafe();
        oldColor = color;
    }

    private void SaveColor(Color paramcolor)
    {
        pawn.TrySetMerrenScaleColor(paramcolor);
    } 
    
    
    //UI methods, ugh
	//pulled all copied UI-drawing methods into one since I am this close to having a mental break parsing it all
	
	//Possibly useful: Widgets.DrawMenuSection(rect) draws light grey box with white border
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
			if (Widgets.ButtonText(BottomButtonsDivider.NewCol(ButSize.x), "StagzMerfolk_UI_Rotate".Translate()))
			{
				portraitRotation.Rotate(RotationDirection.Counterclockwise);
			}
			if (Widgets.ButtonText(BottomButtonsDivider.NewCol(ButSize.x, HorizontalJustification.Right), "Close".Translate()))
			{
				SaveColor(color);
				Close();
			}

			//I divide the window into right and left sides, and firstly draw the right side since it has all actual interface.
			//In the remaining space to the left, I draw the pawn preview later.
			RectDivider rightCol = layout.NewCol(PaletteWidth, HorizontalJustification.Right);
			
			//ColorPalette;
			using (new TextBlock(TextAnchor.MiddleLeft))
			{
				Color? geneticColor = pawn.genes?.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>()?.def.RenderNodeProperties.First().color;
				if (geneticColor != null)
				{
					//the "Genetic Color" box and label at the top of palette
					RectDivider ColorBoxRowGenetic = rightCol.NewRow(ColorIconSizeWithPadding);
					RectDivider ColorBoxGenetic = ColorBoxRowGenetic.NewCol(ColorIconSizeWithPadding);
					Widgets.ColorBox(ColorBoxGenetic.Rect, ref color, (Color) geneticColor, ColorIconSize, ColorIconPadding, delegate { SaveColor(color);});
					Widgets.Label(ColorBoxRowGenetic.Rect, "StagzMerfolk_UI_GeneticColor".Translate().CapitalizeFirst());
				}
				//the "Old Color" box and label at the top of palette
				RectDivider ColorBoxRow = rightCol.NewRow(ColorIconSizeWithPadding);
				RectDivider ColorBox = ColorBoxRow.NewCol(ColorIconSizeWithPadding);
				Widgets.ColorBox(ColorBox.Rect, ref color, oldColor, ColorIconSize, ColorIconPadding, delegate { SaveColor(color); });
				Widgets.Label(ColorBoxRow.Rect, "OldColor".Translate().CapitalizeFirst());
				//the palette itself
				Widgets.ColorSelector(rightCol.Rect, ref color, Stagz_GeneWithScaleColor.defaultColors,
					out var paletteHeight, null, ColorIconSize, ColorIconPadding, delegate { SaveColor(color); });
				rightCol.NewRow(paletteHeight);
			}

			//just a bit of padding between palette and wheel. Arbitrary size
			rightCol.NewRow(ColorIconSizeWithPadding / 2);
			
			//ColorWheel
			RectDivider ColorWheelDiv = rightCol.NewRow(ColorWheelSize);
			Widgets.HSVColorWheel(
				ColorWheelDiv.Rect.ContractedBy((ColorWheelDiv.Rect.width - ColorWheelSize) / 2f, (ColorWheelDiv.Rect.height - ColorWheelSize) / 2f), 
				ref color,
				ref hsvColorWheelDragging);
			
			//Padding
			rightCol.NewRow(ColorIconSizeWithPadding / 2);
			
			//Value slider
			Rect aggregatorRect = rightCol.NewRow(ColorIconSizeWithPadding);
			Color.RGBToHSV(color, out var H, out var S, out var V);
			Widgets.HorizontalSlider(aggregatorRect, ref V, new FloatRange(0,1));
			color = Color.HSVToRGB(H, S, V);
			
			//ColorTextfields
			RectAggregator aggregator = new (aggregatorRect, ContextHash);
			Widgets.ColorTextfields(ref aggregator, ref color, ref textfieldBuffers, ref textfieldColorBuffer, previousFocusedControlName, "colorTextfields");
			
			//DrawPawn - drawn to the left of everything above, thus uses "layout" as rect
			Rect position = layout.Rect;
			RenderTexture image = PortraitsCache.Get(
				pawn,
				new Vector2(position.width / 2, position.height / 2),
				portraitRotation,
				default,
				1f,
				true,
				true,
				false,
				false,
				null,
				null,
				true);
			GUI.DrawTexture(position, image);
			
			TabControl();
			if (Event.current.type == EventType.Layout)
			{
				previousFocusedControlName = GUI.GetNameOfFocusedControl();
			}
		}
	}

	//there was probably a switch statement in source code but I can't pack the decompiled code into that right now
	private static void TabControl()
	{
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab)
		{
			Event.current.Use();
			string text = GUI.GetNameOfFocusedControl();
			if (text.NullOrEmpty())
			{
				text = focusableControlNames[0];
			}
			int num2 = focusableControlNames.IndexOf(text);
			if (num2 < 0)
			{
				num2 = focusableControlNames.Count;
			}
			num2 += 1;
			if (num2 >= focusableControlNames.Count)
			{
				num2 = 0;
			}
			else if (num2 < 0)
			{
				num2 = focusableControlNames.Count - 1;
			}
			GUI.FocusControl(focusableControlNames[num2]);
		}
	}
}
