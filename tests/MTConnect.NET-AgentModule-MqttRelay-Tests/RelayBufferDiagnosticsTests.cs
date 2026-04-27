// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the relay-buffer "missed observations" diagnostic
    /// computation. The previous Module implementation computed
    ///   long missed = (long)(to - lastSent);
    /// using ulong arithmetic. When <c>lastSent &gt; to</c> (a
    /// degenerate but possible state, for example when a persisted
    /// last-sent-sequence file is stale relative to a freshly-restarted
    /// broker whose sequence has rolled), the unsigned subtraction
    /// underflows and the cast to <c>long</c> produces a huge spurious
    /// "missed" figure in the diagnostic log line.
    ///
    /// The corrected helper computes <c>missed</c> only when
    /// <c>lastSent &lt;= to</c>; otherwise it returns 0 so the
    /// diagnostic stays meaningful and the operator does not see
    /// nonsense numbers.
    /// </summary>
    [TestFixture]
    public class RelayBufferDiagnosticsTests
    {
        [Test]
        public void ComputeMissed_returns_zero_when_last_sent_above_to()
        {
            Assert.That(
                RelayBufferDiagnostics.ComputeMissed(to: 5UL, lastSent: 10UL),
                Is.Zero,
                "Underflow guard: when lastSent > to, missed must be 0.");
        }

        [Test]
        public void ComputeMissed_returns_zero_when_last_sent_equals_to()
        {
            Assert.That(
                RelayBufferDiagnostics.ComputeMissed(to: 5UL, lastSent: 5UL),
                Is.Zero);
        }

        [Test]
        public void ComputeMissed_returns_difference_when_last_sent_below_to()
        {
            Assert.That(
                RelayBufferDiagnostics.ComputeMissed(to: 100UL, lastSent: 25UL),
                Is.EqualTo(75L));
        }

        [Test]
        public void ComputeMissed_handles_zero_last_sent()
        {
            Assert.That(
                RelayBufferDiagnostics.ComputeMissed(to: 100UL, lastSent: 0UL),
                Is.EqualTo(100L));
        }

        [Test]
        public void ComputeMissed_round_trips_long_max_value_difference()
        {
            // A 63-bit-fitting positive difference must round-trip
            // through the long return type without sign loss.
            ulong to = (ulong)long.MaxValue;
            Assert.That(
                RelayBufferDiagnostics.ComputeMissed(to, lastSent: 0UL),
                Is.EqualTo(long.MaxValue));
        }
    }
}
