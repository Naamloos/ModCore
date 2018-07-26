-- ModCore Database schema upgrade
-- PostgreSQL version 9.6
--
-- Author:          NaamloosDT
-- Project:         ModCore
-- Version:         2 to 3
-- Last update:     2017-11-20 22:44
-- 
-- --------------------------------------------------------------------------------------------------------------------
-- 
-- This file is a part of ModCore project, licensed under the MIT License.
-- Copyright © 2017 by Ryan (Naamloos)
-- https://github.com/NaamloosDT/ModCore
-- 
-- MIT License
-- 
-- Copyright (c) 2017 Ryan
-- 
-- Permission is hereby granted, free of charge, to any person obtaining a copy
-- of this software and associated documentation files (the "Software"), to deal
-- in the Software without restriction, including without limitation the rights
-- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
-- copies of the Software, and to permit persons to whom the Software is
-- furnished to do so, subject to the following conditions:
-- 
-- The above copyright notice and this permission notice shall be included in all
-- copies or substantial portions of the Software.
-- 
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
-- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
-- SOFTWARE.
-- 
-- --------------------------------------------------------------------------------------------------------------------

-- God bless America.

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "mcore_database_info" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_database_info" PRIMARY KEY AUTOINCREMENT,
    "meta_key" TEXT NOT NULL,
    "meta_value" TEXT NOT NULL
);

CREATE TABLE "mcore_guild_config" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_guild_config" PRIMARY KEY AUTOINCREMENT,
    "guild_id" INTEGER NOT NULL,
    "settings" jsonb NOT NULL
);

CREATE TABLE "mcore_modnotes" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_modnotes" PRIMARY KEY AUTOINCREMENT,
    "contents" TEXT NULL,
    "guild_id" INTEGER NOT NULL,
    "member_id" INTEGER NOT NULL
);

CREATE TABLE "mcore_rolestate_overrides" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_rolestate_overrides" PRIMARY KEY AUTOINCREMENT,
    "channel_id" INTEGER NOT NULL,
    "guild_id" INTEGER NOT NULL,
    "member_id" INTEGER NOT NULL,
    "perms_allow" INTEGER NULL,
    "perms_deny" INTEGER NULL
);

CREATE TABLE "mcore_rolestate_roles" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_rolestate_roles" PRIMARY KEY AUTOINCREMENT,
    "guild_id" INTEGER NOT NULL,
    "member_id" INTEGER NOT NULL
);

CREATE TABLE "mcore_timers" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_timers" PRIMARY KEY AUTOINCREMENT,
    "action_data" jsonb NOT NULL,
    "action_type" INTEGER NOT NULL,
    "channel_id" INTEGER NOT NULL,
    "dispatch_at" timestamptz NOT NULL,
    "guild_id" INTEGER NOT NULL,
    "user_id" INTEGER NOT NULL
);

CREATE TABLE "mcore_warnings" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_warnings" PRIMARY KEY AUTOINCREMENT,
    "guild_id" INTEGER NOT NULL,
    "issued_at" timestamptz NOT NULL,
    "issuer_id" INTEGER NOT NULL,
    "member_id" INTEGER NOT NULL,
    "warning_text" TEXT NOT NULL
);

CREATE UNIQUE INDEX "mcore_database_info_meta_key_key" ON "mcore_database_info" ("meta_key");

CREATE UNIQUE INDEX "mcore_guild_config_guild_id_key" ON "mcore_guild_config" ("guild_id");

CREATE UNIQUE INDEX "mcore_modnotes_member_id_guild_id_key" ON "mcore_modnotes" ("member_id", "guild_id");

CREATE UNIQUE INDEX "mcore_rolestate_overrides_member_id_guild_id_channel_id_key" ON "mcore_rolestate_overrides" ("member_id", "guild_id", "channel_id");

CREATE UNIQUE INDEX "mcore_rolestate_roles_member_id_guild_id_key" ON "mcore_rolestate_roles" ("member_id", "guild_id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20171031041203_InitialCreate', '2.0.0-rtm-26452');

CREATE TABLE "mcore_bans" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_bans" PRIMARY KEY AUTOINCREMENT,
    "ban_reason" TEXT NULL,
    "guild_id" INTEGER NOT NULL,
    "issued_at" timestamptz NOT NULL,
    "user_id" INTEGER NOT NULL
);

CREATE TABLE "mcore_cmd_state" (
    "id" smallint NOT NULL CONSTRAINT "id" PRIMARY KEY AUTOINCREMENT,
    "command_qualified" TEXT NULL
);

CREATE TABLE "mcore_stars" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_stars" PRIMARY KEY AUTOINCREMENT,
    "author_id" INTEGER NOT NULL,
    "channel_id" INTEGER NOT NULL,
    "guild_id" INTEGER NOT NULL,
    "message_id" INTEGER NOT NULL,
    "starboard_entry_id" INTEGER NOT NULL,
    "stargazer_id" INTEGER NOT NULL
);

CREATE TABLE "mcore_tags" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_mcore_tags" PRIMARY KEY AUTOINCREMENT,
    "channel_id" INTEGER NOT NULL,
    "contents" TEXT NULL,
    "created_at" timestamptz NOT NULL,
    "tagname" TEXT NULL,
    "owner_id" INTEGER NOT NULL
);

CREATE UNIQUE INDEX "mcore_stars_member_id_guild_id_key" ON "mcore_stars" ("message_id", "channel_id", "stargazer_id");

CREATE UNIQUE INDEX "mcore_tags_channel_id_tag_name_key" ON "mcore_tags" ("channel_id", "tagname");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20180321034814_CommandDisableFeature', '2.0.0-rtm-26452');


