using System.Collections;
using HarmonyLib;

namespace BlackJacket.NoTrainCards
{
    /// <summary>
    /// <see cref="RotateCards"/> is the effect behind every one of Niv's train cards
    /// ("Choo!", "Chugga!", "Choo choo!", ...): upon being played it rotates all cards
    /// on the table. The same effect is also used by his family-trio transformation
    /// cards ("Rotate all cards clockwise 1 step.").
    /// Skipping the effect keeps the card itself fully playable - it keeps its value,
    /// placement and animations - but the table layout stays untouched.
    /// </summary>
    [HarmonyPatch(typeof(RotateCards), nameof(RotateCards.Execute))]
    internal static class RotateCardsExecutePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Card3D sourceCard, PlayerState executor, ref IEnumerator __result)
        {
            var cfg = NoTrainCardsPlugin.Cfg;
            if (cfg == null || !cfg.DisableRotation.Value)
            {
                return true;
            }

            if (!NoTrainCardsPlugin.IsMuted(executor))
            {
                return true;
            }

            __result = NoTrainCardsPlugin.EmptyCoroutine();
            NoTrainCardsPlugin.LogMuted("card rotation", sourceCard, executor);
            return false;
        }
    }

    /// <summary>
    /// The "Chugga." card additionally shuffles a Train card into a deck through
    /// <see cref="InsertCards"/>. Only inserts that belong to a train card are muted,
    /// so unrelated insert effects (Languish, Impulse, Ignited Soul, ...) stay intact.
    /// </summary>
    [HarmonyPatch(typeof(InsertCards), nameof(InsertCards.Execute))]
    internal static class InsertCardsExecutePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Card3D sourceCard, PlayerState executor, ref IEnumerator __result)
        {
            var cfg = NoTrainCardsPlugin.Cfg;
            if (cfg == null || !cfg.DisableTrainCardInserts.Value)
            {
                return true;
            }

            if (!NoTrainCardsPlugin.IsMuted(executor) || !NoTrainCardsPlugin.IsTrainCard(sourceCard))
            {
                return true;
            }

            __result = NoTrainCardsPlugin.EmptyCoroutine();
            NoTrainCardsPlugin.LogMuted("train card insert", sourceCard, executor);
            return false;
        }
    }
}
