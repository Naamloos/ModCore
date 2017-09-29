-- ModCore Database schema
-- PostgreSQL version 9.6
--
-- Author:          Emzi0767
-- Project:         ModCore
-- Version:         1
-- Last update:     2017-09-29 17:05
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

-- mcore_database_info
-- This table holds metadata related to this database schema. This is information such as schema version, which is used 
-- during the initialization to determine whether the DB is installed properly.
create sequence mcore_database_info_id_seq;
create table mcore_database_info(
    id integer primary key default nextval('mcore_database_info_id_seq'),
    meta_key text not null,
    meta_value text not null,
    unique(meta_key)
);
alter sequence mcore_database_info_id_seq owned by mcore_database_info.id;

-- Insert the metadata
insert into mcore_database_info(meta_key, meta_value) values('schema_version', '1');

-- --------------------------------------------------------------------------------------------------------------------

-- mcore_guild_config
-- This table holds all configuration data for each guild the bot is in.
create sequence mcore_guild_config_id_seq;
create table mcore_guild_config(
    id integer primary key default nextval('mcore_guild_config_id_seq'),
    guild_id bigint not null,
    settings jsonb not null,
    unique(guild_id)
);
alter sequence mcore_guild_config_id_seq owned by mcore_guild_config.id;

-- Config structure (JSON):
-- 
-- {
--   "mute_role_id": 0,
--   "rolestate": {
--     "enabled": true,
--     "ignored_role_ids": [],
--     "ignored_channel_ids": []
--   }
-- }

-- --------------------------------------------------------------------------------------------------------------------

-- mcore_rolestate_roles
-- This table holds information about each user's roles. Rolestate is a mechanic which persists user's roles when they 
-- leave and rejoin a guild. Due to how Discord works, leaving and rejoining a guild makes you lose all your roles and 
-- permission overwrites on any channels. Rolestate prevents that from happening.
create sequence mcore_rolestate_roles_id_seq;
create table mcore_rolestate_roles(
    id integer primary key default nextval('mcore_rolestate_roles_id_seq'),
    member_id bigint not null,
    guild_id bigint not null,
    role_ids bigint[],
    unique(member_id, guild_id)
);
alter sequence mcore_rolestate_roles_id_seq owned by mcore_rolestate_roles.id;

-- --------------------------------------------------------------------------------------------------------------------

-- mcore_rolestate_overrides
-- This table holds information about each user's channel overrides. For more information, see the description of 
-- mcore_rolestate_roles.
create sequence mcore_rolestate_overrides_id_seq;
create table mcore_rolestate_overrides(
    id integer primary key default nextval('mcore_rolestate_overrides_id_seq'),
    member_id bigint not null,
    guild_id bigint not null,
    channel_id bigint not null,
    perms_allow bigint,
    perms_deny bigint,
    unique(member_id, guild_id, channel_id)
);
alter sequence mcore_rolestate_overrides_id_seq owned by mcore_rolestate_overrides.id;

-- --------------------------------------------------------------------------------------------------------------------

-- mcore_warnings
-- This table holds warnings issued by moderators. The purpose of this function is to allow storing certain notes about 
-- bad behaviours.
create sequence mcore_warnings_id_seq;
create table mcore_warnings(
    id integer primary key default nextval('mcore_warnings_id_seq'),
    member_id bigint not null,
    guild_id bigint not null,
    issuer_id bigint not null,
    issued_at timestamp with time zone not null,
    warning_text text not null
);
alter sequence mcore_warnings_id_seq owned by mcore_warnings.id;

-- --------------------------------------------------------------------------------------------------------------------

-- mcore_modnotes
-- Unlike warnings, these can be only set once per guild. The purpose of this is to have system which allows attaching 
-- notes to members on per-guild basis, so that moderators can note down any important information about members.
create sequence mcore_modnotes_id_seq;
create table mcore_modnotes(
    id integer primary key default nextval('mcore_modnotes_id_seq'),
    member_id bigint not null,
    guild_id bigint not null,
    contents text,
    unique(member_id, guild_id)
);
create sequence mcore_modnotes_id_seq owned by mcore_modnotes.id;