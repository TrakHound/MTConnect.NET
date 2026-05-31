// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Relationship</c>.
    /// A single surrogate shape carries every relationship kind (asset,
    /// component, data item, device, and specification); only the properties
    /// relevant to the originating relationship are populated, and the
    /// corresponding <c>ToXxxRelationship</c> method reconstructs the concrete
    /// strongly-typed relationship.
    /// </summary>
    public class JsonRelationship
    {
        /// <summary>
        /// The unique <c>id</c> of the relationship, where the relationship
        /// kind defines one.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The human-readable <c>name</c> of the relationship.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The relationship <c>type</c>, interpreted as the relevant
        /// relationship-type enumeration for the originating kind.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The criticality (CRITICAL or NONCRITICAL) of the relationship.
        /// </summary>
        [JsonPropertyName("criticality")]
        public string Criticality { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the related component, data item, or
        /// specification.
        /// </summary>
        [JsonPropertyName("idRef")]
        public string IdRef { get; set; }

        /// <summary>
        /// Reference to the UUID of the related device, for a device
        /// relationship.
        /// </summary>
        [JsonPropertyName("deviceUuidRef")]
        public string DeviceUuidRef { get; set; }

        /// <summary>
        /// Reference to the <c>assetId</c> of the related asset, for an asset
        /// relationship.
        /// </summary>
        [JsonPropertyName("assetIdRef")]
        public string AssetIdRef { get; set; }

        /// <summary>
        /// The type of the related asset, for an asset relationship.
        /// </summary>
        [JsonPropertyName("assetType")]
        public string AssetType { get; set; }

        /// <summary>
        /// The role (SYSTEM or AUXILIARY) the related device plays, for a
        /// device relationship.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// The hyperlink reference to the related entity.
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The XLink type of the <see cref="Href"/> reference.
        /// </summary>
        [JsonPropertyName("xLinkType")]
        public string XLinkType { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonRelationship() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IAssetRelationship"/>, populating the asset reference
        /// properties.
        /// </summary>
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

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComponentRelationship"/>, populating the component
        /// reference properties.
        /// </summary>
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

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDataItemRelationship"/>, populating the data item
        /// reference properties.
        /// </summary>
        public JsonRelationship(IDataItemRelationship relationship)
        {
            if (relationship != null)
            {
                Name = relationship.Name;
                Type = relationship.Type.ToString();
                IdRef = relationship.IdRef;
            }
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDeviceRelationship"/>, populating the device reference
        /// and XLink properties.
        /// </summary>
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

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISpecificationRelationship"/>, populating the
        /// specification reference properties.
        /// </summary>
        public JsonRelationship(ISpecificationRelationship relationship)
        {
            if (relationship != null)
            {
                Name = relationship.Name;
                Type = relationship.Type.ToString();
                IdRef = relationship.IdRef;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IAssetRelationship"/>.
        /// </summary>
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

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IComponentRelationship"/>, parsing <see cref="Type"/> as
        /// a <see cref="RelationshipType"/>.
        /// </summary>
        public virtual IComponentRelationship ToComponentRelationship()
        {
            var relationship = new ComponentRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<RelationshipType>();
            relationship.IdRef = IdRef;
            return relationship;
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IDataItemRelationship"/>, parsing <see cref="Type"/> as a
        /// <see cref="DataItemRelationshipType"/>.
        /// </summary>
        public virtual IDataItemRelationship ToDataItemRelationship()
        {
            var relationship = new DataItemRelationship();
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<DataItemRelationshipType>();
            relationship.IdRef = IdRef;
            return relationship;
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IDeviceRelationship"/>, parsing <see cref="Type"/> as a
        /// <see cref="RelationshipType"/>.
        /// </summary>
        public virtual IDeviceRelationship ToDeviceRelationship()
        {
            var relationship = new DeviceRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type.ConvertEnum<RelationshipType>();
            return relationship;
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISpecificationRelationship"/>, parsing <see cref="Type"/>
        /// as a <see cref="SpecificationRelationshipType"/>.
        /// </summary>
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