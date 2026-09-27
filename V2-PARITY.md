# ModCore v2 → v3 parity checklist

Source audit: 2026-09-08. Local `master` at `88b3ac8`; local `next/master` at `af44b8f`. No remote refresh or runtime validation performed.

## Scope and status

V2 is the behavior reference, not an implementation to port. Use v3's existing architecture, REST client, localization, interaction framework, and message builders. Configuration belongs in the web app launched inside Discord; do not recreate the old bot configuration menus. Do not change database entities, schema, migrations, or stored formats.

- **Present**: implementation found; not a claim of tested parity.
- **Partial**: implementation exists but concrete behavior is missing.
- **Missing**: no feature implementation found in Consumer/Jobs or the embedded Dashboard.
- **Earlier v3 only**: found in Shard or Services.Web; not counted as connected to Consumer or the embedded Dashboard.
- **Decision needed**: compatibility or persistence mapping needs agreement before implementation.

Unchecked items represent outstanding parity work, including review of partial implementations. Infrastructure is not a separate rewrite task: reuse what exists and assess any missing connection against the specific feature that needs it.

## Commands

V2 references below are paths on `master` under `ModCore/`. V3 command implementations are under `src/ModCore.Services.Consumer/Interactions/`.

| Checklist | V2 behavior / source | V3 status and remaining work |
| --- | --- | --- |
| [x] | `/about` — `Commands/Main.cs` | Present: `AboutCommand.cs`; localized Components V2 response. Failure fallback remains unfinished, so this is implementation presence, not verified parity. |
| [x] | `/coinflip` — `Commands/Main.cs` | Present: `Tools/CoinFlipCommand.cs`, including animation and random result. |
| [ ] | `/avatar`, `/status` — `Commands/Main.cs` | Missing. |
| [ ] | `/command` and command autocomplete — `Commands/Main.cs`, `AutoComplete/SlashCommandAutoComplete.cs` | Missing. |
| [ ] | `/contact`, feedback submission and owner reply — `Commands/Main.cs`, `Modals/FeedbackModal.cs`, `Modals/FeedbackResponseModal.cs`, `Components/BotManagerComponents.cs` | Missing. Include both ends of the feedback exchange. |
| [ ] | `/translate` and language autocomplete — `Commands/Main.cs`, `AutoComplete/DeepLLanguageAutoComplete.cs` | Missing. Verify the external contract when implementing. |
| [ ] | `/snipe`, `/snipeedit`, `/unsnipe`, delete-response button — `Commands/Main.cs`, `Commands/Moderation.cs`, `Components/UtilComponents.cs` | Missing as user features; message cache code alone does not provide this behavior. |
| [ ] | `/nick`, approval/rejection — `Commands/Main.cs`, `Components/NicknameApprovalComponents.cs` | Missing. Existing `DatabaseGuild.NicknameConfirmationChannelId` provides a configuration field. |
| [ ] | `/info member`, `permissions`, `channel-permissions`, `role`, `server`, `channel` — `Commands/Info.cs` | Missing. Preserve permission and visibility checks. |
| [ ] | `/poll` and poll modal — `Commands/Interactive.cs`, `Modals/PollModal.cs` | Missing. |
| [ ] | `/raffle` — `Commands/Interactive.cs` | Missing. Include entry collection and winner selection. |
| [ ] | `/offtopic` — `Commands/Moderation.cs` | Missing. |
| [ ] | `/tempban` — `Commands/Moderation.cs` | Missing. Existing timer type/data can represent unban; Jobs currently dispatches reminders only. |
| [ ] | `/massban` and modal — `Commands/Moderation.cs`, `Modals/MassBanModal.cs` | Missing. |
| [ ] | `/hackban` — `Commands/Moderation.cs` | Partial: `Moderation/HackBanCommand.cs` issues the ban but reports success without inspecting the REST result. Reconcile v2 options and failure behavior. |
| [ ] | `/softban` — `Commands/Moderation.cs` | Missing. Include unban and message deletion behavior. |
| [ ] | `/isolate` — `Commands/Moderation.cs` | Missing. Private moderation thread workflow. |
| [ ] | `/purge regular`, `user`, `regex`, `bots`, `attachments`, `images` — `Commands/Purge.cs` | Missing. Include filtering, authorization, and deletion constraints. |
| [ ] | `/remind me` — `Commands/Reminders.cs` | Partial: `Remind/RemindMeCommand.cs` persists and schedules; validation and localization explicitly unfinished. Jobs delivers the reminder; membership/DM fallback is explicitly unfinished. |
| [ ] | `/remind list`, `stop`, `clear`, reminder ID autocomplete — `Commands/Reminders.cs`, `AutoComplete/ReminderIdAutoComplete.cs` | Missing. Preserve ownership checks and scheduling consistency. |
| [ ] | Reminder snooze/repeat actions — `Components/ReminderComponents.cs`, `Modals/SnoozeModal.cs` | Missing. |
| [ ] | `/level info`, `/level leaderboard` — `Commands/Level.cs` | Missing. Existing level settings/data entities. |
| [ ] | `/starboard info`, `leaderboard`, `random` — `Commands/Starboard.cs` | Missing. Existing starboard/item entities. |
| [ ] | `/tag get`, `set`, `override`, `remove`, `info`, `transfer`, `list` — `Commands/Tags.cs` | Missing. Include ownership, channel overrides, pagination, modals, and tag/owned-tag autocomplete. Existing tag/history entities; inspect field mapping before implementing. |

