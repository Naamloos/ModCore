-- ModCore Database schema upgrade
-- PostgreSQL version 9.6
--
-- Author:          Emzi0767
-- Project:         ModCore
-- Version:         1 to 2
-- Last update:     2017-10-08 23:18
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

-- Update schema version
update mcore_database_info set meta_value=2 where meta_key='schema_version';

-- --------------------------------------------------------------------------------------------------------------------

-- mcore_timers
-- All the timed actions that ModCore should take. This is used for timed actions, such as reminders, bans, or mutes.
create sequence mcore_timers_id_seq;
create table mcore_timers(
	id integer primary key default nextval('mcore_timers_id_seq'),
	guild_id bigint not null,
	channel_id bigint not null,
	user_id bigint not null,
	dispatch_at timestamp with time zone not null, 
	action_type integer not null,
	action_data jsonb
);
alter sequence mcore_timers_id_seq owned by mcore_timers.id;