using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Fo.Pagination;
using Fonet.Fo.Properties;

namespace Fonet.Fo
{
    internal class StandardElementMapping
    {
        public const string Uri = "http://www.w3.org/1999/XSL/Format";

        private static readonly Hashtable FoObjs;

        static StandardElementMapping()
        {
            FoObjs = new Hashtable();

            // Declarations and Pagination and Layout Formatting Objects
            FoObjs.Add( "root", Root.GetMaker() );
            FoObjs.Add( "declarations", Declarations.GetMaker() );
            FoObjs.Add( "color-profile", ColorProfile.GetMaker() );
            FoObjs.Add( "page-sequence", PageSequence.GetMaker() );
            FoObjs.Add( "layout-master-set", LayoutMasterSet.GetMaker() );
            FoObjs.Add( "page-sequence-master", PageSequenceMaster.GetMaker() );
            FoObjs.Add( "single-page-master-reference", SinglePageMasterReference.GetMaker() );
            FoObjs.Add( "repeatable-page-master-reference", RepeatablePageMasterReference.GetMaker() );
            FoObjs.Add( "repeatable-page-master-alternatives", RepeatablePageMasterAlternatives.GetMaker() );
            FoObjs.Add( "conditional-page-master-reference", ConditionalPageMasterReference.GetMaker() );
            FoObjs.Add( "simple-page-master", SimplePageMaster.GetMaker() );
            FoObjs.Add( "region-body", RegionBody.GetMaker() );
            FoObjs.Add( "region-before", RegionBefore.GetMaker() );
            FoObjs.Add( "region-after", RegionAfter.GetMaker() );
            FoObjs.Add( "region-start", RegionStart.GetMaker() );
            FoObjs.Add( "region-end", RegionEnd.GetMaker() );
            FoObjs.Add( "flow", Flow.Flow.GetMaker() );
            FoObjs.Add( "static-content", StaticContent.GetMaker() );
            FoObjs.Add( "title", Title.GetMaker() );

            // Block-level Formatting Objects
            FoObjs.Add( "block", Block.GetMaker() );
            FoObjs.Add( "block-container", BlockContainer.GetMaker() );

            // Inline-level Formatting Objects
            FoObjs.Add( "bidi-override", BidiOverride.GetMaker() );
            FoObjs.Add( "character", Character.GetMaker() );
            FoObjs.Add( "initial-property-set", InitialPropertySet.GetMaker() );
            FoObjs.Add( "external-graphic", ExternalGraphic.GetMaker() );
            FoObjs.Add( "instream-foreign-object", InstreamForeignObject.GetMaker() );
            FoObjs.Add( "inline", Inline.GetMaker() );
            FoObjs.Add( "inline-container", InlineContainer.GetMaker() );
            FoObjs.Add( "leader", Leader.GetMaker() );
            FoObjs.Add( "page-number", PageNumber.GetMaker() );
            FoObjs.Add( "page-number-citation", PageNumberCitation.GetMaker() );

            // Formatting Objects for Tables
            FoObjs.Add( "table-and-caption", TableAndCaption.GetMaker() );
            FoObjs.Add( "table", Table.GetMaker() );
            FoObjs.Add( "table-column", TableColumn.GetMaker() );
            FoObjs.Add( "table-caption", TableCaption.GetMaker() );
            FoObjs.Add( "table-header", TableHeader.GetMaker() );
            FoObjs.Add( "table-footer", TableFooter.GetMaker() );
            FoObjs.Add( "table-body", TableBody.GetMaker() );
            FoObjs.Add( "table-row", TableRow.GetMaker() );
            FoObjs.Add( "table-cell", TableCell.GetMaker() );

            // Formatting Objects for Lists
            FoObjs.Add( "list-block", ListBlock.GetMaker() );
            FoObjs.Add( "list-item", ListItem.GetMaker() );
            FoObjs.Add( "list-item-body", ListItemBody.GetMaker() );
            FoObjs.Add( "list-item-label", ListItemLabel.GetMaker() );

            // Dynamic Effects: Link and Multi Formatting Objects
            FoObjs.Add( "basic-link", BasicLink.GetMaker() );
            FoObjs.Add( "multi-switch", MultiSwitch.GetMaker() );
            FoObjs.Add( "multi-case", MultiCase.GetMaker() );
            FoObjs.Add( "multi-toggle", MultiToggle.GetMaker() );
            FoObjs.Add( "multi-properties", MultiProperties.GetMaker() );
            FoObjs.Add( "multi-property-set", MultiPropertySet.GetMaker() );

            // Out-of-Line Formatting Objects
            FoObjs.Add( "float", Float.GetMaker() );
            FoObjs.Add( "footnote", Footnote.GetMaker() );
            FoObjs.Add( "footnote-body", FootnoteBody.GetMaker() );

            // Other Formatting Objects
            FoObjs.Add( "wrapper", Wrapper.GetMaker() );
            FoObjs.Add( "marker", Marker.GetMaker() );
            FoObjs.Add( "retrieve-marker", RetrieveMarker.GetMaker() );
        }

        public void AddToBuilder( FoTreeBuilder builder )
        {
            builder.AddElementMapping( Uri, FoObjs );
            builder.AddPropertyMapping( Uri, FoPropertyMapping.GetGenericMappings() );
        }
    }
}