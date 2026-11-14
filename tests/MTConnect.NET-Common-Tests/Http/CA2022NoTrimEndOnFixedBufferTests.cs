// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Http
{
    /// <summary>
    /// Permanent regression guard for the F-CR-001 TrimEnd-on-fixed-buffer class of bug in
    /// <c>MTConnectPostResponseHandler.ReadRequestBytes</c>. The pre-fix net9 branch called
    /// <c>Stream.ReadExactlyAsync(bytes, 0, 2 MB)</c> then handed the buffer to a
    /// <c>TrimEnd</c> helper that dropped trailing 0x00 bytes to work around the fact that
    /// the buffer was never actually filled. That shape is wrong on two axes: <c>ReadExactlyAsync</c>
    /// on a body smaller than the buffer throws <see cref="System.IO.EndOfStreamException"/>
    /// (silently swallowed by the outer <c>catch</c>, returning null and dropping the request);
    /// and even where the exact-read semantics happen to work, <c>TrimEnd</c> corrupts payloads
    /// whose final legitimate byte is 0x00 (binary MTConnect assets, UTF-8 documents padded
    /// with NUL, and so on).
    ///
    /// The canonical shape — matching the boost::beast HTTP parser cppagent uses
    /// (<c>src/mtconnect/sink/rest_sink/session_impl.cpp:176-181</c>) — is a short-read
    /// accumulator that respects the underlying stream's Content-Length-aware framing and
    /// truncates to the actually-filled length. The fix unifies every TFM on that pattern
    /// and deletes the <c>TrimEnd(byte[])</c> helper outright.
    ///
    /// This fixture asserts that the helper stays gone; if a future edit re-introduces it or
    /// the branching guard around it, the fixture goes RED before the change lands.
    /// </summary>
    [TestFixture]
    [Category("CA2022NoTrimEndOnFixedBuffer")]
    public class CA2022NoTrimEndOnFixedBufferTests
    {
        /// <summary>Pins that the <c>TrimEnd(byte[])</c> helper on <c>MTConnectPostResponseHandler</c> is deleted — re-adding it signals a TrimEnd-on-fixed-buffer regression.</summary>
        [Test]
        public void MTConnectPostResponseHandler_has_no_TrimEnd_helper()
        {
            var anchor = typeof(MTConnect.Servers.Http.MTConnectHttpServer);
            var handlerType = anchor.Assembly.GetType(
                "MTConnect.Servers.MTConnectPostResponseHandler",
                throwOnError: false);

            Assert.That(handlerType, Is.Not.Null,
                "MTConnectPostResponseHandler not visible via reflection — refactor may have renamed it.");

            var trimEnd = handlerType!.GetMethod(
                "TrimEnd",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(byte[]) },
                modifiers: null);

            Assert.That(trimEnd, Is.Null,
                "MTConnectPostResponseHandler.TrimEnd(byte[]) must not exist. The pre-fix helper dropped "
                + "trailing 0x00 bytes from a 2 MB fixed buffer to compensate for a broken exact-read on "
                + "net9 — corrupting payloads ending in a legitimate 0x00. The correct shape is a "
                + "short-read accumulator (aligned with cppagent's boost::beast HTTP parser) that truncates "
                + "to the actually-filled length. If this test fails, revert the TrimEnd re-addition and "
                + "keep the accumulator.");
        }

        /// <summary>Pins that <c>ReadRequestBytes</c>'s IL contains no call to any <c>TrimEnd</c> method — a broader regression guard covering the class of bug beyond the specific helper name.</summary>
        [Test]
        public void ReadRequestBytes_IL_contains_no_TrimEnd_call()
        {
            var anchor = typeof(MTConnect.Servers.Http.MTConnectHttpServer);
            var handlerType = anchor.Assembly.GetType(
                "MTConnect.Servers.MTConnectPostResponseHandler",
                throwOnError: false);

            Assert.That(handlerType, Is.Not.Null,
                "MTConnectPostResponseHandler not visible via reflection — refactor may have renamed it.");

            var method = handlerType!.GetMethod(
                "ReadRequestBytes",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            Assert.That(method, Is.Not.Null,
                "MTConnectPostResponseHandler.ReadRequestBytes not found — refactor may have renamed it.");

            // The compiler generates an async state machine; the real body lives on a nested
            // struct named "<ReadRequestBytes>d__N" with a MoveNext() method. Walk both surfaces.
            var candidates = new System.Collections.Generic.List<MethodInfo> { method! };
            var stateMachineTypes = handlerType!
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                .Where(t => t.Name.Contains("ReadRequestBytes"));
            foreach (var t in stateMachineTypes)
            {
                var moveNext = t.GetMethod("MoveNext",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (moveNext != null) candidates.Add(moveNext);
            }

            foreach (var m in candidates)
            {
                var body = m.GetMethodBody();
                if (body == null) continue;
                var il = body.GetILAsByteArray();
                if (il == null) continue;
                // Scan for any method-token reference resolving to a method whose name is "TrimEnd".
                var module = m.Module;
                for (var i = 0; i + 4 < il.Length; i++)
                {
                    var opcode = il[i];
                    // call = 0x28, callvirt = 0x6F
                    if (opcode != 0x28 && opcode != 0x6F) continue;
                    var token = System.BitConverter.ToInt32(il, i + 1);
                    MethodBase? target = null;
                    try { target = module.ResolveMethod(token); } catch { }
                    if (target == null) continue;
                    Assert.That(target.Name, Is.Not.EqualTo("TrimEnd"),
                        "ReadRequestBytes calls TrimEnd — the F-CR-001 bug class has regressed. "
                        + "Remove the call and let the short-read accumulator's actual-length truncation "
                        + "carry the semantic.");
                }
            }
        }
    }
}