## Context menus

| Checklist | V2 behavior / source | V3 status |
| --- | --- | --- |
| [ ] | Show Pronouns — `ContextMenu/MemberContextMenu.cs`, `Integrations/PronounDB.cs` | Missing. Verify current integration contract during implementation. |
| [ ] | Copy emoji, Copy sticker — `ContextMenu/MessageContextMenu.cs` | Missing. |
| [ ] | Translate (DeepL) — `ContextMenu/MessageContextMenu.cs` | Missing; shares translation behavior with `/translate`. |

## Event-driven behavior

V2 source paths in this section are under `ModCore/Listeners/` unless specified otherwise.

| Checklist | V2 behavior / source | V3 status / existing persistence |
| --- | --- | --- |
| [ ] | XP accrual and level-up notifications — `LevelUp.cs` | Missing; `DatabaseLevelData`, `DatabaseLevelSettings` exist. |
| [ ] | Star creation/update/removal from reactions — `StarboardListeners.cs` | Missing; `DatabaseStarboard`, `DatabaseStarboardItem` exist. Include self-star/bot exclusions, threshold behavior, and NSFW/channel safeguards from v2. |
| [ ] | Welcome messages and substitutions — `JoinLog.cs` | Missing runtime behavior; `DatabaseWelcomeSettings` exists. Modernize using the existing v3 message representation. |
| [ ] | Join/leave logging — `JoinLog.cs` | Missing; `DatabaseLoggerSettings` exists. |
| [ ] | Member changes and invite logging — `Logging.cs` | Missing; inspect each v2 logging toggle against existing logger fields. |
| [ ] | Deleted/edited message capture, logging, and snipe data — `MessageSnipe.cs` | Missing in Consumer; earlier Shard `Modules/Cache/Events/MessageCacheEvents.cs` is not the full feature. |
| [ ] | Autoroles and restoration of roles, nicknames, channel overrides — `RoleState.cs` | Missing in Consumer; auto-role, role-state, nickname-state, and override-state entities exist. Include update/deletion bookkeeping and guild initialization behavior. |
| [ ] | Jump-link message embeds — `EmbedMessageLinks.cs` | Missing runtime behavior; `DatabaseGuild.EmbedMessageLinks` exists. Preserve disabled/prefixed/always modes and access checks. |
| [ ] | Reaction roles — `Reactions.cs` | Missing; decision needed. Current role-menu entities have menu/role mappings, but no corresponding message/channel/emoji mapping was found. Do not silently replace reaction roles with role menus. |
| [ ] | Role-menu member selection — `Components/RoleMenuConfigComponents.cs` | Missing runtime behavior. Keep member role selection in Discord while administration moves to the web app. |
| [ ] | Invite filtering and exemptions — `Linkfilter.cs` | Missing; decision needed. No current database settings for invite filtering/exemptions found. V2 has an active listener even though its config-menu entry is commented out. |
| [ ] | Unban timer execution and cancellation after manual unban — `Timers.cs`, `UnbanTimerRemove.cs` | Missing in Jobs. `UnbanTimerData`/timer types already exist; no schema change needed for this representation. |
| [ ] | New-guild report, bot-ratio warning, owner leave-guild action — `NoBotFarm.cs`, `Components/BotManagerComponents.cs` | Missing. Preserve as owner operations, not general server settings. |

## Configuration: Discord-embedded web app

The target is `src/ModCore.Services.Dashboard/`. Its client `src/store/useDiscordStore.ts` implements Discord SDK initialization/authentication and guild/channel context; server `Controllers/ApiController.cs` exchanges the OAuth token. These are existing infrastructure, not work to recreate.

