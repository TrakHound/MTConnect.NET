// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Assets;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    public interface IPathModel : IComponentModel
    {
        /// <summary>
        /// The execution status of a component.
        /// </summary>
        Execution Execution { get; set; }
        IDataItemModel ExecutionDataItem { get; }

        /// <summary>
        /// An indication of the reason that EXECUTION is reporting a value of WAIT.
        /// </summary>
        WaitState WaitState { get; set; }
        IDataItemModel WaitStateDataItem { get; }


        CuttingToolModel CuttingTool { get; set; }

        /// <summary>
        /// A reference to the offset variables for a work piece or part associated with a Path in a Controller type component.
        /// </summary>
        string WorkOffset { get; set; }
        IDataItemModel WorkOffsetDataItem { get; }

        /// <summary>
        /// The identity of the primary logic or motion program currently being executed.It is the starting nest level in a call structure and may contain calls to sub programs.
        /// </summary>
        ProgramModel MainProgram { get; set; }

        /// <summary>
        /// The identity of the logic or motion program currently executing.
        /// </summary>
        ProgramModel ActiveProgram { get; set; }

        /// <summary>
        /// The identity of a control program that is used to specify the order of execution of other programs.
        /// </summary>
        ProgramModel ScheduleProgram { get; set; }

        /// <summary>
        /// An indication of the nesting level within a control program that is associated with the code or instructions that is currently being executed.
        /// </summary>
        int ProgramNestLevel { get; set; }
        IDataItemModel ProgramNestLevelDataItem { get; }

        /// <summary>
        /// An indication of the status of the Controller components program editing mode.
        /// </summary>
        ProgramEdit ProgramEdit { get; set; }
        IDataItemModel ProgramEditDataItem { get; }

        /// <summary>
        /// The name of the program being edited.
        /// </summary>
        string ProgramEditName { get; set; }
        IDataItemModel ProgramEditNameDataItem { get; }

        /// <summary>
        /// A reference to the position of a block of program code within a control program.
        /// </summary>
        LineNumberModel LineNumber { get; set; }

        /// <summary>
        /// An optional identifier for a BLOCK of code in a PROGRAM.
        /// </summary>
        string LineLabel { get; set; }
        IDataItemModel LineLabelDataItem { get; }

        /// <summary>
        /// The total count of the number of blocks of program code that have been executed since execution started.
        /// </summary>
        int BlockCount { get; set; }
        IDataItemModel BlockCountDataItem { get; }

        /// <summary>
        /// The line of code or command being executed by a Controller Structural Element.
        /// </summary>
        string Block { get; set; }
        IDataItemModel BlockDataItem { get; }

        /// <summary>
        /// The measurement of the feedrate for the axes, or a single axis, associated with a Path component-a vector.
        /// </summary>
        PathFeedrateModel PathFeedrate { get; set; }

        /// <summary>
        /// The value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.
        /// </summary>
        PathFeedrateOverrideModel PathFeedrateOverride { get; set; }
    }
}
