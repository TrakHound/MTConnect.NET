// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using NUnit.Framework;

namespace MTConnect.Tests.Common
{
    /// <summary>
    /// Pins the contract of <see cref="UnixDateTime"/>,
    /// <see cref="UnixTimeExtensions"/> and their round-trip invariants.
    /// Every mutation-visible constant, branch, and arithmetic step is
    /// covered by at least one assertion so Stryker.NET kills the mutants
    /// Stryker previously reported for this module.
    /// See TrakHound/MTConnect.NET#242.
    /// </summary>
    [TestFixture]
    public class UnixTimeTests
    {
        // --- Constants ---------------------------------------------------------

        /// <summary>Pins the epoch instant to 1970-01-01T00:00:00Z; kills a constant-substitution mutant on the year/month/day/hour/minute/second/Kind arguments.</summary>
        [Test]
        public void EpochTime_IsUnixEpoch_1970_01_01_UtcMidnight()
        {
            var epoch = UnixTimeExtensions.EpochTime;
            Assert.AreEqual(1970, epoch.Year);
            Assert.AreEqual(1, epoch.Month);
            Assert.AreEqual(1, epoch.Day);
            Assert.AreEqual(0, epoch.Hour);
            Assert.AreEqual(0, epoch.Minute);
            Assert.AreEqual(0, epoch.Second);
            Assert.AreEqual(0, epoch.Millisecond);
            Assert.AreEqual(DateTimeKind.Utc, epoch.Kind);
        }

        /// <summary>Pins the <see cref="UnixTimeExtensions.EpochTicks"/> constant against a fresh BCL-computed epoch tick count; kills a constant-value mutant on the literal 621355968000000000.</summary>
        [Test]
        public void EpochTicks_Constant_MatchesBclEpochTicks()
        {
            var bclEpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            Assert.AreEqual(bclEpochTicks, UnixTimeExtensions.EpochTicks);
            Assert.AreEqual(621355968000000000L, UnixTimeExtensions.EpochTicks);
            Assert.AreEqual(UnixTimeExtensions.EpochTime.Ticks, UnixTimeExtensions.EpochTicks);
        }

        // --- ToUnixTime --------------------------------------------------------

        /// <summary>Pins the epoch → 0 mapping; kills off-by-one arithmetic mutants on the epoch subtraction.</summary>
        [Test]
        public void ToUnixTime_AtEpoch_ReturnsZero()
        {
            Assert.AreEqual(0L, UnixTimeExtensions.EpochTime.ToUnixTime());
        }

        /// <summary>Pins the one-tick-after-epoch → 1 mapping; kills constant-substitution mutants on the arithmetic.</summary>
        [Test]
        public void ToUnixTime_OneTickAfterEpoch_ReturnsOne()
        {
            var oneAfter = new DateTime(UnixTimeExtensions.EpochTicks + 1, DateTimeKind.Utc);
            Assert.AreEqual(1L, oneAfter.ToUnixTime());
        }

        /// <summary>Pins the one-tick-before-epoch → -1 mapping; kills a sign/subtract-swap mutant.</summary>
        [Test]
        public void ToUnixTime_OneTickBeforeEpoch_ReturnsNegativeOne()
        {
            var oneBefore = new DateTime(UnixTimeExtensions.EpochTicks - 1, DateTimeKind.Utc);
            Assert.AreEqual(-1L, oneBefore.ToUnixTime());
        }

        /// <summary>Pins the arithmetic on a known-post-epoch Utc value; kills every constant-off arithmetic mutant on the epoch subtraction.</summary>
        [Test]
        public void ToUnixTime_UtcKind_ReturnsUtcTicksMinusEpochTicks()
        {
            var utc = new DateTime(2026, 1, 1, 12, 30, 45, DateTimeKind.Utc);
            var expected = utc.Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, utc.ToUnixTime());
        }

