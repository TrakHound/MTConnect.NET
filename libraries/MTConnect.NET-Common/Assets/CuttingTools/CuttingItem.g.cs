// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_99183806_F570_4387_BA6D_34929C20F954

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Part of of the tool that physically removes the material from the workpiece by shear deformation.
    /// </summary>
    public partial class CuttingItem : ICuttingItem
    {
        public const string DescriptionText = "Part of of the tool that physically removes the material from the workpiece by shear deformation.";


        /// <summary>
        /// Status of the cutting tool.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.CutterStatusType> CutterStatus { get; set; }
        
        /// <summary>
        /// Free-form description of the cutting item.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Material composition for this cutting item.
        /// </summary>
        public string Grade { get; set; }
        
        /// <summary>
        /// Number or numbers representing the individual cutting item or items on the tool.Indices **SHOULD** start numbering with the inserts or CuttingItem furthest from the gauge line and increasing in value as the items get closer to the gauge line. Items at the same distance **MAY** be arbitrarily numbered.> Note: In XML, the representation **MUST** be a single number ('1') or a comma separated set of individual elements ('1,2,3,4'), or as a inclusive range of values as in ('1-10') or any combination of ranges and numbers as in '1-4,6-10,22'. There **MUST NOT** be spaces or non-integer values in the text representation.
        /// </summary>
        public string Indices { get; set; }
        
        /// <summary>
        /// Manufacturer identifier of this cutting item.
        /// </summary>
        public string ItemId { get; set; }
        
        /// <summary>
        /// The tool life measured in tool wear.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.IItemLife> ItemLife { get; set; }
        
        /// <summary>
        /// Free form description of the location on the cutting tool.For clarity, the words `FLUTE`, `INSERT`, and `CARTRIDGE` **SHOULD** be used to assist in noting the location of a CuttingItem. Locus **MAY** be any free form string, but **SHOULD** adhere to the following rules:* The location numbering **SHOULD** start at the furthest CuttingItem and work it’s way back to the CuttingItem closest to the gauge line.* Flutes **SHOULD** be identified as such using the word `FLUTE`:. For example: `FLUTE`: 1, `INSERT`: 2 - would indicate the first flute and the second furthest insert from the end of the tool on that flute.* Other designations such as `CARTRIDGE` **MAY** be included, but should be identified using upper case and followed by a colon (:).
        /// </summary>
        public string Locus { get; set; }
        
        /// <summary>
        /// Manufacturers of the cutting item.This will reference the tool item and adaptive items specifically. The cutting itemsmanufacturers’ will be a property of CuttingItem.> Note: In XML, the representation **MUST** be a comma(,) delimited list of manufacturer names. See CuttingItem Schema Diagrams.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Manufacturers { get; set; }
        
        /// <summary>
        /// A collection of measurements relating to this cutting item.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.IMeasurement> Measurements { get; set; }
        
        /// <summary>
        /// Tool group this item is assigned in the part program.
        /// </summary>
        public string ProgramToolGroup { get; set; }
    }
}