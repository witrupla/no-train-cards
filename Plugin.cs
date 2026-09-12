using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BlackJacket.NoTrainCards
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class NoTrainCardsPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.blackjacket.mods.notraincards";
        public const string PluginName = "Black Jacket - No Train Cards";
        public const string PluginVersion = "1.0.0";

        internal static ManualLogSource Log;
        internal static Settings Cfg;

        private void Awake()
        {
            Log = Logger;
            Cfg = new Settings(Config);

            try
            {
                Harmony.CreateAndPatchAll(typeof(RotateCardsExecutePatch), PluginGuid);
                Harmony.CreateAndPatchAll(typeof(InsertCardsExecutePatch), PluginGuid);
            }
            catch (System.Exception e)
            {
                Log.LogError($"Failed to apply patches: {e}");
            }

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        /// <summary>
        /// Whether effects executed by this player state should be muted.
        /// The opponent (Niv) is always muted; the player's own copies are left alone
        /// unless <c>AffectPlayerCards</c> is enabled.
        /// </summary>
        internal static bool IsMuted(PlayerState executor)
        {
            try
            {
                var cfg = Cfg;
                if (cfg == null || !cfg.Enabled.Value || executor == null)
                {
                    return false;
                }

                return cfg.AffectPlayerCards.Value || !executor.IsPlayer;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// True when the card belongs to the train family. Every train card
        /// ("Choo!", "Chugga!", ...) carries a <see cref="RotateCards"/> effect,
        /// which distinguishes it from ordinary cards.
        /// </summary>
        internal static bool IsTrainCard(Card3D card)
        {
            try
            {
                if (card == null)
                {
                    return false;
                }

                var gameCard = card.GameCard;
                if (gameCard == null)
                {
                    return false;
                }

                var effects = gameCard.Effects;
                if (effects == null)
                {
                    return false;
                }

                for (var i = 0; i < effects.Length; i++)
                {
                    if (effects[i] is RotateCards)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        internal static IEnumerator EmptyCoroutine()
        {
            yield break;
        }

        internal static void LogMuted(string what, Card3D card, PlayerState executor)
        {
            var cfg = Cfg;
            if (cfg == null || !cfg.VerboseLogging.Value)
            {
                return;
            }

            var owner = executor != null && executor.IsPlayer ? "player" : "opponent";
            var cardName = card != null ? card.name : "unknown card";
            Log?.LogInfo($"Muted {what} on {owner} card {cardName}.");
        }
    }

    internal sealed class Settings
    {
        public readonly ConfigEntry<bool> Enabled;
        public readonly ConfigEntry<bool> DisableRotation;
        public readonly ConfigEntry<bool> DisableTrainCardInserts;
        public readonly ConfigEntry<bool> AffectPlayerCards;
        public readonly ConfigEntry<bool> VerboseLogging;

        public Settings(ConfigFile file)
        {
            Enabled = file.Bind("General", "Enabled", true,
                "Stop Niv's train cards from moving cards around.");

            DisableRotation = file.Bind("General", "DisableRotation", true,
                "Mute the rotate effect of all \"Choo!\" / \"Chugga!\" train cards (and Niv's family-trio " +
                "transformation cards that rotate the table). The cards are still played and keep their value, " +
                "but no card on the table is moved.");

            DisableTrainCardInserts = file.Bind("General", "DisableTrainCardInserts", true,
                "Mute the \"Chugga.\" card's extra effect that shuffles another Train card into a deck.");

            AffectPlayerCards = file.Bind("General", "AffectPlayerCards", false,
                "Also mute these effects when you are the one playing such a card. Off by default: " +
                "only the opponent's cards are disabled.");

            VerboseLogging = file.Bind("General", "VerboseLogging", false,
                "Log every muted effect to the BepInEx console. Useful when checking that the mod works.");
        }
    }
}
