using System.Collections;

namespace Fonet.Fo.Properties
{
    internal class FoPropertyMapping
    {
        private static readonly Hashtable Mapping = new Hashtable();

        static FoPropertyMapping()
        {
            Mapping.Add( "source-document", SourceDocumentMaker.Maker( "source-document" ) );
            Mapping.Add( "role", RoleMaker.Maker( "role" ) );
            Mapping.Add( "absolute-position", AbsolutePositionMaker.Maker( "absolute-position" ) );
            Mapping.Add( "top", TopMaker.Maker( "top" ) );
            Mapping.Add( "right", RightMaker.Maker( "right" ) );
            Mapping.Add( "bottom", BottomMaker.Maker( "bottom" ) );
            Mapping.Add( "left", LeftMaker.Maker( "left" ) );
            Mapping.Add( "azimuth", AzimuthMaker.Maker( "azimuth" ) );
            Mapping.Add( "cue-after", CueAfterMaker.Maker( "cue-after" ) );
            Mapping.Add( "cue-before", CueBeforeMaker.Maker( "cue-before" ) );
            Mapping.Add( "elevation", ElevationMaker.Maker( "elevation" ) );
            Mapping.Add( "pause-after", PauseAfterMaker.Maker( "pause-after" ) );
            Mapping.Add( "pause-before", PauseBeforeMaker.Maker( "pause-before" ) );
            Mapping.Add( "pitch", PitchMaker.Maker( "pitch" ) );
            Mapping.Add( "pitch-range", PitchRangeMaker.Maker( "pitch-range" ) );
            Mapping.Add( "play-during", PlayDuringMaker.Maker( "play-during" ) );
            Mapping.Add( "richness", RichnessMaker.Maker( "richness" ) );
            Mapping.Add( "speak", SpeakMaker.Maker( "speak" ) );
            Mapping.Add( "speak-header", SpeakHeaderMaker.Maker( "speak-header" ) );
            Mapping.Add( "speak-numeral", SpeakNumeralMaker.Maker( "speak-numeral" ) );
            Mapping.Add( "speak-punctuation", SpeakPunctuationMaker.Maker( "speak-punctuation" ) );
            Mapping.Add( "speech-rate", SpeechRateMaker.Maker( "speech-rate" ) );
            Mapping.Add( "stress", StressMaker.Maker( "stress" ) );
            Mapping.Add( "voice-family", VoiceFamilyMaker.Maker( "voice-family" ) );
            Mapping.Add( "volume", VolumeMaker.Maker( "volume" ) );
            Mapping.Add( "background-attachment", BackgroundAttachmentMaker.Maker( "background-attachment" ) );
            Mapping.Add( "background-color", BackgroundColorMaker.Maker( "background-color" ) );
            Mapping.Add( "background-image", BackgroundImageMaker.Maker( "background-image" ) );
            Mapping.Add( "background-repeat", BackgroundRepeatMaker.Maker( "background-repeat" ) );
            Mapping.Add( "background-position-horizontal",
                BackgroundPositionHorizontalMaker.Maker( "background-position-horizontal" ) );
            Mapping.Add( "background-position-vertical",
                BackgroundPositionVerticalMaker.Maker( "background-position-vertical" ) );
            Mapping.Add( "border-before-color", BorderBeforeColorMaker.Maker( "border-before-color" ) );
            Mapping.Add( "border-before-style", BorderBeforeStyleMaker.Maker( "border-before-style" ) );
            Mapping.Add( "border-before-width", BorderBeforeWidthMaker.Maker( "border-before-width" ) );
            Mapping.Add( "border-after-color", BorderAfterColorMaker.Maker( "border-after-color" ) );
            Mapping.Add( "border-after-style", BorderAfterStyleMaker.Maker( "border-after-style" ) );
            Mapping.Add( "border-after-width", BorderAfterWidthMaker.Maker( "border-after-width" ) );
            Mapping.Add( "border-start-color", BorderStartColorMaker.Maker( "border-start-color" ) );
            Mapping.Add( "border-start-style", BorderStartStyleMaker.Maker( "border-start-style" ) );
            Mapping.Add( "border-start-width", BorderStartWidthMaker.Maker( "border-start-width" ) );
            Mapping.Add( "border-end-color", BorderEndColorMaker.Maker( "border-end-color" ) );
            Mapping.Add( "border-end-style", BorderEndStyleMaker.Maker( "border-end-style" ) );
            Mapping.Add( "border-end-width", BorderEndWidthMaker.Maker( "border-end-width" ) );
            Mapping.Add( "border-top-color", BorderTopColorMaker.Maker( "border-top-color" ) );
            Mapping.Add( "border-top-style", BorderTopStyleMaker.Maker( "border-top-style" ) );
            Mapping.Add( "border-top-width", BorderTopWidthMaker.Maker( "border-top-width" ) );
            Mapping.Add( "border-bottom-color", BorderBottomColorMaker.Maker( "border-bottom-color" ) );
            Mapping.Add( "border-bottom-style", BorderBottomStyleMaker.Maker( "border-bottom-style" ) );
            Mapping.Add( "border-bottom-width", BorderBottomWidthMaker.Maker( "border-bottom-width" ) );
            Mapping.Add( "border-left-color", BorderLeftColorMaker.Maker( "border-left-color" ) );
            Mapping.Add( "border-left-style", BorderLeftStyleMaker.Maker( "border-left-style" ) );
            Mapping.Add( "border-left-width", BorderLeftWidthMaker.Maker( "border-left-width" ) );
            Mapping.Add( "border-right-color", BorderRightColorMaker.Maker( "border-right-color" ) );
            Mapping.Add( "border-right-style", BorderRightStyleMaker.Maker( "border-right-style" ) );
            Mapping.Add( "border-right-width", BorderRightWidthMaker.Maker( "border-right-width" ) );
            Mapping.Add( "padding-before", PaddingBeforeMaker.Maker( "padding-before" ) );
            Mapping.Add( "padding-after", PaddingAfterMaker.Maker( "padding-after" ) );
            Mapping.Add( "padding-start", PaddingStartMaker.Maker( "padding-start" ) );
            Mapping.Add( "padding-end", PaddingEndMaker.Maker( "padding-end" ) );
            Mapping.Add( "padding-top", PaddingTopMaker.Maker( "padding-top" ) );
            Mapping.Add( "padding-bottom", PaddingBottomMaker.Maker( "padding-bottom" ) );
            Mapping.Add( "padding-left", PaddingLeftMaker.Maker( "padding-left" ) );
            Mapping.Add( "padding-right", PaddingRightMaker.Maker( "padding-right" ) );
            Mapping.Add( "font-family", FontFamilyMaker.Maker( "font-family" ) );
            Mapping.Add( "font-selection-strategy", FontSelectionStrategyMaker.Maker( "font-selection-strategy" ) );
            Mapping.Add( "font-size", FontSizeMaker.Maker( "font-size" ) );
            Mapping.Add( "font-stretch", FontStretchMaker.Maker( "font-stretch" ) );
            Mapping.Add( "font-size-adjust", FontSizeAdjustMaker.Maker( "font-size-adjust" ) );
            Mapping.Add( "font-style", FontStyleMaker.Maker( "font-style" ) );
            Mapping.Add( "font-variant", FontVariantMaker.Maker( "font-variant" ) );
            Mapping.Add( "font-weight", FontWeightMaker.Maker( "font-weight" ) );
            Mapping.Add( "country", CountryMaker.Maker( "country" ) );
            Mapping.Add( "language", LanguageMaker.Maker( "language" ) );
            Mapping.Add( "script", ScriptMaker.Maker( "script" ) );
            Mapping.Add( "hyphenate", HyphenateMaker.Maker( "hyphenate" ) );
            Mapping.Add( "hyphenation-character", HyphenationCharacterMaker.Maker( "hyphenation-character" ) );
            Mapping.Add( "hyphenation-push-character-count",
                HyphenationPushCharacterCountMaker.Maker( "hyphenation-push-character-count" ) );
            Mapping.Add( "hyphenation-remain-character-count",
                HyphenationRemainCharacterCountMaker.Maker( "hyphenation-remain-character-count" ) );
            Mapping.Add( "margin-top", MarginTopMaker.Maker( "margin-top" ) );
            Mapping.Add( "margin-bottom", MarginBottomMaker.Maker( "margin-bottom" ) );
            Mapping.Add( "margin-left", MarginLeftMaker.Maker( "margin-left" ) );
            Mapping.Add( "margin-right", MarginRightMaker.Maker( "margin-right" ) );
            Mapping.Add( "space-before", GenericSpace.Maker( "space-before" ) );
            Mapping.Add( "space-after", GenericSpace.Maker( "space-after" ) );
            Mapping.Add( "start-indent", StartIndentMaker.Maker( "start-indent" ) );
            Mapping.Add( "end-indent", EndIndentMaker.Maker( "end-indent" ) );
            Mapping.Add( "space-end", GenericSpace.Maker( "space-end" ) );
            Mapping.Add( "space-start", GenericSpace.Maker( "space-start" ) );
            Mapping.Add( "relative-position", RelativePositionMaker.Maker( "relative-position" ) );
            Mapping.Add( "alignment-adjust", AlignmentAdjustMaker.Maker( "alignment-adjust" ) );
            Mapping.Add( "alignment-baseline", AlignmentBaselineMaker.Maker( "alignment-baseline" ) );
            Mapping.Add( "baseline-shift", BaselineShiftMaker.Maker( "baseline-shift" ) );
            Mapping.Add( "display-align", DisplayAlignMaker.Maker( "display-align" ) );
            Mapping.Add( "dominant-baseline", DominantBaselineMaker.Maker( "dominant-baseline" ) );
            Mapping.Add( "relative-align", RelativeAlignMaker.Maker( "relative-align" ) );
            Mapping.Add( "block-progression-dimension",
                BlockProgressionDimensionMaker.Maker( "block-progression-dimension" ) );
            Mapping.Add( "content-height", ContentHeightMaker.Maker( "content-height" ) );
            Mapping.Add( "content-width", ContentWidthMaker.Maker( "content-width" ) );
            Mapping.Add( "height", HeightMaker.Maker( "height" ) );
            Mapping.Add( "inline-progression-dimension",
                InlineProgressionDimensionMaker.Maker( "inline-progression-dimension" ) );
            Mapping.Add( "max-height", MaxHeightMaker.Maker( "max-height" ) );
            Mapping.Add( "max-width", MaxWidthMaker.Maker( "max-width" ) );
            Mapping.Add( "min-height", MinHeightMaker.Maker( "min-height" ) );
            Mapping.Add( "min-width", MinWidthMaker.Maker( "min-width" ) );
            Mapping.Add( "scaling", ScalingMaker.Maker( "scaling" ) );
            Mapping.Add( "scaling-method", ScalingMethodMaker.Maker( "scaling-method" ) );
            Mapping.Add( "width", WidthMaker.Maker( "width" ) );
            Mapping.Add( "hyphenation-keep", HyphenationKeepMaker.Maker( "hyphenation-keep" ) );
            Mapping.Add( "hyphenation-ladder-count", HyphenationLadderCountMaker.Maker( "hyphenation-ladder-count" ) );
            Mapping.Add( "last-line-end-indent", LastLineEndIndentMaker.Maker( "last-line-end-indent" ) );
            Mapping.Add( "line-height", LineHeightMaker.Maker( "line-height" ) );
            Mapping.Add( "line-height-shift-adjustment",
                LineHeightShiftAdjustmentMaker.Maker( "line-height-shift-adjustment" ) );
            Mapping.Add( "line-stacking-strategy", LineStackingStrategyMaker.Maker( "line-stacking-strategy" ) );
            Mapping.Add( "linefeed-treatment", LinefeedTreatmentMaker.Maker( "linefeed-treatment" ) );
            Mapping.Add( "white-space-treatment", WhiteSpaceTreatmentMaker.Maker( "white-space-treatment" ) );
            Mapping.Add( "text-align", TextAlignMaker.Maker( "text-align" ) );
            Mapping.Add( "text-align-last", TextAlignLastMaker.Maker( "text-align-last" ) );
            Mapping.Add( "text-indent", TextIndentMaker.Maker( "text-indent" ) );
            Mapping.Add( "white-space-collapse", WhiteSpaceCollapseMaker.Maker( "white-space-collapse" ) );
            Mapping.Add( "wrap-option", WrapOptionMaker.Maker( "wrap-option" ) );
            Mapping.Add( "character", CharacterMaker.Maker( "character" ) );
            Mapping.Add( "letter-spacing", LetterSpacingMaker.Maker( "letter-spacing" ) );
            Mapping.Add( "suppress-at-line-break", SuppressAtLineBreakMaker.Maker( "suppress-at-line-break" ) );
            Mapping.Add( "text-decoration", TextDecorationMaker.Maker( "text-decoration" ) );
            Mapping.Add( "text-shadow", TextShadowMaker.Maker( "text-shadow" ) );
            Mapping.Add( "text-transform", TextTransformMaker.Maker( "text-transform" ) );
            Mapping.Add( "treat-as-word-space", TreatAsWordSpaceMaker.Maker( "treat-as-word-space" ) );
            Mapping.Add( "word-spacing", WordSpacingMaker.Maker( "word-spacing" ) );
            Mapping.Add( "color", ColorMaker.Maker( "color" ) );
            Mapping.Add( "color-profile-name", ColorProfileNameMaker.Maker( "color-profile-name" ) );
            Mapping.Add( "rendering-intent", RenderingIntentMaker.Maker( "rendering-intent" ) );
            Mapping.Add( "clear", ClearMaker.Maker( "clear" ) );
            Mapping.Add( "float", FloatMaker.Maker( "float" ) );
            Mapping.Add( "break-after", GenericBreak.Maker( "break-after" ) );
            Mapping.Add( "break-before", GenericBreak.Maker( "break-before" ) );
            Mapping.Add( "keep-together", KeepTogetherMaker.Maker( "keep-together" ) );
            Mapping.Add( "keep-with-next", KeepWithNextMaker.Maker( "keep-with-next" ) );
            Mapping.Add( "keep-with-previous", KeepWithPreviousMaker.Maker( "keep-with-previous" ) );
            Mapping.Add( "orphans", OrphansMaker.Maker( "orphans" ) );
            Mapping.Add( "widows", WidowsMaker.Maker( "widows" ) );
            Mapping.Add( "clip", ClipMaker.Maker( "clip" ) );
            Mapping.Add( "overflow", OverflowMaker.Maker( "overflow" ) );
            Mapping.Add( "reference-orientation", ReferenceOrientationMaker.Maker( "reference-orientation" ) );
            Mapping.Add( "span", SpanMaker.Maker( "span" ) );
            Mapping.Add( "leader-alignment", LeaderAlignmentMaker.Maker( "leader-alignment" ) );
            Mapping.Add( "leader-pattern", LeaderPatternMaker.Maker( "leader-pattern" ) );
            Mapping.Add( "leader-pattern-width", LeaderPatternWidthMaker.Maker( "leader-pattern-width" ) );
            Mapping.Add( "leader-length", LeaderLengthMaker.Maker( "leader-length" ) );
            Mapping.Add( "rule-style", RuleStyleMaker.Maker( "rule-style" ) );
            Mapping.Add( "rule-thickness", RuleThicknessMaker.Maker( "rule-thickness" ) );
            Mapping.Add( "active-state", ActiveStateMaker.Maker( "active-state" ) );
            Mapping.Add( "auto-restore", AutoRestoreMaker.Maker( "auto-restore" ) );
            Mapping.Add( "case-name", CaseNameMaker.Maker( "case-name" ) );
            Mapping.Add( "case-title", CaseTitleMaker.Maker( "case-title" ) );
            Mapping.Add( "destination-placement-offset",
                DestinationPlacementOffsetMaker.Maker( "destination-placement-offset" ) );
            Mapping.Add( "external-destination", ExternalDestinationMaker.Maker( "external-destination" ) );
            Mapping.Add( "indicate-destination", IndicateDestinationMaker.Maker( "indicate-destination" ) );
            Mapping.Add( "internal-destination", InternalDestinationMaker.Maker( "internal-destination" ) );
            Mapping.Add( "show-destination", ShowDestinationMaker.Maker( "show-destination" ) );
            Mapping.Add( "starting-state", StartingStateMaker.Maker( "starting-state" ) );
            Mapping.Add( "switch-to", SwitchToMaker.Maker( "switch-to" ) );
            Mapping.Add( "target-presentation-context",
                TargetPresentationContextMaker.Maker( "target-presentation-context" ) );
            Mapping.Add( "target-processing-context", TargetProcessingContextMaker.Maker( "target-processing-context" ) );
            Mapping.Add( "target-stylesheet", TargetStylesheetMaker.Maker( "target-stylesheet" ) );
            Mapping.Add( "marker-class-name", MarkerClassNameMaker.Maker( "marker-class-name" ) );
            Mapping.Add( "retrieve-class-name", RetrieveClassNameMaker.Maker( "retrieve-class-name" ) );
            Mapping.Add( "retrieve-position", RetrievePositionMaker.Maker( "retrieve-position" ) );
            Mapping.Add( "retrieve-boundary", RetrieveBoundaryMaker.Maker( "retrieve-boundary" ) );
            Mapping.Add( "format", FormatMaker.Maker( "format" ) );
            Mapping.Add( "grouping-separator", GroupingSeparatorMaker.Maker( "grouping-separator" ) );
            Mapping.Add( "grouping-size", GroupingSizeMaker.Maker( "grouping-size" ) );
            Mapping.Add( "letter-value", LetterValueMaker.Maker( "letter-value" ) );
            Mapping.Add( "blank-or-not-blank", BlankOrNotBlankMaker.Maker( "blank-or-not-blank" ) );
            Mapping.Add( "column-count", ColumnCountMaker.Maker( "column-count" ) );
            Mapping.Add( "column-gap", ColumnGapMaker.Maker( "column-gap" ) );
            Mapping.Add( "extent", ExtentMaker.Maker( "extent" ) );
            Mapping.Add( "flow-name", FlowNameMaker.Maker( "flow-name" ) );
            Mapping.Add( "force-page-count", ForcePageCountMaker.Maker( "force-page-count" ) );
            Mapping.Add( "initial-page-number", InitialPageNumberMaker.Maker( "initial-page-number" ) );
            Mapping.Add( "master-name", MasterNameMaker.Maker( "master-name" ) );
            Mapping.Add( "master-reference", MasterReferenceMaker.Maker( "master-reference" ) );
            Mapping.Add( "maximum-repeats", MaximumRepeatsMaker.Maker( "maximum-repeats" ) );
            Mapping.Add( "media-usage", MediaUsageMaker.Maker( "media-usage" ) );
            Mapping.Add( "odd-or-even", OddOrEvenMaker.Maker( "odd-or-even" ) );
            Mapping.Add( "page-height", PageHeightMaker.Maker( "page-height" ) );
            Mapping.Add( "page-position", PagePositionMaker.Maker( "page-position" ) );
            Mapping.Add( "page-width", PageWidthMaker.Maker( "page-width" ) );
            Mapping.Add( "precedence", PrecedenceMaker.Maker( "precedence" ) );
            Mapping.Add( "region-name", RegionNameMaker.Maker( "region-name" ) );
            Mapping.Add( "border-after-precedence", BorderAfterPrecedenceMaker.Maker( "border-after-precedence" ) );
            Mapping.Add( "border-before-precedence", BorderBeforePrecedenceMaker.Maker( "border-before-precedence" ) );
            Mapping.Add( "border-collapse", BorderCollapseMaker.Maker( "border-collapse" ) );
            Mapping.Add( "border-end-precedence", BorderEndPrecedenceMaker.Maker( "border-end-precedence" ) );
            Mapping.Add( "border-separation", BorderSeparationMaker.Maker( "border-separation" ) );
            Mapping.Add( "border-start-precedence", BorderStartPrecedenceMaker.Maker( "border-start-precedence" ) );
            Mapping.Add( "caption-side", CaptionSideMaker.Maker( "caption-side" ) );
            Mapping.Add( "column-number", ColumnNumberMaker.Maker( "column-number" ) );
            Mapping.Add( "column-width", ColumnWidthMaker.Maker( "column-width" ) );
            Mapping.Add( "empty-cells", EmptyCellsMaker.Maker( "empty-cells" ) );
            Mapping.Add( "ends-row", EndsRowMaker.Maker( "ends-row" ) );
            Mapping.Add( "number-columns-repeated", NumberColumnsRepeatedMaker.Maker( "number-columns-repeated" ) );
            Mapping.Add( "number-columns-spanned", NumberColumnsSpannedMaker.Maker( "number-columns-spanned" ) );
            Mapping.Add( "number-rows-spanned", NumberRowsSpannedMaker.Maker( "number-rows-spanned" ) );
            Mapping.Add( "starts-row", StartsRowMaker.Maker( "starts-row" ) );
            Mapping.Add( "table-layout", TableLayoutMaker.Maker( "table-layout" ) );
            Mapping.Add( "table-omit-footer-at-break", TableOmitFooterAtBreakMaker.Maker( "table-omit-footer-at-break" ) );
            Mapping.Add( "table-omit-header-at-break", TableOmitHeaderAtBreakMaker.Maker( "table-omit-header-at-break" ) );
            Mapping.Add( "direction", DirectionMaker.Maker( "direction" ) );
            Mapping.Add( "glyph-orientation-horizontal",
                GlyphOrientationHorizontalMaker.Maker( "glyph-orientation-horizontal" ) );
            Mapping.Add( "glyph-orientation-vertical",
                GlyphOrientationVerticalMaker.Maker( "glyph-orientation-vertical" ) );
            Mapping.Add( "text-altitude", TextAltitudeMaker.Maker( "text-altitude" ) );
            Mapping.Add( "text-depth", TextDepthMaker.Maker( "text-depth" ) );
            Mapping.Add( "unicode-bidi", UnicodeBidiMaker.Maker( "unicode-bidi" ) );
            Mapping.Add( "writing-mode", WritingModeMaker.Maker( "writing-mode" ) );
            Mapping.Add( "content-type", ContentTypeMaker.Maker( "content-type" ) );
            Mapping.Add( "id", IdMaker.Maker( "id" ) );
            Mapping.Add( "provisional-label-separation",
                ProvisionalLabelSeparationMaker.Maker( "provisional-label-separation" ) );
            Mapping.Add( "provisional-distance-between-starts",
                ProvisionalDistanceBetweenStartsMaker.Maker( "provisional-distance-between-starts" ) );
            Mapping.Add( "ref-id", RefIdMaker.Maker( "ref-id" ) );
            Mapping.Add( "score-spaces", ScoreSpacesMaker.Maker( "score-spaces" ) );
            Mapping.Add( "src", SrcMaker.Maker( "src" ) );
            Mapping.Add( "visibility", VisibilityMaker.Maker( "visibility" ) );
            Mapping.Add( "z-index", ZIndexMaker.Maker( "z-index" ) );
            Mapping.Add( "background", BackgroundMaker.Maker( "background" ) );
            Mapping.Add( "background-position", BackgroundPositionMaker.Maker( "background-position" ) );
            Mapping.Add( "border", BorderMaker.Maker( "border" ) );
            Mapping.Add( "border-bottom", BorderBottomMaker.Maker( "border-bottom" ) );
            Mapping.Add( "border-color", BorderColorMaker.Maker( "border-color" ) );
            Mapping.Add( "border-left", BorderLeftMaker.Maker( "border-left" ) );
            Mapping.Add( "border-right", BorderRightMaker.Maker( "border-right" ) );
            Mapping.Add( "border-style", BorderStyleMaker.Maker( "border-style" ) );
            Mapping.Add( "border-spacing", BorderSpacingMaker.Maker( "border-spacing" ) );
            Mapping.Add( "border-top", BorderTopMaker.Maker( "border-top" ) );
            Mapping.Add( "border-width", BorderWidthMaker.Maker( "border-width" ) );
            Mapping.Add( "cue", CueMaker.Maker( "cue" ) );
            Mapping.Add( "font", FontMaker.Maker( "font" ) );
            Mapping.Add( "margin", MarginMaker.Maker( "margin" ) );
            Mapping.Add( "padding", PaddingMaker.Maker( "padding" ) );
            Mapping.Add( "page-break-after", PageBreakAfterMaker.Maker( "page-break-after" ) );
            Mapping.Add( "page-break-before", PageBreakBeforeMaker.Maker( "page-break-before" ) );
            Mapping.Add( "page-break-inside", PageBreakInsideMaker.Maker( "page-break-inside" ) );
            Mapping.Add( "pause", PauseMaker.Maker( "pause" ) );
            Mapping.Add( "position", PositionMaker.Maker( "position" ) );
            Mapping.Add( "size", SizeMaker.Maker( "size" ) );
            Mapping.Add( "vertical-align", VerticalAlignMaker.Maker( "vertical-align" ) );
            Mapping.Add( "white-space", WhiteSpaceMaker.Maker( "white-space" ) );
            Mapping.Add( "xml:lang", XmlLangMaker.Maker( "xml:lang" ) );
        }

        public static Hashtable GetGenericMappings()
        {
            return Mapping;
        }
    }
}