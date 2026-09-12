# no-train-cards

BepInEx 5 plugin for Black Jacket that stops Niv's train cards from moving your cards around.

## What it does

Niv's train cards - "Choo!", "Chugga!", "Choo choo!", "Chugga chugga!" and their longer
variants - rotate every card on the table when they are played (the `RotateCards` effect).
The "Chugga." card additionally shuffles another Train card into a deck. This plugin still
lets Niv play those cards - they keep their value, placement and animations - but skips
their movement effects:

- **Rotation muted:** no card on either side of the table is moved. This also covers the
  rotate effect on Niv's family-trio transformation cards ("Rotate all cards clockwise 1 step.").
- **Train card insert muted:** "Chugga." no longer shuffles an extra Train card into a deck.
  Insert effects of unrelated cards (Languish, Impulse, Ignited Soul, ...) are untouched.

By default only the opponent's cards are disabled; if you ever play one of these cards
yourself, it works normally (see `AffectPlayerCards` below).

## Install

Copy `NoTrainCards.dll` to `BepInEx/plugins/NoTrainCards/`.

## Build

```sh
dotnet build -c Release
```

The game directory can be overridden with `-p:GameDir=/path/to/FRD`.

## Configuration

Settings live in `BepInEx/config/com.blackjacket.mods.notraincards.cfg` after the first launch.

| Section | Key | Default | Description |
| --- | --- | --- | --- |
| General | Enabled | `true` | Master switch for the plugin. |
| General | DisableRotation | `true` | Mute the rotate effect of all train cards (and Niv's family-trio cards that rotate the table). The cards are still played and keep their value, but nothing moves. |
| General | DisableTrainCardInserts | `true` | Mute the "Chugga." card's extra effect that shuffles another Train card into a deck. |
| General | AffectPlayerCards | `false` | Also mute these effects when you are the one playing such a card. |
| General | VerboseLogging | `false` | Log every muted effect to the BepInEx console. Useful to verify the mod is working. |

## How to verify

Turn on `VerboseLogging`, fight Niv, and watch the BepInEx console. Every time a train card
is played you should see `Muted card rotation on opponent card ...` while the table stays put.
