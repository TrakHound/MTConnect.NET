// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Assets.QIF
{
    public partial class QIFDocumentWrapperAsset
    {
        public const string TypeId = "QIFDocumentWrapper";


        public QIFDocumentWrapperAsset()
        {
            Type = TypeId;
        }


        //protected override IAsset OnProcess(Version mtconnectVersion)
        //{
        //    if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version18)
        //    {
        //        return this;
        //    }

        //    return null;
        //}

        //public override AssetValidationResult IsValid(Version mtconnectVersion)
        //{
        //    var message = "";
        //    var result = true;

        //    if (string.IsNullOrEmpty(Form))
        //    {
        //        message = "Form property is Required";
        //        result = false;
        //    }
        //    else
        //    {
        //        if (Material != null && string.IsNullOrEmpty(Material.Type))
        //        {
        //            message = "Material Type property is Required";
        //            result = false;
        //        }
        //    }

        //    return new AssetValidationResult(result, message);
        //}

        //public override string GenerateHash()
        //{
        //    return GenerateHash(this);
        //}

        //public static string GenerateHash(FileAsset asset)
        //{
        //    if (asset != null)
        //    {
        //        var ids = new List<string>();

        //        ids.Add(ObjectExtensions.GetHashPropertyString(asset).ToSHA1Hash());

        //        // Need to include CuttingItems

        //        return StringFunctions.ToSHA1Hash(ids.ToArray());
        //    }

        //    return null;
        //}
    }
}