        /// <summary>Pins the Local-branch conversion: a Local DateTime is converted to Utc before subtraction; assertion holds in every timezone.</summary>
        [Test]
        public void ToUnixTime_LocalKind_ConvertsToUtcBeforeSubtracting()
        {
            var local = new DateTime(2026, 1, 1, 12, 30, 45, DateTimeKind.Local);
            var expected = TimeZoneInfo.ConvertTimeToUtc(local, TimeZoneInfo.Local).Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, local.ToUnixTime());
        }

        /// <summary>Pins the Unspecified-Kind behaviour of <see cref="UnixTimeExtensions.ToUnixTime"/>: an Unspecified value is treated as UTC (no conversion), so its ticks are subtracted from epoch verbatim.</summary>
        [Test]
        public void ToUnixTime_UnspecifiedKind_TreatedAsUtc()
        {
            var unspec = new DateTime(2026, 1, 1, 12, 30, 45, DateTimeKind.Unspecified);
            var expected = unspec.Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, unspec.ToUnixTime());
        }

        // --- ToUnixUtcTime -----------------------------------------------------

        /// <summary>Pins the Utc no-op path: a Utc DateTime is returned as its tick offset from epoch unchanged.</summary>
        [Test]
        public void ToUnixUtcTime_UtcKind_ReturnsUtcTicksMinusEpochTicks()
        {
            var utc = new DateTime(2026, 6, 15, 10, 20, 30, DateTimeKind.Utc);
            var expected = utc.Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, utc.ToUnixUtcTime());
        }

        /// <summary>Pins the Local-branch conversion of <see cref="UnixTimeExtensions.ToUnixUtcTime"/>; identical semantics to ToUnixTime for Local inputs.</summary>
        [Test]
        public void ToUnixUtcTime_LocalKind_ConvertsToUtcBeforeSubtracting()
        {
            var local = new DateTime(2026, 6, 15, 10, 20, 30, DateTimeKind.Local);
            var expected = TimeZoneInfo.ConvertTimeToUtc(local, TimeZoneInfo.Local).Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, local.ToUnixUtcTime());
        }

        /// <summary>Pins the default Unspecified path: with no unspecifiedAssume argument, the value is treated as Utc and returned unchanged.</summary>
        [Test]
        public void ToUnixUtcTime_UnspecifiedKind_DefaultsToUtcAssumption()
        {
            var unspec = new DateTime(2026, 6, 15, 10, 20, 30, DateTimeKind.Unspecified);
            var expected = unspec.Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, unspec.ToUnixUtcTime());
        }

        /// <summary>Pins the explicit-Utc unspecifiedAssume path: identical to the default.</summary>
        [Test]
        public void ToUnixUtcTime_UnspecifiedKind_ExplicitUtcAssumption_ReturnsTicksMinusEpoch()
        {
            var unspec = new DateTime(2026, 6, 15, 10, 20, 30, DateTimeKind.Unspecified);
            var expected = unspec.Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, unspec.ToUnixUtcTime(DateTimeKind.Utc));
        }

        /// <summary>Pins the Local unspecifiedAssume path: an Unspecified value is stamped Local, then converted to Utc via the machine timezone. Assertion holds in every timezone.</summary>
        [Test]
        public void ToUnixUtcTime_UnspecifiedKind_LocalAssumption_ConvertsAsLocal()
        {
            var unspec = new DateTime(2026, 6, 15, 10, 20, 30, DateTimeKind.Unspecified);
            var asLocal = DateTime.SpecifyKind(unspec, DateTimeKind.Local);
            var expected = TimeZoneInfo.ConvertTimeToUtc(asLocal, TimeZoneInfo.Local).Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, unspec.ToUnixUtcTime(DateTimeKind.Local));
        }

        /// <summary>Pins the Unspecified/unspecifiedAssume=Unspecified degenerate path: the SpecifyKind result is Unspecified so the inner Local check is false and the ticks pass through verbatim (identical to the Utc default).</summary>
        [Test]
        public void ToUnixUtcTime_UnspecifiedKind_UnspecifiedAssumption_PassesTicksThrough()
        {
            var unspec = new DateTime(2026, 6, 15, 10, 20, 30, DateTimeKind.Unspecified);
            var expected = unspec.Ticks - UnixTimeExtensions.EpochTicks;
            Assert.AreEqual(expected, unspec.ToUnixUtcTime(DateTimeKind.Unspecified));
        }

        /// <summary>Pins the ToUnixUTCTime alias: identical result to <see cref="UnixTimeExtensions.ToUnixUtcTime"/> for every Kind + assumption combination.</summary>
        [Test]
        public void ToUnixUTCTime_Alias_MatchesToUnixUtcTime()
        {
            var utc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var local = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var unspec = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

            Assert.AreEqual(utc.ToUnixUtcTime(), utc.ToUnixUTCTime());
            Assert.AreEqual(local.ToUnixUtcTime(), local.ToUnixUTCTime());
            Assert.AreEqual(unspec.ToUnixUtcTime(), unspec.ToUnixUTCTime());
            Assert.AreEqual(unspec.ToUnixUtcTime(DateTimeKind.Local), unspec.ToUnixUTCTime(DateTimeKind.Local));
            Assert.AreEqual(unspec.ToUnixUtcTime(DateTimeKind.Utc), unspec.ToUnixUTCTime(DateTimeKind.Utc));
        }

        // --- FromUnixTime / ToDateTime / ToLocalDateTime ------------------------

        /// <summary>Pins the FromUnixTime(0) → epoch mapping; kills a constant-substitution mutant on the AddTicks argument.</summary>
        [Test]
        public void FromUnixTime_Zero_ReturnsEpochUtc()
        {
            var d = UnixTimeExtensions.FromUnixTime(0L);
            Assert.AreEqual(UnixTimeExtensions.EpochTime, d);
            Assert.AreEqual(DateTimeKind.Utc, d.Kind);
        }

        /// <summary>Pins the FromUnixTime arithmetic on a known-positive tick count.</summary>
        [Test]
        public void FromUnixTime_PositiveTicks_AddsToEpoch()
        {
            var ticks = 12345678901234L;
            var d = UnixTimeExtensions.FromUnixTime(ticks);
            Assert.AreEqual(UnixTimeExtensions.EpochTime.AddTicks(ticks), d);
            Assert.AreEqual(UnixTimeExtensions.EpochTicks + ticks, d.Ticks);
            Assert.AreEqual(DateTimeKind.Utc, d.Kind);
        }

        /// <summary>Pins the FromUnixTime arithmetic on a negative tick count (pre-epoch instant).</summary>
        [Test]
        public void FromUnixTime_NegativeTicks_SubtractsFromEpoch()
        {
            var d = UnixTimeExtensions.FromUnixTime(-1L);
            Assert.AreEqual(new DateTime(UnixTimeExtensions.EpochTicks - 1, DateTimeKind.Utc), d);
            Assert.AreEqual(DateTimeKind.Utc, d.Kind);
        }

        /// <summary>Pins the ToDateTime alias: identical result to <see cref="UnixTimeExtensions.FromUnixTime"/>.</summary>
        [Test]
        public void ToDateTime_LongExtension_MatchesFromUnixTime()
        {
            long[] samples = { 0L, 1L, -1L, 621355968000000000L, -621355968000000000L, 12345678901234L };
            foreach (var ticks in samples)
            {
                Assert.AreEqual(UnixTimeExtensions.FromUnixTime(ticks), ticks.ToDateTime());
            }
        }

        /// <summary>Pins the ToLocalDateTime conversion: the returned instant equals FromUnixTime(ticks).ToLocalTime() and has Kind=Local.</summary>
        [Test]
        public void ToLocalDateTime_LongExtension_ReturnsLocalKindAndCorrectInstant()
        {
            long[] samples = { 0L, 12345678901234L };
            foreach (var ticks in samples)
            {
                var actual = ticks.ToLocalDateTime();
                var expected = UnixTimeExtensions.FromUnixTime(ticks).ToLocalTime();
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(DateTimeKind.Local, actual.Kind);
            }
        }

        // --- Round-trip invariants --------------------------------------------

        /// <summary>Pins ToUnixTime ∘ FromUnixTime = identity on Utc DateTimes across a spread of instants including epoch, pre-epoch, and MTConnect's contemporary window.</summary>
        [Test]
        public void RoundTrip_UtcDateTime_ToUnixTime_ThenFromUnixTime_IsIdentity()
        {
            DateTime[] samples =
            {
                UnixTimeExtensions.EpochTime,
                new DateTime(1969, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 15, 12, 30, 45, DateTimeKind.Utc),
                new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };
            foreach (var d in samples)
            {
                var round = UnixTimeExtensions.FromUnixTime(d.ToUnixTime());
                Assert.AreEqual(d, round);
                Assert.AreEqual(d.Kind, round.Kind);
            }
        }

        /// <summary>Pins ToUnixTime ∘ FromUnixTime = identity on Unspecified DateTimes (treated as Utc): round-trip preserves the tick value, though Kind normalises to Utc on the return.</summary>
        [Test]
        public void RoundTrip_UnspecifiedDateTime_ToUnixTime_ThenFromUnixTime_PreservesTicks()
        {
            var unspec = new DateTime(2026, 6, 15, 12, 30, 45, DateTimeKind.Unspecified);
            var round = UnixTimeExtensions.FromUnixTime(unspec.ToUnixTime());
            Assert.AreEqual(unspec.Ticks, round.Ticks);
            Assert.AreEqual(DateTimeKind.Utc, round.Kind);
        }

        // --- UnixDateTime.Now --------------------------------------------------

        /// <summary>Pins <see cref="UnixDateTime.Now"/> to a bounded window around the harness-observed BCL UtcNow; kills constant-return mutants (Now → 0, Now → 1, etc.) and off-by-epoch mutants.</summary>
        [Test]
        public void Now_ReturnsCurrentUtcInstantWithinTwoSeconds()
        {
            var before = DateTime.UtcNow.ToUnixTime();
            var now = UnixDateTime.Now;
            var after = DateTime.UtcNow.ToUnixTime();

            // Two-second window in each direction absorbs GC / scheduler stalls without ever
            // spanning the epoch offset a mutation could introduce (epoch offset ~ 6e17 ticks).
            var twoSecondsInTicks = TimeSpan.FromSeconds(2).Ticks;
            Assert.That(now, Is.GreaterThanOrEqualTo(before - twoSecondsInTicks));
            Assert.That(now, Is.LessThanOrEqualTo(after + twoSecondsInTicks));
        }

        /// <summary>Pins that <see cref="UnixDateTime.Now"/> is monotone non-decreasing between two consecutive reads on the same thread.</summary>
        [Test]
        public void Now_TwoConsecutiveReads_AreMonotoneNonDecreasing()
        {
            var first = UnixDateTime.Now;
            var second = UnixDateTime.Now;
            Assert.That(second, Is.GreaterThanOrEqualTo(first));
        }
    }
}
