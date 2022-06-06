// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Models.Assets;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;

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
            get => DataItemManager.GetDataItemValue<Execution>(Devices.DataItems.Events.ExecutionDataItem.TypeId);
            set => DataItemManager.AddDataItem(new ExecutionDataItem(Id), value);
        }
        public IDataItemModel ExecutionDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.ExecutionDataItem.TypeId);

        /// <summary>
        /// An indication of the reason that EXECUTION is reporting a value of WAIT.
        /// </summary>
        public WaitState WaitState
        {
            get => DataItemManager.GetDataItemValue<WaitState>(Devices.DataItems.Events.WaitStateDataItem.TypeId);
            set => DataItemManager.AddDataItem(new WaitStateDataItem(Id), value);
        }
        public IDataItemModel WaitStateDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.WaitStateDataItem.TypeId);


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
            get => DataItemManager.GetDataItemValue(Devices.DataItems.Events.WorkOffsetDataItem.TypeId);
            set => DataItemManager.AddDataItem(new WorkOffsetDataItem(Id), value);
        }
        public IDataItemModel WorkOffsetDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.WorkOffsetDataItem.TypeId);


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
            get => DataItemManager.GetDataItemValue(Devices.DataItems.Events.ProgramNestLevelDataItem.TypeId).ToInt();
            set => DataItemManager.AddDataItem(new ProgramNestLevelDataItem(Id), value);
        }
        public IDataItemModel ProgramNestLevelDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.ProgramNestLevelDataItem.TypeId);

        /// <summary>
        /// An indication of the status of the Controller components program editing mode.
        /// </summary>
        public ProgramEdit ProgramEdit
        {
            get => DataItemManager.GetDataItemValue<ProgramEdit>(Devices.DataItems.Events.ProgramEditDataItem.TypeId);
            set => DataItemManager.AddDataItem(new ProgramEditDataItem(Id), value);
        }
        public IDataItemModel ProgramEditDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.ProgramEditDataItem.TypeId);

        /// <summary>
        /// The name of the program being edited.
        /// </summary>
        public string ProgramEditName
        {
            get => DataItemManager.GetDataItemValue(Devices.DataItems.Events.ProgramEditNameDataItem.TypeId);
            set => DataItemManager.AddDataItem(new ProgramEditNameDataItem(Id), value);
        }
        public IDataItemModel ProgramEditNameDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.ProgramEditNameDataItem.TypeId);

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
            get => DataItemManager.GetDataItemValue(Devices.DataItems.Events.LineLabelDataItem.TypeId);
            set => DataItemManager.AddDataItem(new LineLabelDataItem(Id), value);
        }
        public IDataItemModel LineLabelDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.LineLabelDataItem.TypeId);

        /// <summary>
        /// The total count of the number of blocks of program code that have been executed since execution started.
        /// </summary>
        public int BlockCount
        {
            get => DataItemManager.GetDataItemValue(Devices.DataItems.Events.BlockCountDataItem.TypeId).ToInt();
            set => DataItemManager.AddDataItem(new BlockCountDataItem(Id), value);
        }
        public IDataItemModel BlockCountDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.BlockCountDataItem.TypeId);

        /// <summary>
        /// The line of code or command being executed by a Controller Structural Element.
        /// </summary>
        public string Block
        {
            get => DataItemManager.GetDataItemValue(Devices.DataItems.Events.BlockDataItem.TypeId);
            set => DataItemManager.AddDataItem(new BlockDataItem(Id), value);
        }
        public IDataItemModel BlockDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.BlockDataItem.TypeId);

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

            x.Number = DataItemManager.GetDataItemValue(ToolNumberDataItem.TypeId);
            x.NumberDataItem = DataItemManager.GetDataItem(ToolNumberDataItem.TypeId);

            x.Group = DataItemManager.GetDataItemValue(ToolGroupDataItem.TypeId);
            x.GroupDataItem = DataItemManager.GetDataItem(ToolGroupDataItem.TypeId);

            var offsetLength = DataItemManager.GetDataItem(ToolOffsetDataItem.TypeId, ToolOffsetDataItem.SubTypes.LENGTH.ToString());
            var offsetRadial = DataItemManager.GetDataItem(ToolOffsetDataItem.TypeId, ToolOffsetDataItem.SubTypes.RADIAL.ToString());

            if (offsetLength != null || offsetRadial != null)
            {
                x.Offset = new ToolOffsetModel
                {
                    Length = offsetLength != null ? DataItemManager.GetDataItemValue(ToolOffsetDataItem.TypeId, ToolOffsetDataItem.SubTypes.LENGTH.ToString()) : null,
                    LengthDataItem = offsetLength,
                    Radial = offsetRadial != null ? DataItemManager.GetDataItemValue(ToolOffsetDataItem.TypeId, ToolOffsetDataItem.SubTypes.RADIAL.ToString()) : null,
                    RadialDataItem = offsetRadial
                };
            }

            x.AssetId = DataItemManager.GetDataItemValue(ToolAssetIdDataItem.TypeId);
            return x;
        }

        private void SetCuttingTool(CuttingToolModel tool)
        {
            if (tool != null)
            {
                DataItemManager.AddDataItem(new ToolNumberDataItem(Id), tool.Number);
                DataItemManager.AddDataItem(new ToolGroupDataItem(Id), tool.Group);
                DataItemManager.AddDataItem(new ToolOffsetDataItem(Id, ToolOffsetDataItem.SubTypes.LENGTH), tool.Offset?.Length);
                DataItemManager.AddDataItem(new ToolOffsetDataItem(Id, ToolOffsetDataItem.SubTypes.RADIAL), tool.Offset?.Radial);
                DataItemManager.AddDataItem(new ToolAssetIdDataItem(Id), tool.AssetId);
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
            x.Program = DataItemManager.GetDataItemValue(ProgramDataItem.TypeId, programSubType.ToString());
            x.ProgramDataItem = DataItemManager.GetDataItem(ProgramDataItem.TypeId, programSubType.ToString());

            // Header
            x.Header = DataItemManager.GetDataItemValue(ProgramHeaderDataItem.TypeId, headerSubType.ToString());
            x.HeaderDataItem = DataItemManager.GetDataItem(ProgramHeaderDataItem.TypeId, headerSubType.ToString());

            // Comment
            x.Comment = DataItemManager.GetDataItemValue(ProgramCommentDataItem.TypeId, commentSubType.ToString());
            x.CommentDataItem = DataItemManager.GetDataItem(ProgramCommentDataItem.TypeId, commentSubType.ToString());

            // Location
            x.Location = DataItemManager.GetDataItemValue(ProgramLocationDataItem.TypeId, locationSubType.ToString());
            x.LocationDataItem = DataItemManager.GetDataItem(ProgramLocationDataItem.TypeId, locationSubType.ToString());

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
                DataItemManager.AddDataItem(new ProgramDataItem(Id, programSubType), program?.Program);

                // Header
                DataItemManager.AddDataItem(new ProgramHeaderDataItem(Id, headerSubType), program?.Header);

                // Comment
                DataItemManager.AddDataItem(new ProgramCommentDataItem(Id, commentSubType), program?.Comment);

                // Location
                DataItemManager.AddDataItem(new ProgramLocationDataItem(Id, locationSubType), program?.Location);
            }
        }


        private LineNumberModel GetLineNumber()
        {
            var x = new LineNumberModel();

            x.Absolute = DataItemManager.GetDataItemValue(LineNumberDataItem.TypeId, LineNumberDataItem.SubTypes.ABSOLUTE.ToString()).ToInt();
            x.AbsoluteDataItem = DataItemManager.GetDataItem(LineNumberDataItem.TypeId, LineNumberDataItem.SubTypes.ABSOLUTE.ToString());

            x.Incremental = DataItemManager.GetDataItemValue(LineNumberDataItem.TypeId, LineNumberDataItem.SubTypes.INCREMENTAL.ToString()).ToInt();
            x.IncrementalDataItem = DataItemManager.GetDataItem(LineNumberDataItem.TypeId, LineNumberDataItem.SubTypes.INCREMENTAL.ToString());

            return x;

        }

        private void SetLineNumber(LineNumberModel lineNumber)
        {
            if (lineNumber != null)
            {
                DataItemManager.AddDataItem(new LineNumberDataItem(Id, LineNumberDataItem.SubTypes.ABSOLUTE), lineNumber?.Absolute);
                DataItemManager.AddDataItem(new LineNumberDataItem(Id, LineNumberDataItem.SubTypes.INCREMENTAL), lineNumber?.Incremental);
            }
        }


        private PathPositionModel GetPathPosition(DataItemCoordinateSystem coordinateSystem)
        {
            var x = new PathPositionModel();

            //x.Actual = GetSampleValue<PathPositionValue>(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.ACTUAL, coordinateSystem));
            //x.ActualDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.ACTUAL, coordinateSystem));

            //x.Commanded = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.COMMANDED, coordinateSystem));
            //x.CommandedDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.COMMANDED, coordinateSystem));

            //x.Programmed = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROGRAMMED, coordinateSystem));
            //x.ProgrammedDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROGRAMMED, coordinateSystem));

            //x.Probe = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROBE, coordinateSystem));
            //x.ProbeDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.PROBE, coordinateSystem));

            //x.Target = (PathPositionValue)GetSampleValue(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.TARGET, coordinateSystem));
            //x.TargetDataItem = GetDataItem(PathPositionDataItem.NameId, PathPositionDataItem.GetSubTypeId(PathPositionDataItem.SubTypes.TARGET, coordinateSystem));

            return x;
        }

        private void SetPathPosition(PathPositionModel model, DataItemCoordinateSystem coordinateSystem)
        {
            if (model != null)
            {
                DataItemManager.AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.ACTUAL, coordinateSystem), model.Actual);
                DataItemManager.AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.COMMANDED, coordinateSystem), model.Commanded);
                DataItemManager.AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.PROGRAMMED, coordinateSystem), model.Programmed);
                DataItemManager.AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.PROBE, coordinateSystem), model.Probe);
                DataItemManager.AddDataItem(new PathPositionDataItem(Id, PathPositionDataItem.SubTypes.TARGET, coordinateSystem), model.Target);
            }
        }


        private PathFeedrateModel GetPathFeedrate()
        {
            var x = new PathFeedrateModel();

            x.Actual = (PathFeedrateValue)DataItemManager.GetSampleValue(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.ACTUAL.ToString());
            x.ActualDataItem = DataItemManager.GetDataItem(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.ACTUAL.ToString());

            x.Commanded = (PathFeedrateValue)DataItemManager.GetSampleValue(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.COMMANDED.ToString());
            x.CommandedDataItem = DataItemManager.GetDataItem(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.COMMANDED.ToString());

            x.Programmed = (PathFeedrateValue)DataItemManager.GetSampleValue(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.PROGRAMMED.ToString());
            x.ProgrammedDataItem = DataItemManager.GetDataItem(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.PROGRAMMED.ToString());

            x.Rapid = (PathFeedrateValue)DataItemManager.GetSampleValue(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.RAPID.ToString());
            x.RapidDataItem = DataItemManager.GetDataItem(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.RAPID.ToString());

            x.Jog = (PathFeedrateValue)DataItemManager.GetSampleValue(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.JOG.ToString());
            x.JogDataItem = DataItemManager.GetDataItem(PathFeedrateDataItem.TypeId, PathFeedrateDataItem.SubTypes.JOG.ToString());

            return x;

        }

        private void SetPathFeedrate(PathFeedrateModel feedrate)
        {
            if (feedrate != null)
            {
                DataItemManager.AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.ACTUAL), feedrate.Actual);
                DataItemManager.AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.COMMANDED), feedrate.Commanded);
                DataItemManager.AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.PROGRAMMED), feedrate.Programmed);
                DataItemManager.AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.RAPID), feedrate.Rapid);
                DataItemManager.AddDataItem(new PathFeedrateDataItem(Id, PathFeedrateDataItem.SubTypes.JOG), feedrate.Jog);
            }
        }


        private PathFeedrateOverrideModel GetPathFeedrateOverride()
        {
            var x = new PathFeedrateOverrideModel();

            x.Programmed = DataItemManager.GetEventValue<PathFeedrateOverrideValue>(Devices.DataItems.Events.PathFeedrateOverrideDataItem.TypeId, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED.ToString());
            x.ProgrammedDataItem = DataItemManager.GetDataItem(Devices.DataItems.Events.PathFeedrateOverrideDataItem.TypeId, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED.ToString());

            x.Rapid = DataItemManager.GetEventValue<PathFeedrateOverrideValue>(Devices.DataItems.Events.PathFeedrateOverrideDataItem.TypeId, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.RAPID.ToString());
            x.RapidDataItem = DataItemManager.GetDataItem(Devices.DataItems.Events.PathFeedrateOverrideDataItem.TypeId, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.RAPID.ToString());

            x.Jog = DataItemManager.GetEventValue<PathFeedrateOverrideValue>(Devices.DataItems.Events.PathFeedrateOverrideDataItem.TypeId, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.JOG.ToString());
            x.JogDataItem = DataItemManager.GetDataItem(Devices.DataItems.Events.PathFeedrateOverrideDataItem.TypeId, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.JOG.ToString());

            return x;

        }

        private void SetPathFeedrateOverride(PathFeedrateOverrideModel feedrateOverride)
        {
            if (feedrateOverride != null)
            {
                DataItemManager.AddDataItem(new Devices.DataItems.Events.PathFeedrateOverrideDataItem(Id, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED), feedrateOverride?.Programmed);
                DataItemManager.AddDataItem(new Devices.DataItems.Events.PathFeedrateOverrideDataItem(Id, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.RAPID), feedrateOverride?.Rapid);
                DataItemManager.AddDataItem(new Devices.DataItems.Events.PathFeedrateOverrideDataItem(Id, Devices.DataItems.Events.PathFeedrateOverrideDataItem.SubTypes.JOG), feedrateOverride?.Jog);
            }
        }
    }
}
