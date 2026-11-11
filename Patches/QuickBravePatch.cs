using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace QuickBrave.Patches;

// RT (gamepad right trigger) / F (keyboard) tap = Brave x3 for the turn
// character, spread one input per frame — the game consumes one input/frame.
// Fires the game's own Brave handler, so all normal limits apply.
// Dead outside the battle command-selection phase.
[HarmonyPatch(typeof(BtlLayoutCtrl), nameof(BtlLayoutCtrl.Update))]
internal static class QuickBravePatch
{
    internal static BtlTopMenuLayout.MainWndProc CachedProc;

    private static bool s_prevHeld;
    private static readonly List<int> s_queue = new();
    private const int BravesPerTap = 3;

    [HarmonyPostfix]
    private static void Postfix(BtlLayoutCtrl __instance)
    {
        try
        {
            Tick(__instance);
        }
        catch (Exception ex)
        {
            Plugin.Log.LogWarning($"[QuickBrave] tick failed: {ex.Message}");
        }
    }

    private static void Tick(BtlLayoutCtrl layout)
    {
        bool held = IsBraveHeld();
        bool pressed = held && !s_prevHeld;
        s_prevHeld = held;

        var func = BtlFunction.GetInstance();
        if (func == null) { s_queue.Clear(); return; }

        var seq = func.GetBtlSequenceCtrl();
        if (seq == null) { s_queue.Clear(); return; }
        if (seq.GetPhase() != BtlSequenceCtrl.PHASE_INPUT) { s_queue.Clear(); return; }

        var funcLayout = func.GetBtlLayoutCtrl();
        if (layout == null) layout = funcLayout;
        if (layout == null) { s_queue.Clear(); return; }
        if (layout.m_Phase != BtlLayoutCtrl.MainPhase) { s_queue.Clear(); return; }

        var menu = layout.m_pBtlMenuLayoutCtrl;
        if (menu == null) { s_queue.Clear(); return; }
        if (menu.GetCurrentMenuIndex() != BtlMenuLayoutCtrl.Menu_Top) { s_queue.Clear(); return; }

        var charaMgr = func.GetBtlCharaManager();
        if (charaMgr == null) { s_queue.Clear(); return; }

        if (pressed)
        {
            int turn = layout.GetTurnPlayer();
            if (!IsBraveable(charaMgr, turn)) return;
            for (int k = 0; k < BravesPerTap; k++)
                s_queue.Add(turn);
            Plugin.Log.LogInfo($"[QuickBrave] Brave x3 queued (chara {turn}).");
        }

        if (s_queue.Count == 0) return;

        int idx = s_queue[0];
        s_queue.RemoveAt(0);

        var proc = CachedProc;
        if (proc == null) { s_queue.Clear(); return; }
        if (!IsBraveable(charaMgr, idx)) { s_queue.RemoveAll(x => x == idx); return; }

        try
        {
            proc.onBrave();
        }
        catch (Exception ex)
        {
            Plugin.Log.LogWarning($"[QuickBrave] Brave failed: {ex.Message}");
            s_queue.Clear();
        }
    }

    private static bool IsBraveHeld()
    {
        try
        {
            var gp = Gamepad.current;
            if (gp != null)
            {
                try
                {
                    var rt = gp.rightTrigger;
                    if (rt != null && rt.isPressed)
                        return true;
                }
                catch { /* ignore per-control errors */ }
            }
        }
        catch { /* no gamepad / not ready */ }

        try
        {
            var kb = Keyboard.current;
            if (kb != null)
            {
                try
                {
                    var f = kb.fKey;
                    if (f != null && f.isPressed)
                        return true;
                }
                catch { /* ignore per-control errors */ }
            }
        }
        catch { /* no keyboard / not ready */ }

        return false;
    }

    // Player-controlled, alive, and the game says it can act.
    // Deliberately not gated on HasCommand: Braves add action slots,
    // they don't set the character's command.
    private static bool IsBraveable(BtlCharaManager charaMgr, int idx)
    {
        BtlChara chara;
        try { chara = charaMgr.GetBtlChara(idx); }
        catch { return false; }
        if (chara == null) return false;
        try
        {
            return chara.IsPlayer() && !chara.IsDead() && chara.IsCanCommand();
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Captures the live MainWndProc instance the game itself ticks every frame
/// the top menu is open. IL2CPP interop hides private fields from .NET
/// reflection, so the instance is collected here instead of read from
/// BtlTopMenuLayout's private field.
/// </summary>
[HarmonyPatch(typeof(BtlTopMenuLayout.MainWndProc), nameof(BtlTopMenuLayout.MainWndProc.Update))]
internal static class MainWndProcCapture
{
    [HarmonyPostfix]
    private static void Postfix(BtlTopMenuLayout.MainWndProc __instance)
    {
        try
        {
            if (__instance == null) return;
            if (!object.ReferenceEquals(QuickBravePatch.CachedProc, __instance))
                QuickBravePatch.CachedProc = __instance;
        }
        catch { /* never break the game's update */ }
    }
}
