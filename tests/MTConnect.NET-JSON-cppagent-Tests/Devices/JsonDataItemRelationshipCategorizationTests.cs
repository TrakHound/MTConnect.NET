// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// Pins the contract that
    /// <see cref="JsonDataItem"/>'s relationship classifier routes each
    /// <c>IAbstractDataItemRelationship</c> instance into the matching
    /// container bucket on <see cref="JsonRelationshipContainer"/>:
    ///   - <see cref="IDataItemRelationship"/>       -> DataItemRelationships
    ///   - <see cref="ISpecificationRelationship"/>  -> SpecificationRelationships
    ///
    /// Lets the F-P-H6 perf optimization replace the
    /// <c>IsAssignableFrom</c> chain with switch pattern matching
    /// without regressing the routing behavior. The Component- and
    /// Device-relationship branches in the existing classifier are
    /// preserved structurally even though those types do not implement
    /// <c>IAbstractDataItemRelationship</c> and so cannot reach the
    /// loop body via <c>DataItem.Relationships</c>.
    /// </summary>
    [TestFixture]
    public class JsonDataItemRelationshipCategorizationTests
    {
        [Test]
        public void Both_abstract_relationship_kinds_route_to_their_matching_container_bucket()
        {
            var dataItem = new DataItemRelationship { Name = "di" };
            var spec = new SpecificationRelationship { Name = "spec" };

            var source = new DataItem
            {
                Id = "id1",
                Type = "TEST",
                Relationships = new List<IAbstractDataItemRelationship>
                {
                    dataItem,
                    spec,
                },
            };

            var json = new JsonDataItem(source);

            Assert.That(json.Relationships, Is.Not.Null,
                "JsonDataItem must populate Relationships when the source has any.");
            Assert.That(json.Relationships.DataItemRelationships, Has.Count.EqualTo(1));
            Assert.That(json.Relationships.SpecificationRelationships, Has.Count.EqualTo(1));
            Assert.That(json.Relationships.ComponentRelationships, Is.Null);
            Assert.That(json.Relationships.DeviceRelationships, Is.Null);
        }

        [Test]
        public void Empty_relationships_collection_leaves_container_null()
        {
            var source = new DataItem
            {
                Id = "id1",
                Type = "TEST",
                Relationships = new List<IAbstractDataItemRelationship>(),
            };

            var json = new JsonDataItem(source);

            Assert.That(json.Relationships, Is.Null,
                "JsonDataItem must skip the relationship container when the source has none.");
        }

        [Test]
        public void Multiple_relationships_of_same_kind_aggregate_in_their_bucket()
        {
            var first = new DataItemRelationship { Name = "d1" };
            var second = new DataItemRelationship { Name = "d2" };
            var source = new DataItem
            {
                Id = "id1",
                Type = "TEST",
                Relationships = new List<IAbstractDataItemRelationship> { first, second },
            };

            var json = new JsonDataItem(source);

            Assert.That(json.Relationships, Is.Not.Null);
            Assert.That(json.Relationships.DataItemRelationships, Has.Count.EqualTo(2));
            Assert.That(json.Relationships.SpecificationRelationships, Is.Null);
            Assert.That(json.Relationships.ComponentRelationships, Is.Null);
            Assert.That(json.Relationships.DeviceRelationships, Is.Null);
        }
    }
}
