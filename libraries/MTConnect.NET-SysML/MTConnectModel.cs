using MTConnect.SysML.Models.Assets;
using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Models.Observations;
using MTConnect.SysML.Xmi;
using System.Collections.Generic;
using System.Threading;

namespace MTConnect.SysML
{
    /// <summary>
    /// In-memory representation of an MTConnect SysML XMI document, parsed
    /// into the four top-level information models the generator consumes
    /// (Devices, Observations, Assets, Interfaces). Produced by
    /// <see cref="Parse(string)"/>.
    /// </summary>
    public class MTConnectModel
    {
        public MTConnectDeviceInformationModel DeviceInformationModel { get; set; }

        public MTConnectObservationInformationModel ObservationInformationModel { get; set; }

        public MTConnectAssetInformationModel AssetInformationModel { get; set; }

        public MTConnectInterfaceInformationModel IntefaceInformationModel { get; set; }


        /// <summary>
        /// Loads the XMI file at <paramref name="xmiPath"/>, deserializes
        /// it via <see cref="Xmi.XmiDeserializer"/>, builds the four
        /// information models, and runs the cross-package parent resolver
        /// (<see cref="MTConnectClassModel.ResolveDanglingParents"/>) so
        /// classes whose generalization points into a different SysML
        /// package still compile in the local namespace.
        /// </summary>
        /// <param name="xmiPath">Absolute path to the SysML XMI file.</param>
        /// <returns>
        /// A populated <see cref="MTConnectModel"/>; <c>null</c> if
        /// <paramref name="xmiPath"/> is null/empty or the deserializer
        /// cannot produce a document.
        /// </returns>
        public static MTConnectModel Parse(string xmiPath)
        {
            if (!string.IsNullOrEmpty(xmiPath))
            {
                var deserializer = XmiDeserializer.FromFile(xmiPath);
                var doc = deserializer.Deserialize(CancellationToken.None);
                if (doc != null)
                {
                    var mtconnectModel = new MTConnectModel();

                    mtconnectModel.DeviceInformationModel = new MTConnectDeviceInformationModel(doc);
                    mtconnectModel.ObservationInformationModel = new MTConnectObservationInformationModel(doc);
                    mtconnectModel.AssetInformationModel = new MTConnectAssetInformationModel(doc);
                    mtconnectModel.IntefaceInformationModel = new MTConnectInterfaceInformationModel(doc);

                    // Universal post-parse fix-up. Each per-package parser above only walks its own sub-tree of
                    // the XMI, so a class whose generalization points outside the local sub-tree (e.g. v2.7's
                    // Devices.Configurations.AxisDataSet ⇒ Observation.Representations.DataSet) is invisible to
                    // the per-package parser and never reaches the generator. The result is a generated class that
                    // references a non-existent C# parent type and the build fails.
                    //
                    // ResolveDanglingParents scans every Classes list, finds parent references absent from the
                    // local set, looks them up globally by xmi:id, and grafts them into the same list under the
                    // same idPrefix. No-op when there are no dangling parents, so it costs nothing on older XMIs
                    // (preserves the v2.5 dry-run zero-diff guarantee) and absorbs future XMI additions without
                    // code changes here. CollectClassLists is the single place to register additional sub-models'
                    // class lists if/when they begin to surface dangling references.
                    foreach (var (classes, idPrefix) in CollectClassLists(mtconnectModel))
                    {
                        MTConnectClassModel.ResolveDanglingParents(doc, classes, idPrefix);
                    }

                    return mtconnectModel;
                }
            }

            return null;
        }

        private static IEnumerable<(List<MTConnectClassModel> Classes, string IdPrefix)> CollectClassLists(MTConnectModel model)
        {
            var device = model.DeviceInformationModel;
            if (device != null)
            {
                if (device.DataItems?.Classes != null) yield return (device.DataItems.Classes, "Devices");
                if (device.Configurations?.Classes != null) yield return (device.Configurations.Classes, "Devices.Configurations");
                if (device.References?.Classes != null) yield return (device.References.Classes, "Devices.References");
            }
        }
    }
}