Client `src/App.tsx` currently renders `SettingsPlaceholder` panels and restricts authenticated access to a developer ID. Existing `src/ModCore.Services.Web/` controllers/pages are separate implementation evidence; their existence does not establish that embedded settings can load/save.

| Checklist | V2 configuration capability | Evidence / remaining parity |
| --- | --- | --- |
| [ ] | Open configuration in Discord | Earlier v3 `Shard/Modules/Config/ConfigCommands.cs` sends `LaunchActivity`. No Consumer `/config` implementation found. The deployed Discord entry-point configuration is not available in this source audit, so launch availability is unverified. |
| [ ] | Autorole enable/role selection | Embedded placeholder. Earlier Web has GET/POST controller and page. |
| [ ] | Welcome enable/channel/message | Embedded placeholder. Earlier Web has GET/POST controller, validator, message editor and preview. |
| [ ] | Level enable/notifications/redirect channel | Embedded placeholder. Earlier Web has a settings GET controller/page. |
| [ ] | Logger channel and event toggles | Embedded placeholder. Earlier Web has a settings GET controller/page. |
| [ ] | Starboard settings | Embedded placeholder. Earlier Web has a settings GET controller/page. |
| [ ] | Role/nickname/channel-override persistence toggles | Embedded Profile States placeholder. Earlier Web has a settings GET controller/page. |
| [ ] | Nickname approval channel | No dedicated embedded category found; existing guild field supports it. |
| [ ] | Role-menu creation, information, deletion, posting | Embedded placeholder. Earlier Web has a settings GET controller/page. Preserve posting and member-facing Discord interaction. |
| [ ] | Jump-link embedding mode | Embedded placeholder. Earlier Web has a settings GET controller/page. |
| [ ] | Export configuration | Earlier Web `DashboardController` exposes guild JSON download; embedded equivalent not found. |
| [ ] | Reset configuration with confirmation | V2 `Components/ConfigComponents.cs`; embedded equivalent not found. Reset semantics must use existing storage and require explicit user action. |

Old config buttons/select menus are intentionally superseded by the embedded app. Their underlying settings remain parity requirements; their UI is not.

## Owner tools and external API

These are inventoried separately so they are neither silently dropped nor automatically exposed as ordinary user commands.

| Checklist | V2 capability / source | V3 status / decision |
| --- | --- | --- |
| [ ] | Owner C# evaluation — `LegacyCommands/Eval.cs` | Earlier Shard `Modules/Eval/Events/SimpleEvalEvent.cs` exists; Consumer equivalent not found. Decide whether this operational capability remains wanted before reimplementing it. |
| [ ] | Owner `grantxp`, `nukeban`, `clear`, `exit` — `LegacyCommands/Owner.cs` | Missing in Consumer. Preserve owner restrictions if retained; clarify operational equivalents. |
| [ ] | Owner tag import — `LegacyCommands/Owner.cs`, `HansTagImport/` | Decision needed: one-off import tooling versus ongoing product behavior. Do not run migrations/imports as part of parity work. |
| [ ] | `/api/ping`, `/api/prefix`, `/api/invite`, `/api/invite/redirect`, `/api/permissions`, `/api/metadata` — `Api/Controllers/ApiController.cs` | Old external surface; equivalent routes not found in inspected v3 controllers. Confirm remaining consumers before retiring or restoring compatibility. |

## Existing v3 work outside the v2 checklist

- `/ban` exists in Consumer, but the actual ban is commented out and success is unconditional. This is unfinished v3 behavior, not an implemented v2 command (no `/ban` declaration in the inspected v2 command inventory).
- `/warn` and `/infractions` exist in earlier Shard modules; no Consumer equivalents found. They were not declared in the inspected v2 command inventory.
- Ban appeals, tickets, profile settings, and birthdays appear in v3 code/UI/models but were not established as v2 features by this audit. Do not add them to v2 parity solely because a v3 model or placeholder exists.
- Consumer command registration, argument binding, subcommands, REST, pub/sub, localization, Jobs scheduling, and the embedded SDK shell already exist. This checklist does not authorize replacing them.

## Completion criteria for each outstanding feature

Implement the v2 behavior through current v3 patterns; connect its actual command/event/UI entry point; preserve authorization, failure handling, and existing persistence. Record material behavior changes explicitly. Mark implementation completion separately from runtime verification. Persistence gaps above require discussion, not database changes.

This checklist is a source-level inventory, not a claim of complete option-by-option or deployment parity. Detailed v2 branch behavior must be traced when implementing each row. No application code was changed and no builds, tests, or services were run for this audit.
