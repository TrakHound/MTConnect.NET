// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Models.Assets;
using MTConnect.Models.DataItems;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Path is a Component that represents the information for an independent operation or function within a Controller.
    /// </summary>
    public class PathModel : ComponentModel, IPathModel
    {
        /// <summary>
        /// The execution status of a component.
        /// </summary>
        public Execution Execution
        {
            get => GetDataItemValue<Execution>(DataItem.CreateId(Id, Devices.Events.ExecutionDataItem.NameId));
            set => AddDataItem(new ExecutionDataItem(Id), value);
        }
        public IDataItemModel ExecutionDataItem => GetDataItem(Devices.Events.ExecutionDataItem.NameId);

        /// <summary>
        /// An indication of the reason that EXECUTION is reporting a value of WAIT.
        /// </summary>
        public WaitState WaitState
        {
            get => GetDataItemValue<WaitState>(DataItem.CreateId(Id, Devices.Events.WaitStateDataItem.NameId));
            set => AddDataItem(new WaitStateDataItem(Id), value);
        }
        public IDataItemModel WaitStateDataItem => GetDataItem(Devices.Events.WaitStateDataItem.NameId);


        public CuttingToolModel CuttingTool
        {
            get => GetCuttingTool();
            set => SetCuttingTool(value);
        }


        /// <summary>
        /// A reference to the offset variables for a work piece or part associated with a Path in a Controller type component.
        /// </summary>
        public string WorkOffset
        {
            get => GetStringValue(DataItem.CreateId(Id, Devices.Events.WorkOffsetDataItem.NameId));
            set => AddDataItem(new WorkOffsetDataItem(Id), value);
        }
        public IDataItemModel WorkOffsetDataItem => GetDataItem(Devices.Events.WorkOffsetDataItem.NameId);


        /// <summary>
        /// The identity of the primary logic or motion program currently being executed.It is the starting nest level in a call structure and may contain calls to sub programs.
        /// </summary>
        public ProgramModel MainProgram
        {
            get => GetProgram(ProgramDataItem.SubTypes.MAIN, ProgramHeaderDataItem.SubTypes.MAIN, ProgramCommentDataItem.SubTypes.MAIN, ProgramLocationDataItem.SubTypes.MAIN);
            set => SetProgram(value, ProgramDataItem.SubTypes.MAIN, ProgramHeaderDataItem.SubTypes.MAIN, ProgramCommentDataItem.SubTypes.MAIN, ProgramLocationDataItem.SubTypes.MAIN);
        }

        /// <summary>
        /// The identity of the logic or motion program currently executing.
        /// </summary>
        public ProgramModel ActiveProgram
        {
            get => GetProgram(ProgramDataItem.SubTypes.ACTIVE, ProgramHeaderDataItem.SubTypes.ACTIVE, ProgramCommentDataItem.SubTypes.ACTIVE, ProgramLocationDataItem.SubTypes.ACTIVE);
            set => SetProgram(value, ProgramDataItem.SubTypes.ACTIVE, ProgramHeaderDataItem.SubTypes.ACTIVE, ProgramCommentDataItem.SubTypes.ACTIVE, ProgramLocationDataItem.SubTypes.ACTIVE);
        }

        /// <summary>
        /// The identity of a control program that is used to specify the order of execution of other programs.
        /// </summary>
        public ProgramModel ScheduleProgram
        {
            get => GetProgram(ProgramDataItem.SubTypes.SCHEDULE, ProgramHeaderDataItem.SubTypes.SCHEDULE, ProgramCommentDataItem.SubTypes.SCHEDULE, ProgramLocationDataItem.SubTypes.SCHEDULE);
            set => SetProgram(value, ProgramDataItem.SubTypes.SCHEDULE, ProgramHeaderDataItem.SubTypes.SCHEDULE, ProgramCommentDataItem.SubTypes.SCHEDULE, ProgramLocationDataItem.SubTypes.SCHEDULE);
        }

        /// <summary>
        /// An indication of the nesting level within a control program that is associated with the code or instructions that is currently being executed.
        /// </summary>
        public int ProgramNestLevel
        {
            get => GetStringValue(DataItem.CreateId(Id, Devices.Events.ProgramNestLevelDataItem.NameId)).ToInt();
            set => AddDataItem(new ProgramNestLevelDataItem(Id), value);
        }
        public IDataItemModel ProgramNestLevelDataItem => GetDataItem(Devices.Events.ProgramNestLevelDataItem.NameId);

        /// <summary>
        /// An indication of the status of the Controller components program editing mode.
        /// </summary>
        public ProgramEdit ProgramEdit
        {
            get => GetDataItemValue<ProgramEdit>(DataItem.CreateId(Id, Devices.Events.ProgramEditDataItem.NameId));
            set => AddDataItem(new ProgramEditDataItem(Id), value);
        }
        public IDataItemModel ProgramEditDataItem => GetDataItem(Devices.Events.ProgramEditDataItem.NameId);

        /// <summary>
        /// The name of the program being edited.
        /// </summary>
        public string ProgramEditName
        {
            get => GetStringValue(DataItem.CreateId(Id, Devices.Events.ProgramEditNameDataItem.NameId));
            set => AddDataItem(new ProgramEditNameDataItem(Id), value);
        }
        public IDataItemModel ProgramEditNameDataItem => GetDataItem(Devices.Events.ProgramEditNameDataItem.NameId);

        /// <summary>
        /// A reference to the position of a block of program code within a control program.
        /// </summary>
        public LineNumberModel LineNumber
        {
            get => GetLineNumber();
            set => SetLineNumber(value);
        }

        /// <summary>
        /// An optional identifier for a BLOCK of code in a PROGRAM.
        /// </summary>
        public string LineLabel
        {
            get => GetStringValue(DataItem.CreateId(Id, Devices.Events.LineLabelDataItem.NameId));
            set => AddDataItem(new LineLabelDataItem(Id), value);
        }
        public IDataItemModel LineLabelDataItem => GetDataItem(Devices.Events.LineLabelDataItem.NameId);

        /// <summary>
        /// The total count of the number of blocks of program code that have been executed since execution started.
        /// </summary>
        public int BlockCount
        {
            get => GetStringValue(DataItem.CreateId(Id, Devices.Events.BlockCountDataItem.NameId)).ToInt();
            set => AddDataItem(new BlockCountDataItem(Id), value);
        }
        public IDataItemModel BlockCountDataItem => GetDataItem(Devices.Events.BlockCountDataItem.NameId);

        /// <summary>
        /// The line of code or command being executed by a Controller Structural Element.
        /// </summary>
        public string Block
        {
            get => GetStringValue(DataItem.CreateId(Id, Devices.Events.BlockDataItem.NameId));
            set => AddDataItem(new BlockDataItem(Id), value);
        }
        public IDataItemModel BlockDataItem => GetDataItem(Devices.Events.BlockDataItem.NameId);

        /// <summary>
        /// A measured or calculated position of a control point reported by a piece of equipment. 
        /// </summary>
        public PathPositionModel MachinePathPosition
        {
            get => GetPathPosition(DataItemCoordinateSystem.MACHINE);
            set => SetPathPosition(value, DataItemCoordinateSystem.MACHINE);
        }

        /// <summary>
        /// A measured or calculated position of a control point reported by a piece of equipment. 
        /// </summary>
        public PathPositionModel WorkPathPosition
        {
            get => GetPathPosition(DataItemCoordinateSystem.WORK);
            set => SetPathPosition(value, DataItemCoordinateSystem.WORK);
        }

        /// <summary>
        /// The measurement of the feedrate for the axes, or a single axis, associated with a Path component-a vector.
        /// </summary>
        public PathFeedrateModel PathFeedrate
        {
            get => GetPathFeedrate();
            set => SetPathFeedrate(value);
        }

        /// <summary>
        /// The value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.
        /// </summary>
        public PathFeedrateOverrideModel PathFeedrateOverride
        {
            get => GetPathFeedrateOverride();
            set => SetPathFeedrateOverride(value);
        }



        public PathModel() 
        {
            Type = PathComponent.TypeId;
        }

        public PathModel(string componentId)
        {
            Id = componentId;
            Type = PathComponent.TypeId;
        }


        private CuttingToolModel GetCuttingTool()
        {
            var x = new CuttingToolModel();

            x.Number = GetStringValue(ToolNumberDataItem.NameId);
            x.NumberDataItem = GetDataItem(ToolNumberDataItem.NameId);

            x.Group = GetStringValue(ToolGroupDataItem.NameId);
            x.GroupDataItem = GetDataItem(ToolGroupDataItem.NameId);

            var offsetLength = GetDataItem(ToolOffsetDataItem.NameId, ToolOffsetDataItem.GetSubTypeId(ToolOffsetDataItem.SubTypes.LENGTH));
            var offsetRadial = GetDataItem(ToolOffsetDataItem.NameId, ToolOffsetDataItem.GetSubTypeId(ToolOffsetDataItem.SubTypes.RADIAL));

            if (offsetLength != null || offsetRadial != null)
            {
                x.Offset = new ToolOffsetModel
                {
                    Length = offsetLength != null ? GetStringValue(ToolOffsetDataItem.NameId, ToolOffsetDataItem.GetSubTypeId(ToolOffsetDataItem.SubTypes.LENGTH)) : null,
                    LengthDataItem = offsetLength,
                    Radial = offsetRadial != null ? GetStringValue(ToolOffsetDataItem.NameId, ToolOffsetDataItem.GetSubTypeId(ToolOffsetDataItem.SubTypes.RADIAL)) : null,
                    RadialDataItem = offsetRadial
                };
            }

            x.AssetId = GetStringValue(ToolAssetIdDataItem.NameId);
            return x;
        }

        private void SetCuttingTool(CuttingToolModel tool)
        {
            if (tool != null)
            {
                AddDataItem(new ToolNumberDataItem(Id), tool.Number);
                AddDataItem(new ToolGroupDataItem(Id), tool.Group);
                AddDataItem(new ToolOffsetDataItem(Id, ToolOffsetDataItem.SubTypes.LENGTH), tool.Offset?.Length);
                AddDataItem(new ToolOffsetDataItem(Id, ToolOffsetDataItem.SubTypes.RADIAL), tool.Offset?.Radial);
                AddDataItem(new ToolAssetIdDataItem(Id), tool.AssetId);
            }
        }


        private ProgramModel GetProgram(
            ProgramDataItem.SubTypes programSubType,
            ProgramHeaderDataItem.SubTypes headerSubType,
            ProgramCommentDataItem.SubTypes commentSubType,
            ProgramLocationDataItem.SubTypes locationSubType
            )
        {
            var x = new ProgramModel();

            // Program
            x.Program = GetStringValue(ProgramDataItem.NameId, ProgramDataItem.GetSubTypeId(programSubType));
            x.ProgramDataItem = GetDataItem(ProgramDataItem.NameId, ProgramDataItem.GetSubTypeId(programSubType));

            // Header
            x.Header = GetStringValue(ProgramHeaderDataItem.NameId, ProgramHeaderDataItem.GetSubTypeId(headerSubType));
            x.HeaderDataItem = GetDataItem(ProgramHeaderDataItem.NameId, ProgramHeaderDataItem.GetSubTypeId(headerSubType));

            // Comment
            x.Comment = GetStringValue(ProgramCommentDataItem.NameId, ProgramCommentDataItem.GetSubTypeId(commentSubType));
            x.CommentDataItem = GetDataItem(ProgramCommentDataItem.NameId, ProgramCommentDataItem.GetSubTypeId(commentSubType));

            // Location
            x.Location = GetStringValue(ProgramLocationDataItem.TypeId, ProgramLocationDataItem.GetSubTypeId(locationSubType));
            x.LocationDataItem = GetDataItem(ProgramLocationDataItem.NameId, ProgramLocationDataItem.GetSubTypeId(locationSubType));

            return x;

        }

        private void SetProgram(
            ProgramModel program,
            ProgramDataItem.SubTypes programSubType,
            ProgramHeaderDataItem.SubTypes headerSubType,
            ProgramCommentDataItem.SubTypes commentSubType,
            ProgramLocationDataItem.SubTypes locationSubType
            )
        {
            if (program != null)
            {
                // Program
                AddDataItem(new ProgramDataItem(Id, programSubType), program?.Program);

                // Header
                AddDataItem(new ProgramHeaderDataItem(Id, headerSubType), program?.Header);

                // Comment
                AddDataItem(new ProgramCommentDataItem(Id, commentSubType), program?.Comment);

                // Location
                AddDataItem(new ProgramLocationDataItem(Id, locationSubType), program?.Location);
            }
        }


        private LineNumberModel GetLineNumber()
        {
            var x = new LineNumberModel();

            x.Absolute = GetStringValue(LineNumberDataItem.NameId, LineNumberDataItem.GetSubTypeId(LineNumberDataItem.SubTypes.ABSOLUTE)).ToInt();
            x.AbsoluteDataItem = GetDataItem(LineNumberDataItem.NameId, LineNumberDataItem.GetSubTypeId(LineNumberDataItem.SubTypes.ABSOLUTE));

            x.Incremental = GetStringValue(LineNumberDataItem.NameId, LineNumberDataItem.GetSubTypeId(LineNumberDataItem.SubTypes.INCREMENTAL)).ToInt();
            x.IncrementalDataItem = GetDataItem(LineNumberDataItem.NameId, LineNumberDataItem.GetSubTypeId(LineNumberDataItem.SubTypes.INCREMENTAL));

            return x;

        }

        private void SetLineNumber(LineNumberModel lineNumber)
        {
            if (lineNumber != null)
            {
                AddDataItem(new LineNumberDataItem(Id, LineNumberDataItem.SubTypes.ABSOLUTE), lineNumber?.Absolute);
                AddDataItem(new LineNumberDataItem(Id, LineNumberDataItem.SubTypes.INCREMENTAL), lineNumber?.Incremental);
            }
        }


        private PathPositionModel GetPathPosition(DataItemCoordinateSystem coordinateSystem)
        {
            var x = new PathPositionModel();

            x.Actual = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.ACTUAL, coordinateSystem));
            x.ActualDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.ACTUAL, coordinateSystem));

            x.Commanded = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.COMMANDED, coordinateSystem));
            x.CommandedDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.COMMANDED, coordinateSystem));

            x.Programmed = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROGRAMMED, coordinateSystem));
            x.ProgrammedDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROGRAMMED, coordinateSystem));

            x.Probe = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROBE, coordinateSystem));
            x.ProbeDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROBE, coordinateSystem));

            x.Target = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.TARGET, coordinateSystem));
            x.TargetDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.TARGET, coordinateSystem));

            return x;
        }

        private void SetPathPosition(PathPositionModel model, DataItemCoordinateSystem coordinateSystem)
        {
            if (model != null)
            {
                AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.ACTUAL, coordinateSystem), model.Actual);
                AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.COMMANDED, coordinateSystem), model.Commanded);
                AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.PROGRAMMED, coordinateSystem), model.Programmed);
                AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.PROBE, coordinateSystem), model.Probe);
                AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.TARGET, coordinateSystem), model.Target);
            }
        }


        private PathFeedrateModel GetPathFeedrate()
        {
            var x = new PathFeedrateModel();

            x.Actual = (PathFeedrateValue)GetSampleValue(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.ACTUAL));

            x.Commanded = (PathFeedrateValue)GetSampleValue(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.COMMANDED));

            x.Programmed = (PathFeedrateValue)GetSampleValue(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = GetDataItem(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.PROGRAMMED));

            x.Rapid = (PathFeedrateValue)GetSampleValue(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.RAPID));
            x.RapidDataItem = GetDataItem(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.RAPID));

            x.Jog = (PathFeedrateValue)GetSampleValue(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.JOG));
            x.JogDataItem = GetDataItem(PathFeedrateDataItem.NameId, PathFeedrateDataItem.GetSubTypeId(PathFeedrateDataItem.SubTypes.JOG));

            return x;

        }

        private void SetPathFeedrate(PathFeedrateModel feedrate)
        {
            if (feedrate != null)
            {
                AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.ACTUAL), feedrate.Actual);
                AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.COMMANDED), feedrate.Commanded);
                AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.PROGRAMMED), feedrate.Programmed);
                AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.RAPID), feedrate.Rapid);
                AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.JOG), feedrate.Jog);
            }
        }


        private PathFeedrateOverrideModel GetPathFeedrateOverride()
        {
            var x = new PathFeedrateOverrideModel();

            x.Programmed = GetEventValue<PathFeedrateOverrideValue>(PathFeedrateOverrideDataItem.NameId, PathFeedrateOverrideDataItem.GetSubTypeId(PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = GetDataItem(PathFeedrateOverrideDataItem.NameId, PathFeedrateOverrideDataItem.GetSubTypeId(PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED));

            x.Rapid = GetEventValue<PathFeedrateOverrideValue>(PathFeedrateOverrideDataItem.NameId, PathFeedrateOverrideDataItem.GetSubTypeId(PathFeedrateOverrideDataItem.SubTypes.RAPID));
            x.RapidDataItem = GetDataItem(PathFeedrateOverrideDataItem.NameId, PathFeedrateOverrideDataItem.GetSubTypeId(PathFeedrateOverrideDataItem.SubTypes.RAPID));

            x.Jog = GetEventValue<PathFeedrateOverrideValue>(PathFeedrateOverrideDataItem.NameId, PathFeedrateOverrideDataItem.GetSubTypeId(PathFeedrateOverrideDataItem.SubTypes.JOG));
            x.JogDataItem = GetDataItem(PathFeedrateOverrideDataItem.NameId, PathFeedrateOverrideDataItem.GetSubTypeId(PathFeedrateOverrideDataItem.SubTypes.JOG));

            return x;

        }

        private void SetPathFeedrateOverride(PathFeedrateOverrideModel feedrateOverride)
        {
            if (feedrateOverride != null)
            {
                AddDataItem(new PathFeedrateOverrideDataItem(Id, PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED), feedrateOverride?.Programmed);
                AddDataItem(new PathFeedrateOverrideDataItem(Id, PathFeedrateOverrideDataItem.SubTypes.RAPID), feedrateOverride?.Rapid);
                AddDataItem(new PathFeedrateOverrideDataItem(Id, PathFeedrateOverrideDataItem.SubTypes.JOG), feedrateOverride?.Jog);
            }
        }
    }
}
