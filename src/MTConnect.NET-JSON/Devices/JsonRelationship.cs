// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRelationship
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("criticality")]
        public string Criticality { get; set; }

        [JsonPropertyName("idRef")]
        public string IdRef { get; set; }

        [JsonPropertyName("deviceUuidRef")]
        public string DeviceUuidRef { get; set; }

        [JsonPropertyName("assetIdRef")]
        public string AssetIdRef { get; set; }

        [JsonPropertyName("assetType")]
        public string AssetType { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("xLinkType")]
        public string XLinkType { get; set; }


        public JsonRelationship() { }

        public JsonRelationship(IAssetRelationship relationship)
        {
            if (relationship != null)
            {
                Id = relationship.Id;
                Name = relationship.Name;
                AssetIdRef = relationship.AssetIdRef;
                AssetType = relationship.AssetType;
                Criticality = relationship.Criticality.ToString();
                Href = relationship.Href;
            }
        }

        public JsonRelationship(IComponentRelationship relationship)
        {
            if (relationship != null)
            {
                Id = relationship.Id;
                Name = relationship.Name;
                Type = relationship.Type.ToString();
                Criticality = relationship.Criticality.ToString();
                IdRef = relationship.IdRef;
            }
        }

        public JsonRelationship(IDataItemRelationship relationship)
        {
            if (relationship != null)
            {
                Name = relationship.Name;
                Type = relationship.Type.ToString();
                IdRef = relationship.IdRef;
            }
        }

        public JsonRelationship(IDeviceRelationship relationship)
        {
            if (relationship != null)
            {
                Id = relationship.Id;
                Name = relationship.Name;
                Type = relationship.Type.ToString();
                Criticality = relationship.Criticality.ToString();
                DeviceUuidRef = relationship.DeviceUuidRef;
                Role = relationship.Role.ToString();
                Href = relationship.Href;
                XLinkType = relationship.XLinkType;
            }
        }

        public JsonRelationship(ISpecificationRelationship relationship)
        {
            if (relationship != null)
            {
                Name = relationship.Name;
                Type = relationship.Type.ToString();
                IdRef = relationship.IdRef;
            }
        }


        public virtual IAssetRelationship ToAssetRelationship()
        {
            var relationship = new AssetRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.AssetIdRef = AssetIdRef;
            relationship.AssetType = AssetType;
            relationship.Href = Href;
            return relationship;
        }

        public virtual IComponentRelationship ToComponentRelationship()
        {
            var relationship = new ComponentRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<RelationshipType>();
            relationship.IdRef = IdRef;
            return relationship;
        }

        public virtual IDataItemRelationship ToDataItemRelationship()
        {
            var relationship = new DataItemRelationship();
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<DataItemRelationshipType>();
            relationship.IdRef = IdRef;
            return relationship;
        }

        public virtual IDeviceRelationship ToDeviceRelationship()
        {
            var relationship = new DeviceRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<RelationshipType>();
            return relationship;
        }

        public virtual ISpecificationRelationship ToSpecificationRelationship()
        {
            var relationship = new SpecificationRelationship();
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<SpecificationRelationshipType>();
            relationship.IdRef = IdRef;
            return relationship;
        }
    }
}